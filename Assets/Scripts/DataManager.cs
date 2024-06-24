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
    public int Wood;
    public int Ore;
    public int Population;

    public bool IsGameLoaded {  get; private set; }

    // Data to save for later between sessions
    [Serializable]
    public class SaveData
    {
        public List<IndividualSave> data;
    }

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

        LoadAllGames();

    }


    // Initialize the internal saved data classes
    private void InitializeDataClass()
    {
        SavedGames = new SaveData();
        SavedGames.data = new List<IndividualSave>();
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

        SavedGames.data.Add(save);

        SaveAllGames();
    }

    private void SaveAllGames()
    {
        string json = JsonUtility.ToJson(SavedGames);
        File.WriteAllText(Application.persistentDataPath + "savefile.json", json);
    }
    

    // Load the entire saved games list, and then choose an individual game from the list to load
    //
    // NOTE: See above note on saved games to potentially increase performance here
    private void LoadAllGames()
    {
        if (SavedGames == null)
        {
            InitializeDataClass();
        }

        string path = Application.persistentDataPath + "savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

            SavedGames.data = loadedData.data;
        }
    }

    public void LoadGame(int index = 0)
    {
        if (SavedGames.data.Count > 0)
        {
            IndividualSave loadedGame = SavedGames.data[index];

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
        
    }


    // Handle deleting saves from the list by removing the game from the list and then saving the list once again
    public void DeleteGame(int index)
    {
        SavedGames.data.RemoveAt(index);
        SaveAllGames();
    }

    // Handle clearing all data from a list and then saving the cleared list
    public void DeleteAllGames()
    {
        SavedGames.data.Clear();
        SaveAllGames();
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
}
