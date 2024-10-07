using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool mainMenu;
    [SerializeField] private bool peacefulGameMode; // Peaceful / Resource managment

    [Header("Main")]
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private Player player; public Player Player => player;
    [SerializeField] private MainMenuUI menuUI; public MainMenuUI MenuUI => menuUI;
    [SerializeField] private ResourceManager resourceManager; public ResourceManager ResourceManager => resourceManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Transform spawnPos;
    public Vector3 SpawnPos => spawnPos ? spawnPos.position : Vector3.up;

    [Header("MainMenu")]
    [SerializeField] private LayerMask raycastLayerMask;

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
    private Camera mainCam;

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
        if(mainMenu && player)
        {
            if (!mainCam) mainCam = Camera.main;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 300, raycastLayerMask))
            {
                player.transform.position = hit.point;
                //Debug.Log(hit.transform.name);
            }
        }
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
        audioManager.NightStart();
    }

    public void NewDay()
    {
        if(resourceManager) resourceManager.NewDay();
        audioManager.NewDay();
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

    // HELP
    public Vector3 GetNearestLockedCage(Vector3 pos)
    {
        Vector3 cagePos = pos;
        float minDist = float.MaxValue;

        for (int i = 0; i < cageArray.Length; i++)
        {
            if (cageArray[i] == null || cageArray[i].Used) continue;

            float dist = Vector3.Distance(cageArray[i].transform.position, pos);
            if(dist < minDist)
            {
                minDist = dist;
                cagePos = cageArray[i].transform.position;
            }
        }

        return cagePos;
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

        if(creatureList.Count >= TotalCreatureOnMap)
        {
            ShowEndScreen(true);
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

    public void ShowEndScreen(bool win)
    {
        if (mainMenu) return;

        gameDataScriptable.GameIsEnded = true;
        if(menuUI) menuUI.ShowEndScreen(win);
    }
}
