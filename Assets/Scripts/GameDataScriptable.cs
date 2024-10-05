using UnityEngine;

[CreateAssetMenu(fileName = "GameDataScriptable", menuName = "Scriptable Objects/GameDataScriptable")]
public class GameDataScriptable : ScriptableObject
{
    [Header("Main Actors")]
    [SerializeField] private Player player; public Player Player { get => player; set => player = value; }
    [SerializeField] private BaseCamp camp; public BaseCamp Camp { get => camp; set => camp = value; }

    [Header("World")]
    [SerializeField, Range(0.0f, 1.0f)] private float currentTime; 
    public float CurrentTime { get => currentTime; set => currentTime = value; }
    [SerializeField] private float totalTime;
    public float TotalTime { get => totalTime; set => totalTime = value; }

    [SerializeField] private bool night;
    public bool Night { get => night; set => night = value; }

    [Header("Team")]
    [SerializeField] private int creatureCount;
    public int CreatureCount { get => creatureCount; set => creatureCount = value; }
}
