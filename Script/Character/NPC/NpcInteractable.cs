using UnityEngine;

public class NpcInteractable : MonoBehaviour, IInteractable
{
    public string _npcName;

    public void Interact(PlayerController player)
    {
        Debug.Log($"{_npcName}과(와) 상호작용이 되었다");
    }
    public string GetInteractPrompt()
    {
        return $"{_npcName}에게\n말을 건다";
    }
}
