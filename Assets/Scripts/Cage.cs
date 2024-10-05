using UnityEngine;

public class Cage : Interactible
{
    [SerializeField] private Creature[] creatureArray;
    [SerializeField] private GameObject door;

    public override string UIText()
    {
        return "Unlock";
    }

    public override void SetState(Player player, bool state)
    {
        if (used && state) return;

        door.SetActive(!state);
        for (int i = 0; i < creatureArray.Length; i++)
        {
            creatureArray[i].Free();
        }
        used = state;
    }
}
