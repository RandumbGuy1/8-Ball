﻿using System.Collections;
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
    [SerializeField] private GameObject interactionUI;

    private IInteractable interactable;

    void Awake()
    {
        player.PlayerInput.OnInteractInput += CheckForInteractable;

        GameManager.Instance.OnGameStateChanged += (GameState newState) => {
            interactionUI.SetActive(newState == GameState.Gameplay);
        };

    }

    private void CheckForInteractable(bool interact)
    {
        Ray ray = player.PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.SphereCast(ray, interactionRadius, out var hit, interactionRange + (player.transform.position - player.PlayerCam.transform.position).magnitude, Interactables, QueryTriggerInteraction.Ignore))
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
            if (text == null)
            {
                ResetInteraction(false);
                return;
            }

            interactionUI.SetActive(true);

            interactionText.text = text;
            interactionKeyBindText.text = player.PlayerInput.InteractKey.ToString();

            if (interact) interactable.OnInteract(player);

        }
        else ResetInteraction();
    }

    private void ResetInteraction(bool noInteraction = true)
    {
        interactionUI.SetActive(false);
        interactionText.text = "";

        if (interactable == null || !noInteraction) return;

        interactable.OnEndHover(player);
        interactable = null;
    }
}
