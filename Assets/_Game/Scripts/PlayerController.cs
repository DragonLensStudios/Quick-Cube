using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Screen = UnityEngine.Device.Screen;
using TouchPhase = UnityEngine.TouchPhase;

public class PlayerController : MonoBehaviour
{
    public static Action<int> onCoinsChanged;
    public static Action<int> onScoreChanged;
    public static Action<int> onLivesChanged;
    public static Action<double> onMultiplierChanged;
    public static Action onGameOver;

    [SerializeField] private string username;
    [SerializeField] private int coins;
    [SerializeField] private int score;
    [SerializeField] private int lives;
    [SerializeField] private float moveSpeed = 8, baseSpeed = 8, maxSpeed = 50, jumpForce = 5;
    [SerializeField] private double coinMultiplierMod = 1, coinMaxMultiplier = 10;
    [SerializeField] private int moveDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance = 0.55f;
    [SerializeField] private Vector3 leftBoundary, rightBoundary;
    [SerializeField] private float screenCenterX;
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
    
    public int Lives
    {
        get => lives;
        set => lives = value;
    }
    
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Clamp(value, baseSpeed, maxSpeed);
    }
    
    public float BaseSpeed
    {
        get => baseSpeed;
        set => baseSpeed = value;
    }
    
    public float MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }
    
    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }
    
    public double CoinMultiplierMod
    {
        get => coinMultiplierMod;
        set => coinMultiplierMod = value;
    }
    
    public double CoinMaxMultiplier
    {
        get => coinMaxMultiplier;
        set => coinMaxMultiplier = value;
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

    public Vector3 LeftBoundary
    {
        get => leftBoundary;
        set => leftBoundary = value;
    }

    public Vector3 RightBoundary
    {
        get => rightBoundary;
        set => rightBoundary = value;
    }
    
    public float ScreenCenterX
    {
        get => screenCenterX;
        set => screenCenterX = value;
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
        username = PlayerPrefs.GetString("playerUsername");
        coins = PlayerPrefs.GetInt("playerCoins");
        playerInput = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        startPosition = transform.position;
        screenCenterX = Screen.width * 0.5f;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Move.performed += MoveOnperformed;
        playerInput.Player.Move.canceled += MoveOncanceled;
        playerInput.Player.Jump.performed += JumpOnperformed;
        onGameOver += () => gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Move.performed -= MoveOnperformed;
        playerInput.Player.Move.canceled -= MoveOncanceled;
        playerInput.Player.Jump.performed -= JumpOnperformed;
        onGameOver -= () => { };
    }

    private void JumpOnperformed(InputAction.CallbackContext input)
    {
        if (GetIsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
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
        #if UNITY_ANDROID
        // if there are any touches currently
        if(Input.touchCount > 0)
        {
            // get the first one
            Touch firstTouch = Input.GetTouch(0);
            if (Input.touchCount > 1)
            {
                Touch secondTouch = Input.GetTouch(1);

                if (secondTouch.phase == TouchPhase.Began)
                {
                    movement = Vector2.zero;
                }    
            }
            else
            {
                // if it began this frame
                if(firstTouch.phase == TouchPhase.Began)
                {
                    if(firstTouch.position.x > screenCenterX)
                    {
                        movement.x += 1;
                        // if the touch position is to the right of center
                        // move right
                    }
                    else if(firstTouch.position.x < screenCenterX)
                    {
                        movement.x -= 1;
                        // if the touch position is to the left of center
                        // move left
                    }
                }
                else if (firstTouch.phase == TouchPhase.Ended)
                {
                    movement = Vector2.zero;
                }
            }
            

        }
        #endif
        var newPos = new Vector3(movement.x * moveSpeed, transform.position.y, moveSpeed);
        var clampedX = Mathf.Clamp(newPos.x, leftBoundary.x, rightBoundary.x);

        if (transform.position.x <= rightBoundary.x && transform.position.x >= leftBoundary.x)
        {
            transform.position += new Vector3(clampedX, newPos.y, newPos.z) * Time.deltaTime;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftBoundary.x, rightBoundary.x),
                transform.position.y, transform.position.z);
        }

        Vector3 delta = transform.position - startPosition;
        moveDistance = (int)delta.magnitude;
        score = moveDistance;
        ScoreChanged(score);
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
        PlayerPrefs.SetInt("playerCoins", coinsValue);
        PlayerPrefs.Save();
        onCoinsChanged?.Invoke(coinsValue);
    }

    public static void LivesChanged(int livesValue)
    {
        onLivesChanged?.Invoke(livesValue);
        if (livesValue <= 0)
        {
            GameOver();
        }
    }

    public static void MultiplierChanged(double multiplierValue)
    {
        onMultiplierChanged?.Invoke(multiplierValue);
    }

    public static void GameOver()
    {
        onGameOver?.Invoke();
    }
}