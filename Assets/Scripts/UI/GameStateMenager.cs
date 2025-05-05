using UnityEngine;

namespace Curier
{
    public class GameStateManager : MonoBehaviour
    {
        public MainMenuState mainMenuState;
        public PlayState playState;
        public PauseState pauseState;

        private void Start()
        {
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            DisableAllStates();
            mainMenuState.gameObject.SetActive(true);
            Time.timeScale = 0f; // Останавливаем игру
        }

        public void StartGame()
        {
            DisableAllStates();
            playState.gameObject.SetActive(true);
            Time.timeScale = 1f; // Запускаем игру
        }

        public void PauseGame()
        {
            pauseState.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            pauseState.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        private void DisableAllStates()
        {
            mainMenuState.gameObject.SetActive(false);
            playState.gameObject.SetActive(false);
            pauseState.gameObject.SetActive(false);
        }
    }
}
