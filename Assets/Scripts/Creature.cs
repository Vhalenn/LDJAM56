using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform model;
    [SerializeField] private GameObject readySignal;

    [Header("Var")]
    [SerializeField] private CreatureType type; public CreatureType Type => type;
    [SerializeField] private Vector2 randomSpeed = new Vector2(3f,4f);
    [SerializeField] private float randomizedSpeed;
    [SerializeField] private Vector2 randomScale = new Vector2(0.8f,1.2f); 
    [SerializeField] private Vector2 randomStop = new Vector2(4f, 6f);
    [SerializeField] private float randomizedStop;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] actionAudioClip;

    [Header("State")]
    [SerializeField] private CreatureBehaviour behaviour;
    public bool Trapped => behaviour == CreatureBehaviour.Trapped;
    public bool Resting => behaviour == CreatureBehaviour.Rest;
    public bool Delivery => behaviour == CreatureBehaviour.Delivery;
    public bool Following => behaviour == CreatureBehaviour.Follow;

    [Header("Storage")]
    [SerializeField] private ResourceType resourceCarried;
    [SerializeField] int resourceQuantity;
    private bool CampExists => gameDataScriptable.Camp;

    private Player player;
    private Player Player
    {
        get
        {
            if (!player) player = gameDataScriptable.Player;
            return player;
        }
    }

    public int CapacityCarried
    {
        get
        {
            if (type == CreatureType.Leaf) return 4;
            if (type == CreatureType.Branch) return 6;
            else return 15;
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
        readySignal.SetActive(false);

        if (!Trapped) gameDataScriptable.AddCreature(this);
    }

    private void FixedUpdate()
    {
        if (Trapped) return;
        if(readySignal) readySignal.SetActive(NearPlayer);

        if (!gameDataScriptable) return;

        if(resourceQuantity > 0) ChangeBehaviour(CreatureBehaviour.Delivery);
        else if (CampExists && gameDataScriptable.Night) ChangeBehaviour(CreatureBehaviour.Rest);
        else ChangeBehaviour(CreatureBehaviour.Follow);

        ApplyBehaviour();
    }

    public void Free()
    {
        ChangeBehaviour(CreatureBehaviour.Follow);
        gameDataScriptable.AddCreature(this);

        PlaySound(actionAudioClip);
    }

    public void ChangeBehaviourPublic(CreatureBehaviour behaviour) => ChangeBehaviour(behaviour);
    private void ChangeBehaviour(CreatureBehaviour behaviour)
    {
        if (this.behaviour == behaviour) return;
        this.behaviour = behaviour;

        // When camp is destroyed
        if (behaviour == CreatureBehaviour.Rest && !CampExists)
        {
            this.behaviour = CreatureBehaviour.Follow;
        }

        agent.speed = Resting || Delivery ? randomizedSpeed * 1.5f : randomizedSpeed;
        agent.stoppingDistance = Resting || Delivery ? randomizedStop * 0.33f : randomizedStop;
    }

    private Vector3 CampPos
    {
        get
        {
            if (gameDataScriptable.Camp) return gameDataScriptable.Camp.transform.position;
            else return Player.transform.position;
        }
    }
    private Vector3 GoalPos
    {
        get
        {
            if (CampExists && (Delivery || Resting)) return CampPos;
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
                //if(gameDataScriptable.Camp == null) ChangeBehaviour(CreatureBehaviour.Follow);

                if(!CampExists)
                {
                    agent.SetDestination(GoalPos);
                }
                else
                {
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

        // Audio
        PlaySound(actionAudioClip);

        // Do a jump
    }
    
    // Audio
    private void PlaySound(AudioClip[] audioClipArray)
    {
        if (audioClipArray == null || audioClipArray.Length == 0) return;

        audioSource.volume = Random.Range(0.1f, 0.5f);
        audioSource.pitch = Random.Range(0.8f, 1.3f);
        audioSource.PlayOneShot(audioClipArray[Random.Range(0, audioClipArray.Length - 1)]);
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

