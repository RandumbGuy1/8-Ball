using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    [Header("Submerge Settings")]
    [SerializeField] private List<AudioClip> splashClips = new List<AudioClip>();
    [SerializeField] private AudioClip underWaterClip;
    [SerializeField] private AudioClip ripplesClip;
    [Space(10)]
    [SerializeField] private GameObject waterRipples;
    [SerializeField] private GameObject waterSplash;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private float buoyancy;
    [SerializeField] private float submergenceRequired = 0.1f;
    [SerializeField] private float waterDrag;
    [SerializeField] private float waterAngularDrag;

    [Header("Refrences")]
    [SerializeField] private Collider waterRegion;

    private Dictionary<Rigidbody, SubmergeeData> submergees = new Dictionary<Rigidbody, SubmergeeData>();

    void FixedUpdate()
    {
        List<Rigidbody> rbToRemove = new List<Rigidbody>();

        foreach (SubmergeeData entry in submergees.Values)
        {
            if (entry.Rb == null)
            {
                rbToRemove.Add(entry.Rb);
                continue;
            }

            if (!submergees.ContainsKey(entry.Rb)) continue;

            float submergence = EvaluateSubmergence(entry.Col);
            if (submergence < submergenceRequired) continue;
            
            //Apply Water Drag
            entry.Rb.velocity *= 1f - waterDrag * submergence * Time.fixedDeltaTime;
            entry.Rb.angularVelocity *= 1f - waterAngularDrag * submergence * Time.fixedDeltaTime;

            //Apply Bouyancy
            entry.Rb.AddForce((1f - buoyancy * (submergence * submergence)) * Time.fixedDeltaTime * Physics.gravity, ForceMode.VelocityChange);

            //Apply Ripple Effects
            if (entry.Rb.velocity.sqrMagnitude > 36f || submergence > 0.75f) entry.Ripples.Play();
            else entry.Ripples.Stop();

            //Play underwater ambience
            if (entry.Player == null) continue;

            entry.Player.CameraBody.SetUnderWaterVolumeWeight(submergence * submergence);

            if (submergence > 0.85f) AudioManager.Instance.PlayOnce(underWaterClip);
            else AudioManager.Instance.StopSound(underWaterClip);
        }

        foreach (Rigidbody rb in rbToRemove) submergees.Remove(rb);
    }

    void OnTriggerEnter(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == null || submergees.ContainsKey(rb)) return;

        ParticleSystem ripples = ObjectPooler.Instance.Spawn(waterRipples, true, col.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        ripples.transform.SetParent(rb.transform);

        PlayerRef submergedPlayer = col.GetComponent<PlayerRef>();
        SubmergeeData toAdd = new SubmergeeData(submergedPlayer, col, rb, ripples);
        submergees.Add(rb, toAdd);

        PlaySplashEffect(toAdd, 2);

        //Allow Player to Swim   
        if (submergedPlayer == null) return;
        submergedPlayer.PlayerMovement.InWater = true;
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == null || !submergees.ContainsKey(rb)) return;

        PlaySplashEffect(submergees[rb], 1);

        PlayerRef submergedPlayer = submergees[rb].Player;
        ParticleSystem ripples = submergees[rb].Ripples;
        submergees.Remove(rb);

        ripples.transform.SetParent(null);
        ripples.gameObject.SetActive(false);
        ripples.Stop();
        AudioManager.Instance.StopSound(ripplesClip);

        if (submergedPlayer == null) return;

        submergedPlayer.CameraBody.SetUnderWaterVolumeWeight(0);
        submergedPlayer.PlayerMovement.InWater = false;
        AudioManager.Instance.StopSound(underWaterClip);
    }

    private void PlaySplashEffect(SubmergeeData data, int splashCount, int iterCount = 1)
    {
        if (iterCount > 2) return;

        float magnitude = data.Rb.velocity.magnitude;
        float magnitudeMulti = Mathf.Max(1f, magnitude * 0.25f) / 10;

        if (iterCount == 1) AudioManager.Instance.PlayOnce(splashClips[Mathf.RoundToInt(Random.Range(0, splashClips.Count - 1))], Mathf.Clamp01(magnitudeMulti));

        if (magnitude < 10f) return;

        Vector3 colToWater = transform.position - data.Col.transform.position;
        Vector3 normal = Physics.Raycast(data.Col.transform.position, colToWater.normalized, out var hit, colToWater.magnitude, waterMask) ? hit.normal : Vector3.up;
        ParticleSystem splash = ObjectPooler.Instance.Spawn(waterSplash, true, data.Col.transform.position, Quaternion.LookRotation(normal * (iterCount % 2 == 0 ? -1f : 1f))).GetComponent<ParticleSystem>();
        
        splash.transform.localScale = Vector3.one * magnitudeMulti;

        ParticleSystem.EmissionModule em = splash.emission;
        em.rateOverTimeMultiplier = magnitudeMulti;

        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = splash.velocityOverLifetime;
        velocityOverLifetime.radialMultiplier += magnitudeMulti;

        iterCount++;
        PlaySplashEffect(data, splashCount, iterCount);
    }

    /*
    private float EvaluateSubmergence(Collider col)
    {
        if (!Physics.Raycast(
            col.transform.position + col.transform.up * submergenceOffset,
            -col.transform.up, out var hit, col.bounds.size.y,
            waterMask, QueryTriggerInteraction.Collide)) return 1f;

        return 1f - hit.distance / col.bounds.size.y;
    }
    */

    private float EvaluateSubmergence(Collider submergee)
    {
        float total = 1f;
        Bounds obj = submergee.bounds;
        Bounds region = waterRegion.bounds;

        for (int i = 0, dimensions = 3; i < dimensions; i++)
        {
            float dist = obj.min[i] > region.center[i] ?
                obj.max[i] - region.max[i] :
                region.min[i] - obj.min[i];

            total *= Mathf.Clamp01(1f - dist / obj.size[i]);
        }

        return total;
    }
}
