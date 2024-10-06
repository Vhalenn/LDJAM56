using UnityEngine;
using DG.Tweening;

public class Resource : Interactible
{
    [SerializeField] private ResourceDataScriptable data;
    [SerializeField] private ResourceType type => data.Type;
    [SerializeField] private Vector2Int quantity => data.Quantity;
    [SerializeField] private float respawnChance; public float RespawnChance => respawnChance;

    [Header("Elements")]
    [SerializeField] private GameObject model;

    [Header("Storage")]
    private Tween tween;

    private void Start()
    {
        float respawnChance = data.GetRespawnChance();
    }

    public override int RequireCreature()
    {
        return 1;
    }

    public override string UIText()
    {
        return $"<sprite name={data.Type}>";
    }

    public override void SetState(Player player, bool state)
    {
        tween?.Kill();

        if(player && state) // Try to use it
        {
            player.AddResource(data);
            player.RemoveInteractible(this);
        }

        tween = model.transform.DOScale(state ? 0.01f : 1f, state ? 0.15f : 1f);
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