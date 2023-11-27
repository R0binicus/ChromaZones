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

        EventManager.EventInitialise(EventType.PAUSE_TOGGLE);
    }

    public void OnPauseToggle(InputAction.CallbackContext context)
    {
        Debug.Log("HEY");
        if (context.started)
        {
            EventManager.EventTrigger(EventType.PAUSE_TOGGLE, null);
        }
    }
}
