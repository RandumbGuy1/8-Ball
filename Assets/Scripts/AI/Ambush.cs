using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambush : MonoBehaviour
{
    [Header("Light FX")]
    [SerializeField] private PanelFlash flash;
    [SerializeField] private Color green;
    [SerializeField] private Color red;
    [SerializeField] private Light[] searchLights;
    private float greenRedRatio = 0f;
    private float greenRedElapsed = 0f;

    [Header("Sound FX")]
    [SerializeField] private AudioClip ambushClip;
    [SerializeField] private AudioClip ambushHitClip;
    [SerializeField] private PlayerRef player;
    [SerializeField] float ambienceDistance;

    [Header("Movement Settings FX")]
    [SerializeField] private bool woke = false;
    [SerializeField] private Transform[] ambushPoints;
    [SerializeField] private float travelTime;
    [SerializeField] private float timeBetweenPoints;

    [Header("Kill Settings")]
    [SerializeField] private float killRadius;
    [SerializeField] private LayerMask killable;
    [SerializeField] private AmbushSafeZone[] safeZones;

    private Transform lastAmbushPoint = null;
    private bool backTracking = false;
    private float elapsed = 9999f;
    private int currentPoint = 0;

    private List<Collider> ambushPrey = new List<Collider>();
    private AudioSource ambushSource = null;

    void Start()
    {
        ambushSource = AudioManager.Instance.PlayOnce(ambushClip, transform.position);
    }

    void Update()
    {
        UpdateColorToggle();
        UpdateAudio();

        if (ambushPoints.Length <= 1) return;

        Transform destination = ambushPoints[currentPoint];

        if (elapsed >= travelTime + timeBetweenPoints)
        {
            currentPoint = GetNextPoint(currentPoint);
            transform.position = destination.position;
            lastAmbushPoint = destination;

            elapsed = 0f;
            ambushPrey.Clear();
            return;
        }

        if (!woke) return;

        elapsed += Time.deltaTime;

        if (elapsed >= travelTime) return;

        transform.LookAt(destination.position);
        transform.position = Vector3.Lerp(lastAmbushPoint.position, destination.position, elapsed / travelTime);

        Collider[] cols = Physics.OverlapSphere(transform.position, killRadius, killable);
        foreach (Collider col in cols) if (!ambushPrey.Contains(col)) AmbushHit(col);
    }

    private void UpdateColorToggle()
    {
        greenRedElapsed += Time.deltaTime * 10f;
        greenRedRatio = Mathf.PingPong(greenRedElapsed, 1f);

        foreach (Light light in searchLights) light.color = Color.Lerp(green, red, greenRedRatio > 0.5f ? 1f : 0f);
    }

    private void UpdateAudio()
    {
        if (ambushSource == null) return;

        float volumeRatio = Mathf.Clamp01((ambienceDistance - Vector3.Distance(transform.position, player.transform.position)) / ambienceDistance);
        ambushSource.volume = volumeRatio;
    }

    private void AmbushHit(Collider col)
    {
        //No Hit effects if player in safe zone
        foreach (AmbushSafeZone safeZone in safeZones) if (safeZone.SafeColliders.Contains(col)) return;

        //Add prey to the list
        ambushPrey.Add(col);

        //Register player hit
        PlayerRef player = col.GetComponent<PlayerRef>();
        if (player != null)
        {
            player.PlayerMovement.GoLimp(3f);
            player.CameraBody.CamShaker.ShakeOnce(new PerlinShake(ShakeData.Create(45f, 8f, 3.5f, 12f)));
            AudioManager.Instance.PlayOnce(ambushHitClip, player.transform.position, 0.5f);
            StartCoroutine(flash.SequenceFlash(Color.black, 20f, 0.05f, 0.2f, 10));
        }

        //Register physics object
        Rigidbody hit = col.GetComponent<Rigidbody>();
        if (hit == null) return;

        hit.AddForce((transform.forward + Vector3.up + (col.transform.position - transform.position).normalized).normalized * 30f, ForceMode.VelocityChange);
        hit.AddTorque(40f * Random.insideUnitSphere, ForceMode.VelocityChange);
    }

    public void AwakeAmbush(bool woke = true) => this.woke = woke;

    private int GetNextPoint(int currentPoint)
    {
        if (backTracking)
        {
            if (currentPoint - 1 < 0)
            {
                backTracking = false;
                return currentPoint + 1;
            }

            return currentPoint - 1;
        }

        if (currentPoint + 1 == ambushPoints.Length)
        {
            backTracking = true;
            return currentPoint - 1;
        }

        return currentPoint + 1;
    }
}
