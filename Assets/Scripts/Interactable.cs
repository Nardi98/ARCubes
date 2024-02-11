using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 2.0f;
    [SerializeField] private float explosionStrength = 1000.0f;
    [SerializeField] private LayerMask interactablesMask = 6;
    // Start is called before the first frame update


    private void OnDestroy()
    {
        //looks for all the objects with a collider of the interactables layer within the explosion radius
        Collider[] interactablesInRange = Physics.OverlapSphere(gameObject.transform.position, explosionRadius, interactablesMask);

        foreach (Collider col in interactablesInRange)
        {
            Rigidbody targetRigidBody;
            
            //checks if the game object in range has a rigid body
            if (col.gameObject.TryGetComponent<Rigidbody>(out targetRigidBody))
            {
                //if a rigid body is found computes the distance between objects and the explosion force than add the force
                float distance = Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position);
                Vector3 explosionForce = explosionStrength * (col.gameObject.transform.position - gameObject.transform.position).normalized / (distance * distance);
                targetRigidBody.AddForce(explosionForce);

            }
            else
            {
                col.gameObject.transform.localScale = col.gameObject.transform.localScale*2;
            }
        }
        
    }
}
