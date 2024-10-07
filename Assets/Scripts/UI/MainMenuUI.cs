using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private CanvasGroup looseMenu;
    [SerializeField] private CanvasGroup winMenu;
    [SerializeField] private UI_StickToTarget stickToTarget;
    public UI_StickToTarget StickToTarget => stickToTarget;

    private void Start()
    {
        if(pauseMenu) SetCanvasGroupState(false, pauseMenu);
        if(winMenu) SetCanvasGroupState(false, winMenu);
        if(looseMenu) SetCanvasGroupState(false, looseMenu);
    }

    public void PauseMenuSwitchState()
    {
        if (!pauseMenu) return;

        bool newState = pauseMenu.alpha < 0.5f;
        Debug.Log($"PauseMenuSwtich -> {newState}");

        Time.timeScale = newState ? 0.001f : 1f; // Pause time in pause menu
        SetCanvasGroupState(newState, pauseMenu);
    }

    public void ShowEndScreen(bool win)
    {
        Time.timeScale = 1f; // Pause time in pause menu
        SetCanvasGroupState(false, pauseMenu);
        SetCanvasGroupState(win, winMenu);
        SetCanvasGroupState(!win, looseMenu);
    }

    private void SetCanvasGroupState(bool state, CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = state ? 1 : 0;

        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
    }
}
