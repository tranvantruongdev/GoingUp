using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("GUI Components")]
    public GameObject MainMenuGuiGameObject;
    public GameObject PauseGuiGameObject, GameplayGuiGameObject, GameOverGuiGameObject;

    public GameStateEnum GameStateEnum;

    bool _isClicked;

    // Use this for initialization
    void Start()
    {
        MainMenuGuiGameObject.SetActive(true);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(false);
        GameOverGuiGameObject.SetActive(false);
        GameStateEnum = GameStateEnum.MAIN_MENU;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameStateEnum == GameStateEnum.MAIN_MENU && !_isClicked)
        {
            if (IsButtonClicked())
                return;

            AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
            ShowGameplayUI();
            AudioManager.S_Instance.PlayMusicClip(AudioManager.S_Instance.GameMusicAudio);
        }
        else if (Input.GetMouseButtonUp(0) && _isClicked && GameStateEnum == GameStateEnum.MAIN_MENU)
            _isClicked = false;
    }

    //show main menu
    public void ShowMainMenuUI()
    {
        ScoreManager.Instance.StopCountingCouroutine();
        ScoreManager.Instance.ResetTheCurrentScoreValue();
        MainMenuGuiGameObject.SetActive(true);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(false);
        GameOverGuiGameObject.SetActive(false);
        if (GameStateEnum == GameStateEnum.PAUSED_GAME)
            Time.timeScale = 1;

        GameStateEnum = GameStateEnum.MAIN_MENU;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);

        GameManager.S_Instance.ClearSceneUI();
        GameManager.S_Instance.CreateNewScene();
        GameManager.S_Instance.IsStarted = false;
    }

    //show pause menu
    public void ShowPauseMenuUI()
    {
        if (GameStateEnum == GameStateEnum.PAUSED_GAME)
            return;

        PauseGuiGameObject.SetActive(true);
        Time.timeScale = 0;
        GameStateEnum = GameStateEnum.PAUSED_GAME;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
    }

    //hide pause menu
    public void HidePauseMenuUI()
    {
        PauseGuiGameObject.SetActive(false);
        Time.timeScale = 1;
        GameStateEnum = GameStateEnum.RUNNING;
        AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.ButtonClickAudio);
    }

    //show gameplay gui
    public void ShowGameplayUI()
    {
        MainMenuGuiGameObject.SetActive(false);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(true);
        GameOverGuiGameObject.SetActive(false);
        GameStateEnum = GameStateEnum.RUNNING;
        AudioManager.S_Instance.PlayMusicClip(AudioManager.S_Instance.GameMusicAudio);
    }

    //show game over gui
    public void ShowGameOverUI()
    {
        MainMenuGuiGameObject.SetActive(false);
        PauseGuiGameObject.SetActive(false);
        GameplayGuiGameObject.SetActive(false);
        GameOverGuiGameObject.SetActive(true);
        GameStateEnum = GameStateEnum.GAMEOVER;
    }

    //check if user click any menu button
    public bool IsButtonClicked()
    {
        bool tmp = false;

        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            tmp |= result.gameObject.GetComponent<Button>() != null;
        }

        return tmp;
    }

    public void SetPauseGameForShop(bool isPaused)
    {
        if (isPaused)
        {
            GameStateEnum = GameStateEnum.PAUSED_GAME;
        }
        else
        {
            GameStateEnum = GameStateEnum.MAIN_MENU;
        }
    }
}
