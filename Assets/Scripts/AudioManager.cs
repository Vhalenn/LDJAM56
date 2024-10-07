using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameDataScriptable gameDataScriptable;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private AudioClip dayMusic;

    private const float MUSIC_VOLUME = 0.33f;
    bool isPlaying = false;
    Tween musicTween;
    private float FadeOutTime => isPlaying ? 3f : 0.1f;
    private float FadeInTime => isPlaying ? 2f : 1f;

    public void NightStart()
    {
        musicTween?.Kill();

        musicTween = audioSource.DOFade(0, FadeOutTime).OnComplete(() => StartMusic(nightMusic));
    }

    public void NewDay()
    {
        musicTween?.Kill();
        musicTween = audioSource.DOFade(0, FadeOutTime).OnComplete(() => StartMusic(dayMusic));
    }

    private void StartMusic(AudioClip clip)
    {
        musicTween?.Kill();
        audioSource.clip = clip;
        musicTween = audioSource.DOFade(MUSIC_VOLUME, FadeInTime);
        audioSource.Play();
        isPlaying = true;
    }
}
