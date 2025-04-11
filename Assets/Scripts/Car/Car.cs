using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 100f;

    public Transform driverSeat;
    public GameObject player;

    public Camera playerCamera;
    public Camera carCamera;

    public WheelCollider frontLeftCollider;

    private bool isDriving = false;
    private Rigidbody rb;

    private float carSpeed;
    private float localVelocityX;
    private float localVelocityZ;

    private bool deceleratingCar = false;
    private float steeringAxis = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        carCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        // Вход в машину
        if (!isDriving && Vector3.Distance(player.transform.position, transform.position) < 3f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterCar();
            }
        }

        // Выход из машины
        if (isDriving && Input.GetKeyDown(KeyCode.F))
        {
            ExitCar();
        }

        if (!isDriving) return;

        // === ДАННЫЕ О МАШИНЕ ===
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(rb.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(rb.linearVelocity).z;

        // === УПРАВЛЕНИЕ ===
        if (Input.GetKey(KeyCode.W))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            GoForward();
        }

        if (Input.GetKey(KeyCode.S))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            GoReverse();
        }

        if (Input.GetKey(KeyCode.A))
            TurnLeft();

        if (Input.GetKey(KeyCode.D))
            TurnRight();

        if (Input.GetKey(KeyCode.Space))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            Handbrake();
        }

        if (Input.GetKeyUp(KeyCode.Space))
            RecoverTraction();

        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            ThrottleOff();

        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
        {
            InvokeRepeating("DecelerateCar", 0f, 0.1f);
            deceleratingCar = true;
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
            ResetSteeringAngle();

        AnimateWheelMeshes();
    }

    void FixedUpdate()
    {
        // Можно переместить физику сюда по желанию
    }

    void EnterCar()
    {
        player.SetActive(false);
        isDriving = true;
        playerCamera.gameObject.SetActive(false);
        carCamera.gameObject.SetActive(true);
    }

    void ExitCar()
    {
        player.transform.position = driverSeat.position + Vector3.right * 2;
        player.SetActive(true);
        isDriving = false;
        carCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
    }

    // ==== Заглушки управления ====
    void GoForward() { /* Добавь физику движения вперёд */ }
    void GoReverse() { /* Добавь задний ход */ }
    void TurnLeft() { /* Добавь поворот влево */ }
    void TurnRight() { /* Добавь поворот вправо */ }
    void Handbrake() { /* Реализуй ручник */ }
    void RecoverTraction() { /* Возврат сцепления */ }
    void ThrottleOff() { /* Нет газа */ }
    void DecelerateCar() { /* Плавное торможение */ }
    void ResetSteeringAngle() { /* Руль в ноль */ }
    void AnimateWheelMeshes() { /* Обновление колёс */ }
}
