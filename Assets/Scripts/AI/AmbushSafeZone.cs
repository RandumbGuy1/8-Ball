using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushSafeZone : MonoBehaviour
{
    public List<Collider> SafeColliders { get; private set; } = new List<Collider>();

    void OnTriggerEnter(Collider col)
    {
        if (!SafeColliders.Contains(col)) SafeColliders.Add(col);
    }

    void OnTriggerExit(Collider col)
    {
        if (SafeColliders.Contains(col)) SafeColliders.Remove(col);
    }
}
