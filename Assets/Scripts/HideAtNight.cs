using UnityEngine;

public class HideAtNight : MonoBehaviour
{
    [SerializeField] GameDataScriptable gameDataScriptable;
    [SerializeField] ParticleSystem particleSystem;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(particleSystem & gameDataScriptable)
        {
            if (particleSystem.isPlaying && gameDataScriptable.Night) particleSystem.Stop();
            else if (!particleSystem.isPlaying && !gameDataScriptable.Night) particleSystem.Play();
        }
    }
}
