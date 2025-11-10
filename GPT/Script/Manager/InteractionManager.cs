using UnityEngine;

public class InteractionManager : SceneStaticObj<InteractionManager>
{
    IInteractable _currentTarget;

    public void SetTarget(IInteractable target)
    {
        _currentTarget = target;
    }

    public void TryInteract(PlayerController player)
    {
        if (_currentTarget != null)
        {
            _currentTarget.Interact(player);
        }
    }

    public bool HasTarget() { return _currentTarget != null; }

    public string GetPrompt()
    {
        return _currentTarget != null ? _currentTarget.GetInteractPrompt() : string.Empty;
    }
}
