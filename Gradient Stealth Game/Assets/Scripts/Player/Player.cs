using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody2D _rb;
    private ColourManager _colourManager;
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;

    // Regions
    public int RegionState = 0;
    private bool _isPlayerHiding = false;

    //movement
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _velocityPower;
    [SerializeField] private float _moveAccel;
    [SerializeField] private float _moveDecel;
    private Vector2 _moveDirection;
    private bool _moveBool = false;

    // Data
    private bool _isDead;
    [SerializeField] private Sprite _hidingSprite;
    [SerializeField] private Sprite _normalSprite;
    private bool _isCollidingObstacle = false;

    // Sounds
    [Header("Sounds")]
    [SerializeField] private string _obscuredName = "PlayerObscured";
	private AudioSource _obscuredSound;
    [SerializeField] private string _visibleName = "PlayerVisible";
	private AudioSource _visibleSound;

    void Awake()
    {
        EventManager.EventInitialise(EventType.COLOUR_CHANGE_BOOL);
        // Set values and components
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _obscuredSound = GameObject.Find(_obscuredName).GetComponent<AudioSource>();
        _visibleSound = GameObject.Find(_visibleName).GetComponent<AudioSource>();

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
            // Calculate desired velocity
            float targetVelocity = _moveDirection.magnitude * _moveSpeed;

            // Find diff between desired velocity and current velocity
            float velocityDif = targetVelocity - _rb.velocity.magnitude;

            // Check whether to accel or deccel
            float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? _moveAccel :
                _moveDecel;

            // Calc force by multiplying accel and velocity diff, and applying velocity power
            float movementForce = Mathf.Pow(Mathf.Abs(velocityDif) * accelRate, _velocityPower)
                * Mathf.Sign(velocityDif);

            _rb.AddForce(movementForce * _moveDirection, ForceMode2D.Impulse);
        }
    }

    // Change between visible and 'hiding'
    public void HidingSprite()
    {
        _obscuredSound.Play();
        _spriteRenderer.sprite = _hidingSprite;
    }

    public void NormalSprite()
    {
        _visibleSound.Play();
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
        RegionState = input;
        if (RegionState == 3)
        {
            EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
            if (!_isPlayerHiding)
            {
                HidingSprite();
                _isPlayerHiding = true;
            }
        }
        else 
        {
            if (_isPlayerHiding)
            {
                NormalSprite();
                _isPlayerHiding = false;
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

        if (moveBool && _rb.velocity != Vector2.zero)
        {
            if (RegionState == 3)
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
            _moveDirection = (Vector2)data;
            if (_moveDirection == Vector2.zero)
            {
                _rb.velocity = new Vector2(0, 0);
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
