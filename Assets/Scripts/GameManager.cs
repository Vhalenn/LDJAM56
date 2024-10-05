using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private Player player; public Player Player => player;
    [SerializeField] private MainMenuUI menuUI; public MainMenuUI MenuUI => menuUI;

    private void Awake()
    {
        Instance = this;

        gameDataScriptable.Player = player;
    }

    private void Update()
    {
        
    }
}
