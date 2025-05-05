using UnityEngine;

public class CameraCar : MonoBehaviour
{
 public Transform target;          // Объект, за которым следует камера (машина)
    public Vector3 offset = new Vector3(0, 5, -10); // Смещение камеры
    public float smoothSpeed = 0.125f;  // Скорость следования камеры
    public float rotationSpeed = 5f;    // Скорость вращения камеры
    public float verticalRotationSpeed = 2f;  // Скорость вертикального вращения камеры
    public float minYAngle = -20f;     // Минимальный угол наклона камеры по оси Y
    public float maxYAngle = 80f;      // Максимальный угол наклона камеры по оси Y

    private float currentRotationX = 0f;  // Угол вращения камеры по оси X (вертикально)
    private float currentRotationY = 0f;  // Угол вращения камеры по оси Y (горизонтально)

    void LateUpdate()
    {
        if (target == null) return;

        // Получаем движение мыши
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // Обновляем углы вращения камеры на основе движения мыши
        currentRotationX -= verticalInput * verticalRotationSpeed;
        currentRotationY += horizontalInput * rotationSpeed;

        // Ограничиваем вертикальное вращение камеры (чтобы она не переворачивалась)
        currentRotationX = Mathf.Clamp(currentRotationX, minYAngle, maxYAngle);

        // Создаём кватернион для вращения вокруг цели (машины)
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);

        // Рассчитываем желаемую позицию камеры
        Vector3 desiredPosition = target.position + rotation * offset;

        // Плавное движение камеры
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Камера всегда смотрит на машину
        transform.LookAt(target);
    }
}
