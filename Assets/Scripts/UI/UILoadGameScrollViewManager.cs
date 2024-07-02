using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadGameScrollViewManager : MonoBehaviour
{

    [SerializeField] private Transform contentField;
    [SerializeField] private GameObject listElement;

    private List<GameObject> savedGames = new List<GameObject>();

    public bool isLoaded {  get; private set; }
    public int currentlySelectedIndex {  get; private set; }
    public string currentlySelectedName { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        UpdateListObjects();
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

        elementUI.SetGameIndex(i);
        elementUI.SetNameText(DataManager.Instance.SavedGames.gameNames[i]);
        elementUI.SetDateText(DataManager.Instance.SavedGames.dates[i]);
        elementUI.SetTimeText(DataManager.Instance.SavedGames.times[i]);

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

}
