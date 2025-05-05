using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera playerCamera;      // Камера персонажа
    public Camera carCamera;         // Камера машины
    public Transform player;         // Трансформ персонажа
    public Transform car;            // Трансформ машины
    public KeyCode enterCarKey = KeyCode.E; // Клавиша для входа в машину

    private bool isInCar = false;    // Флаг для проверки, в машине ли персонаж

    void Update()
    {
        // Проверка, если персонаж рядом с машиной и нажал клавишу для входа
        if (Vector3.Distance(player.position, car.position) < 3f && Input.GetKeyDown(enterCarKey))
        {
            ToggleCamera();
        }
    }

    void ToggleCamera()
    {
        if (isInCar)
        {
            // Возвращаем камеру к персонажу
            playerCamera.gameObject.SetActive(true);
            carCamera.gameObject.SetActive(false);
            isInCar = false;
        }
        else
        {
            // Переключаем на камеру машины
            playerCamera.gameObject.SetActive(false);
            carCamera.gameObject.SetActive(true);
            isInCar = true;
        }
    }
}
