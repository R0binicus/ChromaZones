using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody2D _rb;
    private ColourManager _colourManager;
    private SpriteRenderer _spriteRenderer;

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
    private bool _canMove;
    [SerializeField] private Sprite _hidingSprite;
    [SerializeField] private Sprite _normalSprite;
    public bool _isCollidingObstacle = false;

    // Sounds
    [Header("Sounds")]
    [SerializeField] private Sound _soundPlayerObscured;
    [SerializeField] private Sound _soundPlayerVisible;
    private bool _startSoundDisabler = true;

    void Awake()
    {
        EventManager.EventInitialise(EventType.COLOUR_CHANGE_BOOL);
        EventManager.EventInitialise(EventType.INIT_PLAYER);

        // Set values and components
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _canMove = true;
        _spriteRenderer.sprite = _normalSprite;
        _startSoundDisabler = false;
        StartCoroutine(SoundDisabler());
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventSubscribe(EventType.LOSE, LoseHandler);
        EventManager.EventSubscribe(EventType.WIN, WinHandler);
        EventManager.EventSubscribe(EventType.PLAYER_MOVE_BOOL, MoveBoolHandler);
        EventManager.EventSubscribe(EventType.PLAYER_MOVE_VECT2D, MoveVect2DHandler);
        EventManager.EventSubscribe(EventType.PLAYER_SPAWNPOINT, SpawnPointHandler);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, StartLevelHandler);
        EventManager.EventSubscribe(EventType.RESTART_LEVEL, ResetHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, LoseHandler);
        EventManager.EventUnsubscribe(EventType.WIN, WinHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_MOVE_BOOL, MoveBoolHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_MOVE_VECT2D, MoveVect2DHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_SPAWNPOINT, SpawnPointHandler);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, StartLevelHandler);
        EventManager.EventUnsubscribe(EventType.RESTART_LEVEL, ResetHandler);
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    void Start()
    {
        EventManager.EventTrigger(EventType.INIT_PLAYER, this);
        EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
    }

    private void FixedUpdate()
    {
        if (_canMove)
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
        if (_startSoundDisabler)
        {
            _startSoundDisabler = false;
        }
        else
        {
            EventManager.EventTrigger(EventType.SFX, _soundPlayerObscured);
        }
        
        _spriteRenderer.sprite = _hidingSprite;
    }

    public void NormalSprite()
    {
        if (_startSoundDisabler)
        {
            _startSoundDisabler = false;
        }
        else
        {
            EventManager.EventTrigger(EventType.SFX, _soundPlayerVisible);
        }
        
        _spriteRenderer.sprite = _normalSprite;
    }

    public void LoseHandler(object data)
    {
        _canMove = false;
        _rb.velocity = Vector2.zero;
        // Player colour gets converted to Enemy!!!
        Color convertedColour = new Color(1f, 0.2983692f, 0.2509804f);
        _spriteRenderer.color = convertedColour;
        StopAllCoroutines();
    }

    public void WinHandler(object data)
    {
        _canMove = false;
        _startSoundDisabler = true;
        StartCoroutine(SoundDisabler());
        StartCoroutine(EventCoroutine(false));
    }

    private void ColourManagerHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("ColourManagerHandler is null");
        }

        _colourManager = (ColourManager)data;
    }

    public void ResetHandler(object data)
    {
        StartCoroutine(SoundDisabler());
    }

    // Reset Player's data
    public void StartLevelHandler(object data)
    {
        StartCoroutine(SoundDisabler());

        // if (RegionState != 3)
        // {
        //     NormalSprite();
        // }
        // else
        // {
        //     HidingSprite();
        // }

        Color colourReset = new Color(0f, 0.0972971f, 0.6f);
        _spriteRenderer.color = colourReset;
        _rb.velocity = Vector3.zero;
        _moveDirection = Vector2.zero;
        _canMove = true;
    }

    // Assign player new starting location when changing/restarting levels
    public void SpawnPointHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player needs starting location!");
        }

        Vector3 startLoc = (Vector3)data;
        transform.position = startLoc;
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

        if (_canMove && gameObject.activeSelf)
        {
            StartCoroutine(EventCoroutine(moveBool));
        }
    }

    private IEnumerator EventCoroutine(bool moveBool)
    {
        if (_isCollidingObstacle && _canMove)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else if (_canMove)
        {
            yield return new WaitForSeconds(0.1f);
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

    private IEnumerator SoundDisabler()
    {
        _startSoundDisabler = true;
        yield return new WaitForSeconds(0.5f);
        _startSoundDisabler = false;
    }

    private void MoveVect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("MoveVect2DHandler is null");
        }

        if (_canMove)
        {
            _rb.velocity = Vector2.zero;
            _moveDirection = (Vector2)data;
            //if (_moveDirection == Vector2.zero)
            //{
            //    _rb.velocity = new Vector2(0, 0);
            //}

            //if (_moveDirection.x == 0)
            //{
            //    _rb.velocity = new Vector2(0, _rb.velocity.y);
            //}
            //if (_moveDirection.y == 0)
            //{
            //    _rb.velocity = new Vector2(_rb.velocity.x, 0);
            //}
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
