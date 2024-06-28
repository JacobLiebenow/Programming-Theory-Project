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
    [SerializeField] private GameObject loadGameScreen;
    [SerializeField] private TextMeshProUGUI noSavedGamesText;

    [SerializeField] private UILoadGameScrollViewManager loadGameScrollViewManager;

    private bool isShowingNoSavedGamesText = false;
    private IEnumerator noSavedGamesTextCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Saves will be saved to : " + Application.persistentDataPath);
        SetMainMenuActive();
    }


    // Screen transition functions
    private void SetMainMenuActive()
    {
        mainMenuScreen.SetActive(true);
        settingsMenuScreen.SetActive(false);
        loadGameScreen.SetActive(false);
    }

    private void SetSettingsMenuActive()
    {
        settingsMenuScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    private void SetLoadGameScreenActive()
    {
        loadGameScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }


    // Handle UI button behavior
    //
    // On new game clicked, transition the screen to the New Game menu.  This menu will allow the player to create a game scene based on size data and seed data.
    public void OnNewGameClicked()
    {
        if(DataManager.Instance != null)
        {
            DataManager.Instance.SetNewGameDefaultData();
        }
        SceneManager.LoadScene(1);
    }

    // On load game clicked, load the last game the player played.  Eventually, this will transition to a list view that will allow the player to choose what save they want to load.
    public void OnLoadGameScreenClicked()
    {
        if (DataManager.Instance != null && DataManager.Instance.SavedGames.gameNames.Count > 0)
        {
            SetLoadGameScreenActive();
        }
        else
        {
            if (isShowingNoSavedGamesText)
            {
                StopCoroutine(noSavedGamesTextCoroutine);
            }

            noSavedGamesTextCoroutine = ShowNoSavedGamesText();
            StartCoroutine(noSavedGamesTextCoroutine);
        }
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

    // From the selected save, load game
    public void OnLoadGameClicked()
    {
        DataManager.Instance.SetLoadedGameIndex(loadGameScrollViewManager.currentlySelectedIndex);
        DataManager.Instance.LoadGame();
        SceneManager.LoadScene(1);
    }


    // Set a coroutine to show the game loaded text for X seconds, then set it inactive
    private IEnumerator ShowNoSavedGamesText()
    {
        isShowingNoSavedGamesText = true;
        noSavedGamesText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        isShowingNoSavedGamesText = false;
        noSavedGamesText.gameObject.SetActive(false);
    }
}
