using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIMainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject settingsMenuScreen;
    [SerializeField] private TextMeshProUGUI gameLoadedText;

    private bool isShowingGameLoadedText = false;
    private IEnumerator gameLoadedTextCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetMainMenuActive();
    }


    // Screen transition functions
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


    // Handle UI button behavior
    //
    // On new game clicked, transition the screen to the New Game menu.  This menu will allow the player to create a game scene based on size data and seed data.
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene(1);
    }

    // On load game clicked, load the last game the player played.  Eventually, this will transition to a list view that will allow the player to choose what save they want to load.
    public void OnLoadGameClicked()
    {
        if(isShowingGameLoadedText)
        {
            StopCoroutine(gameLoadedTextCoroutine);
        }

        gameLoadedTextCoroutine = ShowGameLoadedText();
        StartCoroutine(gameLoadedTextCoroutine);
    }

    // On settings clicked, the settings menu will be loaded for the player, which can be manipulated
    public void OnSettingsClicked()
    {
        SetSettingsMenuActive();
    }

    // On exit game clicked, exit the game and/or Play Mode
    public void OnExitGameClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // From a different menu, return to the main menu
    public void OnReturnToMainMenuClicked()
    {
        SetMainMenuActive();
    }


    // Set a coroutine to show the game loaded text for X seconds, then set it inactive
    private IEnumerator ShowGameLoadedText()
    {
        isShowingGameLoadedText = true;
        gameLoadedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        isShowingGameLoadedText = false;
        gameLoadedText.gameObject.SetActive(false);
    }
}
