using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelFlash : MonoBehaviour
{
    [SerializeField] private Image panel;
    private float flashSpeed = 1f;
    private float pauseTime = 0f;
    private float pauseElapsed = 0f;

    void Update()
    {
        pauseElapsed += Time.deltaTime;
        if (pauseElapsed < pauseTime) return;

        panel.color = Color.Lerp(panel.color, Color.clear, Time.deltaTime * flashSpeed);
    }

    public void Flash(Color colorToFlash, float flashSpeed, float pauseTime = 0f)
    {
        panel.color = colorToFlash;
        this.flashSpeed = flashSpeed;

        pauseElapsed = 0f;
        this.pauseTime = pauseTime;
    }

    public IEnumerator SequenceFlash(Color colorToFlash, float flashSpeed, float pauseTime, float timeBetweenFlashes, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            Flash(colorToFlash, flashSpeed, pauseTime);
            yield return new WaitForSeconds(timeBetweenFlashes);
        }
    }
}
