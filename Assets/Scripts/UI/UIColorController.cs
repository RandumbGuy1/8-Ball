using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorController : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float startOpacity;
    [SerializeField] private float animationSmoothing;

    private float desiredOpacity;
    private float smoothOpacity;
    private float opacityVel;

    void OnDisable()
    {
        smoothOpacity = 0f;
    }

    void Update()
    {
        smoothOpacity = Mathf.SmoothDamp(smoothOpacity, desiredOpacity, ref opacityVel, animationSmoothing);

        Color newOpacity = image.color;
        newOpacity.a = smoothOpacity;

        image.color = newOpacity;
    }

    public void SnapOpacity(float newOpacity) => smoothOpacity = Mathf.Clamp(newOpacity, 0f, 225f);
    public void SetOpacity(float newOpacity) => desiredOpacity = Mathf.Clamp(newOpacity, 0f, 225f);
}
