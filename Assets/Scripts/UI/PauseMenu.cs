using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool PauseGame = false;              // Состояние игры (на паузе или нет)
    public GameObject pauseGameMenu;            // Ссылка на UI меню паузы

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseGame)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1f;                    // Возобновляем ход времени
        PauseGame = false;
    }

    public void Pause()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;                    // Останавливаем время
        PauseGame = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;                    // Убедимся, что время идёт при загрузке меню
        SceneManager.LoadScene("Menu");         // Загрузка сцены с именем "Menu"
    }
}
