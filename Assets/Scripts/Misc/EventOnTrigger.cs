using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEvent;
    [SerializeField] private LayerMask triggerLayer;

    void OnTriggerEnter(Collider col)
    {
        int layer = col.gameObject.layer;
        if (triggerLayer != (triggerLayer | 1 << layer)) return;

        triggerEvent?.Invoke();
    }
}
