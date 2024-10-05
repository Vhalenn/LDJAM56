using UnityEngine;
using DG.Tweening;

public class Resource : Interactible
{
    [SerializeField] private ResourceDataScriptable data;
    [SerializeField] private ResourceType type => data.Type;
    [SerializeField] private Vector2Int quantity => data.Quantity;

    [Header("Elements")]
    [SerializeField] private GameObject model;

    [Header("Storage")]
    private Tween tween;

    public override bool RequireCreature()
    {
        return true;
    }

    public override string UIText()
    {
        return $"Grab [{data.Type}]";
    }

    public override void SetState(Player player, bool state)
    {
        tween?.Kill();

        if(player && state) // Try to use it
        {
            player.AddResource(data);
            player.RemoveInteractible(this);
        }

        tween = model.transform.DOScale(state ? 0.01f : 1f, 0.15f);
        used = state;
    }
}

public enum ResourceType
{
    Food,  // To feed the creatures
    Metal, // To protect the shelter
    Wood,  // To build bridges to go further, to stay warm
    Rocks, // To build staircase
    Flint, // To keep warm ++
}