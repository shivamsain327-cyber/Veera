using System;
using System.Collections.Generic;
using UnityEngine;
using CityLifeIndia.Core;
using CityLifeIndia.SaveSystem;
using CityLifeIndia.UI;

namespace CityLifeIndia.Missions
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        [Header("Available Missions (5 MVP Missions)")]
        [SerializeField] private List<MissionData> allMissions = new List<MissionData>();

        [Header("Runtime State")]
        [SerializeField] private MissionData activeMission;
        [SerializeField] private int currentCheckpointIndex = 0;
        [SerializeField] private float timeRemaining = 0f;
        [SerializeField] private bool isMissionRunning = false;

        [Header("Waypoints & Visuals")]
        [SerializeField] private GameObject waypointMarkerPrefab;
        private GameObject spawnedWaypointMarker;

        public MissionData ActiveMission => activeMission;
        public float TimeRemaining => timeRemaining;
        public bool IsMissionRunning => isMissionRunning;
        public int CurrentCheckpointIndex => currentCheckpointIndex;

        public event Action<MissionData> OnMissionStarted;
        public event Action<int, int> OnCheckpointProgress; // current, total
        public event Action<MissionData, bool> OnMissionEnded; // mission, success
        public event Action<float> OnTimerTick;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (allMissions.Count == 0)
            {
                CreateDefaultMissions();
            }
        }

        private void Update()
        {
            if (!isMissionRunning || activeMission == null) return;

            // Timer countdown
            if (activeMission.timeLimitSeconds > 0)
            {
                timeRemaining -= Time.deltaTime;
                OnTimerTick?.Invoke(timeRemaining);

                if (timeRemaining <= 0)
                {
                    FailMission("Time Expired!");
                    return;
                }
            }

            CheckPlayerNearCurrentCheckpoint();
        }

        public void StartMission(string missionId)
        {
            var target = allMissions.Find(m => m.missionId == missionId);
            if (target != null)
            {
                StartMission(target);
            }
        }

        public void StartMission(MissionData mission)
        {
            activeMission = mission;
            currentCheckpointIndex = 0;
            timeRemaining = mission.timeLimitSeconds;
            isMissionRunning = true;

            UpdateWaypointVisual();
            OnMissionStarted?.Invoke(activeMission);
            GameManager.Instance?.SetState(GameState.MissionActive);

            Debug.Log($"[MissionManager] Started mission: {mission.title}");
        }

        private void CheckPlayerNearCurrentCheckpoint()
        {
            if (currentCheckpointIndex >= activeMission.checkpoints.Count) return;

            var cp = activeMission.checkpoints[currentCheckpointIndex];
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            float dist = Vector3.Distance(player.transform.position, cp.position);
            if (dist <= cp.triggerRadius)
            {
                AdvanceCheckpoint();
            }
        }

        private void AdvanceCheckpoint()
        {
            currentCheckpointIndex++;
            OnCheckpointProgress?.Invoke(currentCheckpointIndex, activeMission.checkpoints.Count);

            if (currentCheckpointIndex >= activeMission.checkpoints.Count)
            {
                CompleteMission();
            }
            else
            {
                UpdateWaypointVisual();
            }
        }

        private void CompleteMission()
        {
            isMissionRunning = false;
            Debug.Log($"[MissionManager] Mission Completed: {activeMission.title}!");

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.AddRewards(activeMission.rewardCoins, activeMission.rewardXP);
                SaveManager.Instance.MarkMissionCompleted(activeMission.missionId);
            }

            if (spawnedWaypointMarker != null)
            {
                Destroy(spawnedWaypointMarker);
            }

            OnMissionEnded?.Invoke(activeMission, true);
            GameManager.Instance?.SetState(GameState.MissionComplete);
        }

        public void FailMission(string reason)
        {
            isMissionRunning = false;
            Debug.Log($"[MissionManager] Mission Failed: {reason}");

            if (spawnedWaypointMarker != null)
            {
                Destroy(spawnedWaypointMarker);
            }

            OnMissionEnded?.Invoke(activeMission, false);
            GameManager.Instance?.SetState(GameState.Gameplay);
        }

        public void CancelMission()
        {
            if (!isMissionRunning) return;
            isMissionRunning = false;
            if (spawnedWaypointMarker != null) Destroy(spawnedWaypointMarker);
            activeMission = null;
            GameManager.Instance?.SetState(GameState.Gameplay);
        }

        private void UpdateWaypointVisual()
        {
            if (activeMission == null || currentCheckpointIndex >= activeMission.checkpoints.Count)
            {
                if (spawnedWaypointMarker != null) Destroy(spawnedWaypointMarker);
                return;
            }

            Vector3 targetPos = activeMission.checkpoints[currentCheckpointIndex].position;

            if (spawnedWaypointMarker == null && waypointMarkerPrefab != null)
            {
                spawnedWaypointMarker = Instantiate(waypointMarkerPrefab, targetPos, Quaternion.identity);
            }
            else if (spawnedWaypointMarker != null)
            {
                spawnedWaypointMarker.transform.position = targetPos;
            }
        }

        private void CreateDefaultMissions()
        {
            // 1. Delivery Mission
            var m1 = ScriptableObject.CreateInstance<MissionData>();
            m1.missionId = "m_delivery_01";
            m1.title = "Jaipur Chai & Parcel Delivery";
            m1.description = "Pick up traditional parcel from Central Market and deliver to the Tech Hub office before tea turns cold!";
            m1.type = MissionType.Delivery;
            m1.rewardCoins = 300;
            m1.rewardXP = 150;
            m1.timeLimitSeconds = 85f;
            m1.checkpoints.Add(new MissionCheckpoint { position = new Vector3(80, 0, 50), triggerRadius = 5f, instructionText = "Pick up parcel at Market Dhaba" });
            m1.checkpoints.Add(new MissionCheckpoint { position = new Vector3(-120, 0, 180), triggerRadius = 5f, instructionText = "Deliver to Tech Park Office" });
            allMissions.Add(m1);

            // 2. Taxi Mission
            var m2 = ScriptableObject.CreateInstance<MissionData>();
            m2.missionId = "m_taxi_01";
            m2.title = "City Taxi: Tourist to Palace Hotel";
            m2.description = "Pick up the arriving passenger from Central Transport Hub and drive smoothly to Jaipur Palace Hotel.";
            m2.type = MissionType.Taxi;
            m2.rewardCoins = 450;
            m2.rewardXP = 200;
            m2.timeLimitSeconds = 110f;
            m2.checkpoints.Add(new MissionCheckpoint { position = new Vector3(-200, 0, -100), triggerRadius = 6f, instructionText = "Pick up passenger at Bus Terminal" });
            m2.checkpoints.Add(new MissionCheckpoint { position = new Vector3(150, 0, -150), triggerRadius = 6f, instructionText = "Drop passenger at Grand Palace Gate" });
            allMissions.Add(m2);

            // 3. Driving Challenge
            var m3 = ScriptableObject.CreateInstance<MissionData>();
            m3.missionId = "m_drive_01";
            m3.title = "Rajasthan Speedway Sprint";
            m3.description = "Race your vehicle across 4 rapid checkpoints around the main city boulevard within 60 seconds.";
            m3.type = MissionType.DrivingChallenge;
            m3.rewardCoins = 200;
            m3.rewardXP = 350;
            m3.timeLimitSeconds = 60f;
            m3.checkpoints.Add(new MissionCheckpoint { position = new Vector3(0, 0, 100), triggerRadius = 7f, instructionText = "Checkpoint 1: North Avenue" });
            m3.checkpoints.Add(new MissionCheckpoint { position = new Vector3(120, 0, 50), triggerRadius = 7f, instructionText = "Checkpoint 2: East Roundabout" });
            m3.checkpoints.Add(new MissionCheckpoint { position = new Vector3(60, 0, -120), triggerRadius = 7f, instructionText = "Checkpoint 3: South Gate" });
            m3.checkpoints.Add(new MissionCheckpoint { position = new Vector3(-80, 0, -20), triggerRadius = 7f, instructionText = "Finish Line: Grand Plaza" });
            allMissions.Add(m3);

            // 4. Exploration Mission
            var m4 = ScriptableObject.CreateInstance<MissionData>();
            m4.missionId = "m_explore_01";
            m4.title = "Discover Amber Hill Viewpoint";
            m4.description = "Climb the hill road to discover the scenic sunrise viewpoint overlooking the fictional Jaipur cityscape.";
            m4.type = MissionType.Exploration;
            m4.rewardCoins = 150;
            m4.rewardXP = 300;
            m4.timeLimitSeconds = 0f; // No timer
            m4.checkpoints.Add(new MissionCheckpoint { position = new Vector3(250, 15, 200), triggerRadius = 10f, instructionText = "Reach Amber Viewpoint Overlook" });
            allMissions.Add(m4);

            // 5. Photography / Shop Mission
            var m5 = ScriptableObject.CreateInstance<MissionData>();
            m5.missionId = "m_photo_01";
            m5.title = "Hawa Mahal Photography Snap";
            m5.description = "Find the photo spot in front of the iconic Palace of Winds landmark and capture a picture.";
            m5.type = MissionType.Photography;
            m5.rewardCoins = 500;
            m5.rewardXP = 200;
            m5.timeLimitSeconds = 120f;
            m5.checkpoints.Add(new MissionCheckpoint { position = new Vector3(50, 0, -220), triggerRadius = 6f, instructionText = "Align in front of Hawa Mahal Monument" });
            allMissions.Add(m5);
        }

        public List<MissionData> GetAllMissions() => allMissions;
    }
}
