using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public SaveData SavedGames;

    public string SaveName;
    public int Seed;
    public int Width;
    public int Height;
    public int Padding;
    public GameObject GameGrid;
    public int Wood = 0;
    public int Ore = 0;
    public int Population = 0;

    public bool IsGameLoaded {  get; private set; }

    private string protectedDirectory = "Protected";
    private string savedGamesDirectory = "Saved Games";
    private string saveManifestName = "gamenamesmanifest";

    // Data to save for later between sessions
    [Serializable]
    public class SaveData
    {
        public List<string> gameNames = new List<string>();
    }

    [Serializable]
    public class IndividualSave
    {
        public string SaveName;
        public int Seed;
        public int Width;
        public int Height;
        public int Padding;
        public GameObject GameGrid;
        public int Wood;
        public int Ore;
        public int Population;
    }

    [Serializable]
    public class PlayerSettings
    {

    }


    // Awake is called when the object is loaded into the scene
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDataClass();
        LoadAllGames();

    }


    // Initialize the internal saved data classes
    private void InitializeDataClass()
    {
        Debug.Log("Initializing SavedGames...");
        SavedGames = new SaveData();
    }


    // Handle saving individual game data by setting a new IndividualSave object, adding it to the saved data list, and then saving the list
    //
    // NOTE: This could potentially cause saved game bloat, and it might make more sense to just create saves in a new folder, and loading
    // file names individually based off that
    public void SaveGame()
    {
        IndividualSave save = new IndividualSave();

        save.SaveName = SaveName;
        save.Seed = Seed;
        save.Width = Width;
        save.Height = Height;
        save.Padding = Padding;
        save.GameGrid = GameGrid;
        save.Wood = Wood;
        save.Ore = Ore;
        save.Population = Population;

        string json = JsonUtility.ToJson(save);
        string path = Application.persistentDataPath + "/" + savedGamesDirectory + "/" + save.SaveName + ".json";

        if(!Directory.Exists(Application.persistentDataPath + "/" + savedGamesDirectory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGamesDirectory);
        }

        File.WriteAllText(path, json);

        Debug.Log("Data to be written: " + json);
        Debug.Log("Saving data to: " + path);

        SavedGames.gameNames.Add(save.SaveName);
        SaveAllGames();
        
    }

    private void SaveAllGames()
    {
        string json = JsonUtility.ToJson(SavedGames);
        string path = Application.persistentDataPath + "/" + protectedDirectory + "/" + saveManifestName + ".json";

        if (!Directory.Exists(Application.persistentDataPath + "/" + protectedDirectory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + protectedDirectory);
        }

        Debug.Log("Data to be written: " + json);
        Debug.Log("Saving data to: " + path);

        File.WriteAllText(path, json);

        Debug.Log("Game saved successfully!");
    }
    

    // Load the entire saved games list, and then choose an individual game from the list to load
    //
    // NOTE: See above note on saved games to potentially increase performance here
    public void LoadGame(int index = 0)
    {
        if (SavedGames.gameNames.Count > 0)
        {
            string gaveSaveName = SavedGames.gameNames[index];
            string path = Application.persistentDataPath + "/" + savedGamesDirectory + "/" + gaveSaveName + ".json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                IndividualSave loadedGame = JsonUtility.FromJson<IndividualSave>(json);

                SaveName = loadedGame.SaveName;
                Seed = loadedGame.Seed;
                Width = loadedGame.Width;
                Height = loadedGame.Height;
                Padding = loadedGame.Padding;
                GameGrid = loadedGame.GameGrid;
                Wood = loadedGame.Wood;
                Ore = loadedGame.Ore;
                Population = loadedGame.Population;

                IsGameLoaded = true;
            }
            else
            {
                Debug.Log("Game not found at file location: " + path);
            }
        }
        
    }

    private void LoadAllGames()
    {

        string path = Application.persistentDataPath + "/" + protectedDirectory + "/" + saveManifestName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

            SavedGames = loadedData;
        }
    }


    // Handle deleting saves from the list by removing the game from the list and then saving the list once again
    /*public void DeleteGame(int index)
    {
        if(SaveName == SavedGames.data[index].SaveName)
        {
            IsGameLoaded = false;
        }
        SavedGames.data.RemoveAt(index);
        SaveAllGames();
    }

    // Handle clearing all data from a list and then saving the cleared list
    public void DeleteAllGames()
    {
        IsGameLoaded = false;
        SavedGames.data.Clear();
        SaveAllGames();
    }*/


    // Handle saving, loading, and reseting setting data
    public void SaveSettings()
    {

    }

    public void LoadSettings()
    {

    }

    public void ResetSettings()
    {

    }
}
