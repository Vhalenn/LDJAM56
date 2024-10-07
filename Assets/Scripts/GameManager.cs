using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool gameMode; // Peaceful / 

    [Header("Main")]
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private Player player; public Player Player => player;
    [SerializeField] private MainMenuUI menuUI; public MainMenuUI MenuUI => menuUI;
    [SerializeField] private ResourceManager resourceManager; public ResourceManager ResourceManager => resourceManager;
    [SerializeField] private Transform spawnPos;
    public Vector3 SpawnPos => spawnPos.position;


    [Header("Team")]
    [SerializeField] private int totalCreatureOnMap; public int TotalCreatureOnMap => totalCreatureOnMap;
    [SerializeField] private Cage[] cageArray;
    [SerializeField] private int creatureCount => creatureList.Count;
    public int CreatureCount => creatureCount;
    [SerializeField] private List<Creature> creatureList;
    [SerializeField] private SerializedDictionnary<CreatureType, int> creatureDico;
    public Creature FirstCreature
    {
        get
        {
            if(creatureList == null) creatureList = new List<Creature>();
            if (creatureList.Count == 0) return null;
            return creatureList[0];
        }
    }


    [Header("Player life")]
    [SerializeField] private int maxPlayerLife = 100;
    [SerializeField] private int playerLife = 100;
    private void Awake()
    {
        Instance = this;

        Clean();
        gameDataScriptable.Game = this;
        gameDataScriptable.Player = player;
        gameDataScriptable.UI = menuUI;

        playerLife = maxPlayerLife;
    }

    private void Update()
    {
        
    }

    [ContextMenu("Count all creature")]
    private void GetTotalCreatureOnMap()
    {
        var creatureArray = FindObjectsByType<Creature>(FindObjectsSortMode.InstanceID);
        totalCreatureOnMap = creatureArray.Length;

        cageArray = FindObjectsByType<Cage>(FindObjectsSortMode.InstanceID);
    }

    public void NightStart()
    {
        
    }

    public void NewDay()
    {
        resourceManager.NewDay();
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        Clean();
    }

    public void Clean()
    {
        gameDataScriptable.Clean();
        creatureList = new();
    }

    // TEAM
    public void AddCreature(Creature creature)
    {
        if (creatureList == null) creatureList = new();

        if (!creatureList.Contains(creature))
        {
            if (creatureDico == null) creatureDico = new();

            if (!creatureDico.ContainsKey(creature.Type)) creatureDico.Add(creature.Type, 0);

            creatureDico[creature.Type]++;
            creatureList.Add(creature);
        }
    }

    public int QuantityPlayerHas(CreatureType creaType)
    {
        if (creatureDico == null) creatureDico = new();

        if (!creatureDico.ContainsKey(creaType)) return 0;
        return creatureDico[creaType];
    }

    public Creature FindAvailableCreature()
    {
        if (creatureList == null) creatureList = new();

        for (int i = 0; i < creatureList.Count; i++)
        {
            Creature crea = creatureList[i];
            if (crea == null) continue;
            if (crea.NearPlayer && crea.ResourceQuantity <= 0) return crea;
        }

        return null;
    }
}
