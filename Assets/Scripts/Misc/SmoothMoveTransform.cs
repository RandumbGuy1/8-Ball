using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveTransform : MonoBehaviour
{
    [SerializeField] private Transform affected;
    [SerializeField] private float posSmoothTime;
    [SerializeField] private float rotAngle;
    private Vector3 targetPos, posVel = Vector3.zero;

    void Start()
    {
        targetPos = affected.position;
    }

    void Update()
    {
        if ((targetPos - affected.position).sqrMagnitude < 0.001f) return;

        affected.position = Vector3.SmoothDamp(affected.position, targetPos, ref posVel, posSmoothTime);
        affected.Rotate(Vector3.up, rotAngle);
    }

    public void Fly(float height) => targetPos.y += height;
}
