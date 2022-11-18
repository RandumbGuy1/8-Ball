using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunBody : MonoBehaviour, IItem
{
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private GunHeadBob gunBobSettings = new GunHeadBob();
    [SerializeField] private GunRecoil gunRecoilSettings = new GunRecoil();
    [SerializeField] private GunSway gunSwaySettings = new GunSway();
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

        player.PlayerInput.OnMouseButtonDownInput -= FakeShoot;
    }

    private void FakeShoot(int index)
    {
        if (index != 0 || !gameObject.activeSelf) return;

        gunRecoilSettings.AddRecoil();
        AudioManager.Instance.PlayOnce(shootSound, transform.position);
    }

    public void ItemUpdate(PlayerRef player)
    {
        gunRecoilSettings.Update();
        gunSwaySettings.Update(player);
        gunBobSettings.Update(player.CameraBody.CamHeadBob.ViewBobSnapOffset);

        transform.localPosition = gunRecoilSettings.RecoilOffsetPos - player.CameraBody.CamHeadBob.ViewBobOffset * 0.03f;
        transform.localRotation = Quaternion.Euler(gunSwaySettings.SwayOffsetRot + gunRecoilSettings.RecoilOffsetRot + CameraBody.ToEuler(player.CameraBody.CamHeadBob.ViewBobOffset * 2f));
    }

    public void OnPickup(PlayerRef player)
    {
        itemCol.enabled = false;
        pickup.PickedUp = true;

        Destroy(itemRb);
        itemRb = null;

        player.PlayerInput.OnMouseButtonDownInput += FakeShoot;
    }
}

[System.Serializable]
public class GunSway
{
    [Header("Variables")]
    [SerializeField] private float swayAmount;
    [SerializeField] private float tiltMulti;
    [SerializeField] private float dampingRatio;
    [SerializeField] private float angularFrequency;

    public Vector3 SwayOffsetRot { get; private set; } = Vector3.zero;
    private Vector3 swayVel = Vector3.zero;

    public void Update(PlayerRef player)
    {
        Vector3 swayDelta = swayAmount * player.CameraBody.CamLookSettings.RotationDelta;
        swayDelta.z = -swayDelta.y * tiltMulti;
        swayDelta.x *= -1;

        Vector3 smoothSway = SwayOffsetRot;

        HarmonicMotion.Calculate(ref smoothSway, ref swayVel, swayDelta,
            HarmonicMotion.CalcDampedSpringMotionParams(dampingRatio, angularFrequency));
        SwayOffsetRot = smoothSway;
    }
}

[System.Serializable]
public class GunHeadBob
{
    [Header("Variables")]
    [SerializeField] private float headBobPosMulti;
    [SerializeField] private float headBobRotMulti;
    [SerializeField] private float dampingRatio;
    [SerializeField] private float angularFrequency;

    private Vector3 bobOffset = Vector3.zero;
    public Vector3 BobOffsetPos => bobOffset * headBobPosMulti;
    public Vector3 BobOffsetRot => new Vector3(bobOffset.y, bobOffset.x, bobOffset.z) * headBobRotMulti;
    private Vector3 bobVel = Vector3.zero;

    public void Update(Vector3 headBobDelta)
    {
        Vector3 smoothHeadBob = bobOffset;

        HarmonicMotion.Calculate(ref smoothHeadBob, ref bobVel, headBobDelta,
            HarmonicMotion.CalcDampedSpringMotionParams(dampingRatio, angularFrequency));
        bobOffset = smoothHeadBob;
    }
}

[System.Serializable]
public class GunRecoil
{
    [Header("Variables")]
    [SerializeField] private float recoilAmount;
    [SerializeField] private float recoilAimRatio;
    [SerializeField] private float dampingRatio;
    [SerializeField] private float angularFrequency;

    [SerializeField] private Vector3 recoilOffsetPos = Vector3.zero;
    [SerializeField] private Vector3 recoilOffsetRot = Vector3.zero;

    public Vector3 RecoilOffsetPos { get; private set; } = Vector3.zero;
    public Vector3 RecoilOffsetRot { get; private set; } = Vector3.zero;

    private Vector3 desiredRecoilOffsetPos = Vector3.zero;
    private Vector3 desiredRecoilOffsetRot = Vector3.zero;

    private Vector3 recoilOffsetPosVel = Vector3.zero;
    private Vector3 recoilOffsetRotVel = Vector3.zero;

    public void AddRecoil()
    {
        desiredRecoilOffsetPos = recoilOffsetPos;
        desiredRecoilOffsetRot = recoilOffsetRot;

        desiredRecoilOffsetRot.z *= UnityEngine.Random.Range(1f, -1f);
    }

    public void Update()
    {
        desiredRecoilOffsetPos = Vector3.Lerp(desiredRecoilOffsetPos, Vector3.zero, 15f * Time.deltaTime);
        desiredRecoilOffsetRot = Vector3.Lerp(desiredRecoilOffsetRot, Vector3.zero, 15f * Time.deltaTime);

        Vector3 recoilDeltaPos = recoilAmount * desiredRecoilOffsetPos;
        Vector3 recoilDeltaRot = recoilAmount * desiredRecoilOffsetRot;

        Vector3 smoothRecoilPos = RecoilOffsetPos;
        Vector3 smoothRecoilRot = RecoilOffsetRot;

        HarmonicMotion.Calculate(ref smoothRecoilPos, ref recoilOffsetPosVel, recoilDeltaPos,
            HarmonicMotion.CalcDampedSpringMotionParams(dampingRatio, angularFrequency));
        RecoilOffsetPos = smoothRecoilPos;

        HarmonicMotion.Calculate(ref smoothRecoilRot, ref recoilOffsetRotVel, recoilDeltaRot,
            HarmonicMotion.CalcDampedSpringMotionParams(dampingRatio, angularFrequency));
        RecoilOffsetRot = smoothRecoilRot;
    }
}
