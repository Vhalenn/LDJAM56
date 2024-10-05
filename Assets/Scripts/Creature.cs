using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform model;
    [SerializeField] private GameObject readySignal;

    [Header("Var")]
    [SerializeField] private Vector2 randomSpeed = new Vector2(3f,4f);
    [SerializeField] private float randomizedSpeed;
    [SerializeField] private Vector2 randomScale = new Vector2(0.8f,1.2f); 
    [SerializeField] private Vector2 randomStop = new Vector2(4f, 6f);
    [SerializeField] private float randomizedStop;

    [Header("State")]
    [SerializeField] private CreatureBehaviour behaviour;
    public bool Trapped => behaviour == CreatureBehaviour.Trapped;
    public bool Resting => behaviour == CreatureBehaviour.Rest;
    public bool Delivery => behaviour == CreatureBehaviour.Delivery;
    public bool Following => behaviour == CreatureBehaviour.Follow;

    [Header("Storage")]
    [SerializeField] private ResourceType resourceCarried;
    [SerializeField] int resourceQuantity;

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

        resourceQuantity = 0;

        if (!Trapped) gameDataScriptable.AddCreature(this);
    }

    private void FixedUpdate()
    {
        if (Trapped) return;
        if(readySignal) readySignal.SetActive(NearPlayer);

        if (!gameDataScriptable) return;

        if(resourceQuantity > 0) ChangeBehaviour(CreatureBehaviour.Delivery);
        else if (gameDataScriptable.Night) ChangeBehaviour(CreatureBehaviour.Rest);
        else ChangeBehaviour(CreatureBehaviour.Follow);

        ApplyBehaviour();
    }

    public void Free()
    {
        ChangeBehaviour(CreatureBehaviour.Follow);
        gameDataScriptable.AddCreature(this);
    }

    public void ChangeBehaviourPublic(CreatureBehaviour behaviour) => ChangeBehaviour(behaviour);
    private void ChangeBehaviour(CreatureBehaviour behaviour)
    {
        if (this.behaviour == behaviour) return;
        this.behaviour = behaviour;

        // When camp is destroyed
        if (behaviour == CreatureBehaviour.Rest && gameDataScriptable.Camp == null)
        {
            this.behaviour = CreatureBehaviour.Follow;
        }

        agent.speed = Resting || Delivery ? randomizedSpeed * 1.5f : randomizedSpeed;
        agent.stoppingDistance = Resting || Delivery ? randomizedStop * 0.33f : randomizedStop;
    }

    private Vector3 CampPos => gameDataScriptable.Camp.transform.position;
    private Vector3 GoalPos
    {
        get
        {
            if (gameDataScriptable.Camp != null && (Delivery || Resting)) return CampPos;
            else return Player.transform.position;
        }
    }

    private float GoalDist
    {
        get
        {
            return Vector3.Distance(transform.position, GoalPos);
        }
    }

    public bool NearPlayer => Following && GoalDist < randomizedStop * 1.5f;

    private void ApplyBehaviour()
    {
        switch (behaviour)
        {
            case CreatureBehaviour.Follow:
                agent.SetDestination(GoalPos);
                break;

            case CreatureBehaviour.Rest:
                agent.SetDestination(GoalPos);
                break;

            case CreatureBehaviour.Delivery:
                if(gameDataScriptable.Camp == null) ChangeBehaviour(CreatureBehaviour.Follow);

                float campDistance = Vector3.Distance(transform.position, CampPos);
                if(campDistance > randomizedStop * 1.5f) // Still far
                {
                    agent.SetDestination(CampPos);
                }
                else // Arrived
                {
                    gameDataScriptable.Camp.Delivery(resourceCarried, resourceQuantity);
                    resourceQuantity = 0;
                    ChangeBehaviour(CreatureBehaviour.Follow);
                }

                break;

            default:
                break;

        }
    }

    // ACTIONS
    public void DoDelivery(ResourceType resourceCarried, int resourceQuantity)
    {
        Debug.Log($"{transform.name} -> DoDelivery({resourceCarried},{resourceQuantity}");

        this.resourceCarried = resourceCarried;
        this.resourceQuantity = resourceQuantity;
        ChangeBehaviour(CreatureBehaviour.Delivery);
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
    Trapped, // When not available at start
    Wait,
    Follow,  // Near Player
    Rest,
    Delivery, // When bringing resource to Player
}

