using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubEquipController : MonoBehaviour
{
    [SerializeField] private RectTransform equipUI;
    [SerializeField] private float animationSmoothing;
    private Vector3 positionOffset;
    private Vector3 positionOffsetVel;

    void Update()
    {
        if (equipUI.localPosition == positionOffset) return;

        equipUI.localPosition = Vector3.SmoothDamp(equipUI.localPosition, positionOffset, ref positionOffsetVel, animationSmoothing);

        if ((positionOffset - equipUI.localPosition).sqrMagnitude < 0.001f) equipUI.localPosition = positionOffset;
    }

    public void HideUI(bool hide = true) => positionOffset = hide ? Vector3.right * 250f : Vector3.zero;
    public void SetPositionOffset(Vector3 position) => positionOffset = position;
}
