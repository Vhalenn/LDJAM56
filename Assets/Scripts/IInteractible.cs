using UnityEngine;

public interface IInteractible
{
    int RequireCreature();
    string UIText();
    void SetState(Player player, bool state);
}
