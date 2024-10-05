using UnityEngine;

public interface IInteractible
{
    bool RequireCreature();
    string UIText();
    void SetState(Player player, bool state);
}
