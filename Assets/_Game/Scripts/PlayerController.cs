using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static Action<int> onCoinsChanged;
    public static Action<int> onScoreChanged;
    
    [SerializeField] private string username;
    [SerializeField] private int coins;
    [SerializeField] private int score;
    [SerializeField] private float moveSpeed, jumpForce;
    [SerializeField] private int moveDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance = 0.55f;

    private Vector3 startPosition;
    private Vector2 movement;
    private bool isGrounded;
    private Rigidbody rb;
    private MeshRenderer mesh;
    private PlayerInputActions playerInput;
    
    public string Username
    {
        get => username;
        set => username = value;
    }

    public int Coins
    {
        get => coins;
        set => coins = value;
    }

    public int Score
    {
        get => score;
        set => score = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }

    public int MoveDistance
    {
        get => moveDistance;
        set => moveDistance = value;
    }

    public LayerMask GroundLayer
    {
        get => groundLayer;
        set => groundLayer = value;
    }

    public float GroundDistance
    {
        get => groundDistance;
        set => groundDistance = value;
    }

    public Vector3 StartPosition
    {
        get => startPosition;
        set => startPosition = value;
    }

    public Vector2 Movement
    {
        get => movement;
        set => movement = value;
    }

    public bool IsGrounded
    {
        get => isGrounded;
        set => isGrounded = value;
    }

    public Rigidbody Rb
    {
        get => rb;
        set => rb = value;
    }

    public MeshRenderer Mesh
    {
        get => mesh;
        set => mesh = value;
    }

    private void Awake()
    {
        playerInput = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Move.performed += MoveOnperformed;
        playerInput.Player.Move.canceled += MoveOncanceled;
        playerInput.Player.Jump.performed += JumpOnperformed;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Move.performed -= MoveOnperformed;
        playerInput.Player.Move.canceled -= MoveOncanceled;
        playerInput.Player.Jump.performed -= JumpOnperformed;
    }

    private void JumpOnperformed(InputAction.CallbackContext input)
    {
        if (GetIsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x,jumpForce, 0);
        }
    }

    private void MoveOnperformed(InputAction.CallbackContext input)
    {
        movement = input.ReadValue<Vector2>();
    }
    private void MoveOncanceled(InputAction.CallbackContext input)
    {
        movement = input.ReadValue<Vector2>();
    }

    private void Update()
    {
        transform.position += new Vector3(movement.x * moveSpeed , transform.position.y, moveSpeed) * Time.deltaTime;
        Vector3 delta = transform.position - startPosition;
        moveDistance = (int)delta.magnitude;
    }
    
    public bool GetIsGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundLayer);
        return isGrounded;
    }

    public static void ScoreChanged(int scoreValue)
    {
        onScoreChanged?.Invoke(scoreValue);
    }

    public static void CoinsChanged(int coinsValue)
    {
        onCoinsChanged?.Invoke(coinsValue);
    }
}
