using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Min(0)] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField, Min(0)] private float _mouseSensitivity = 200f;
    [SerializeField, Min(0)] private float _verticalLookLimit = 80f;
    private Transform _cameraTransform;

    private CharacterController _characterController;
    private float _verticalRotation;
    private Vector3 _velocity;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        if (!_characterController)
            Debug.LogError("CharacterController not found!");
    }

    private void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_verticalLookLimit, _verticalLookLimit);
        _cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 direction = transform.right * input.x + transform.forward * input.z;

        _characterController.Move(direction * _moveSpeed * Time.deltaTime);

        HandleGravity();
    }

    private void HandleGravity()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
