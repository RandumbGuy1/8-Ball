using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicClub : MonoBehaviour, IClub, IItem
{
    [SerializeField] private GunSway gunSwaySettings = new GunSway();
    [SerializeField] private ClubStats stats = new ClubStats();
    [SerializeField] private ItemRigidbodySettings rigidbodySettings = new ItemRigidbodySettings();
    [SerializeField] private ItemHoldSettings holdSettings = new ItemHoldSettings();
    [SerializeField] private ItemArtSettings clubSpriteSettings = new ItemArtSettings();
    [SerializeField] private List<ItemArtSettings> clubAbilitySpriteSettings = new List<ItemArtSettings>();

    [Header("Refrences")]
    [SerializeField] private ItemPickup pickup;
    [SerializeField] private Collider itemCol;

    public ItemRigidbodySettings RigidbodySettings => rigidbodySettings;
    public ClubStats Stats => stats;
    public ItemHoldSettings HoldSettings => holdSettings;
    public ItemArtSettings SpriteSettings => clubSpriteSettings;
    public List<ItemArtSettings> AbilitySpriteSettings => clubAbilitySpriteSettings;

    private Rigidbody itemRb;

    void Awake()
    {
        itemRb = gameObject.AddComponent<Rigidbody>();
        rigidbodySettings.SetRigidbody(itemRb);
    }

    public void OnDrop(PlayerRef player, Action<Rigidbody> DropForce)
    {
        itemCol.enabled = true;
        pickup.PickedUp = false;

        if (itemRb == null)
        {
            itemRb = gameObject.AddComponent<Rigidbody>();
            rigidbodySettings.SetRigidbody(itemRb);
            DropForce(itemRb);
        }
    }

    public void OnPickup(PlayerRef player)
    {
        itemCol.enabled = false;
        pickup.PickedUp = true;

        Destroy(itemRb);
        itemRb = null;
    }

    public void ItemUpdate(PlayerRef player)
    {
        gunSwaySettings.Update(player);

        transform.localPosition = -player.CameraBody.CamHeadBob.ViewBobOffset * 0.03f;
        transform.localRotation = Quaternion.Euler(gunSwaySettings.SwayOffsetRot + CameraBody.ToEuler(player.CameraBody.CamHeadBob.ViewBobOffset * 2f) - player.CameraBody.CamIdleSway.HeadSwayOffset);
    }

    public void ThrustBalls(PlayerRef player)
    {
        print(player + ": Thrusted my balls :(");
    }

    public void UseClubAbility(PlayerRef player)
    {
        print(player + ": Used club ability on my balls :(");
    }

    public void UseMovementAbility(PlayerRef player)
    {
        print(player + ": Used movement ability on my balls :(");
    }
}
