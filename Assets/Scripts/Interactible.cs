using UnityEngine;

public class Interactible : MonoBehaviour, IInteractible
{
    [SerializeField] protected bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (other.TryGetComponent(out Player player))
        {
            player.AddInteractible(this, gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.RemoveInteractible(this);
        }
    }

    public virtual bool RequireCreature()
    {
        return false;
    }

    public virtual string UIText()
    {
        return string.Empty;
    }

    public virtual void SetState(Player player, bool state)
    {
        
    }
}
