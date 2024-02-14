using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Interactable
{
    //Summary: 
    //      class that inerth from interactable. When interacted with
    //      it produces an explosion that generate debrits. If the debrits
    //      are under a certain sie dey disapear after a certain time

    [Header("Explosion Parameters")]
    [SerializeField] private float explosionRadius = 2.0f;
    [SerializeField] private float explosionStrength = 1000.0f;
    [SerializeField] private LayerMask interactablesMask = 6;
    // Start is called before the first frame update

    [Space(5)]
    [Header("Debrits parameters")]
    [SerializeField] private int debriCount = 10;
    [SerializeField] private GameObject debritPrefab = null;

    [Space(10)]
    [Header("Self destruction attributes")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float destroyTime = 10f;

    private float passedTime = 0;

    private void Update()
    {
        //checks if the scale is superior to a certain minimum if it
        //isn't then starts a timer and destroy the object
        if (transform.localScale.x <= minScale)
        {
            passedTime += Time.deltaTime;
            if (passedTime > destroyTime)
            {
                Destroy(gameObject);
            }
        }
    }

    //Summary:
    //      Make the interactable explode producing debris and a force
    //      affecting the other interactables.
    public override void Interact()
    {


        if (gameObject.transform.localScale.x >= 0.01)
        {
            SpawnDebrits(debritPrefab, debriCount);

            //Computes the explosion radius using the scale of the object and the initial explosion radius
            float finalExplosionRadius = explosionRadius * gameObject.transform.localScale.magnitude * 15;

            //looks for all the objects with a collider of the interactables layer within the explosion radius
            Collider[] interactablesInRange = Physics.OverlapSphere(gameObject.transform.position, finalExplosionRadius, interactablesMask);

            //for each object found with a rigid body it applies a force dependent on the distance and scale
            //of the exploding object. The spawned debris will be also affected.
            foreach (Collider col in interactablesInRange)
            {
                Rigidbody targetRigidBody;

                //checks if the game object in range has a rigid body
                if (col.gameObject.TryGetComponent<Rigidbody>(out targetRigidBody))
                {
                    //if a rigid body is found computes the distance between objects and the explosion force than add the force
                    float distance = Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position);

                    Vector3 explosionForce = explosionStrength * (col.gameObject.transform.position - gameObject.transform.position).normalized / (distance);
                    Vector3 explosionForceScale = explosionForce * gameObject.transform.localScale.magnitude * 4;

                    targetRigidBody.AddForce(explosionForceScale);

                }
            }
        }
        Destroy(gameObject);
    }
    //Summary:
    //      Spawns the debrits with randomly around the objects coordinates with random sizes
    private void SpawnDebrits(GameObject prefab, int number)
    {

        if (gameObject.transform.localScale.x >= 0.05)
        {
            for (int i = 0; i < number; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0f, 0.1f), Random.Range(-0.1f, 0.1f));
                GameObject debrit = Instantiate(prefab, gameObject.transform.position + randomOffset, Quaternion.identity);
                debrit.transform.localScale = gameObject.transform.localScale / (Random.Range(1.5f, 4f));
            }
        }
    }

}
