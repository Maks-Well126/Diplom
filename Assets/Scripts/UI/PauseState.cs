using UnityEngine;

namespace Curier
{
    public class PauseState : MonoBehaviour
    {
        public GameObject pauseUI;
        public GameStateManager manager;

        private void OnEnable()
        {
            pauseUI.SetActive(true);
        }

        private void OnDisable()
        {
            pauseUI.SetActive(false);
        }

        public void OnResume()
        {
            manager.ResumeGame();
        }

        public void OnMainMenu()
        {
            manager.ShowMainMenu();
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}
