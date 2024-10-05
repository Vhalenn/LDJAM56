using UnityEngine;
using UnityEngine.Events;

public class ActivateWithKeyPress : MonoBehaviour
{
    [SerializeField] KeyCode keyCode;
    [SerializeField] UnityEvent keyEvent;
    [SerializeField] bool onlyInEditor;

    private void Update()
    {
        if (onlyInEditor && !Application.isEditor) return;

        if (Input.GetKeyDown(keyCode)) Activate(); // The action
    }

    private void Activate()
    {
        Debug.Log($"ActivateKeyPress on {transform.name} -> Activate()");
        if (keyEvent != null && keyEvent.GetPersistentEventCount() > 0) keyEvent?.Invoke();
    }

}
