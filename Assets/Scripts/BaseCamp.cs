using System.Collections.Generic;
using UnityEngine;

public class BaseCamp : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;

    [Header("Var")]
    [SerializeField] private int campIndex = 0; public int CampIndex => campIndex;
    [SerializeField] private int maxLifePoints = 1000; public int MaxLifePoints => maxLifePoints;

    [Header("Storage")]
    [SerializeField] private int lifePoints = 0; public int LifePoints => lifePoints;

    [SerializeField] private SerializedDictionnary<ResourceType, int> resourcesDico = new();

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
    }

    private void Update()
    {
        
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
        }
    }
}
