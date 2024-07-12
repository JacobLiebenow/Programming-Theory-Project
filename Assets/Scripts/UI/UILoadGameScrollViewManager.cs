using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILoadGameScrollViewManager : MonoBehaviour
{

    [SerializeField] private Transform contentField;
    [SerializeField] private GameObject listElement;

    private List<GameObject> savedGames = new List<GameObject>();

    public bool isLoaded {  get; private set; }
    public int currentlySelectedIndex {  get; private set; }
    public string currentlySelectedName { get; private set; }

    public enum SortOptions
    {
        NameAsc = 0,
        NameDesc = 1,
        TimeAsc = 2,
        TimeDesc = 3
    }


    // Start is called before the first frame update
    void Start()
    {
        if(DataManager.Instance != null)
        {
            SortList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateListObjects()
    {
        if(DataManager.Instance != null && DataManager.Instance.SavedGames.gameNames.Count > 0)
        {
            ClearList();
            GameObject firstElement = CreateListElement(0);
            UILoadGameScrollElement elementUI = firstElement.GetComponent<UILoadGameScrollElement>();
            currentlySelectedIndex = elementUI.gameIndex;
            currentlySelectedName = DataManager.Instance.SavedGames.gameNames[currentlySelectedIndex];
            elementUI.ToggleSelection();

            Debug.Log("Current index as of UpdateListObjects(): " + currentlySelectedIndex);
            if (DataManager.Instance.SavedGames.gameNames.Count > 1)
            {
                Vector2 lastElementPosition = firstElement.GetComponent<RectTransform>().anchoredPosition;

                for (int i = 1; i < DataManager.Instance.SavedGames.gameNames.Count; i++)
                {
                    Debug.Log("Cycle: " + i);
                    GameObject newElement = CreateListElement(i);
                    newElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(lastElementPosition.x, lastElementPosition.y - newElement.GetComponent<RectTransform>().rect.height);

                    lastElementPosition = newElement.GetComponent<RectTransform>().anchoredPosition;
                }
            }

            isLoaded = true;
        }
        else
        {
            Debug.Log("DataManager not found!");
        }
    }

    private GameObject CreateListElement(int i)
    {
        GameObject element = Instantiate(listElement, contentField, false);
        UILoadGameScrollElement elementUI = element.GetComponent<UILoadGameScrollElement>();
        DateTime elementTimestamp = DateTime.Parse(DataManager.Instance.SavedGames.timestamps[i]);

        elementUI.SetGameIndex(i);
        elementUI.SetNameText(DataManager.Instance.SavedGames.gameNames[i]);
        elementUI.SetDateText(elementTimestamp.ToString("d"));
        elementUI.SetTimeText(elementTimestamp.ToString("T"));

        savedGames.Add(element);

        return element;
    }

    private void ClearList()
    {
        for(int i = 0; i < contentField.childCount; i++)
        {
           Destroy(contentField.GetChild(i).gameObject);
        }

        savedGames.Clear();
    }

    public void HandleElementSelected(int i)
    {
        contentField.GetChild(currentlySelectedIndex).GetComponent<UILoadGameScrollElement>().ToggleSelection();
        currentlySelectedIndex = i;
        currentlySelectedName = DataManager.Instance.SavedGames.gameNames[i];
        contentField.GetChild(currentlySelectedIndex).GetComponent<UILoadGameScrollElement>().ToggleSelection();
    }

    public void SortList(int i = 3)
    {
        List<string> names = new List<string>();
        List<string> timestamps = new List<string>();

        switch (i)
        {
            case (int)SortOptions.NameAsc:
                names = DataManager.Instance.SavedGames.gameNames.OrderBy(x => x).ToList();

                foreach(string name in names)
                {
                    int oldIndex = DataManager.Instance.SavedGames.gameNames.IndexOf(name);
                    timestamps.Add(DataManager.Instance.SavedGames.timestamps[oldIndex]);
                }
                break;
            case (int)SortOptions.NameDesc:
                names = DataManager.Instance.SavedGames.gameNames.OrderByDescending(x => x).ToList();

                foreach (string name in names)
                {
                    int oldIndex = DataManager.Instance.SavedGames.gameNames.IndexOf(name);
                    timestamps.Add(DataManager.Instance.SavedGames.timestamps[oldIndex]);
                }
                break;
            case (int)SortOptions.TimeAsc:
                timestamps = DataManager.Instance.SavedGames.timestamps.OrderBy(x => x).ToList();

                foreach(string timestamp in timestamps)
                {
                    int oldIndex = DataManager.Instance.SavedGames.timestamps.IndexOf(timestamp);
                    names.Add(DataManager.Instance.SavedGames.gameNames[oldIndex]);
                }
                break;
            case (int)SortOptions.TimeDesc:
                timestamps = DataManager.Instance.SavedGames.timestamps.OrderByDescending(x => x).ToList();

                foreach (string timestamp in timestamps)
                {
                    int oldIndex = DataManager.Instance.SavedGames.timestamps.IndexOf(timestamp);
                    names.Add(DataManager.Instance.SavedGames.gameNames[oldIndex]);
                }
                break;
        }

        SetDataOrder(names, timestamps);
    }

    private void SetDataOrder(List<string> names, List<string> timestamps)
    {
        DataManager.Instance.SavedGames.gameNames = names;
        DataManager.Instance.SavedGames.timestamps = timestamps;
        DataManager.Instance.SaveAllGames();

        UpdateListObjects();
    }

}
