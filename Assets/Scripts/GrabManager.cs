using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager 
{
    //
    //Object that manages the possible interactions with a grabbed object 
    //The grab manager is connected to an interactable so in gets created on the interactable selection 
    //
    //possible actions are:
    //      Move interactable by moving the holder
    //      resize the interactable using two fingers
    //      Scale  the interactable using a float

    private GameObject selectedInteractable;
    private Rigidbody rigidBody;

    

    public GameObject SelectedInteractable { get => selectedInteractable;}

    public GrabManager(GameObject selectedInteractable)
    {
        this.selectedInteractable = selectedInteractable;
        if(selectedInteractable.TryGetComponent<Rigidbody>(out rigidBody))
        {
            rigidBody.isKinematic = true;
        }
    }


    //Summary:
    //      Moves the interactable based on the rotation and movement of the holder. 
    //      The distance between the older and the interactable doesen't change
    public void MoveInteractable(Quaternion holderRotation, Vector3 holderPosition)
    {
        //computes the delta rotation and position comparing the ones povided in input with the current ones
        Quaternion deltaRotation = Camera.main.transform.rotation * Quaternion.Inverse(holderRotation);
        Vector3 deltaPosition = Camera.main.transform.position - holderPosition;

        //computes the vecotor that goes from the holder to the selected object
        Vector3 pos = selectedInteractable.transform.position - Camera.main.transform.position;
        
        //rotates the vector that goes from the holder to the object of the delta rotation
        Vector3 rotatedpos = deltaRotation * pos;
        
        //Modifies the current position and rotation 
        selectedInteractable.transform.position += deltaPosition + rotatedpos - pos;
        selectedInteractable.transform.rotation = deltaRotation * selectedInteractable.transform.rotation;
        
    }

    //Summary:
    //      Scale uniformely an object based on the difference betweeen two distances between fingers touching the screen
    //      the new scal will be clamped between a min and max value
    public void ScaleTouch(float oldFingersDistance, float currentFingerDistance, float minScale, float maxScale)
    {
        // computes the delta distance between teh fingers. The value is clamped and scaled by a factor of 100
        // to make controlling it easier.
        float deltaDistance = Mathf.Clamp(currentFingerDistance - oldFingersDistance, -10f, 10f)/100;
        
        float scaleFactor;


        if(deltaDistance >= 0f)
        {
            scaleFactor = deltaDistance + 1;
        }
        else
        {
            scaleFactor = 1f/(-deltaDistance+1);
            
        }

        Scale(scaleFactor, minScale, maxScale);

        
    }

    //Summary:
    //      Uniformely scales the selectedObject in the three dimesions, The scale is clamped between a min and a max value 
    public void Scale(float scaleFactor, float minScale, float maxScale)
    {
        selectedInteractable.transform.localScale = ClampUniformVector(selectedInteractable.transform.localScale * scaleFactor, 0.05f, 0.5f);
    }

    //Summary:
    //      uniformely clamps the three values of a Vector3 between a min and a max value
    private Vector3 ClampUniformVector(Vector3 value, float min, float max)
    {
        value.x = Mathf.Clamp(value.x, min, max);
        value.y = Mathf.Clamp(value.y, min, max);
        value.z = Mathf.Clamp(value.z, min, max);
        return value;
    }

    // Summary:
    //      cancel the grab restoring the standard RigidBody behaviour
    // 
    public void CancelGrab()
    {

         
        //rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        selectedInteractable = null;
    }
}
