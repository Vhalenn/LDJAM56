using UnityEngine;
using UnityEngine.Events;

public class ActivateWithKeyPress : MonoBehaviour
{
    [SerializeField] KeyCode keyCode;
    [SerializeField] string btnName = string.Empty;

    [Header("Action")]
    [SerializeField] UnityEvent keyEvent;
    [SerializeField] bool onlyInEditor;

    private void Update()
    {
        if (onlyInEditor && !Application.isEditor) return;

        if(!string.IsNullOrEmpty(btnName) && Input.GetButtonDown(btnName)) Activate();
        else if (Input.GetKeyDown(keyCode)) Activate(); // The action
    }

    private void Activate()
    {
        Debug.Log($"ActivateKeyPress on {transform.name} -> Activate()");
        if (keyEvent != null && keyEvent.GetPersistentEventCount() > 0) keyEvent?.Invoke();
    }

}
