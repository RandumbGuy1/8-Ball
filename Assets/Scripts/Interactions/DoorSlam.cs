using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlam : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip bustDownClip;
    [SerializeField] private float snapTime;
    [SerializeField] private Transform[] wantedPlayerPositions; 
    [SerializeField] private Rigidbody rb;

    public PlayerRef Player { get; set; }
    public GameObject GameObject => gameObject;

    public string GetDescription(PlayerRef player) => rb.isKinematic ? "Break Down" : null;

    public void OnEndHover(PlayerRef player)
    {

    }

    public void OnInteract(PlayerRef player)
    {
        rb.isKinematic = false;
        StartCoroutine(SnapPlayerToSlam(player));
    }

    public void OnStartHover(PlayerRef player)
    {

    }

    private Vector3 GetClosestPosition(Vector3 playerPos)
    {
        Vector3 closestPos = wantedPlayerPositions[0].position;
        foreach (Transform t in wantedPlayerPositions)
        {
            if ((playerPos - t.position).sqrMagnitude < (playerPos - closestPos).sqrMagnitude) 
                closestPos = t.position;
        }

        return closestPos;
    }

    IEnumerator SnapPlayerToSlam(PlayerRef player)
    {
        Vector3 wantedPlayerPosition = GetClosestPosition(player.transform.position);

        player.CameraBody.LookAt((transform.position - wantedPlayerPosition).normalized);
        player.CameraBody.CamShaker.ShakeOnce(new PerlinShake(ShakeData.Create(5f, 7f, 0.8f, 10f)));
        player.PlayerMovement.Rb.isKinematic = true;
        float elapsed = 0;

        while (elapsed < snapTime)
        {
            player.PlayerMovement.Rb.MovePosition(Vector3.Lerp(player.PlayerMovement.Rb.position, wantedPlayerPosition, elapsed/snapTime));
            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(snapTime * 0.3f);

        player.CameraBody.CamShaker.ShakeOnce(new PerlinShake(ShakeData.Create(15f, 6f, 0.9f, 10f)));
        AudioManager.Instance.PlayOnce(bustDownClip, transform.position);

        player.PlayerMovement.Rb.isKinematic = false;
        player.PlayerMovement.Rb.velocity = Vector3.zero;
        player.PlayerMovement.Rb.AddForce((transform.position - player.transform.position) * 50f, ForceMode.VelocityChange);
        player.PlayerMovement.Rb.AddForce(Vector3.down * 50f, ForceMode.Impulse);
    }
}
