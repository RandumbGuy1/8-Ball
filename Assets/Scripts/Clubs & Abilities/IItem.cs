using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IItem 
{
    ItemRigidbodySettings RigidbodySettings { get; }
    ItemHoldSettings HoldSettings { get; }
    ItemArtSettings SpriteSettings { get; }
    List<ItemArtSettings> AbilitySpriteSettings { get; }

    void OnPickup(PlayerRef player);
    void OnDrop(PlayerRef player, Action<Rigidbody> DropForce);
}

[System.Serializable]
public struct ItemArtSettings
{
    [SerializeField] private string itemText;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Vector3 scale;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Vector3 position;
    [SerializeField] private bool notVisible;

    public string ItemText => itemText;
    public Sprite ItemSprite => itemSprite;
    public Vector3 Scale => scale;
    public Vector3 Rotation => rotation;
    public Vector3 Position => position;
    public bool NotVisible => notVisible;
}

[System.Serializable]
public struct ItemRigidbodySettings
{
    [SerializeField] private CollisionDetectionMode collisionDetectionMode;
    [SerializeField] private RigidbodyInterpolation interpolation;
    [SerializeField] private float drag;
    [SerializeField] private float angularDrag;
    [SerializeField] private float mass;

    public void SetRigidbody(Rigidbody newRb)
    {
        newRb.drag = drag;
        newRb.angularDrag = angularDrag;
        newRb.mass = mass;
        newRb.collisionDetectionMode = collisionDetectionMode;
        newRb.interpolation = interpolation;
    }

    public Rigidbody CreateBodyWithSettings()
    {
        Rigidbody newRb = new Rigidbody();
        newRb.drag = drag;
        newRb.angularDrag = angularDrag;
        newRb.mass = mass;
        newRb.collisionDetectionMode = collisionDetectionMode;
        newRb.interpolation = interpolation;

        return newRb;
    }
}

[System.Serializable]
public struct ItemHoldSettings
{
    [SerializeField] private Vector3 defaultPos;
    [SerializeField] private Vector3 defaultRot;
    [SerializeField] private float pickupSmoothTime;

    [SerializeField] private Vector3 switchOffsetPos;
    [SerializeField] private Vector3 switchOffsetRot;

    public Vector3 DefaultPos => defaultPos;
    public Vector3 DefaultRot => defaultRot;
    public float PickupSmoothTime => pickupSmoothTime;

    public Vector3 SwitchOffsetPos => switchOffsetPos;
    public Vector3 SwitchOffsetRot => switchOffsetRot;
}
