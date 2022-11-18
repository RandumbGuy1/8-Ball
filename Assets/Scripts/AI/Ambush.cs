using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambush : MonoBehaviour
{
    [Header("Light FX")]
    [SerializeField] private SpriteRenderer render;
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
    [SerializeField] private Transform[] ambushPoints;
    [Space(10)]
    [SerializeField] private bool woke = false;
    [SerializeField] private int cycles = 2;
    [SerializeField] private float travelTime;
    [SerializeField] private float timeBetweenCycles;
    private Queue<Transform> destinationQueue = new Queue<Transform>();

    [Header("Kill Settings")]
    [SerializeField] private float ambushForce;
    [SerializeField] private float killRadius;
    [SerializeField] private LayerMask killable;
    [SerializeField] private AmbushSafeZone[] safeZones;

    private Transform lastAmbushPoint = null;
    private float moveElapsed = 0f;
    private float cooldownElapsed = 0f;

    private List<Collider> ambushPrey = new List<Collider>();
    private AudioSource ambushSource = null;

    void Start()
    {
        ambushSource = AudioManager.Instance.PlayOnce(ambushClip, transform.position);

        moveElapsed = travelTime;
        cooldownElapsed = timeBetweenCycles;
    }

    Transform currentDestination = null;
    void Update()
    {
        UpdateColorToggle();

        //Return if no points to travel to
        if (ambushPoints.Length < 2) return;

        cooldownElapsed += Time.deltaTime;
        if (cooldownElapsed < timeBetweenCycles)
        {
            ambushSource.volume = 0f;
            UpdateGFX(false);
            return;
        }

        UpdateAudio();
        UpdateGFX(true);

        //If elapsed path time exceeds the time it takes to go to each point, set new destination
        if (moveElapsed >= travelTime)
        {
            if (destinationQueue.Count <= 1)
            {
                destinationQueue = GenerateCyclePaths(cycles);
                currentDestination = destinationQueue.Dequeue();
                transform.position = currentDestination.position;
                cooldownElapsed = 0f;
                return;
            }

            transform.position = currentDestination.position;
            lastAmbushPoint = currentDestination;

            currentDestination = destinationQueue.Dequeue();
            ambushPrey.Clear();
            moveElapsed = 0f;
        }

        if (!woke) return;

        //Move ambush to next destination using lerp
        transform.LookAt(currentDestination.position);
        transform.position = Vector3.Lerp(lastAmbushPoint.position, currentDestination.position, moveElapsed / travelTime);

        moveElapsed += Time.deltaTime;

        //Track any victims to ambush
        Collider[] cols = Physics.OverlapSphere(transform.position, killRadius, killable);
        foreach (Collider col in cols) AmbushHit(col);
    }

    //Update ambush epileptic effects
    private void UpdateColorToggle()
    {
        greenRedElapsed += Time.deltaTime * 14f;
        greenRedRatio = Mathf.PingPong(greenRedElapsed, 1f);

        foreach (Light light in searchLights)
        {
            light.color = greenRedRatio > 0.5f ? green : red;
        }
    }

    //Update ambush volume
    private void UpdateAudio()
    {
        if (ambushSource == null) return;

        float volumeRatio = Mathf.Clamp01((ambienceDistance - Vector3.Distance(transform.position, player.transform.position)) / ambienceDistance);
        if (volumeRatio > 0.8f) volumeRatio = 1f;
        
        ambushSource.volume =  volumeRatio;
    }

    //Function for ambushes victims
    private void AmbushHit(Collider col)
    {
        //Dont attack if already attacked this cycle
        if (ambushPrey.Contains(col)) return;

        //No Hit effects if player in safe zone
        foreach (AmbushSafeZone safeZone in safeZones) if (safeZone.SafeColliders.Contains(col)) return;

        //Add prey to the list
        ambushPrey.Add(col);

        //Register player hit
        PlayerRef player = col.GetComponent<PlayerRef>();
        if (player != null)
        {
            //return;

            player.PlayerMovement.GoLimp(3f);
            player.CameraBody.CamShaker.ShakeOnce(new PerlinShake(ShakeData.Create(45f, 8f, 3.5f, 12f)));
            AudioManager.Instance.PlayOnce(ambushHitClip, player.transform.position, 0.5f);
            StartCoroutine(flash.SequenceFlash(Color.black, 20f, 0.05f, 0.15f, 15));
        }

        //Register physics object
        Rigidbody hit = col.GetComponent<Rigidbody>();
        if (hit == null) return;

        hit.AddForce((transform.forward + Vector3.up + (col.transform.position - transform.position).normalized).normalized * ambushForce, ForceMode.VelocityChange);
        hit.AddTorque(200f * Random.insideUnitSphere, ForceMode.VelocityChange);
    }

    public void AwakeAmbush(bool woke = true) => this.woke = woke;

    private bool backTracking = false;
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

    private Queue<Transform> GenerateCyclePaths(int cycle)
    {
        Queue<Transform> queuedDestinations = new Queue<Transform>();

        //Generate paths based on given amount
        int currentPoint = 0;
        for (int i = 1; i <= cycle; i++)
        {
            for (int j = 0; j++ < ambushPoints.Length;)
            {
                currentPoint = GetNextPoint(currentPoint);
                queuedDestinations.Enqueue(ambushPoints[currentPoint]);
            }
        }

        return queuedDestinations;
    }

    private void UpdateGFX(bool active)
    {
        render.enabled = active;
        foreach (Light light in searchLights) light.enabled = active;
    }
}
