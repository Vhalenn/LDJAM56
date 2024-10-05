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

    public void Clean()
    {
        
    }

    public void AddCreature(Creature creature) => game.AddCreature(creature);
    public Creature FindAvailableCreature() => game.FindAvailableCreature();
}
