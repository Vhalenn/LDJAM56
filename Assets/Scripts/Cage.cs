using UnityEngine;

public class Cage : Interactible
{
    [SerializeField] private Creature[] creatureArray;
    [SerializeField] private GameObject door;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    public override string UIText()
    {
        return "Unlock";
    }

    public override void SetState(Player player, bool state)
    {
        if (used && state) return;

        door.SetActive(!state);
        for (int i = 0; i < creatureArray.Length; i++)
        {
            creatureArray[i].Free();
        }


        if (audioSource && audioClip)
        {
            audioSource.volume = Random.Range(0.5f, 0.9f);
            audioSource.pitch = Random.Range(0.6f, 0.9f);
            audioSource.PlayOneShot(audioClip);
        }

        player.RemoveInteractible(this);
        used = state;
    }
}
