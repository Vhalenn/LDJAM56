using System.Collections.Generic;
using UnityEngine;

public class Buildable : Interactible
{
    [SerializeField] private GameObject model;
    
    [SerializeField] private ResourceDataScriptable requirementRessource;
    [SerializeField] private int requirementQuantity;

    private void Start()
    {
        model.SetActive(false);
    }

    public override string UIText()
    {
        if(requirementRessource == null) return $"Build";

        return $"Build [{requirementRessource.Type}] x {requirementQuantity}";
    }

    public override void SetState(Player player, bool state)
    {
        if (used && state) return;

        player.RemoveInteractible(this);
        model.SetActive(state);
        used = state;

    }
}
