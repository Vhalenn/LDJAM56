using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class BaseCamp : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private TextMeshPro resourceText;

    [Header("Var")]
    [SerializeField] private int campIndex = 0; public int CampIndex => campIndex;
    [SerializeField] private float maxLifePoints = 1000; public float MaxLifePoints => maxLifePoints;
    private float foodConsumedRate = 0.025f;
    private float woodConsumedRate = 0.005f;

    [Header("Storage")]
    [SerializeField] private bool activeCamp;
    [SerializeField] private float lifePoints = 0; public float LifePoints => lifePoints;
    public float FoodLevel
    {
        get
        {
            if (!gameDataScriptable || gameDataScriptable.CreatureCount == 0 || gameDataScriptable.CreatureCount == 0) return 0;
            return FoodCount / (gameDataScriptable.CreatureCount * 0.5f);
        }
    }

    public float WoodLevel
    {
        get
        {
            if (!gameDataScriptable || gameDataScriptable.CreatureCount == 0) return 0;
            return WoodCount / (gameDataScriptable.CreatureCount * 0.25f);
        }
    }

    public int LifePointPercent
    {
        get
        {
            return Mathf.CeilToInt((LifePoints / MaxLifePoints) * 100.0f);
        }
    }

    [SerializeField] private float foodConsumed;
    [SerializeField] private float woodConsumed;

    [SerializeField] private SerializedDictionnary<ResourceType, int> resourcesDico = new();
    public SerializedDictionnary<ResourceType, int> ResourcesDico => resourcesDico;

    public int FoodCount => resourcesDico.ContainsKey(ResourceType.Food) ? resourcesDico[ResourceType.Food]:0;
    public int WoodCount => resourcesDico.ContainsKey(ResourceType.Wood) ? resourcesDico[ResourceType.Wood]:0;

    public bool Usable
    {
        get
        {
            return lifePoints >= 0;
        }
    }

    private void Start()
    {
        if(campIndex == 0) gameDataScriptable.Camp = this;

        lifePoints = maxLifePoints;
        UpdateUIInfos();
    }

    private void Update()
    {
        if (!gameDataScriptable) return;

        activeCamp = gameDataScriptable.Camp == this;
        if (!activeCamp) return;

        if(gameDataScriptable.Night) // Consume food
        {
            int creaCount = gameDataScriptable.CreatureCount;
            bool changed = false;

            // FOOD LOOP
            foodConsumed += Time.deltaTime * foodConsumedRate * creaCount;
            woodConsumed += Time.deltaTime * woodConsumedRate * creaCount;
            changed |= ResourceLifeCheck(ResourceType.Food, ref foodConsumed, creaCount);
            changed |= ResourceLifeCheck(ResourceType.Wood, ref woodConsumed, creaCount);

            if (changed) UpdateUIInfos(); ;
        }
    }

    private bool ResourceLifeCheck(ResourceType type, ref float consumed, int creaCount)
    {
        bool changed = false;

        // FOOD LOOP
        consumed += Time.deltaTime * foodConsumedRate * creaCount;
        if (foodConsumed >= 1)
        {
            if (!resourcesDico.ContainsKey(type)) resourcesDico.Add(type, 0);
            changed = true;

            if (resourcesDico[type] > 0)
            {
                consumed = 0;
                resourcesDico[type] -= 1;
            }
            else // Takes damage
            {
                lifePoints -= Time.deltaTime * 3f;
            }
        }

        return changed;
    }

    public void Delivery(ResourceType resourceCarried, int resourceQuantity)
    {
        if (resourceQuantity <= 0) return;

        if(resourcesDico.ContainsKey(resourceCarried))
        {
            resourcesDico[resourceCarried] += resourceQuantity;
        }
        else
        {
            resourcesDico.Add(resourceCarried, resourceQuantity);
        }

        UpdateUIInfos();
    }

    public void UpdateUIInfos()
    {
        if (resourceText)
        {
            resourceText.text = Utility.GetResourceAmountText(resourcesDico, 0) + $"\n\n {LifePointPercent}%";
        }

        gameDataScriptable.FoodLevel = FoodLevel;
        gameDataScriptable.CampLevel = LifePointPercent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Usable) return;

        if(other.TryGetComponent(out Player player)) // If this is the player
        {
            gameDataScriptable.Camp = this;

            SerializedDictionnary<ResourceType, int> playerResources = player.ResourcesDico;

            foreach(KeyValuePair<ResourceType, int> pair in playerResources)
            {
                Delivery(pair.Key, pair.Value);
            }

            player.ClearRessourceDico();
        }
    }
}
