using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIMainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject settingsMenuScreen;

    // Start is called before the first frame update
    void Start()
    {
        SetMainMenuActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetMainMenuActive()
    {
        mainMenuScreen.SetActive(true);
        settingsMenuScreen.SetActive(false);

    }

    private void SetSettingsMenuActive()
    {
        settingsMenuScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void OnNewGameClicked()
    {

    }

    public void OnLoadGameClicked()
    {

    }

    public void OnSettingsClicked()
    {
        SetSettingsMenuActive();
    }

    public void OnExitGameClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
Application.Quit();
#endif
    }

    public void OnReturnToMainMenuClicked()
    {
        SetMainMenuActive();
    }
}
