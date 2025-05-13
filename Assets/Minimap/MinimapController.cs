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

    [Header("Direction Indicator Settings")]
    [SerializeField] private GameObject directionIndicatorPrefab;  // Префаб указателя направления
    [SerializeField] private float indicatorDistance = 50f;        // Расстояние от центра миникарты до указателя
    [SerializeField] private float indicatorHeight = 10f;          // Высота указателя над землёй
    private GameObject currentDirectionIndicator;                  // Текущий указатель направления
    private RectTransform minimapRect;                            // RectTransform миникарты

    private Vector3 lastPosition;
    private float currentRotation;
    private float initialIconRotation; // Сохраняем начальный поворот иконки

    private GameObject currentPin;
    private Vector3 currentPinTargetPosition;
    private GameObject currentPinPrefab;

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

        // Получаем RectTransform миникарты
        if (minimapImage != null)
        {
            minimapRect = minimapImage.rectTransform;
        }

        // Проверяем наличие префаба указателя
        if (directionIndicatorPrefab == null)
        {
            Debug.LogWarning("Direction indicator prefab is not assigned!");
        }

        // Скрываем указатель при старте
        HideDirectionIndicator();
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Обновляем позицию камеры миникарты
            Vector3 newPosition = playerTransform.position;
            newPosition.y = minimapHeight;
            minimapCamera.transform.position = newPosition;

            // Вращаем камеру по Y в зависимости от игрока
            float playerYRotation = playerTransform.eulerAngles.y;
            minimapCamera.transform.rotation = Quaternion.Euler(90, playerYRotation, 0);

            // Отключаем поворот иконки игрока (она всегда смотрит вверх)
            if (playerIconRectTransform != null)
            {
                playerIconRectTransform.localRotation = Quaternion.identity;
            }
        }

        // Обновляем позицию пина каждый кадр
        UpdatePinPosition();
    }

    // Публичный метод для обновления указателя направления
    public void UpdateDirectionIndicator(Vector3 targetPosition, bool showIndicator)
    {
        if (!showIndicator)
        {
            HideDirectionIndicator();
            return;
        }

        if (minimapCamera == null || minimapRect == null || directionIndicatorPrefab == null) return;

        if (currentDirectionIndicator == null)
        {
            currentDirectionIndicator = Instantiate(directionIndicatorPrefab, minimapRect);
        }

        // Convert target position to viewport coordinates
        Vector3 targetViewportPoint = minimapCamera.WorldToViewportPoint(targetPosition);
        
        // Check if target is visible on minimap
        bool isTargetVisible = targetViewportPoint.x >= 0 && targetViewportPoint.x <= 1 &&
                             targetViewportPoint.y >= 0 && targetViewportPoint.y <= 1 &&
                             targetViewportPoint.z > 0;

        Vector2 markerPos;
        float angle = 0f;

        if (isTargetVisible)
        {
            // If target is visible, show exact position
            markerPos = new Vector2(
                (targetViewportPoint.x - 0.5f) * minimapRect.rect.width,
                (targetViewportPoint.y - 0.5f) * minimapRect.rect.height
            );
        }
        else
        {
            // If target is outside minimap, show direction on edge
            Vector3 directionToTarget = targetPosition - playerTransform.position;
            directionToTarget.y = 0;
            directionToTarget.Normalize();

            // Convert direction to local space relative to camera rotation
            float cameraYRotation = minimapCamera.transform.rotation.eulerAngles.y;
            Vector3 localDirection = Quaternion.Euler(0, -cameraYRotation, 0) * directionToTarget;

            // Calculate angle for rotation
            angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

            // Place marker on edge with offset from center
            float radius = minimapRect.rect.width * 0.5f * 0.80f; // Reduced from 0.85f to 0.80f to move marker closer to center
            float edgeX = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float edgeY = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            markerPos = new Vector2(edgeX, edgeY);
        }

        // Update indicator position and keep it vertical
        currentDirectionIndicator.GetComponent<RectTransform>().anchoredPosition = markerPos;
        currentDirectionIndicator.transform.rotation = Quaternion.identity;
    }

    // Публичный метод для скрытия указателя
    public void HideDirectionIndicator()
    {
        if (currentDirectionIndicator != null)
        {
            Destroy(currentDirectionIndicator);
            currentDirectionIndicator = null;
        }
    }

    // Метод для отображения пина (иконки места) на миникарте
    public void ShowPinOnMinimap(Vector3 targetPosition, GameObject pinPrefab)
    {
        if (minimapCamera == null || minimapRect == null || pinPrefab == null) return;

        if (currentPin == null || currentPinPrefab != pinPrefab)
        {
            if (currentPin != null)
                Destroy(currentPin);
            currentPin = Instantiate(pinPrefab, minimapRect);
            currentPinPrefab = pinPrefab;
        }
        currentPinTargetPosition = targetPosition;
        UpdatePinPosition();
    }

    // Вызывайте этот метод каждый кадр для обновления позиции пина
    public void UpdatePinPosition()
    {
        if (currentPin == null) return;

        float radius = minimapRect.rect.width * 0.5f * 0.95f;

        // Позиция цели на миникарте (viewport -> UI)
        Vector3 targetViewportPoint = minimapCamera.WorldToViewportPoint(currentPinTargetPosition);
        Vector2 markerPos = new Vector2(
            (targetViewportPoint.x - 0.5f) * minimapRect.rect.width,
            (targetViewportPoint.y - 0.5f) * minimapRect.rect.height
        );

        // Если маркер вне круга — прижимаем к границе
        if (markerPos.magnitude > radius)
        {
            markerPos = markerPos.normalized * radius;
        }

        RectTransform pinRect = currentPin.GetComponent<RectTransform>();
        pinRect.anchoredPosition = markerPos;
        pinRect.localRotation = Quaternion.identity;
    }

    // Для удаления пина
    public void HidePin()
    {
        if (currentPin != null)
        {
            Destroy(currentPin);
            currentPin = null;
            currentPinPrefab = null;
        }
    }
} 