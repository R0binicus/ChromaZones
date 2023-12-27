using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, InputActions.IGameplayActions
{
    InputActions _inputs;

    void Awake()
    {
        _inputs = new InputActions();
        _inputs.Gameplay.SetCallbacks(this);

        EventManager.EventInitialise(EventType.PAUSE_TOGGLE);
        EventManager.EventInitialise(EventType.PLAYER_MOVE_BOOL);
        EventManager.EventInitialise(EventType.PLAYER_MOVE_VECT2D);
    }

    void OnEnable()
    {
        _inputs.Gameplay.Enable();
    }

    void OnDisable()
    {
        _inputs.Gameplay.Disable();
    }

    public void OnPauseToggle(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.EventTrigger(EventType.PAUSE_TOGGLE, null);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_MOVE_VECT2D, _inputs.Gameplay.Move.ReadValue<Vector2>());
        if (context.performed)
        {
            EventManager.EventTrigger(EventType.PLAYER_MOVE_BOOL, true);
        }
        else
        {
            EventManager.EventTrigger(EventType.PLAYER_MOVE_BOOL, false); 
        }
    }

    public void OnMuteMusic(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.EventTrigger(EventType.MUTEMUSIC_TOGGLE, null);
        }
    }
}
