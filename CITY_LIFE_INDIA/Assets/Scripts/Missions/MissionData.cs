using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityLifeIndia.Missions
{
    public enum MissionType
    {
        Delivery,
        Taxi,
        DrivingChallenge,
        Exploration,
        Photography
    }

    [System.Serializable]
    public class MissionCheckpoint
    {
        public Vector3 position;
        public float triggerRadius = 4.0f;
        public string instructionText;
    }

    [CreateAssetMenu(fileName = "NewMission", menuName = "CityLifeIndia/Mission Data")]
    public class MissionData : ScriptableObject
    {
        public string missionId;
        public string title;
        [TextArea(2, 4)] public string description;
        public MissionType type;
        public int rewardCoins = 250;
        public int rewardXP = 150;
        public float timeLimitSeconds = 90f; // 0 for unlimited
        public List<MissionCheckpoint> checkpoints = new List<MissionCheckpoint>();
    }
}
