﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificaitonFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset;

    void Awake() => offset = transform.position - target.position;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
