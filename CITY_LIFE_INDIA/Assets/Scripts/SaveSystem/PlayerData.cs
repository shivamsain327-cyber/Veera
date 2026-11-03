using System;
using System.Collections.Generic;

namespace CityLifeIndia.SaveSystem
{
    [Serializable]
    public class PlayerData
    {
        public string playerName = "Player 1";
        public int coins = 1000;
        public int xp = 0;
        public int playerLevel = 1;

        // Customization
        public int skinIndex = 1;
        public string shirtColorHex = "E03A3A"; // Jaipur Red
        public string pantsColorHex = "2B3E50"; // Denim Blue

        // Vehicle
        public string selectedVehicleId = "car_01";
        public string carColorHex = "D32F2F";
        public List<string> unlockedVehicles = new List<string>() { "car_01", "bike_01" };

        // Missions
        public List<string> completedMissionIds = new List<string>();

        // Settings
        public float masterVolume = 1.0f;
        public float sfxVolume = 1.0f;
        public float musicVolume = 0.8f;
        public int graphicsQualityIndex = 1; // 0 = Low, 1 = Med, 2 = High (GDD Section 9)
    }
}
