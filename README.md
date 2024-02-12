# ARCubes
 AN ar project made with Unity, openXR and ARfundation


## Thouch manager
This class manages the Touch inputs it inherits from monobehaviour. It used the enhanced touch system. 
Allows to detect touches of an ARplane, Touches of an interactable, prolonged touches on an interaclable, and multiple touches while dragging the fingers to scale an interactable. Based on if the touched wall is vertical or orizontal a different interactable is spawned. 
If an interactable is long pressed it uses the Grab manager to manage the movment of the object.

## Grab Manager
The GrabManager does not inherith from monobehviour. A grab manager is connected to a single grab "event". When an interactable is long pressed the touch manager instantiate a grab manager. 
The grab manager register the selected interactable and checks if the interactable is movable. If it is it controls the movement of the object based on the position and rotation of the holder
(in the case of this app the phone)

## Interactable
Parent class of all the interactables. 
It has a protected movable attribute 
It has the declaration of a virtual method interact

### Bomb
Child class of interactable
When touched it explode producing a shockwave and debrits

### SpaceButton
A button positioned in space. When interacted with it changes color for a certain period of time. 
