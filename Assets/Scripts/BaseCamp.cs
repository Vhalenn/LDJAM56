using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class BaseCamp : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private TextMeshPro resourceText;
    [SerializeField] private GameObject whenActive;
    [SerializeField] private Transform metalPlateParent;

    [Header("Var")]
    [SerializeField] private int campIndex = 0; public int CampIndex => campIndex;
    [SerializeField] private float maxLifePoints = 1000; public float MaxLifePoints => maxLifePoints;
    private float foodConsumedRate = 0.075f;
    private float woodConsumedRate = 0.02f;
    private float damageRate = 0.12f;

    [Header("Storage")]
    [SerializeField] private bool activeCamp;
    [SerializeField] private float lifePoints = 0; public float LifePoints => lifePoints;
    public float FoodLevel
    {
        get
        {
            if (!gameDataScriptable || gameDataScriptable.CreatureCount == 0 || gameDataScriptable.CreatureCount == 0) return 0;
            return FoodCount / (gameDataScriptable.CreatureCount * 0.1f);
        }
    }

    public float WoodLevel // NOT USED
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
        if (campIndex == 0) SetBaseCamp();

        lifePoints = maxLifePoints;
        UpdateUIInfos();
    }

    private void Update()
    {
        if (!gameDataScriptable) return;

        activeCamp = gameDataScriptable.Camp == this;
        whenActive.SetActive(activeCamp);
        if (!activeCamp) return;

        if(gameDataScriptable.Night) // Consume food
        {
            int creaCount = gameDataScriptable.CreatureCount;
            bool changed = false;

            // FOOD LOOP
            changed |= ResourceLifeCheck(ResourceType.Food, ref foodConsumed, foodConsumedRate, creaCount);
            //changed |= ResourceLifeCheck(ResourceType.Wood, ref woodConsumed, woodConsumedRate, creaCount);
            // wood consumtion feels harsh

            if (changed) UpdateUIInfos();

            if(!Usable) // CAMP DESTROYED
            {
                foreach(KeyValuePair<ResourceType, int> pair in resourcesDico)
                {
                    gameDataScriptable.Player.AddResource(pair.Key, pair.Value);
                }
                resourcesDico = new();

                gameDataScriptable.CampInfoData = string.Empty;
                gameDataScriptable.Camp = null;
                gameObject.SetActive(false);
                // Message : Ho no the camp have been destroyed, seek another one
            }
        }
    }

    private bool ResourceLifeCheck(ResourceType type, ref float consumed, float consumeRate, int creaCount)
    {
        bool changed = false;

        consumed += Time.deltaTime * consumeRate * creaCount;
        if (consumed >= 1)
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
                lifePoints -= Time.deltaTime * damageRate * creaCount;
                if (activeCamp && lifePoints < 0) gameDataScriptable.Game.ShowEndScreen(false); // Loose
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

    [ContextMenu("Update UI")]
    public void UpdateUIInfos()
    {
        if (!activeCamp)
        {
            if (resourceText) resourceText.text = string.Empty;
            return;
        }

        int lifePercentage = LifePointPercent;
        if (resourceText)
        {
            resourceText.text = Utility.GetResourceAmountText(resourcesDico, 0) + $"\n   {lifePercentage}%";
            gameDataScriptable.CampInfoData = resourceText.text;
        }

        if(metalPlateParent)
        {
            int childCount = metalPlateParent.childCount;
            for (int i = 0; i < metalPlateParent.childCount; i++)
            {
                float elementPercentage = (i / (float)childCount)*100.0f;
                metalPlateParent.GetChild(i).gameObject.SetActive(lifePercentage > elementPercentage);
            }
        }


        gameDataScriptable.FoodLevel = FoodLevel;
        gameDataScriptable.CampLevel = LifePointPercent;
    }

    private void SetBaseCamp()
    {
        if(gameDataScriptable.Camp != null && gameDataScriptable.Camp != this)
        {
            gameDataScriptable.Camp.gameObject.SetActive(false); // Remove previous camp
        }

        gameDataScriptable.Camp = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Usable) return;

        if(other.TryGetComponent(out Player player)) // If this is the player
        {
            
            SetBaseCamp();

            SerializedDictionnary<ResourceType, int> playerResources = player.ResourcesDico;

            foreach(KeyValuePair<ResourceType, int> pair in playerResources)
            {
                Delivery(pair.Key, pair.Value);
            }

            player.ClearRessourceDico();
        }
    }
}
