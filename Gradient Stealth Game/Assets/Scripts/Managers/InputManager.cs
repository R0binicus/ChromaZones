using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, InputActions.IGameplayActions
{
    InputActions _inputs;

    void Awake()
    {
        _inputs = new InputActions();
        _inputs.Gameplay.SetCallbacks(this);
        _inputs.Gameplay.Enable();
        _inputs.Gameplay.Move.performed += MovingTrue;
        _inputs.Gameplay.Move.canceled += MovingFalse;

        EventManager.EventInitialise(EventType.PAUSE_TOGGLE);
        EventManager.EventInitialise(EventType.PLAYER_MOVE_BOOL);
        EventManager.EventInitialise(EventType.PLAYER_MOVE_VECT2D);
    }

    public void OnPauseToggle(InputAction.CallbackContext context)
    {
        Debug.Log("HEY");
        if (context.started)
        {
            EventManager.EventTrigger(EventType.PAUSE_TOGGLE, null);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_MOVE_VECT2D, _inputs.Gameplay.Move.ReadValue<Vector2>());
    }

    private void MovingTrue(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_MOVE_BOOL, true);
    }

    private void MovingFalse(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_MOVE_BOOL, false);
    }
}
