using UnityEngine;

using TMPro;

public class UI_StickToTarget : MonoBehaviour
{
    [SerializeField] Camera worldCam;
    [SerializeField] Canvas _canvas;

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private bool attachedOnUI;
    [SerializeField] private TextMeshProUGUI actionText;

    [Header("Parameters")]
    [SerializeField] private bool adaptToScale = false;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector2 margin;
    [SerializeField] private float offsetWithSecondaryTarget = 1; // Linked with "targetSecondary"
    [Range(0, 1)][SerializeField] private float lerpSpeed = 0.1f; // Linked with "targetSecondary"

    [Header("Storage")]
    [SerializeField] Vector3 targetPos;
    [SerializeField] Vector3 viewportPosition;
    [SerializeField] Vector3 finalPosition;
    [SerializeField] Vector3 offset2D;

    private RectTransform rTransform;
    public RectTransform RTransform
    {
        get
        {
            if (rTransform == null) rTransform = GetComponent<RectTransform>();
            return rTransform;
        }
    }
    [SerializeField] private enum VerPlace { Top, Center, Bottom };
    [SerializeField] private enum HorPlace { Left, Center, Right };

    private void Awake()
    {
        Attach(null, Vector3.zero);
    }

    public void Attach(Transform target, Vector3 offset)
    {
        //if(target) Debug.Log($"Attach to {target.name}");

        this.target = target;
        this.offset = offset;
        attachedOnUI = false;

        gameObject.SetActive(target != null);
    }

    public void SetText(string text)
    {
        if(actionText) actionText.text = text;
    }

    public void ForcePos(Vector2 uiPos)
    {
        RTransform.anchoredPosition = uiPos;
        attachedOnUI = true;
    }


    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        targetPos = target.position + offset;
        viewportPosition = WorldPosToUI(worldCam, _canvas, targetPos);
        
        finalPosition = Vector3.Lerp(finalPosition, viewportPosition, lerpSpeed * 2f);
        

        RTransform.position = finalPosition;
    }

    public static Vector3 WorldPosToUI(Camera _worldCam, Canvas canvas, Vector3 pos)
    {
        return _worldCam.WorldToScreenPoint(pos);
    }

    public static Vector3 ClampToScreenBorders(Vector3 pos)
    {
        float width = Screen.width;
        float height = Screen.height;
        pos.x = Mathf.Clamp(pos.x, 0, width);
        pos.y = Mathf.Clamp(pos.y, 0, height);
        return pos;
    }

    private void OnDrawGizmos()
    {
        if (!target) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
    }

    public bool TargetIsVisible()
    {
        if (target) return target.gameObject.activeInHierarchy;
        else if (attachedOnUI) return true;
        else return false;
    }
}
