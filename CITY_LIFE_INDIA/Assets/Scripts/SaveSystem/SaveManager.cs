using System;
using System.IO;
using UnityEngine;

namespace CityLifeIndia.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_FILE_NAME = "city_life_save.json";
        private string saveFilePath;

        [SerializeField] private PlayerData currentData;
        public PlayerData CurrentData => currentData;

        public event Action<PlayerData> OnDataLoaded;
        public event Action<int, int> OnRewardsUpdated; // coins, xp

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            LoadGame();
        }

        public void SaveGame()
        {
            try
            {
                string json = JsonUtility.ToJson(currentData, true);
                File.WriteAllText(saveFilePath, json);
                Debug.Log($"[SaveManager] Game saved successfully to: {saveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to save game: {ex.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    currentData = JsonUtility.FromJson<PlayerData>(json);
                    Debug.Log("[SaveManager] Loaded existing save data.");
                }
                else
                {
                    currentData = new PlayerData();
                    SaveGame();
                    Debug.Log("[SaveManager] Created fresh player profile.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Error loading save data: {ex.Message}. Resetting.");
                currentData = new PlayerData();
            }

            OnDataLoaded?.Invoke(currentData);
        }

        public void AddRewards(int coins, int xp)
        {
            if (currentData == null) return;

            currentData.coins += coins;
            currentData.xp += xp;

            // Level progression calculation: Every 500 XP = 1 Level
            int calculatedLevel = 1 + (currentData.xp / 500);
            if (calculatedLevel > currentData.playerLevel)
            {
                currentData.playerLevel = calculatedLevel;
                Debug.Log($"[SaveManager] Level Up! New Level: {currentData.playerLevel}");
            }

            SaveGame();
            OnRewardsUpdated?.Invoke(currentData.coins, currentData.xp);
        }

        public void MarkMissionCompleted(string missionId)
        {
            if (currentData == null) return;

            if (!currentData.completedMissionIds.Contains(missionId))
            {
                currentData.completedMissionIds.Add(missionId);
                SaveGame();
            }
        }

        public void ResetSaveData()
        {
            currentData = new PlayerData();
            SaveGame();
            OnDataLoaded?.Invoke(currentData);
        }
    }
}
