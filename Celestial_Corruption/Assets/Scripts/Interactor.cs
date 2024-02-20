using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractorRange;

    // TODO:
    // Change for new input system
    // Change transform to be a head like component which will inherit the direction of the camera
    void Update()
    {
        // Change for new input system
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 rayOrigin = InteractorSource.position;  

            // In the future change this to be a head like component which will inherit the direction of the camera
            // Create a ray from the middle of the InteractorSource by changing the y value to be in the middle of the object
            rayOrigin.y = InteractorSource.position.y + InteractorSource.GetComponent<Collider>().bounds.size.y / 2;
             
            Ray r = new Ray(rayOrigin, InteractorSource.forward);
            Debug.DrawRay(r.origin, r.direction * InteractorRange, Color.red, 1);

            if (Physics.Raycast(r, out RaycastHit hit, InteractorRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}
