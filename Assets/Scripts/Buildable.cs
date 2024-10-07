using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Buildable : Interactible
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject navMeshBlocker;
    [SerializeField] private bool useRigidbody;
    [SerializeField] private bool reverseModelState;

    [Header("Requirement")]
    [SerializeField] private ResourceDataScriptable requirementRessource;
    [SerializeField] private int requirementQuantity;

    [SerializeField] private CreatureType requiredCreatureType;
    [SerializeField] private int requiredCreatureQuantity;

    private void Start()
    {
        SetState(null, false);
    }

    public override int RequireCreature()
    {
        return 0;
    }

    public override string UIText()
    {
        if(requirementRessource == null) return $"<sprite name=Hammer>";
        string color;
        string text;

        // Check player creature number
        int playerCrea = gameDataScriptable.QuantityPlayerHas(requiredCreatureType);
        if(playerCrea < requiredCreatureQuantity)
        {
            color = playerCrea > requirementQuantity ? "a4ffaa" : "ffa4b2";

            text = $"<color=#{color}>{requiredCreatureQuantity}</color><size=75%>({playerCrea})</size> <sprite name=crea_{requiredCreatureType}>";
            return text;
        }
        // ELSE -> player has enough creatures

        if(requirementQuantity <= 0)
        {
            text = $"<sprite name=Hammer> > <sprite name=Explode>";
            return text;
        }
        // Else -> Need ressources

        // Check player ressources
        int playerQuantity = gameDataScriptable.ResourceQuantityPlayerHas(requirementRessource.Type);
        color = playerQuantity > requirementQuantity ? "a4ffaa" : "ffa4b2";

        text = $"<sprite name=Hammer><sprite name=Arrow><color=#{color}>{requirementQuantity}</color><size=75%>({playerQuantity})</size> <sprite name={requirementRessource.Type}>";
        return text;
    }

    public override void SetState(Player player, bool state)
    {
        if (used && state) return;

        if(state)
        {
            // Check if player has enought resources
            if (!player) return;

            int playerCrea = gameDataScriptable.QuantityPlayerHas(requiredCreatureType);
            if(playerCrea < requiredCreatureQuantity)
            {
                Debug.Log($"Players has {playerCrea} vs {requiredCreatureQuantity} needed");
                return;
            }

            int playerQuantity = gameDataScriptable.ResourceQuantityPlayerHas(requirementRessource.Type);

            if (gameDataScriptable.HasResource(requirementRessource.Type, requirementQuantity))
            {
                Debug.Log($"Player has {playerQuantity} resource -> Removing {requirementQuantity}");
                gameDataScriptable.RemovePlayerResource(requirementRessource.Type, requirementQuantity);
            }
            else // Not enough quantity
            {
                return;
            }
        }

        if(player) player.RemoveInteractible(this);

        if(useRigidbody)
        {
            if(navMeshBlocker) navMeshBlocker.SetActive(reverseModelState ? !state : state);

            Rigidbody[] rigidBodyArray = model.GetComponentsInChildren<Rigidbody>();

            foreach(Rigidbody rigidbody in rigidBodyArray)
            {
                rigidbody.isKinematic = !state;

                if(state)
                {
                    rigidbody.AddExplosionForce(50, transform.position, 15);
                    rigidbody.transform.DOScale(0.01f, 15f).OnComplete(() => rigidbody.gameObject.SetActive(false));
                }
            }
        }
        else
        {
            if(reverseModelState) model.SetActive(!state);
            else model.SetActive(state);
        }
        used = state;

    }
}
