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
    [SerializeField] private GameObject loadGameConfirmationScreen;
    [SerializeField] private GameObject mainMenuConfirmationScreen;
    [SerializeField] private GameObject mainMenuFromWorldCreationConfirmationScreen;
    [SerializeField] private GameObject exitGameConfirmationScreen;
    [SerializeField] private GameObject overwriteSaveConfirmationScreen;
    [SerializeField] private GameObject deleteGameConfirmationScreen;

    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject saveGameButton;
    [SerializeField] private GameObject loadGameButton;

    [SerializeField] private TMP_InputField saveGameNameInputField;

    [SerializeField] private TMP_Dropdown gameSortDropdown;

    private bool isShowingStatusText = false;
    private IEnumerator showStatusText;
    private string gameSavedText = "(Game saved!)";
    private string noSavedGamesFoundText = "(No saved games found!)";
    private string dataManagerNotFoundText = "(DataManager not found!)";

    //ENCAPSULATION
    public bool isGamePaused {  get; private set; }
    
    private enum SaveGameStates
    {
        saveGame = 0,
        saveAndLoad = 1,
        saveAndMainMenu = 2,
        saveAndQuit = 3
    }

    private int saveState = 0;



    // Start is called before the first frame update
    void Start()
    {
        gameSortDropdown.value = (int)UILoadGameScrollViewManager.SortOptions.TimeDesc;

        if (!terrainGenerator.isMapSet)
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
        ResetScreenStates();
        pauseScreen.SetActive(true);

        if(DataManager.Instance != null || terrainGenerator.isMapSet)
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
        ResetScreenStates();
        settingsScreen.SetActive(true);
    }

    private void SetMainScreenActive()
    {
        ResetScreenStates();
        Time.timeScale = 1f;
    }

    private void SetSaveGameScreenActive()
    {
        ResetScreenStates();
        saveGameScreen.SetActive(true);
    }

    private void SetLoadGameScreenActive()
    {
        ResetScreenStates();
        loadGameScreen.SetActive(true);
    }
    
    private void SetLoadGameConfirmationScreenActive()
    {
        ResetScreenStates();
        loadGameConfirmationScreen.SetActive(true);
    }

    private void SetMainMenuConfirmationScreenActive()
    {
        ResetScreenStates();
        mainMenuConfirmationScreen.SetActive(true);
    }

    private void SetMainMenuFromWorldCreationConfirmationScreenActive()
    {
        ResetScreenStates();
        mainMenuFromWorldCreationConfirmationScreen.SetActive(true);
    }

    private void SetExitGameConfirmationScreenActive()
    {
        ResetScreenStates();
        exitGameConfirmationScreen.SetActive(true);
    }

    private void SetOverwriteSaveConfirmationScreenActive()
    {
        ResetScreenStates();
        overwriteSaveConfirmationScreen.SetActive(true);
    }

    private void SetDeleteGameConfirmationScreenActive()
    {
        ResetScreenStates();
        deleteGameConfirmationScreen.SetActive(true);
    }

    private void ResetScreenStates()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        saveGameScreen.SetActive(false);
        loadGameScreen.SetActive(false);
        loadGameConfirmationScreen.SetActive(false);
        mainMenuConfirmationScreen.SetActive(false);
        mainMenuFromWorldCreationConfirmationScreen.SetActive(false);
        exitGameConfirmationScreen.SetActive(false);
        overwriteSaveConfirmationScreen.SetActive(false);
        deleteGameConfirmationScreen.SetActive(false);
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
    public void OnLoadGameSaveClicked()
    {
        saveState = (int)SaveGameStates.saveAndLoad;
        SetSaveGameScreenActive();
    }

    public void OnLoadGameWithoutSaveClicked()
    {
        LoadGame();
    }

    public void OnReturnToMainMenuSaveClicked()
    {
        saveState = (int)SaveGameStates.saveAndMainMenu;
        SetSaveGameScreenActive();
    }

    public void OnReturnToMainMenuWithoutSaveClicked()
    {
        ReturnToMainMenu();
    }

    public void OnExitSaveClicked()
    {
        saveState = (int)SaveGameStates.saveAndQuit;
        SetSaveGameScreenActive();
    }

    public void OnExitWithoutSaveClicked()
    {
        QuitGame();
    }

    public void OnOverwriteSaveConfirmClicked()
    {
        HandleSaveGame();
    }

    public void OnDeleteGameClicked()
    {
        SetDeleteGameConfirmationScreenActive();
    }

    public void OnDeleteGameConfirmClicked()
    {
        DeleteGame();
        SetLoadGameScreenActive();
    }

    public void OnReturnFromDeleteGameClicked()
    {
        SetLoadGameScreenActive();
    }

    
    // Save game screen button handler
    public void OnConfirmSaveClicked()
    {
        if(DataManager.Instance != null && DataManager.Instance.SavedGames.gameNames.Contains(saveGameNameInputField.text))
        {
            SetOverwriteSaveConfirmationScreenActive();
        }
        else
        {
            HandleSaveGame();
        }
        
    }


    // Load game button handler
    public void OnLoadGameClicked()
    {
        SetLoadGameConfirmationScreenActive();
    }

    public void OnSortDropdownChanged()
    {
        if(DataManager.Instance != null)
        {
            loadGameScrollViewManager.SortList(gameSortDropdown.value);
        }
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

    private void HandleSaveGame()
    {
        SaveGameFromUI();
        switch (saveState)
        {
            case (int)SaveGameStates.saveAndLoad:
                LoadGame();
                break;
            case (int)SaveGameStates.saveAndMainMenu:
                ReturnToMainMenu();
                break;
            case (int)SaveGameStates.saveAndQuit:
                QuitGame();
                break;
            default:
                SetPauseScreenActive();
                break;
        }
    }

    private void SaveGameFromUI()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveName = saveGameNameInputField.text;
            DataManager.Instance.SaveGame();
            SetStatusTextCoroutine(gameSavedText);
            loadGameScrollViewManager.SortList(gameSortDropdown.value);
        }
        else
        {
            Debug.Log("Data Manager not present!  Game will not be saved!");
            SetStatusTextCoroutine(dataManagerNotFoundText);
        }
    }

    private void LoadGame()
    {
        Time.timeScale = 1f;
        DataManager.Instance.SetLoadedGameIndex(loadGameScrollViewManager.currentlySelectedIndex);
        DataManager.Instance.LoadGame();
        SceneManager.LoadScene(1);
    }

    private void DeleteGame()
    {
        DataManager.Instance.DeleteGame(loadGameScrollViewManager.currentlySelectedIndex, loadGameScrollViewManager.currentlySelectedName);
        loadGameScrollViewManager.UpdateListObjects();
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

}
