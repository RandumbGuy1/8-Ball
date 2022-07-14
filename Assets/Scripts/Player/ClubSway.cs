using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        startPos = weaponPos.localPosition;
        startRot = new Vector3(weaponPos.localRotation.x, weaponPos.localRotation.y, weaponPos.localRotation.z);
    }

    void Update()
    {
        IItem itemToSway = player.ClubHolder.EquippedItem;
        if (itemToSway == null) return;

        CalculatePickupOffset(player.ClubHolder.ItemGameObject.transform, itemToSway);
        CalculateSwitchOffset(itemToSway);

        Vector3 newPos = startPos + itemToSway.HoldSettings.DefaultPos + switchOffsetPos;
        Quaternion newRot =  Quaternion.Euler(startRot + itemToSway.HoldSettings.DefaultRot + switchOffsetRot);

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

    private void CalculatePickupOffset(Transform itemTransform, IItem itemToSway)
    {
        if (itemTransform.localPosition == Vector3.zero && itemTransform.localRotation == Quaternion.Euler(Vector3.zero)) return;

        itemTransform.localPosition = Vector3.SmoothDamp(itemTransform.localPosition, Vector3.zero, ref pickUpPosVel, itemToSway.HoldSettings.PickupSmoothTime);
        itemTransform.localRotation = Quaternion.Lerp(itemTransform.localRotation, Quaternion.Euler(Vector3.zero), 1 / itemToSway.HoldSettings.PickupSmoothTime * Time.deltaTime);
    
        if (itemTransform.localPosition.sqrMagnitude + transform.localEulerAngles.sqrMagnitude < 0.00002f)
        {
            itemTransform.localPosition = Vector3.zero;
            itemTransform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
