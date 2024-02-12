using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;


[RequireComponent(requiredComponent: typeof(ARRaycastManager), requiredComponent2: typeof(ARRaycastManager))]
public class TouchManager : MonoBehaviour
{
    //Interactable to spawn
    [SerializeField] private GameObject prefab;

    //managers 
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;

    //hit lists
    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    [SerializeField] private LayerMask interactablesLayer;
    //distance between the touched point and the spawn point
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    //variable related to souch control
    private bool thouchActive = false;
    private bool scaleActive = false;
    private EnhancedTouch.Finger firstFinger;
    private EnhancedTouch.Finger secondFinger;
    private float fingersDistance;
    private float currentFingersDistance;

    //object in the screne used to debug
    public GameObject debuggerGameObject;

    //grab manager, controls the movement and scaling of the grabbed object
    private GrabManager grabManager;

    //rotation and position of the camera at the previous frame
    private Quaternion cameraRotation;
    private Vector3 cameraPosition;


    // Start is called before the first frame update
    void Start()
    {
        //retrivering the managers from the scene
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (thouchActive) {
            //checks if the screen touch lasted more than 0.2 seconds
            if(Time.realtimeSinceStartup - firstFinger.currentTouch.time > 0.2)
            {
                //if no grab manager exists it cheks if an interactable is being touched and 
                //if it is it creates a new grab manager and passes the interactable reference to it
                if(grabManager == null)
                {
                    SelectInteractable();
                                   }
                //if the grab manager already exists then controls the movement of the interactable 
                else
                {
                    grabManager.MoveInteractable(cameraRotation, cameraPosition);
                }

                //saves the position and rotation of the camera at this frame 
                cameraRotation = Camera.main.transform.rotation;
                cameraPosition = Camera.main.transform.position;
                
            }
        }
        if(scaleActive)
        {
            //Move in a function
            currentFingersDistance = (firstFinger.currentTouch.screenPosition - secondFinger.currentTouch.screenPosition).magnitude;
            grabManager.ScaleTouch(fingersDistance, currentFingersDistance, 0.05f, 0.5f);
            fingersDistance = currentFingersDistance;

        }


    }


    //when enabled subscribe to to events
    private void OnEnable()
    {
        EnhancedTouch.EnhancedTouchSupport.Enable();
        //Event subscription
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        EnhancedTouch.Touch.onFingerUp += FingerUp;
    }

    private void OnDisable()
    {
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.Touch.onFingerUp -= FingerUp;
        
    }

    //Summary:
    //      Called when a new finger touches the screen, handles it differently
    //      dipending if it is the first, the second or anotehr finger
    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index > 1)
        {
            return;
        }

        if (finger.index == 0)
        {
            thouchActive = true;
            firstFinger = finger;
        }
        else
        {
            scaleActive = true;
            secondFinger = finger;
            //computes the distance between the two fingers
            fingersDistance = (firstFinger.currentTouch.screenPosition - secondFinger.currentTouch.screenPosition).magnitude;
        }

        
    }

    //Summary:
    //      called when a finger is lifted from the screen. Checks if it was
    //      the first finger or the second finger if it was the first finger
    //      checks if it was a tap (duretion under 0.2s). If it was creates
    //      a new cube in case it a plane wa tapped or, if a cube was tapped,
    //      makes it explode
    private void FingerUp(EnhancedTouch.Finger finger)
    {
        //checks what finger was lifted
        if (finger.index >1)
        {
            return;
        }
        if (finger.index == 1)
        {
            scaleActive = false;
            return;
        }

        // cancel the grab
        if (grabManager != null)
        {
            grabManager.CancelGrab();
            grabManager = null;
        }



        //checks if the last tap duration was under 0.2s if it is creates or deastroy an interactable
        if (finger.lastTouch.isTap)
        {
            //creates a ray from the point touched in the screen to the world 
            Ray ray = Camera.main.ScreenPointToRay(firstFinger.currentTouch.screenPosition);
            RaycastHit hit;

            //checks if the raycast hits an object if yes it destroys it
            if (Physics.Raycast(ray, out hit, 100f, interactablesLayer))
            {
                Interactable interactableTouched;
                if (hit.transform.gameObject.TryGetComponent<Interactable>(out interactableTouched))
                {
                    interactableTouched.Explode();
                }
            }

            //if no object was it checks if the ray hits a plane if it does creates a new interactable
            else if (raycastManager.Raycast(firstFinger.currentTouch.screenPosition, hitList, TrackableType.PlaneWithinPolygon))
            {
                Pose pose = hitList[0].pose;
                GameObject obj = Instantiate(prefab, pose.position + spawnOffset, pose.rotation);
            }
        }
    }


    //Summary:
    //      select an interactable if one is long pressed
    private void SelectInteractable()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(firstFinger.currentTouch.screenPosition);
        
        RaycastHit hit;
        //checks if more than one finger is down if yes return

        //checks if the ray coming from the touched screen position hits an object in the interactable layer
        if (Physics.Raycast(ray, out hit, 100f, interactablesLayer) && grabManager == null)
        {
            grabManager = new GrabManager(hit.transform.gameObject);
        }
        
    }
   

}
