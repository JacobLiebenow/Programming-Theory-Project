using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;

    private float camHeight;
    private float camWidth;
    private float boundsLeftX;
    private float boundsRightX;
    private float boundsTopY;
    private float boundsBottomY;

    private Vector3 startingCameraPosition;

    private bool isDragging = false;
    public float moveSpeed = 2f;
    private float dragSpeed;
    private float dragSpeedMult = 3f;

    // Start is called before the first frame is processed
    private void Start()
    {
        SetCameraDimensions();
        SetCameraBounds();

        dragSpeed = moveSpeed * dragSpeedMult;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMiddleMouseDrag();
        HandleWASDMovement();
    }

    //ABSTRACTION
    private void SetCameraDimensions()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
        Debug.Log("Cam Height: " + camHeight);
        Debug.Log("Cam Width: " + camWidth);
    }

    //ABSTRACTION
    private void SetCameraBounds()
    {
        boundsLeftX = camWidth + terrainGenerator.gridPadding;
        boundsRightX = terrainGenerator.width - camWidth + terrainGenerator.gridPadding;
        boundsTopY = terrainGenerator.height - camHeight + terrainGenerator.gridPadding;
        boundsBottomY = camHeight + terrainGenerator.gridPadding;
    }

    //ABSTRACTION
    private void HandleMiddleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            startingCameraPosition = Input.mousePosition;
            Debug.Log("Moving camera!");
            isDragging = true;
        }
        else if (Input.GetMouseButton(2))
        {
            Vector3 newPos = Camera.main.ScreenToViewportPoint(Input.mousePosition - startingCameraPosition);
            Vector3 moveTo = new Vector3(newPos.x * dragSpeed * Time.deltaTime, newPos.y * dragSpeed * Time.deltaTime, 0);

            transform.Translate(moveTo);
            ClampCameraPosition();

        }
        else if (Input.GetMouseButtonUp(2) && isDragging)
        {
            isDragging = false;
        }
    }

    //ABSTRACTION
    private void HandleWASDMovement()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isDragging)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            ClampCameraPosition();
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isDragging)
        {
            transform.Translate(Vector3.up * -moveSpeed * Time.deltaTime);
            ClampCameraPosition();
        }

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isDragging)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            ClampCameraPosition();
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isDragging)
        {
            transform.Translate(Vector3.left * -moveSpeed * Time.deltaTime);
            ClampCameraPosition();
        }
    }

    //ABSTRACTION
    private void ClampCameraPosition()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundsLeftX, boundsRightX), Mathf.Clamp(transform.position.y, boundsBottomY, boundsTopY), Camera.main.transform.position.z);
    }
}
