using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class UILoadGameScrollElement : MonoBehaviour
{
    private UILoadGameScrollViewManager scrollViewManager;

    [SerializeField] private GameObject selectedBackground;
    [SerializeField] private Image raycastTarget;

    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI timeText;

    public int gameIndex {  get; private set; }
    public bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        scrollViewManager = transform.parent.parent.parent.GetComponent<UILoadGameScrollViewManager>();
    }

    // Text element initialization handler functions
    private void SetIndexText(string newText)
    {
        indexText.text = newText;
    }

    public void SetNameText(string newText) 
    {
        nameText.text = newText;
    }

    public void SetDateText(string newText)
    {
        dateText.text = newText;
    }

    public void SetTimeText(string newText)
    {
        timeText.text = newText;
    }


    // Set the gameIndex attribute to a given i
    public void SetGameIndex(int i)
    {
        gameIndex = i;
        SetIndexText((i + 1) + ".");
    }


    // When this object is clicked, set it to selected, note its index, and update the overall scroll view
    public void OnElementClicked()
    {
        if (scrollViewManager.currentlySelectedIndex != gameIndex)
        {
            scrollViewManager.HandleElementSelected(gameIndex);
        }
    }

    public void ToggleSelection()
    {
        if (isSelected)
        {
            isSelected = false;
            selectedBackground.SetActive(false);
        }
        else
        {
            isSelected = true;
            selectedBackground.SetActive(true);
        }
    }
}
