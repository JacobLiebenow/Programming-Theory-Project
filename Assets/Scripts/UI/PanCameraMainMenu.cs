using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PanCameraMainMenu : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Tilemap referenceTilemap;
    [SerializeField] private Image fadeToBlackBackground;
    private Color fadeToBlackDefaultDarkColor;
    private Color fadeToBlackDefaultTransparentColor;

    private float camHeight;
    private float camWidth;
    private int boundsPaddingX;
    private int boundsPaddingY;

    private Vector3 startingCameraPosition;
    private Vector3 endingCameraPosition;

    private int currentStartX, currentStartY;
    private int currentEndX, currentEndY;
    private float hypotenuse = 0f;
    private float minimumTravelDistance = 30f;

    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float fadeDistance = 2f;
    [SerializeField] private float darkDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {
        SetCameraDimensions();
        SetCameraBounds();
        SetFadeToBlackDefaultColor();

        GenerateRandomStartingPosition();
        GenerateRandomEndingPosition();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPosition();
        HandleBackgroundFade();
    }

    private void SetCameraDimensions()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
        Debug.Log("Cam Height: " + camHeight);
        Debug.Log("Cam Width: " + camWidth);
    }

    private void SetCameraBounds()
    {
        boundsPaddingX = Mathf.RoundToInt(camWidth) + 1;
        boundsPaddingY = Mathf.RoundToInt(camHeight) + 1;
    }

    private void SetFadeToBlackDefaultColor()
    {
        fadeToBlackDefaultDarkColor = new Color(fadeToBlackBackground.color.r, fadeToBlackBackground.color.g, fadeToBlackBackground.color.b, 1);
        fadeToBlackDefaultTransparentColor = new Color(fadeToBlackBackground.color.r, fadeToBlackBackground.color.g, fadeToBlackBackground.color.b, 0);
    }

    private void GenerateRandomStartingPosition()
    {
        currentStartX = Random.Range(boundsPaddingX, gridWidth - boundsPaddingX);
        currentStartY = Random.Range(boundsPaddingY, gridHeight - boundsPaddingY);

        Vector3Int startingGridCoordinates = new Vector3Int(currentStartX, currentStartY, 0);

        Vector3 startingCameraPositionRaw = referenceTilemap.CellToWorld(startingGridCoordinates);

        startingCameraPosition = new Vector3(startingCameraPositionRaw.x, startingCameraPositionRaw.y, transform.position.z);
        transform.position = startingCameraPosition;
    }

    private void GenerateRandomEndingPosition()
    {
        do
        {
            currentEndX = Random.Range(boundsPaddingX, gridWidth - boundsPaddingX);
            currentEndY = Random.Range(boundsPaddingY, gridHeight - boundsPaddingY);

            hypotenuse = Mathf.Sqrt(Mathf.Pow(currentEndX - currentStartX, 2) + Mathf.Pow(currentEndY - currentStartY, 2));
        } while (hypotenuse < minimumTravelDistance);

        Vector3Int endingGridCoordinates = new Vector3Int(currentEndX, currentEndY, 0);

        Vector3 endingCameraPositionRaw = referenceTilemap.CellToWorld(endingGridCoordinates);
        endingCameraPosition = new Vector3(endingCameraPositionRaw.x, endingCameraPositionRaw.y, transform.position.z);
    }

    // Steadily move from the starting position to the ending positions
    private void MoveToPosition()
    {
        transform.Translate((endingCameraPosition - transform.position).normalized * Time.deltaTime * moveSpeed);

        if(Mathf.Abs(endingCameraPosition.x - transform.position.x) < 0.5f && Mathf.Abs(endingCameraPosition.y - transform.position.y) < 0.5f)
        {
            GenerateRandomStartingPosition();
            GenerateRandomEndingPosition();
        }
    }

    // Fade in as the camera leaves the starting location, and fade out as the camera reaches the ending location
    private void HandleBackgroundFade()
    {
        if (hypotenuse - (endingCameraPosition - transform.position).magnitude < fadeDistance && hypotenuse - (endingCameraPosition - transform.position).magnitude > darkDistance)
        {
            fadeToBlackBackground.color = new Color(fadeToBlackDefaultDarkColor.r, fadeToBlackDefaultDarkColor.g, fadeToBlackDefaultDarkColor.b, 1 - (((hypotenuse - (endingCameraPosition - transform.position).magnitude - darkDistance)) / (fadeDistance - darkDistance)));
        } 
        else if ((endingCameraPosition - transform.position).magnitude < fadeDistance && (endingCameraPosition - transform.position).magnitude > darkDistance)
        {
            fadeToBlackBackground.color = new Color(fadeToBlackDefaultDarkColor.r, fadeToBlackDefaultDarkColor.g, fadeToBlackDefaultDarkColor.b, ((fadeDistance - darkDistance) - ((endingCameraPosition - transform.position).magnitude - darkDistance)) / (fadeDistance - darkDistance));
        } 
        else if ((hypotenuse - (endingCameraPosition - transform.position).magnitude < fadeDistance && hypotenuse - (endingCameraPosition - transform.position).magnitude < darkDistance)
            || ((endingCameraPosition - transform.position).magnitude < fadeDistance && (endingCameraPosition - transform.position).magnitude < darkDistance))
        {
            fadeToBlackBackground.color = fadeToBlackDefaultDarkColor;
        }
        else
        {
            fadeToBlackBackground.color = fadeToBlackDefaultTransparentColor;
        }
    }

}
