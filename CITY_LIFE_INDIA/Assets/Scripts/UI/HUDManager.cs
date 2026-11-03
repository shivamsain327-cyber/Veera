using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CityLifeIndia.Player;
using CityLifeIndia.Vehicles;
using CityLifeIndia.Missions;
using CityLifeIndia.SaveSystem;
using CityLifeIndia.World;

namespace CityLifeIndia.UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Currency & Stats")]
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI xpText;
        [SerializeField] private TextMeshProUGUI levelText;

        [Header("Vehicle UI")]
        [SerializeField] private GameObject speedometerPanel;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private GameObject enterVehicleButton;
        [SerializeField] private GameObject exitVehicleButton;

        [Header("Mission UI")]
        [SerializeField] private GameObject missionTrackerPanel;
        [SerializeField] private TextMeshProUGUI missionTitleText;
        [SerializeField] private TextMeshProUGUI missionObjectiveText;
        [SerializeField] private TextMeshProUGUI missionTimerText;

        [Header("District Banner")]
        [SerializeField] private GameObject districtBanner;
        [SerializeField] private TextMeshProUGUI districtNameText;

        private PlayerController player;
        private VehicleController currentVehicle;

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.OnRewardsUpdated += UpdateCurrencyDisplay;
                UpdateCurrencyDisplay(SaveManager.Instance.CurrentData.coins, SaveManager.Instance.CurrentData.xp);
            }

            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.OnMissionStarted += HandleMissionStarted;
                MissionManager.Instance.OnCheckpointProgress += HandleCheckpointProgress;
                MissionManager.Instance.OnMissionEnded += HandleMissionEnded;
                MissionManager.Instance.OnTimerTick += UpdateMissionTimer;
            }

            if (WorldManager.Instance != null)
            {
                WorldManager.Instance.OnDistrictEntered += HandleDistrictEntered;
            }

            if (speedometerPanel != null) speedometerPanel.SetActive(false);
            if (missionTrackerPanel != null) missionTrackerPanel.SetActive(false);
            if (enterVehicleButton != null) enterVehicleButton.SetActive(false);
            if (exitVehicleButton != null) exitVehicleButton.SetActive(false);
        }

        private void Update()
        {
            CheckVehicleInteraction();
        }

        private void CheckVehicleInteraction()
        {
            if (player == null) return;

            if (player.IsInVehicle)
            {
                if (enterVehicleButton != null) enterVehicleButton.SetActive(false);
                if (exitVehicleButton != null) exitVehicleButton.SetActive(true);
                if (speedometerPanel != null) speedometerPanel.SetActive(true);

                if (currentVehicle == null)
                {
                    currentVehicle = FindFirstObjectByType<VehicleController>();
                }

                if (currentVehicle != null && speedText != null)
                {
                    speedText.text = $"{Mathf.RoundToInt(currentVehicle.CurrentSpeedKmH)} KM/H";
                }
            }
            else
            {
                if (speedometerPanel != null) speedometerPanel.SetActive(false);
                if (exitVehicleButton != null) exitVehicleButton.SetActive(false);

                bool nearVehicle = player.CurrentNearbyVehicle != null;
                if (enterVehicleButton != null && enterVehicleButton.activeSelf != nearVehicle)
                {
                    enterVehicleButton.SetActive(nearVehicle);
                }
            }
        }

        public void OnClickEnterVehicle()
        {
            if (player != null) player.TryEnterVehicle();
        }

        public void OnClickExitVehicle()
        {
            if (currentVehicle != null && currentVehicle.IsOccupied)
            {
                currentVehicle.ExitVehicle();
            }
        }

        public void OnClickJump()
        {
            if (player != null) player.Jump();
        }

        private void UpdateCurrencyDisplay(int coins, int xp)
        {
            if (coinsText != null) coinsText.text = $"₹ {coins:N0}";
            if (xpText != null) xpText.text = $"XP: {xp}";
            if (levelText != null && SaveManager.Instance != null)
            {
                levelText.text = $"LVL {SaveManager.Instance.CurrentData.playerLevel}";
            }
        }

        private void HandleMissionStarted(MissionData mission)
        {
            if (missionTrackerPanel != null) missionTrackerPanel.SetActive(true);
            if (missionTitleText != null) missionTitleText.text = mission.title;
            if (missionObjectiveText != null && mission.checkpoints.Count > 0)
            {
                missionObjectiveText.text = mission.checkpoints[0].instructionText;
            }
        }

        private void HandleCheckpointProgress(int current, int total)
        {
            if (MissionManager.Instance != null && MissionManager.Instance.ActiveMission != null)
            {
                var mission = MissionManager.Instance.ActiveMission;
                if (current < total && missionObjectiveText != null)
                {
                    missionObjectiveText.text = mission.checkpoints[current].instructionText;
                }
            }
        }

        private void HandleMissionEnded(MissionData mission, bool success)
        {
            if (missionTrackerPanel != null) missionTrackerPanel.SetActive(false);
        }

        private void UpdateMissionTimer(float seconds)
        {
            if (missionTimerText != null)
            {
                int mins = Mathf.FloorToInt(seconds / 60f);
                int secs = Mathf.FloorToInt(seconds % 60f);
                missionTimerText.text = $"{mins:00}:{secs:00}";
            }
        }

        private void HandleDistrictEntered(CityDistrict district)
        {
            if (districtBanner != null && districtNameText != null)
            {
                districtNameText.text = district.districtName;
                districtBanner.SetActive(true);
                CancelInvoke(nameof(HideDistrictBanner));
                Invoke(nameof(HideDistrictBanner), 3.5f);
            }
        }

        private void HideDistrictBanner()
        {
            if (districtBanner != null) districtBanner.SetActive(false);
        }
    }
}
