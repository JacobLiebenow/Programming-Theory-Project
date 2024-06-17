using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float dragSpeed = 2f;
    private Vector3 startingCameraPosition;
    private bool isDragging = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            startingCameraPosition = Input.mousePosition;
            Debug.Log("Moving camera!");
            isDragging = true;
        } 
        else if(Input.GetMouseButton(2))
        {
            Vector3 newPos = Camera.main.ScreenToViewportPoint(Input.mousePosition - startingCameraPosition);
            Vector3 moveTo = new Vector3(newPos.x * dragSpeed * Time.deltaTime, newPos.y * dragSpeed * Time.deltaTime, 0);

            transform.Translate(moveTo);
            Debug.Log("Camera is moving");
        } 
        else if (Input.GetMouseButtonUp(2) && isDragging)
        {
            isDragging = false;
        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isDragging)
        {
            transform.Translate(Vector3.up * dragSpeed * Time.deltaTime);
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isDragging)
        {
            transform.Translate(Vector3.up * -dragSpeed * Time.deltaTime);
        }

        if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isDragging)
        {
            transform.Translate(Vector3.left * dragSpeed * Time.deltaTime);
        }
        else if((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isDragging) 
        {
            transform.Translate(Vector3.left * -dragSpeed * Time.deltaTime);
        }
    }
}
