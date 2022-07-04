using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCollider : MonoBehaviour
{
    [Header("Interactive Intensity")]
    [SerializeField] private LayerMask collidesWithGrass;
    [SerializeField] private float swayAmount;
    [SerializeField] private float dragAmount;
    [SerializeField] private Renderer grassRenderer;

    void OnTriggerStay(Collider other)
    {
        int layer = other.gameObject.layer;
        if (collidesWithGrass == (collidesWithGrass | 1 << layer)) return;

        print("lmao gras toucher!");
    }
}
