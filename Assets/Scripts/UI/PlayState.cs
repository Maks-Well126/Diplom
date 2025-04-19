using UnityEngine;

namespace Curier
{
    public class PlayState : MonoBehaviour
    {
        public GameObject playUI;
        public GameStateManager manager;

        private void OnEnable()
        {
            playUI.SetActive(true);
        }

        private void OnDisable()
        {
            playUI.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                manager.PauseGame();
            }
        }
    }
}
