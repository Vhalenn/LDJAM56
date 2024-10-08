using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform backpackTr;
    [SerializeField] private TextMeshPro resourceText;
    [SerializeField] private AnimCharacterTween animTween;

    [Header("Param")]
    [SerializeField] private float maxAcce = 5;
    [SerializeField] private float maxSpeed = 5;
    [SerializeField, Range(0f,1f)] private float rotSpeed = 0.5f;

    [Header("Interactions")]
    [SerializeField] private SerializedDictionnary<IInteractible, GameObject> interactionDico = new();
    [SerializeField] private IInteractible actualTarget;
    [SerializeField] private string interactibleText;
    [SerializeField] private SerializedDictionnary<ResourceType, int> resourcesDico = new();
    public SerializedDictionnary<ResourceType, int> ResourcesDico => resourcesDico;

    public int QuantityCarried => Utility.GetDictionaryQuantity(resourcesDico);

    private const int MAX_QUANTITY_CARRIED_PLAYER = 30;
    public bool IsFull => QuantityCarried >= MAX_QUANTITY_CARRIED_PLAYER;
    private Vector3 SPAWN_POS => gameDataScriptable.Game.SpawnPos;

    [Header("Arrow")]
    [SerializeField] private Transform helpArrow;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    [Header("Storage")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject[] backpackObjects;
    private Transform camTransform;
    [SerializeField] private Vector2 playerInput;
    [SerializeField] private Vector3 velocity;
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    [SerializeField] private Vector3 cageDir;

    private void Start()
    {
        if(!mainCam) mainCam = Camera.main;

        backpackObjects = new GameObject[backpackTr.childCount];
        for (int i = 0; i < backpackObjects.Length; i++)
        {
            backpackObjects[i] = backpackTr.GetChild(i).gameObject;
        }
        UpdateBackpack();
    }


    private void Update()
    {
        if (transform.position.y < -5.0f)
        {
            Respawn();
        }

        if(audioSource)
        {
            audioSource.volume = velocity.magnitude / maxSpeed;
        }

        if (!gameDataScriptable || gameDataScriptable.GameIsEnded)
        {
            velocity = Vector3.zero;
            return;
        }

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);

        SetUITargetText();
        if (Input.GetButtonDown("Use")) UseInteractible();

        AskForDelivery();

        HelpUpdate();
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + cageDir);
    }

    private void FixedUpdate()
    {
        MoveChara();
    }

    private void Respawn()
    {
        if (gameDataScriptable && gameDataScriptable.Camp)
        {
            transform.position = gameDataScriptable.Camp.transform.position + Vector3.up;
            return;
        }

        if (gameDataScriptable && gameDataScriptable.CreatureCount > 0)
        {
            Creature crea = gameDataScriptable.GetFirstCreature;
            if(crea)
            {
                transform.position = crea.transform.position + Vector3.up;
            }
        }

        transform.position = SPAWN_POS;
    }

    // Movement
    #region Movement

    private void MoveChara() // On Fixed Update
    {
        velocity = rb.linearVelocity;

        Vector3 camDir = FollowingCamAngle(playerInput);
        AdjustVelocity(camDir * maxSpeed);

        if (playerInput.magnitude > 0.01f)
        {
            rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(velocity, Vector3.up), rotSpeed);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        rb.linearVelocity = velocity;
        animTween.VelMagnitude = velocity.magnitude;
    }

    private void AdjustVelocity(Vector3 desiredVelocity)
    {/*
        if (!OnGround && noAirControl)
        {
            return;
            // This function was preventing doing a clean parabol in the air
        }
        */
        
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        
        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        //float acceleration = OnGround ? maxAcce;
        float maxSpeedChange = maxAcce * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    // Utility

    private Vector3 ContactNormal => Vector3.up;

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - ContactNormal * Vector3.Dot(vector, ContactNormal);
    }
    

    public Vector3 GetCamDir()
    {
        if (!mainCam) return Vector3.zero;
        if (!camTransform) camTransform = mainCam.transform;
        if (!camTransform) return Vector3.zero;

        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward; // forward
    }

    private Vector3 FollowingCamAngle(Vector2 input)
    {
        if (!mainCam) return Vector3.zero;
        if (camTransform == null) camTransform = mainCam.transform;
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 dir = forward * input.y + right * input.x;
        return dir;
    }
    #endregion
    // End - Movement

    // Interactions
    #region Interactions
    private bool DicoEmpty => (interactionDico == null || interactionDico.Count <= 0);

    public void AddInteractible(IInteractible interactible, GameObject gO) // When enter a new Interactible
    {
        if (interactible == null || gO == null) return;
        interactionDico.TryAdd(interactible, gO);

        UpdateUITarget();
    }

    public void RemoveInteractible(IInteractible interactible)
    {
        if (interactible == null) return;

        if (interactionDico.ContainsKey(interactible))
        {
            interactionDico.Remove(interactible);
        }

        UpdateUITarget();
    }

    private void UpdateUITarget() 
    {
        if (!DicoEmpty)
        {
            KeyValuePair<IInteractible, GameObject> pair = interactionDico.First();
            GameObject target = pair.Value;
            if (target == null) return;

            gameDataScriptable.UI.StickToTarget.Attach(target.transform, Vector3.one);

            interactibleText = pair.Key.UIText();
        }
        else
        {
            gameDataScriptable.UI.StickToTarget.Attach(null, Vector3.one);
            interactibleText = string.Empty;
        }
    }

    private void SetUITargetText() // Called really often
    {
        if (DicoEmpty) return;
        bool requireCrea = interactionDico.First().Key.RequireCreature() > 0;

        if(!requireCrea)
        {
            SetUITargetText(interactibleText);
        }
        else if (IsFull && gameDataScriptable.FindAvailableCreature() == null)// If dont need crea OR have creatures around
        {
            SetUITargetText("Full");
        }
        else
        {
            SetUITargetText(interactibleText);
        }
    }

    private void SetUITargetText(string text)
    {
        gameDataScriptable.UI.StickToTarget.SetText(text);
    }

    private void UseInteractible()
    {
        if (DicoEmpty) return;

        IInteractible interactible = interactionDico.First().Key;
        if (interactible.RequireCreature() > 0 && IsFull)
        {
            SetUITargetText("Full");
            return;
        }

        Debug.Log("Player.UseInteractible()");

        if(interactible != null) interactible.SetState(this, true);

        UpdateUITarget();
    }

    public void AddResource(ResourceDataScriptable ressource)
    {
        if (ressource == null) return;
        AddResource(ressource.Type, ressource.GetRandomQuantity());

        // Play sound
        if (audioSource && audioClip)
        {
            audioSource.volume = Random.Range(0.01f, 0.25f);
            audioSource.pitch = Random.Range(0.8f, 1.5f);
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void AddResource(ResourceType type, int quantity) // When camp is destroyed
    {
        if (resourcesDico == null) resourcesDico = new();

        if (resourcesDico.ContainsKey(type))
        {
            resourcesDico[type] += quantity;
        }
        else
        {
            resourcesDico.Add(type, quantity);
        }

        UpdateBackpack();
    }

    private void AskForDelivery()
    {
        if (QuantityCarried <= 0/* || gameDataScriptable.Camp == null*/) return;
        
        // Can do delivery
        Creature crea = gameDataScriptable.FindAvailableCreature();
        if (!crea) return;

        KeyValuePair<ResourceType, int> toDeliver = resourcesDico.Last();

        int quantity = Mathf.Min(toDeliver.Value, crea.CapacityCarried);
        crea.DoDelivery(toDeliver.Key, quantity);

        resourcesDico[toDeliver.Key] -= quantity;
        if (resourcesDico[toDeliver.Key] <= 0)
        {
            resourcesDico.Remove(toDeliver.Key);
        }

        UpdateBackpack();
        
    }
        

    public void ClearRessourceDico() // Called by camp when going manually
    {
        resourcesDico = new();
        UpdateBackpack();
    }

    #endregion
    // End - Interactions

    // Visuals
    #region Visual
    public void UpdateBackpack()
    {
        int quantity = QuantityCarried;

        if (resourceText)
        {
            resourceText.text = Utility.GetResourceAmountText(resourcesDico, MAX_QUANTITY_CARRIED_PLAYER);
        }

        quantity /= 2;
        for (int i = 0; i < backpackObjects.Length; i++)
        {
            backpackObjects[i].SetActive(i < quantity);
        }
    }

    #endregion
    // END - Visuals

    public void SwitchHelpMode()
    {
        helpArrow.gameObject.SetActive(!helpArrow.gameObject.activeSelf);
    }

    private void HelpUpdate()
    {
        if (!helpArrow.gameObject.activeSelf) return;
        // ELSE - help activated
        
        Vector3 pos = transform.position;
        cageDir = gameDataScriptable.Game.GetNearestLockedCage(pos) - pos;
        cageDir.y = 0;
        helpArrow.rotation = Quaternion.LookRotation(cageDir, Vector3.up);
    }
}
