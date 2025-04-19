using UnityEngine;

namespace Curier
{
    public class MainMenuState : MonoBehaviour
    {
        public GameObject mainMenuUI;
        public GameStateManager manager;

        private void OnEnable()
        {
            mainMenuUI.SetActive(true);
        }

        private void OnDisable()
        {
            mainMenuUI.SetActive(false);
        }

        public void OnStartButton()
        {
            manager.StartGame();
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}
