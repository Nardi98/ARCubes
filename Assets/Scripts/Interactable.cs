using UnityEngine;


public class Interactable : MonoBehaviour
{
    //Summary:
    //      Parent class for all the interactables
    protected bool movable = true;

    public bool Movable { get => movable; }

    // Summary:
    //      Virtual interact method to be overritten in child classes
    public virtual void Interact()
    {

    }
   
}
