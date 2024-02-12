using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAR : Interactable
{
    //Summary;
    //      Button present placable in space that changes
    //      color when interacted with
    private Renderer renderer;

    [SerializeField] private Color interactedColor = Color.red;
    private Color normalColor;

    [SerializeField] private float interactionDutration = 5f;
    private float passedTime = 0f;



    private void Start()
    {
        movable = false;
        renderer = gameObject.GetComponent<Renderer>();
        normalColor = renderer.material.color;
    }

    
    private void Update()
    {
        //Timer to change back the color
        if (renderer.material.color != normalColor)
        {
            passedTime += Time.deltaTime;
            if (passedTime > interactionDutration)
            {
                ChangeColor(normalColor);
            }
        }
    }

    //Summary:
    //      When interacted with the color of the material is changed
    public override void Interact()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        ChangeColor(interactedColor);
        passedTime = 0f;
    }

    //Summary:
    //      Changes the color of the material with the input color
    private void ChangeColor(Color color)
    {
        renderer.material.color = color;
    }

}
