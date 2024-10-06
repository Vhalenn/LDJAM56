using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataScriptable", menuName = "Scriptable Objects/GameDataScriptable")]
public class GameDataScriptable : ScriptableObject
{
    [Header("Main Actors")]
    [SerializeField] private GameManager game; public GameManager Game { get => game; set => game = value; }
    [SerializeField] private Player player; public Player Player { get => player; set => player = value; }
    [SerializeField] private MainMenuUI ui; public MainMenuUI UI { get => ui; set => ui = value; }
    [SerializeField] private BaseCamp camp; public BaseCamp Camp { get => camp; set => camp = value; }

    [Header("World")]
    [SerializeField, Range(0.0f, 1.0f)] private float currentTime; 
    public float CurrentTime { get => currentTime; set => currentTime = value; }
    [SerializeField] private float totalTime;
    public float TotalTime { get => totalTime; set => totalTime = value; }

    [SerializeField] private bool night;
    public bool Night { get => night; set => night = value; }

    [Header("Team")]
    public int CreatureCount => game.CreatureCount;
    public Creature GetFirstCreature => game.FirstCreature;


    [Header("Wind")]
    [SerializeField] private float windForce;
    public float WindForce { get => windForce; set => windForce = value; }
    [SerializeField] private float foodLevel; public float FoodLevel { get => foodLevel; set => foodLevel = value; }
    [SerializeField] private float campLevel; public float CampLevel { get => campLevel; set => campLevel = value; }

    public void Clean()
    {
        game = null;
        player = null;
        ui = null;
        camp = null;

        currentTime = 0;
        totalTime = 0;
        night = false;

        windForce = 0;
    }

    public void AddCreature(Creature creature) => game.AddCreature(creature);
    public Creature FindAvailableCreature() => game.FindAvailableCreature();

    public int QuantityPlayerHas(CreatureType creaType) => game.QuantityPlayerHas(creaType);

    public int ResourceQuantityPlayerHas(ResourceType resourceType)
    {
        int quantityPlayerHas = 0;
        if (camp && camp.ResourcesDico.ContainsKey(resourceType))
        {
            quantityPlayerHas += camp.ResourcesDico[resourceType];
        }
        if (player && player.ResourcesDico.ContainsKey(resourceType))
        {
            quantityPlayerHas += player.ResourcesDico[resourceType];
        }

        return quantityPlayerHas;
    }

    public bool HasResource(ResourceType resourceType, int quantity)
    {
        return ResourceQuantityPlayerHas(resourceType) >= quantity;
    }

    public void RemovePlayerResource(ResourceType resourceType, int quantity)
    {
        int quantityStillNeeded = quantity;
        if (camp && camp.ResourcesDico.ContainsKey(resourceType))
        {
            if(camp.ResourcesDico[resourceType] >= quantity) // Enough in camp
            {
                camp.ResourcesDico[resourceType] -= quantity;
                quantityStillNeeded = 0;
            }
            else // Need to also remove in Player Inventory
            {
                quantityStillNeeded = quantity - camp.ResourcesDico[resourceType];
                camp.ResourcesDico[resourceType] = 0;

            }
            
        }

        camp.UpdateUIInfos();

        if (quantityStillNeeded <= 0) return; // Enough in camp

        if (player && player.ResourcesDico.ContainsKey(resourceType))
        {
            if (player.ResourcesDico[resourceType] >= quantity) // Enough
            {
                player.ResourcesDico[resourceType] -= quantity;
            }
            else // Need to also remove in Player Inventory
            {
                player.ResourcesDico[resourceType] = 0;
                Debug.LogError("Tried to remove more than what players has");

            }
        }
        else
        {
            Debug.LogError("Tried to remove more than what players has");
        }

        player.UpdateBackpack();
    }
}
