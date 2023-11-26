using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private ColourManager _colourManager;
    private SpriteRenderer _spriteRenderer;
    private Transform transform;
    private Vector2 origin;

    // Regions
    public int regionState = 0;
    public int regionLayer = 0;
    private bool isPlayerHiding = false;

    //movement
    [SerializeField] private float moveSpeed = 3f;
    private Vector2 moveDirection;
    public PlayerInputActions playerMovement;
    private InputAction _move;

    // Data
    bool _isDead;
    [SerializeField] Sprite _hidingSprite;
    [SerializeField] Sprite _normalSprite;

    [Header("Sounds")]
    [SerializeField] private string obscuredName = "PlayerObscured";
	private AudioSource obscuredSound;
    [SerializeField] private string visibleName = "PlayerVisible";
	private AudioSource visibleSound;
    [SerializeField] private string moveName = "PlayerMove";
	private AudioSource moveSound;

    void Awake()
    {
        
        EventManager.EventInitialise(EventType.COLOUR_CHANGE_BOOL);
        // Set values and components
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        obscuredSound = GameObject.Find(obscuredName).GetComponent<AudioSource>();
        visibleSound = GameObject.Find(visibleName).GetComponent<AudioSource>();
        moveSound = GameObject.Find(moveName).GetComponent<AudioSource>();

        origin = transform.position;
        _isDead = false;
        _spriteRenderer.sprite = _normalSprite;

        playerMovement = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _move = playerMovement.Player.Move;
        _move.Enable();
        _move.performed += MovingTrue;
        _move.canceled += MovingFalse;

        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventSubscribe(EventType.LOSE, Death);
    }

    private void OnDisable()
    {
        _move.Disable();
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, Death);
    }

    void Start()
    {
        EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
    }

    void Update()
    {
        //
    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
            ProcessInputs();
            // if player is doing movement inputs, move the player and add to colour time counter
            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
                if (!moveSound.isPlaying)
                {
                    moveSound.Play();
                }
            }
            else // set velocity to zero
            {
                moveSound.Stop();
                rb.velocity = new Vector2(0, 0);
            }
        }
    }

    private void ProcessInputs()
    {
        moveDirection = _move.ReadValue<Vector2>();
    }

    // Change between visible and 'hiding'
    public void HidingSprite()
    {
        obscuredSound.Play();
        _spriteRenderer.sprite = _hidingSprite;
    }

    public void NormalSprite()
    {
        visibleSound.Play();
        _spriteRenderer.sprite = _normalSprite;
    }

    public void Death(object data)
    {
        _isDead = true;
        // Player colour gets converted to Enemy!!!
        Color convertedColour = new Color(1f, 0.2983692f, 0.2509804f);
        _spriteRenderer.color = convertedColour;
    }

    private void ColourManagerHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("ColourManagerHandler is null");
        }

        _colourManager = (ColourManager)data;
    }

    public void NewState(int input)
    {
        regionState = input;
        if (regionState == 3)
        {
            EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
            if (!isPlayerHiding)
            {
                HidingSprite();
                isPlayerHiding = true;
            }
        }
        else 
        {
            if (isPlayerHiding)
            {
                NormalSprite();
                isPlayerHiding = false;
            }
        }
    }

    private void MovingTrue(InputAction.CallbackContext context)
    {
        if (regionState == 3)
        {
            EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
        }
        else
        {
            EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, true);
        }
    }

    private void MovingFalse(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
    }
}
