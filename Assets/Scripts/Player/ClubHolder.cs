﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClubHolder : MonoBehaviour
{
    [Header("Equip Settings")]
    [SerializeField] private float throwForce;
    [SerializeField] private List<GameObject> clubs = new List<GameObject>();
    [SerializeField] private List<GameObject> queueClubs = new List<GameObject>();
    [SerializeField] private ClubEquipController clubUI;
    [SerializeField] private int selectedClub;
    [SerializeField] private int maxClubs;
    [SerializeField] private float clubSwitchCooldown;
    private float switchTimer = 0f;

    public float ClubSwitchCooldown => clubSwitchCooldown;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI clubEquipText;
    [SerializeField] private Image clubEquipArt;

    [SerializeField] private List<TextMeshProUGUI> clubAbilitiesText = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> clubAbilitiesArt = new List<Image>();
    private Vector3[] spriteStartingPositions;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;
    [SerializeField] private Transform weaponPos;
    [SerializeField] private ClubSway clubSway;

    public IClub EquippedClub { get; private set; }
    public IItem EquippedItem { get; private set; }
    public GameObject ItemGameObject { get; private set; }

    void Awake()
    {
        //Save all positions of the starting UI images
        spriteStartingPositions = new Vector3[clubAbilitiesArt.Count];
        for (int i = 0; i < spriteStartingPositions.Length; i++)
            spriteStartingPositions[i] = clubAbilitiesArt[i].rectTransform.localPosition;

        player.PlayerInput.OnClubDropInput += CheckForDrop;
        player.PlayerInput.OnClubSelectInput += CheckForSwitchClub;

        //If no clubs reset everything
        if (queueClubs.Count <= 0) ResetData();
    }

    private void CheckForSwitchClub(int newSelect)
    {
        if (EquippedClub == null) return;

        float lastTimer = switchTimer;
        switchTimer = Mathf.Max(0f, switchTimer - Time.deltaTime);

        //Update UI when the new club is fully equipped
        if (lastTimer != 0 && switchTimer == 0)
        {
            clubUI.HideUI(false);
            UpdateClubUI();
        }

        //If not inputting a keybind or using an invalid one return
        if (newSelect == -1 || newSelect + 1 > clubs.Count) return;

        int previousClub = selectedClub;
        selectedClub = newSelect;

        if (previousClub != selectedClub) SelectClub();
    }

    private void CheckForDrop(bool dropping)
    {   
        //Add the qued clubs into inventory
        if (queueClubs.Count > 0)
        {
            foreach (GameObject club in queueClubs) AddClub(club);
            queueClubs.Clear();
        }

        if (!dropping || EquippedItem == null || clubs.Count == 0) return;

        DropClub();
    }

    public void AddClub(GameObject newClub)
    {
        if (clubs.Contains(newClub)) return;

        IItem newItem = newClub.GetComponent<IItem>();
        if (newItem == null) return;

        if (clubs.Count >= maxClubs)
        {
            DropClub(true);
            clubs.Insert(selectedClub, newClub);
        }
        else
        {
            clubs.Add(newClub);
            selectedClub = clubs.Count - 1;
        }

        SelectClub(false);
        switchTimer = clubSwitchCooldown;
        
        newItem.OnPickup(player);
        newClub.transform.SetParent(weaponPos);
    }

    private void SelectClub(bool switching = true)
    {
        if (clubs.Count <= 0 || switchTimer > 0f) return;

        for (int i = 0; i < clubs.Count; i++) clubs[i].SetActive(i == selectedClub);

        ItemGameObject = clubs[selectedClub];

        EquippedClub = ItemGameObject.GetComponent<IClub>();
        EquippedItem = ItemGameObject.GetComponent<IItem>();
        switchTimer = clubSwitchCooldown;
        clubUI.HideUI();

        if (switching) clubSway.AddSwitchOffset(EquippedItem.HoldSettings.SwitchOffsetPos, EquippedItem.HoldSettings.SwitchOffsetRot);
    }

    private void DropClub(bool pickupDrop = false)
    {
        if (clubs.Count <= 0 || switchTimer > 0f) return;

        ItemGameObject.transform.SetParent(null);

        EquippedItem.OnDrop(player, (rigidbody) => {
            rigidbody.velocity = player.PlayerMovement.Rb.velocity;
            rigidbody.AddForce(player.PlayerCam.transform.forward * throwForce + Vector3.up * 1.3f, ForceMode.Impulse);

            Vector3 rand = Vector3.zero;

            rand.x = Random.Range(-1f, 1f);
            rand.y = Random.Range(-1f, 1f);
            rand.z = Random.Range(-1f, 1f);

            rigidbody.AddTorque(3f * throwForce * rand.normalized, ForceMode.Impulse);
        } );
        clubs.RemoveAt(selectedClub);

        if (clubs.Count > 0 && !pickupDrop)
        {
            selectedClub = selectedClub + 1 < clubs.Count ? selectedClub : clubs.Count - 1;
            SelectClub();
            return;
        }

        if (clubs.Count != 0) return;

        ResetData();
    }

    private void UpdateClubUI()
    {
        if (EquippedItem == null) return;

        clubEquipArt.color = Color.white;
        clubEquipText.text = EquippedItem.SpriteSettings.ItemText;
        clubEquipArt.sprite = EquippedItem.SpriteSettings.ItemSprite;

        for (int i = 0; i < EquippedItem.AbilitySpriteSettings.Count; i++)
        {
            ItemArtSettings currentSettings = EquippedItem.AbilitySpriteSettings[i];

            if (currentSettings.NotVisible)
            {
                clubAbilitiesText[i].text = "";
                clubAbilitiesArt[i].color = Color.clear;
                continue;
            }

            clubAbilitiesArt[i].color = Color.white;
            clubAbilitiesText[i].text = currentSettings.ItemText;
            clubAbilitiesArt[i].sprite = currentSettings.ItemSprite;

            clubAbilitiesArt[i].rectTransform.localScale = currentSettings.Scale;
            clubAbilitiesArt[i].rectTransform.localRotation = Quaternion.Euler(currentSettings.Rotation);
            clubAbilitiesArt[i].rectTransform.localPosition = spriteStartingPositions[i] + currentSettings.Position;
        }
    }

    void ResetData()
    {
        clubEquipText.text = "";
        clubEquipArt.color = Color.clear;

        foreach (TextMeshProUGUI text in clubAbilitiesText) text.text = "";
        foreach (Image sprite in clubAbilitiesArt) sprite.color = Color.white;

        clubUI.HideUI();
        switchTimer = 0f;

        EquippedClub = null;
        EquippedItem = null;
        ItemGameObject = null;

        clubSway.ResetMovementValues();
    }
}