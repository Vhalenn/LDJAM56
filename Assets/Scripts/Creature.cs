using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform model;

    [Header("Var")]
    [SerializeField] private Vector2 randomSpeed = new Vector2(3f,4f);
    [SerializeField] private float randomizedSpeed;
    [SerializeField] private Vector2 randomScale = new Vector2(0.8f,1.2f); 
    [SerializeField] private Vector2 randomStop = new Vector2(4f, 6f);
    [SerializeField] private float randomizedStop;

    [Header("State")]
    [SerializeField] private CreatureBehaviour behaviour;
    private bool Resting => behaviour == CreatureBehaviour.Rest;


    private Player player;
    private Player Player
    {
        get
        {
            if (!player) player = gameDataScriptable.Player;
            return player;
        }
    }
    
    private void Start()
    {
        model.localScale = Vector3.one * Random.Range(randomScale.x, randomScale.y);

        randomizedSpeed = Random.Range(randomSpeed.x, randomSpeed.y);
        agent.speed = randomizedSpeed;

        randomizedStop = Random.Range(randomStop.x, randomStop.y);
        agent.stoppingDistance = randomizedStop;
    }

    private void FixedUpdate()
    {
        if (!gameDataScriptable) return;


        if(gameDataScriptable.Night) ChangeBehaviour(CreatureBehaviour.Rest);
        else ChangeBehaviour(CreatureBehaviour.Follow);

        ApplyBehaviour();
    }

    private void ChangeBehaviour(CreatureBehaviour behaviour)
    {
        if (this.behaviour == behaviour) return;
        this.behaviour = behaviour;

        agent.speed = Resting ? randomizedSpeed * 1.5f : randomizedSpeed;
        agent.stoppingDistance = Resting ? randomizedStop * 0.33f : randomizedStop;
    }     

    private void ApplyBehaviour()
    {
        switch (behaviour)
        {
            case CreatureBehaviour.Follow:
                if (!Player) return;
                agent.SetDestination(Player.transform.position);
                break;

            case CreatureBehaviour.Rest:
                agent.SetDestination(gameDataScriptable.Camp.transform.position);
                break;

            default:
                break;

        }
    }
}


public enum CreatureType
{
    Leaf,
    Branch,
    Rock,
}

public enum CreatureBehaviour
{
    Wait,
    Follow,
    Rest,
}

