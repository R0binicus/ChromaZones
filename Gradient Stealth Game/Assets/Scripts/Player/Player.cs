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
    private bool _moveBool = false;

    // Data
    bool _isDead;
    [SerializeField] Sprite _hidingSprite;
    [SerializeField] Sprite _normalSprite;
    private bool _isCollidingObstacle = false;

    // Sounds
    [Header("Sounds")]
    [SerializeField] private string obscuredName = "PlayerObscured";
	private AudioSource obscuredSound;
    [SerializeField] private string visibleName = "PlayerVisible";
	private AudioSource visibleSound;

    void Awake()
    {
        EventManager.EventInitialise(EventType.COLOUR_CHANGE_BOOL);
        // Set values and components
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        obscuredSound = GameObject.Find(obscuredName).GetComponent<AudioSource>();
        visibleSound = GameObject.Find(visibleName).GetComponent<AudioSource>();

        origin = transform.position;
        _isDead = false;
        _spriteRenderer.sprite = _normalSprite;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventSubscribe(EventType.LOSE, Death);
        EventManager.EventSubscribe(EventType.PLAYER_MOVE_BOOL, MoveBoolHandler);
        EventManager.EventSubscribe(EventType.PLAYER_MOVE_VECT2D, MoveVect2DHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, Death);
        EventManager.EventUnsubscribe(EventType.PLAYER_MOVE_BOOL, MoveBoolHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_MOVE_VECT2D, MoveVect2DHandler);
        StopAllCoroutines();
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
            // if player is doing movement inputs, move the player and add to colour time counter
            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                //rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
                rb.AddForce(moveDirection * moveSpeed * 2, ForceMode2D.Impulse);
            }
        }
        //if (_isCollidingObstacle)
        //{
        //    
        //}
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

    private void MoveBoolHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("MoveBoolHandler is null");
        }
        var moveBool = (bool)data;
        _moveBool = moveBool;

        StartCoroutine(EventCoroutine(moveBool));
    }

    private IEnumerator EventCoroutine(bool moveBool)
    {

        if (_isCollidingObstacle)
        {
            yield return new WaitForSeconds(0.2f);
        }

        if (moveBool && rb.velocity != Vector2.zero)
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
        else
        {
            EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
        }
    }

    private void MoveVect2DHandler(object data)
    {

        if (data == null)
        {
            Debug.Log("MoveBoolHandler is null");
        }

        if (!_isDead)
        {
            moveDirection = (Vector2)data;
            if (moveDirection == Vector2.zero)
            {
                rb.velocity = new Vector2(0, 0);
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Obstacle"))
        {
            _isCollidingObstacle = true;
            MoveBoolHandler(_moveBool);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Obstacle"))
        {
            _isCollidingObstacle = false;
            MoveBoolHandler(_moveBool);
        }
    }
}
