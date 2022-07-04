using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractions : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask InteractionObstruction;
    [SerializeField] private LayerMask Interactables;
    [SerializeField] private float interactionRange;
    [SerializeField] private float interactionRadius;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;
    [SerializeField] private TextMeshProUGUI interactionKeyBindText;
    [SerializeField] private TextMeshProUGUI interactionText;

    private IInteractable interactable;

    void Awake()
    {
        player.PlayerInput.OnInteractInput += CheckForInteractable;
    }

    private void CheckForInteractable(bool interact)
    {
        if (Physics.SphereCast(player.PlayerCam.transform.position, interactionRadius, player.PlayerCam.transform.forward, out var hit, interactionRange, Interactables, QueryTriggerInteraction.Ignore))
        {
            GameObject currentleyLookingAt = hit.transform.gameObject;

            if (Physics.Linecast(player.PlayerCam.transform.position, currentleyLookingAt.transform.position, InteractionObstruction))
            {
                ResetInteraction();
                return;
            }

            if (interactable == null)
            {
                IInteractable interactableTemp = hit.transform.GetComponent<IInteractable>();

                if (interactableTemp == null) return;

                interactable = interactableTemp;
                interactable.OnStartHover(player);
            }
            else if (currentleyLookingAt != interactable.GameObject)
            {
                ResetInteraction();
                return;
            }

            string text = interactable.GetDescription(player);
            interactionText.gameObject.SetActive(text != null);

            if (!interactionText.gameObject.activeInHierarchy)
            {
                interactionText.text = " ";
                return;
            }

            interactionText.text = text;
            interactionKeyBindText.text = player.PlayerInput.InteractKey.ToString();

            if (interact) interactable.OnInteract(player);

        }
        else if (interactionText.text != " ") ResetInteraction();
    }

    private void ResetInteraction()
    {
        interactionText.gameObject.SetActive(false);
        interactionText.text = " ";

        if (interactable == null) return;

        interactable.OnEndHover(player);
        interactable = null;
    }
}
