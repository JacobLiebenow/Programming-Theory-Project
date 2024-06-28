using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIGameManager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private UILoadGameScrollViewManager loadGameScrollViewManager;

    [SerializeField] private GameObject worldCreationScreen;
    [SerializeField] private GameObject activeGameUIScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject saveGameScreen;
    [SerializeField] private GameObject loadGameScreen;
    [SerializeField] private GameObject mainMenuConfirmationScreen;
    [SerializeField] private GameObject mainMenuFromWorldCreationConfirmationScreen;
    [SerializeField] private GameObject exitGameConfirmationScreen;

    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject saveGameButton;
    [SerializeField] private GameObject loadGameButton;

    [SerializeField] private TMP_InputField saveGameNameInputField;

    private bool isShowingStatusText = false;
    private IEnumerator showStatusText;
    private string gameSavedText = "(Game saved!)";
    private string noSavedGamesFoundText = "(No saved games found!)";
    private string dataManagerNotFoundText = "(DataManager not found!)";

    //ENCAPSULATION
    public bool isGamePaused {  get; private set; }




    // Start is called before the first frame update
    void Start()
    {
        if(!terrainGenerator.isMapSet)
        {
            SetWorldCreationScreenActive();
        }
        else
        {
            SetActiveGameUIScreenActive();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If escape is hit, pause the game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HandlePauseButtonPressed();
        }
    }


    // Screen state handler functions
    private void SetWorldCreationScreenActive()
    {
        worldCreationScreen.SetActive(true);
    }

    private void SetWorldCreationScreenInactive()
    {
        worldCreationScreen.SetActive(false);
    }

    private void SetActiveGameUIScreenActive()
    {
        activeGameUIScreen.SetActive(true);
    }

    private void SetActiveGameUIScreenInactive()
    {
        activeGameUIScreen.SetActive(false);
    }

    private void SetPauseScreenActive()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        settingsScreen.SetActive(false);
        saveGameScreen.SetActive(false);
        loadGameScreen.SetActive(false);
        mainMenuConfirmationScreen.SetActive(false);
        mainMenuFromWorldCreationConfirmationScreen.SetActive(false);
        exitGameConfirmationScreen.SetActive(false);

        if(terrainGenerator.isMapSet)
        {
            saveGameButton.SetActive(true);
            loadGameButton.SetActive(true);
        }
        else
        {
            saveGameButton.SetActive(false);
            loadGameButton.SetActive(false);
        }
    }

    private void SetSettingsScreenActive()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    private void SetMainScreenActive()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        saveGameScreen.SetActive(false);
        loadGameScreen.SetActive(false);
        mainMenuConfirmationScreen.SetActive(false);
        mainMenuFromWorldCreationConfirmationScreen.SetActive(false);
        exitGameConfirmationScreen.SetActive(false);
    }

    private void SetSaveGameScreenActive()
    {
        pauseScreen.SetActive(false);
        saveGameScreen.SetActive(true);
    }

    private void SetLoadGameScreenActive()
    {
        pauseScreen.SetActive(false);
        loadGameScreen.SetActive(true);
    }

    private void SetMainMenuConfirmationScreenActive()
    {
        mainMenuConfirmationScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    private void SetMainMenuFromWorldCreationConfirmationScreenActive()
    {
        mainMenuFromWorldCreationConfirmationScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    private void SetExitGameConfirmationScreenActive()
    {
        exitGameConfirmationScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }




    // Main pause screen button handler functions
    public void OnResumeGameClicked()
    {
        HandlePauseButtonPressed();
    }

    public void OnSaveGameScreenClicked()
    {
        if(DataManager.Instance != null)
        {
            SetSaveGameScreenActive();
        }
        else
        {
            SetStatusTextCoroutine(dataManagerNotFoundText);
        }

    }

    public void OnLoadGameScreenClicked()
    {
        if(DataManager.Instance != null && DataManager.Instance.SavedGames.gameNames.Count > 0)
        {
            SetLoadGameScreenActive();
        }
        else
        {
            SetStatusTextCoroutine(noSavedGamesFoundText);
        }
    }

    public void OnSettingsClicked()
    {
        SetSettingsScreenActive();
    }

    public void OnMainMenuClicked()
    {
        if(terrainGenerator.isMapSet)
        {
            SetMainMenuConfirmationScreenActive();
        } 
        else
        {
            SetMainMenuFromWorldCreationConfirmationScreenActive();
        }
        
    }

    public void OnExitGameClicked()
    {
        SetExitGameConfirmationScreenActive();
    }


    // Settings menu button/slider handlers
    public void OnApplyClicked()
    {

    }


    // General return handler function
    public void OnReturnClicked()
    {
        SetPauseScreenActive();
    }

    // Confirmation screen handler functions
    public void OnReturnToMainMenuSaveClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void OnReturnToMainMenuWithoutSaveClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void OnExitSaveClicked()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void OnExitWithoutSaveClicked()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    
    // Save game screen button handler
    public void OnConfirmSaveClikced()
    {
        if(DataManager.Instance != null)
        {
            DataManager.Instance.SaveName = saveGameNameInputField.text;
            DataManager.Instance.SaveGame();
            SetStatusTextCoroutine(gameSavedText);
        }
        else
        {
            Debug.Log("Data Manager not present!  Game will not be saved!");
            SetStatusTextCoroutine(dataManagerNotFoundText);
        }

        SetPauseScreenActive();
    }


    // Load game button handler
    public void OnLoadGameClicked()
    {
        DataManager.Instance.SetLoadedGameIndex(loadGameScrollViewManager.currentlySelectedIndex);
        DataManager.Instance.LoadGame();
        SceneManager.LoadScene(1);
    }



    // Handle screen state when the pause button is pressed
    private void HandlePauseButtonPressed()
    {
        if (!isGamePaused && !terrainGenerator.isMapSet)
        {
            isGamePaused = true;
            SetPauseScreenActive();
            SetWorldCreationScreenInactive();
        }
        else if (isGamePaused && !terrainGenerator.isMapSet)
        {
            isGamePaused = false;
            SetMainScreenActive();
            SetWorldCreationScreenActive();
        }
        else if (!isGamePaused && terrainGenerator.isMapSet)
        {
            isGamePaused = true;
            SetPauseScreenActive();
            SetActiveGameUIScreenInactive();
        }
        else if (isGamePaused && terrainGenerator.isMapSet)
        {
            isGamePaused = false;
            SetMainScreenActive();
            SetActiveGameUIScreenActive();
        }
    }




    // Thread to show the status text for a short period of time on the pause screen
    private IEnumerator ShowStatusText(string newText)
    {
        isShowingStatusText = true;
        statusText.text = newText;
        statusText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        isShowingStatusText = false;
        statusText.gameObject.SetActive(false);
    }

    // Separate function to handle the status text coroutine macro-logic 
    private void SetStatusTextCoroutine(string newText)
    {
        if (isShowingStatusText)
        {
            StopCoroutine(showStatusText);
        }

        showStatusText = ShowStatusText(newText);
        StartCoroutine(showStatusText);
    }
}
