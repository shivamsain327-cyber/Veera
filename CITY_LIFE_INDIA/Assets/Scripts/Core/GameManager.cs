using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CityLifeIndia.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Gameplay,
        InVehicle,
        Paused,
        MissionActive,
        MissionComplete,
        GameOver
    }

    public enum GameMode
    {
        SinglePlayer,
        Multiplayer
    }

    /// <summary>
    /// Central Game Manager controlling game state, session flow, and scene transitions.
    /// Follows the Technical Architecture defined in Section 7 of City Life India GDD.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("State Settings")]
        [SerializeField] private GameState currentState = GameState.Boot;
        [SerializeField] private GameMode currentGameMode = GameMode.SinglePlayer;

        [Header("Scene Configuration")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string cityWorldScene = "CityWorld";
        [SerializeField] private string multiplayerLobbyScene = "MultiplayerLobby";

        public event Action<GameState> OnGameStateChanged;
        public event Action<GameMode> OnGameModeChanged;

        public GameState CurrentState => currentState;
        public GameMode CurrentGameMode => currentGameMode;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        public void SetState(GameState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            Debug.Log($"[GameManager] Game State Changed to: {newState}");

            switch (newState)
            {
                case GameState.Gameplay:
                case GameState.InVehicle:
                case GameState.MissionActive:
                    Time.timeScale = 1.0f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    break;
            }

            OnGameStateChanged?.Invoke(currentState);
        }

        public void SetGameMode(GameMode mode)
        {
            currentGameMode = mode;
            OnGameModeChanged?.Invoke(mode);
        }

        public void LoadCityWorld(GameMode mode = GameMode.SinglePlayer)
        {
            SetGameMode(mode);
            SetState(GameState.Gameplay);
            SceneManager.LoadScene(cityWorldScene);
        }

        public void ReturnToMainMenu()
        {
            SetState(GameState.MainMenu);
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(mainMenuScene);
        }

        public void PauseGame()
        {
            if (currentState == GameState.Gameplay || currentState == GameState.InVehicle || currentState == GameState.MissionActive)
            {
                SetState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                SetState(GameState.Gameplay);
            }
        }

        public void QuitGame()
        {
            Debug.Log("[GameManager] Quitting application...");
            Application.Quit();
        }
    }
}
