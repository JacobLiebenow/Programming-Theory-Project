using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TransformOscillator : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.05f;
    [SerializeField] private float frequencyRadians = 0.75f;
    [SerializeField] private float amplitudeOffset = 1f;
    [SerializeField] private float frequencyOffset = 0f;
    private float baseX;
    private float baseY;

    // After the base elements are loaded in, call to set the base scale values
    private void Start()
    {
        baseX = transform.localScale.x;
        baseY = transform.localScale.y;
    }


    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            OscillateTransform();
        }
    }

    // Make the given UI component's X and Y-scale values shift at a sinusoidal rate
    private void OscillateTransform()
    {
        float newX = baseX * (amplitude * Mathf.Sin(frequencyRadians * Mathf.PI * Time.unscaledTime + frequencyOffset) + amplitudeOffset);
        float newY = baseY * (amplitude * Mathf.Sin(frequencyRadians * Mathf.PI * Time.unscaledTime + frequencyOffset) + amplitudeOffset);
        Vector2 newScale = new Vector2(newX, newY);
        transform.localScale = newScale;
    }
}
