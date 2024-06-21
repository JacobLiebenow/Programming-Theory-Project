using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject settingsScreen;

    // Start is called before the first frame update
    void Start()
    {
        SetMainMenuActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetMainMenuActive()
    {
        mainMenuScreen.SetActive(true);
        settingsScreen.SetActive(false);

    }

    private void SetSettingsMenuActive()
    {
        settingsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }
}
