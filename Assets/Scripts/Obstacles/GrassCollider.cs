using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCollider : MonoBehaviour
{
    [Header("Interactive Intensity")]
    [SerializeField] private LayerMask collidesWithGrass;
    [SerializeField] private float swayAmount;
    [SerializeField] private float returnSpeed;
    [SerializeField] private Renderer grassRenderer;
    [SerializeField] private SphereCollider grassCol;

    private Vector3 desiredOffset = Vector3.zero;
    private Vector3 smoothOffset = Vector3.zero;
    private int collisionCount = 0;

    private string grassX = "Vector1_C2B311AE";
    private string grassY = "Vector1_FFFD2E3C";
    private string grassZ = "Vector1_DC8465DA";

    void FixedUpdate()
    {
        if (collisionCount <= 0) desiredOffset = Vector3.Lerp(desiredOffset, Vector3.zero, returnSpeed * 0.4f * Time.fixedDeltaTime);
        smoothOffset = Vector3.Lerp(smoothOffset, desiredOffset, returnSpeed * Time.fixedDeltaTime);

        grassRenderer.material.SetFloat(grassX, smoothOffset.x);
        grassRenderer.material.SetFloat(grassY, smoothOffset.y);
        grassRenderer.material.SetFloat(grassZ, smoothOffset.z);
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if (collidesWithGrass != (collidesWithGrass | 1 << layer)) return;

        collisionCount++;
    }

    void OnTriggerStay(Collider other)
    {
        int layer = other.gameObject.layer;
        if (collidesWithGrass != (collidesWithGrass | 1 << layer)) return;

        Vector3 swayDir = transform.position - other.transform.position;
        swayDir.y = -0.1f;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null) desiredOffset = (grassCol.radius / swayDir.magnitude) * swayAmount * swayDir.normalized;
        else desiredOffset = (grassCol.radius / swayDir.magnitude) * swayAmount * swayDir.normalized + (rb.velocity.normalized * 0.6f);
    }

    void OnTriggerExit(Collider other)
    {
        int layer = other.gameObject.layer;
        if (collidesWithGrass != (collidesWithGrass | 1 << layer)) return;

        if (collisionCount > 0) collisionCount--;
    }
}
