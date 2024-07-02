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
    public int Wood = 0;
    public int Ore = 0;
    public int Population = 0;
    public List<SaveableTileData> TileData = new List<SaveableTileData>();

    public bool IsGameLoaded {  get; private set; }
    public int LoadedGameIndex { get; private set; }

    private string protectedDirectory = "Protected";
    private string savedGamesDirectory = "Saved Games";
    private string saveManifestName = "gamenamesmanifest";

    // Data to save for later between sessions
    [Serializable]
    public class SaveData
    {
        public List<string> gameNames = new List<string>();
        public List<string> dates = new List<string>();
        public List<string> times = new List<string>();
    }

    [Serializable]
    public class IndividualSave
    {
        public string SaveName;
        public int Seed;
        public int Width;
        public int Height;
        public int Padding;
        public int Wood;
        public int Ore;
        public int Population;
        public List<SaveableTileData> TileData = new List<SaveableTileData>();
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
        LoadedGameIndex = 0;

    }


    // Initialize the internal saved data classes
    private void InitializeDataClass()
    {
        Debug.Log("Initializing SavedGames...");
        SavedGames = new SaveData();
    }


    // Reset the data attributes to their default state.  Called when New Game is clicked.
    public void SetNewGameDefaultData()
    {
        SaveName = null;
        Seed = 0;
        Width = 0;
        Height = 0;
        Padding = 0;
        Wood = 0;
        Ore = 0;
        Population = 0;
        TileData.Clear();

        IsGameLoaded = false;
    }


    // Save the game to its own individual JSON file in the Saved Games directory
    public void SaveGame()
    {
        IndividualSave save = new IndividualSave();

        save.SaveName = SaveName;
        save.Seed = Seed;
        save.Width = Width;
        save.Height = Height;
        save.Padding = Padding;
        save.Wood = Wood;
        save.Ore = Ore;
        save.Population = Population;
        save.TileData = TileData;

        string json = JsonUtility.ToJson(save);
        string path = Application.persistentDataPath + "/" + savedGamesDirectory + "/" + save.SaveName + ".json";

        if(!Directory.Exists(Application.persistentDataPath + "/" + savedGamesDirectory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + savedGamesDirectory);
        }

        File.WriteAllText(path, json);

        Debug.Log("Data to be written: " + json);
        Debug.Log("Saving data to: " + path);

        // If the saved game already exists, overwrite it in the manifest.  Otherwise, add it to the manifest.
        if(SavedGames.gameNames.Contains(save.SaveName))
        {
            int index = SavedGames.gameNames.IndexOf(save.SaveName);
            SavedGames.dates[index] = DateTime.Now.Date.ToString();
            SavedGames.times[index] = DateTime.Now.TimeOfDay.ToString();
        } 
        else
        {
            SavedGames.gameNames.Add(save.SaveName);
            SavedGames.dates.Add(DateTime.Now.Date.ToString());
            SavedGames.times.Add(DateTime.Now.TimeOfDay.ToString());
        }
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
    

    // Load a saved game based off the given index from its individual save file
    public void LoadGame()
    {
        if (SavedGames.gameNames.Count > 0)
        {
            string gaveSaveName = SavedGames.gameNames[LoadedGameIndex];
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
                Wood = loadedGame.Wood;
                Ore = loadedGame.Ore;
                Population = loadedGame.Population;
                TileData = loadedGame.TileData;

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
    public void DeleteGame(int index, string gameName)
    {
        SavedGames.gameNames.RemoveAt(index);
        SavedGames.dates.RemoveAt(index);
        SavedGames.times.RemoveAt(index);

        string path = Application.persistentDataPath + "/" + savedGamesDirectory + "/" + gameName + ".json";
        if (File.Exists(path)) 
        { 
            File.Delete(path);
        }
    }


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

   
    public void SetLoadedGameIndex(int i)
    {
        LoadedGameIndex = i;
    }
}
