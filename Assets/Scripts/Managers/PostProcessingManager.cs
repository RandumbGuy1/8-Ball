using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;


public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume volume;
    
    [Header("Exposure Settings")]
    [SerializeField] private float exposureSmoothing;
    private float exposureVel;
    private float desiredExposure;
    private float setExposure;

    private Exposure exposure;
    private bool updateExposure;

    public static PostProcessingManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        updateExposure = volume.profile.TryGet(out exposure);
        setExposure = exposure.fixedExposure.value;
        desiredExposure = setExposure;

        StartCoroutine(UpdatePostProcessing());
    }

    public void AddExposure(float value)
    {
        desiredExposure -= value;
    }

    private IEnumerator UpdatePostProcessing()
    {
        while (true)
        {
            if (updateExposure)
            {
                desiredExposure = Mathf.Lerp(desiredExposure, setExposure, 2f * Time.deltaTime);
                SmoothlyChangeValues(result => exposure.fixedExposure.value = result, () => exposure.fixedExposure.value, exposureSmoothing, desiredExposure, ref exposureVel);
            }
            yield return null;
        }
    }

    private void SmoothlyChangeValues(Action<float> SetValue, Func<float> Value, float smoothing, float intensity, ref float velocity)
    {
        if (Value() == intensity) return;

        SetValue(Mathf.SmoothDamp(Value(), intensity, ref velocity, smoothing));

        if (Math.Abs(intensity - Value()) < 0.01f) SetValue(intensity);
    }
}
