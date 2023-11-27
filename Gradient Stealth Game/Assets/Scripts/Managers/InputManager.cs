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
        //Debug.Log("OnMove");
        //if (context.started)
        //{
        //    //EventManager.EventTrigger(EventType.PAUSE_TOGGLE, null);
        //}
    }

    private void MovingTrue(InputAction.CallbackContext context)
    {
        Debug.Log("MovingTrue");
        EventManager.EventTrigger(EventType.PLAYER_MOVE_BOOL, true);
        //if (regionState == 3)
        //{
        //    EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
        //}
        //else
        //{
        //    EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, true);
        //}
    }

    private void MovingFalse(InputAction.CallbackContext context)
    {
        Debug.Log("MovingFalse");
        EventManager.EventTrigger(EventType.PLAYER_MOVE_BOOL, false);
        //EventManager.EventTrigger(EventType.COLOUR_CHANGE_BOOL, false);
    }
}
