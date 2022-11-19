using UnityEngine;

public class ClubSway : MonoBehaviour
{
    private Vector3 switchOffsetPos = Vector3.zero, switchOffsetRot = Vector3.zero;
    private Vector3 switchPosVel = Vector3.zero, switchRotVel = Vector3.zero;

    private Vector3 pickUpPosVel = Vector3.zero;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;
    [SerializeField] private Transform weaponPos;

    private Vector3 startPos, startRot;

    private Vector3 pickupOffsetPos;
    private Quaternion pickupOffsetRot; 

    void Awake()
    {
        startPos = weaponPos.localPosition;
        startRot = new Vector3(weaponPos.localRotation.x, weaponPos.localRotation.y, weaponPos.localRotation.z);
    }

    void Update()
    {
        IItem itemToSway = player.ClubHolder.EquippedItem;
        if (itemToSway == null) return;

        CalculatePickupOffset(ref pickupOffsetPos, ref pickupOffsetRot, itemToSway);
        CalculateSwitchOffset(itemToSway);

        Vector3 newStartPos = startPos - player.CameraBody.TPSOffset;
        Vector3 newPos = newStartPos + itemToSway.HoldSettings.DefaultPos + switchOffsetPos + pickupOffsetPos;
        Quaternion newRot = pickupOffsetRot * Quaternion.Euler(startRot + itemToSway.HoldSettings.DefaultRot + switchOffsetRot);

        weaponPos.localPosition = newPos;
        weaponPos.localRotation = newRot;
    }

    public void AddSwitchOffset(Vector3 pos, Vector3 rot)
    {
        switchOffsetPos = pos;
        switchOffsetRot = rot;
    }

    public void ResetMovementValues()
    {
        switchPosVel = Vector3.zero;
        switchRotVel = Vector3.zero;

        switchOffsetPos = Vector3.zero;
        switchOffsetRot = Vector3.zero;
    }

    private void CalculateSwitchOffset(IItem itemToSway)
    {
        if (switchOffsetPos == Vector3.zero && switchOffsetRot == Vector3.zero) return;

        switchOffsetPos = Vector3.SmoothDamp(switchOffsetPos, Vector3.zero, ref switchPosVel, itemToSway.HoldSettings.SwitchSmoothTime);
        switchOffsetRot = Vector3.SmoothDamp(switchOffsetRot, Vector3.zero, ref switchRotVel, itemToSway.HoldSettings.SwitchSmoothTime);

        if (switchOffsetPos.sqrMagnitude + switchOffsetRot.sqrMagnitude < 0.00002f)
        {
            switchOffsetPos = Vector3.zero;
            switchOffsetRot = Vector3.zero;
        }
    }

    public void ReigsterNewPickup(Transform child)
    {
        pickupOffsetPos = child.localPosition;
        pickupOffsetRot = child.localRotation;

        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
    }   

    private void CalculatePickupOffset(ref Vector3 localPosition, ref Quaternion localRotation, IItem itemToSway)
    {
        if (localPosition == Vector3.zero && localRotation == Quaternion.Euler(Vector3.zero)) return;

        localPosition = Vector3.SmoothDamp(localPosition, Vector3.zero, ref pickUpPosVel, itemToSway.HoldSettings.PickupSmoothTime);
        localRotation = Quaternion.Lerp(localRotation, Quaternion.Euler(Vector3.zero), 1 / itemToSway.HoldSettings.PickupSmoothTime * Time.deltaTime);
    
        if (localPosition.sqrMagnitude + transform.localEulerAngles.sqrMagnitude < 0.00002f)
        {
            localPosition = Vector3.zero;
            localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
