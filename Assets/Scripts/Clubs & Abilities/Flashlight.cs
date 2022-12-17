using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flashlight : MonoBehaviour, IItem
{
    [SerializeField] private Light emmisionLight;
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private GunSway swaySettings = new GunSway();
    [SerializeField] private ItemRigidbodySettings rigidbodySettings = new ItemRigidbodySettings();
    [SerializeField] private ItemHoldSettings holdSettings = new ItemHoldSettings();
    [SerializeField] private ItemArtSettings clubSpriteSettings = new ItemArtSettings();
    [SerializeField] private List<ItemArtSettings> clubAbilitySpriteSettings = new List<ItemArtSettings>();

    [Header("Refrences")]
    [SerializeField] private ItemPickup pickup;
    [SerializeField] private Collider itemCol;

    public ItemRigidbodySettings RigidbodySettings => rigidbodySettings;
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

        player.PlayerInput.OnMouseButtonDownInput -= ToggleFlashlight;
    }

    private void ToggleFlashlight(int index)
    {
        if (index != 0 || !gameObject.activeSelf) return;

        emmisionLight.gameObject.SetActive(!emmisionLight.gameObject.activeSelf);
        AudioManager.Instance.PlayOnce(switchSound, transform.position);
    }

    public void ItemUpdate(PlayerRef player)
    {
        swaySettings.Update(player);

        transform.localPosition = -player.CameraBody.CamHeadBob.ViewBobOffset * 0.03f;
        transform.localRotation = Quaternion.Euler(swaySettings.SwayOffsetRot + CameraBody.ToEuler(player.CameraBody.CamHeadBob.ViewBobOffset * 2f) - player.CameraBody.CamIdleSway.HeadSwayOffset);
    }

    public void OnPickup(PlayerRef player)
    {
        itemCol.enabled = false;
        pickup.PickedUp = true;

        Destroy(itemRb);
        itemRb = null;

        player.PlayerInput.OnMouseButtonDownInput += ToggleFlashlight;
    }
}
