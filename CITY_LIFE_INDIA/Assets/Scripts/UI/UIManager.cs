using UnityEngine;
using CityLifeIndia.Core;
using CityLifeIndia.SaveSystem;
using CityLifeIndia.Missions;

namespace CityLifeIndia.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject garagePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject missionResultPanel;

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
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
            else
            {
                ShowHUD();
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case GameState.Gameplay:
                case GameState.InVehicle:
                case GameState.MissionActive:
                    ShowHUD();
                    break;
                case GameState.Paused:
                    ShowPauseMenu();
                    break;
            }
        }

        public void ShowMainMenu()
        {
            HideAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        public void ShowHUD()
        {
            HideAllPanels();
            if (hudPanel != null) hudPanel.SetActive(true);
        }

        public void ShowPauseMenu()
        {
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        public void HidePauseMenu()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            GameManager.Instance?.ResumeGame();
        }

        public void ShowGarage()
        {
            if (garagePanel != null) garagePanel.SetActive(true);
        }

        public void HideGarage()
        {
            if (garagePanel != null) garagePanel.SetActive(false);
        }

        public void ShowSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        public void HideSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void ShowMissionResult(bool success, int coins, int xp)
        {
            if (missionResultPanel != null)
            {
                missionResultPanel.SetActive(true);
            }
        }

        private void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (hudPanel != null) hudPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (garagePanel != null) garagePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (missionResultPanel != null) missionResultPanel.SetActive(false);
        }
    }
}
