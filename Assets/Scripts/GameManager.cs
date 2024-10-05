using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Main")]
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private Player player; public Player Player => player;
    [SerializeField] private MainMenuUI menuUI; public MainMenuUI MenuUI => menuUI;


    [Header("Team")]
    [SerializeField] private int creatureCount => creatureList.Count;
    public int CreatureCount => creatureCount;
    [SerializeField] private List<Creature> creatureList;

    private void Awake()
    {
        Instance = this;

        Clean();
        gameDataScriptable.Game = this;
        gameDataScriptable.Player = player;
        gameDataScriptable.UI = menuUI;
    }

    private void Update()
    {
        
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        Clean();
    }

    public void Clean()
    {
        creatureList = new();
    }

    // TEAM
    public void AddCreature(Creature creature)
    {
        if (creatureList == null) creatureList = new();

        if (!creatureList.Contains(creature))
        {
            creatureList.Add(creature);
        }
    }

    public Creature FindAvailableCreature()
    {
        if (creatureList == null) creatureList = new();

        for (int i = 0; i < creatureList.Count; i++)
        {
            if (creatureList[i].NearPlayer) return creatureList[i];
        }

        return null;
    }
}
