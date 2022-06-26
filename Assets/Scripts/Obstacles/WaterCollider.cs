using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    [Header(" Submerge Settings")]
    [SerializeField] private float dragMultiplier;
    [SerializeField] private float angularDragMultiplier;

    private Dictionary<Rigidbody, float> submergees = new Dictionary<Rigidbody, float>();

    void OnTriggerEnter(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == null) return;

        submergees.Add(rb, EvaluateSubmergence(col));

        rb.drag += dragMultiplier;
        rb.angularDrag += angularDragMultiplier;
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.drag -= dragMultiplier;
        rb.angularDrag -= angularDragMultiplier;
    }

    private float EvaluateSubmergence(Collider col)
    {
        return 1f;
    }
}
