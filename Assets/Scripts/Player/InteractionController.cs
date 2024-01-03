using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    public void Interact();
}

public class InteractionController : MonoBehaviour
{
    [SerializeField] Transform interactorSource;
    [SerializeField] float interactRange;
    private Vector2 mousePos;
    Laser laser;

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(interactorSource.position, interactRange);

            foreach (Collider2D collider in colliders)
            {
                // Check for IInteractable in the current collider's GameObject or its children
                IInteractable interactObj = collider.gameObject.GetComponent<IInteractable>();

                if (interactObj == null)
                {
                    // Check for IInteractable in parent objects
                    interactObj = collider.gameObject.GetComponentInParent<IInteractable>();
                }

                if (interactObj != null)
                {
                    interactObj.Interact();
                    break; // If an interactable is found, exit the loop
                }
            }
        }
    }

    public void OnLaserMove(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(interactorSource.position, interactRange);

            foreach (Collider2D collider in colliders)
            {
                laser = collider.gameObject.GetComponent<Laser>();

                if (laser == null)
                {
                    laser = collider.gameObject.GetComponentInParent<Laser>();
                }

                if (laser != null)
                {
                    laser.ManuallyMoveLaser();
                    break;
                }
            }
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if(laser != null)
        {
            laser.mousePos = context.ReadValue<Vector2>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(interactorSource.position, interactRange);
    }
}
