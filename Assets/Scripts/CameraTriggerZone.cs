using UnityEngine;
using Unity.Cinemachine;

public class CameraTriggerZone : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vCam;

    [SerializeField] private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            vCam.Priority = 50;
            used = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            vCam.Priority = -50;
            used = false;
        }
    }
}
