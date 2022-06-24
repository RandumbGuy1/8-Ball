using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOnWalls : MonoBehaviour
{
    [SerializeField] private LayerMask environment;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float bounceMultiplier;

    void OnCollisionEnter(Collision col)
    {
        int layer = col.gameObject.layer;
        if (environment != (environment | 1 << layer)) return;

        ContactPoint contact = col.GetContact(0);

        if (Mathf.Abs(contact.normal.y) > 0.1f) return;

        Vector3 reflectionVel = Vector3.Reflect(rb.velocity, contact.normal);
        rb.velocity = reflectionVel * bounceMultiplier;
    }
}
