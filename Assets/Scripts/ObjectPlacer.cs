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
public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    private List<RaycastHit> virtualHitList = new List<RaycastHit>();

    [SerializeField]
    private LayerMask interactablesLayer;
    [SerializeField]
    private Vector3 spawnOffset = Vector3.zero;

    private bool thouchActive = false;
    private EnhancedTouch.Finger activeFinger;

    private GameObject selectedInteractable = null;
    private Vector3 selectionVector;

    public GameObject debuggerGameObject;

    private Quaternion cameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        //retrivering the managers from the scene
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        
        
        cameraRotation = Camera.main.transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (thouchActive ) {
            if(Time.realtimeSinceStartup - activeFinger.currentTouch.time > 0.2)
            {

                if(selectedInteractable == null)
                {
                    SelectInteractable();
                    
                }
                else
                {
                    Drag(selectedInteractable,selectionVector,activeFinger.currentTouch);
                }
                cameraRotation = Camera.main.transform.rotation;
                
            }

            

        }


    }



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
        
        thouchActive = false;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0)
        {
            return;
        }
        thouchActive = true;
        activeFinger = finger;

        
    }

    private void FingerUp(EnhancedTouch.Finger finger)
    {
        debuggerGameObject.SetActive(!debuggerGameObject.active);
        //debuggerObject.SetActive(!debuggerObject.active);
        if (finger.index != 0)
        {
            return;
        }

        if (selectedInteractable != null)
        {
            selectedInteractable.GetComponent<Rigidbody>().useGravity = true;
            selectedInteractable = null;
        }


        if (finger.lastTouch.isTap)
        {
            Ray ray = Camera.main.ScreenPointToRay(activeFinger.currentTouch.screenPosition);
            RaycastHit hit;
            //checks if more than one finger is down if yes return

            if (Physics.Raycast(ray, out hit, 100f, interactablesLayer))
            {
                Destroy(hit.transform.gameObject);
            }
            else if (raycastManager.Raycast(activeFinger.currentTouch.screenPosition, hitList, TrackableType.PlaneWithinPolygon))
            {
                Pose pose = hitList[0].pose;
                GameObject obj = Instantiate(prefab, pose.position + spawnOffset, pose.rotation);
            }
        }
    }

    private void SelectInteractable()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(activeFinger.currentTouch.screenPosition);
        
        RaycastHit hit;
        //checks if more than one finger is down if yes return

        if (Physics.Raycast(ray, out hit, 100f, interactablesLayer) && selectedInteractable == null)
        {
            selectedInteractable = hit.transform.gameObject;
            selectionVector = selectedInteractable.transform.position - Camera.main.transform.position;//ScreenToWorldPoint(activeFinger.currentTouch.screenPosition);
            selectedInteractable.GetComponent<Rigidbody>().useGravity = false;
        }
        
    }

    private void Drag(GameObject selectedObject, Vector3 movingVector, EnhancedTouch.Touch touch )
    {
        //manage the rotation of the object
        Quaternion deltaRotation = Camera.main.transform.rotation * Quaternion.Inverse(cameraRotation);
        selectedObject.transform.rotation = selectedObject.transform.rotation * deltaRotation;

        //manage the position based on the camera position
        selectedObject.transform.position = Camera.main.transform.position/*.ScreenToWorldPoint(touch.screenPosition) * movingVector.magnitude */+ movingVector;
    }

}
