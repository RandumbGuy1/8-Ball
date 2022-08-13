using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CameraFilters 
{
    [SerializeField] private Volume regularVolume;
    [SerializeField] private Volume underWaterVolume;
    [SerializeField] private float filterSmoothTime;
    private float targetWeight = 0f;

    public void UpdateUnderWaterFilter()
    {
        if (Mathf.Abs(targetWeight - underWaterVolume.weight) < 0.01f)
        {
            underWaterVolume.weight = targetWeight;
            return;
        }

        underWaterVolume.weight = Mathf.Lerp(underWaterVolume.weight, targetWeight, filterSmoothTime * Time.deltaTime);
    }

    public void SetUnderWaterVolumeWeight(float value) => targetWeight = value;
}
