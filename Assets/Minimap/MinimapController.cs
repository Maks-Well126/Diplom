using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("Minimap Settings")]
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minimapHeight = 5f;
    [SerializeField] private float minimapSize = 5f;

    [Header("Player Icon Settings")]
    [SerializeField] private RectTransform playerIconRectTransform;
    [SerializeField] private float smoothRotation = 5f; // Сглаживание поворота иконки

    private Vector3 lastPosition;
    private float currentRotation;
    private float initialIconRotation; // Сохраняем начальный поворот иконки

    private void Start()
    {
        if (minimapCamera == null)
        {
            Debug.LogError("Minimap camera is not assigned!");
            return;
        }

        // Настраиваем камеру миникарты
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = minimapSize;
        minimapCamera.transform.position = new Vector3(0, minimapHeight, 0);
        minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

        lastPosition = playerTransform.position;
        
        // Сохраняем начальный поворот иконки
        if (playerIconRectTransform != null)
        {
            initialIconRotation = playerIconRectTransform.localRotation.eulerAngles.z;
            currentRotation = initialIconRotation;
        }
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Обновляем позицию камеры миникарты
            Vector3 newPosition = playerTransform.position;
            newPosition.y = minimapHeight;
            minimapCamera.transform.position = newPosition;

            // Обновляем поворот иконки игрока
            if (playerIconRectTransform != null)
            {
                // Получаем направление движения
                Vector3 moveDirection = (playerTransform.position - lastPosition);
                
                if (moveDirection.magnitude > 0.01f) // Если игрок движется
                {
                    // Вычисляем угол движения
                    float targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                    
                    // Плавно поворачиваем иконку, учитывая начальный поворот
                    currentRotation = Mathf.LerpAngle(currentRotation, -targetRotation + initialIconRotation, Time.deltaTime * smoothRotation);
                    playerIconRectTransform.localRotation = Quaternion.Euler(0, 0, currentRotation);
                }

                lastPosition = playerTransform.position;
            }
        }
    }
} 