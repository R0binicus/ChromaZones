using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
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

    // Data
    public float ColourChangeSpeed = 0.1f;
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
        // Set values and components
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //_colourManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ColourManager>();
        obscuredSound = GameObject.Find(obscuredName).GetComponent<AudioSource>();
        visibleSound = GameObject.Find(visibleName).GetComponent<AudioSource>();
        moveSound = GameObject.Find(moveName).GetComponent<AudioSource>();

        origin = transform.position;
        _isDead = false;
        _spriteRenderer.sprite = _normalSprite;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventSubscribe(EventType.LOSE, Death);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, Death);
    }

    void Update()
    {
        if (!_isDead)
        {
            if (rb.velocity != Vector2.zero && regionState != 3)
            {
                _colourManager.colour = 0.5f * ColourChangeSpeed;
            }
            else
            {
                _colourManager.colour = 0f;
            }

            ProcessInputs();
        }
    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
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
        //// Multiplies current x and y coordinates together for the colour 
        //// Subtracts the origin coords from the equasion, so the starting position should not affect the colour of the regions
        //gameManager.colourCoords = (transform.position.x) * (transform.position.y);
        ////gameManager.colourCoords = Vector3.Distance(transform.position, origin) 
    }

    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
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
}
