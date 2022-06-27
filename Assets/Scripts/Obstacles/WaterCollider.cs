using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    [Header("Submerge Settings")]
    [SerializeField] private GameObject waterSplash;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private float buoyancy;
    [SerializeField] private float submergenceRequired = 0.1f;
    [SerializeField] private float waterDrag;
    [SerializeField] private float waterAngularDrag;
    [SerializeField] private float submergenceOffset = 0.5f;

    private Dictionary<Rigidbody, Collider> submergees = new Dictionary<Rigidbody, Collider>();

    void FixedUpdate()
    {
        foreach (KeyValuePair<Rigidbody, Collider> entry in submergees)
        {
            float submergence = EvaluateSubmergence(entry.Value);
            if (submergence < submergenceRequired) continue;
            
            //Apply Water Drag
            entry.Key.velocity *= 1f - waterDrag * submergence * Time.fixedDeltaTime;
            entry.Key.angularVelocity *= 1f - waterAngularDrag * submergence * Time.fixedDeltaTime;

            //Apply Bouyancy
            entry.Key.AddForce((1f - buoyancy * (submergence * submergence)) * Time.fixedDeltaTime * Physics.gravity, ForceMode.VelocityChange);

            //Allow Player to Swim
            PlayerMovement submergedPlayer = entry.Value.GetComponent<PlayerMovement>();
            if (submergedPlayer == null) continue;

            submergedPlayer.InWater = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == null || submergees.ContainsKey(rb)) return;

        PlaySplashEffect(col, rb);
        submergees.Add(rb, col);
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == null || !submergees.ContainsKey(rb)) return;

        PlaySplashEffect(col, rb);
        submergees.Remove(rb);

        PlayerMovement submergedPlayer = col.GetComponent<PlayerMovement>();
        if (submergedPlayer == null) return;

        submergedPlayer.InWater = false;
    }

    private void PlaySplashEffect(Collider col, Rigidbody rb)
    {
        float magnitude = rb.velocity.magnitude;
        if (magnitude < 10f) return;

        Vector3 colToWater = transform.position - col.transform.position;
        Vector3 normal = Physics.Raycast(col.transform.position, colToWater.normalized, out var hit, colToWater.magnitude, waterMask) ? hit.normal : Vector3.up;
        ParticleSystem splash = ObjectPooler.Instance.Spawn(waterSplash, true, col.transform.position, Quaternion.LookRotation(normal)).GetComponent<ParticleSystem>();

        float magnitudeMulti = Mathf.Max(1f, magnitude * 0.25f) / 10;
        splash.transform.localScale = Vector3.one * magnitudeMulti;

        ParticleSystem.EmissionModule em = splash.emission;
        em.rateOverTimeMultiplier = magnitudeMulti;

        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = splash.velocityOverLifetime;
        velocityOverLifetime.radialMultiplier += magnitudeMulti;
    }

    private float EvaluateSubmergence(Collider col)
    {
        if (!Physics.Raycast(
            col.transform.position + col.transform.up * submergenceOffset,
            -col.transform.up, out var hit, col.bounds.size.y,
            waterMask, QueryTriggerInteraction.Collide)) return 1f;

        return 1f - hit.distance / col.bounds.size.y;
    }
}
