using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Par�metros ajustables en el Inspector de Unity
    [Header("Configuraci�n de Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 500f;
    public float gravity = -20f; // Mayor gravedad para asegurar el contacto con el suelo

    // Componente principal para el movimiento y colisiones
    private CharacterController characterController;
    private Vector3 velocity; // Almacena la velocidad (incluida la gravedad)

    // Referencia a la c�mara para orientar el movimiento
    [Header("Referencias")]
    public Transform cameraTransform;

    void Start()
    {
        // Obtiene el componente CharacterController al inicio
        characterController = GetComponent<CharacterController>();

        // Bloquea y esconde el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Manejo de la Gravedad y Contacto con el Suelo (Colisi�n Vertical)
        // Solo aplicamos una peque�a fuerza negativa cuando est� en el suelo
        // para asegurar que isGrounded sea True y que se "pegue" al piso.
        if (characterController.isGrounded)
        {
            // Evita que la gravedad acumule un valor grande
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
            // NOTA: Si quisieras Saltar, aqu� agregar�as: 
            // if (Input.GetButtonDown("Jump")) { velocity.y = jumpHeight; }
        }

        // Aplica la gravedad en cada frame
        velocity.y += gravity * Time.deltaTime;

        // 2. Manejo de Input y Direcci�n
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Crea un vector de movimiento en el plano XZ (horizontal)
        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 3. Aplicaci�n del Movimiento
        if (inputDirection.magnitude >= 0.1f)
        {
            // Calcular el �ngulo de rotaci�n basado en la c�mara
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Rotar el personaje suavemente hacia la direcci�n de movimiento
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Calcular la direcci�n final de movimiento
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Mueve el Character Controller
            characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // Aplica el vector de velocidad (gravedad) en el movimiento
        // Esto es lo que maneja la "ca�da" o el movimiento vertical.
        characterController.Move(velocity * Time.deltaTime);
    }
}
