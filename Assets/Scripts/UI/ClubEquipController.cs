using UnityEngine;

public class ClubEquipController : MonoBehaviour
{
    [SerializeField] private RectTransform equipUI;
    [SerializeField] private float animationSmoothing;
    [SerializeField] private Vector3 hideDirection;

    private Vector3 startPos;
    private Vector3 desiredPositionOffset;
    private Vector3 smoothPositionOffset;
    private Vector3 positionOffsetVel;

    void Awake()
    {
        GameManager.Instance.OnGameStateChanged += (GameState newState) => {
            gameObject.SetActive(newState == GameState.Gameplay);
        };

        startPos = equipUI.localPosition;

        HideUI();
    }

    void Update()
    {
        if (smoothPositionOffset == desiredPositionOffset) return;

        smoothPositionOffset = Vector3.SmoothDamp(smoothPositionOffset, desiredPositionOffset, ref positionOffsetVel, animationSmoothing);
        equipUI.localPosition = startPos + smoothPositionOffset;

        if ((desiredPositionOffset - smoothPositionOffset).sqrMagnitude < 0.001f) smoothPositionOffset = desiredPositionOffset;
    }

    public void HideUI(bool hide = true) => desiredPositionOffset = hide ? hideDirection : Vector3.zero;
    public void SetPositionOffset(Vector3 position) => desiredPositionOffset = position;

    public void HideUISnap(bool hide = true)
    {
        desiredPositionOffset = hide ? hideDirection : Vector3.zero;
        smoothPositionOffset = hide ? hideDirection : Vector3.zero;
    }
}
