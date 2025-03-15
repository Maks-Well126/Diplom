using UnityEngine;

public class Player : MonoBehaviour
{  public float moveSpeed = 2f;
    public float rotationSpeed = 2f;
    public float jumpForce = 2f;
    public float gravity = 9.81f;
    
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;

    public Transform cameraTransform; // Камера следует за персонажем
    public Transform cameraPivot; // Точка привязки камеры

    public float mouseSensitivity = 2f;
    private float yaw = 0f;
    private float pitch = 0f;
    
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float cameraDistance = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Отключаем физическое вращение
        Cursor.lockState = CursorLockMode.Locked; // Прячем курсор
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void HandleMouseLook()
    {
        // Управление камерой с помощью мыши
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // Ограничение наклона

        // Вращаем точку привязки камеры
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
        
        // Размещаем камеру за персонажем
        Vector3 offset = cameraPivot.forward * -cameraDistance;
        cameraTransform.position = cameraPivot.position + offset;
        cameraTransform.LookAt(cameraPivot);
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Двигаем персонажа относительно камеры
        Vector3 move = cameraPivot.forward * vertical + cameraPivot.right * horizontal;
        move.y = 0; // Убираем движение вверх-вниз
        moveDirection = move.normalized;

        // Проверяем, стоит ли персонаж на земле
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Прыжок
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    void ApplyMovement()
    {
        if (moveDirection.magnitude > 0)
        {
            // Поворачиваем персонажа в направлении движения
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Перемещение персонажа
        Vector3 move = moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // Применяем гравитацию
        if (!isGrounded)
        {
            rb.linearVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        }

        // Привязываем камеру к персонажу
        cameraPivot.position = transform.position + new Vector3(0, 0.3f, 0);
    }
    }
