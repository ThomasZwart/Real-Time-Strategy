using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerSelectionScript : MonoBehaviour {

    public GUIStyle MouseDragSkin;
    private static Vector3 mouseDownPoint;
    private static Vector3 mouseUpPoint;
    public static bool userIsDragging;


    // Use this for initialization
    private GameObject UI;
    RaycastHit hit;


    private void OnGUI()
    {
        if (userIsDragging)
        {          
            GUI.Box(new Rect(mouseDownPoint.x, Screen.height - mouseDownPoint.y, Input.mousePosition.x - mouseDownPoint.x, mouseDownPoint.y - Input.mousePosition.y), "", MouseDragSkin);
        }      
    }
    void Start () {
		
	}
	// Update is called once per frame
	void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPoint = Input.mousePosition;
            userIsDragging = true;
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.collider.tag == "Town")
                {
                    if (hit.transform.GetComponent<CityStats>().isSelected)
                        hit.transform.GetComponent<CityStats>().isSelected = false;
                    else
                    {
                        StaticLibrary.DeselectAll();
                        hit.transform.GetComponent<CityStats>().isSelected = true;
                    }
                    hit.transform.Find("CityCanvas").gameObject.SetActive(true); // ACtivates the canvas of the city hit                 
                }
                else if (hit.collider.tag == "Army")
                {
                    if (hit.transform.GetComponent<ArmyStats>().isSelected)
                        hit.transform.GetComponent<ArmyStats>().isSelected = false;
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) // Able to select more armies with Cntrl Down
                        {
                            hit.transform.GetComponent<ArmyStats>().isSelected = true;
                            StaticLibrary.DeselectTown();
                        }
                        else
                        {
                            StaticLibrary.DeselectAll();
                            hit.transform.GetComponent<ArmyStats>().isSelected = true;
                        }                      
                    }
                }
                
                else if (hit.collider.name == "Terrain" && !StaticLibrary.pointerOnUI && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) 
                    // Only when you click on the ground will all units get deselected
                {
                    StaticLibrary.DeselectAll();
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) // 
        {
            mouseUpPoint = Input.mousePosition;

            foreach (GameObject army in GameObject.FindGameObjectsWithTag("Army"))
            {
                Vector3 armyPosition = Camera.main.WorldToScreenPoint(army.transform.position);
                if ((armyPosition.x >= mouseDownPoint.x && armyPosition.x <= mouseUpPoint.x && armyPosition.y <= mouseDownPoint.y && armyPosition.y >= mouseUpPoint.y)
                    || (armyPosition.x <= mouseDownPoint.x && armyPosition.x >= mouseUpPoint.x && armyPosition.y >= mouseDownPoint.y && armyPosition.y <= mouseUpPoint.y)
                    || (armyPosition.x <= mouseDownPoint.x && armyPosition.x >= mouseUpPoint.x && armyPosition.y <= mouseDownPoint.y && armyPosition.y >= mouseUpPoint.y)
                    || (armyPosition.x >= mouseDownPoint.x && armyPosition.x <= mouseUpPoint.x && armyPosition.y >= mouseDownPoint.y && armyPosition.y <= mouseUpPoint.y))
                {
                    army.GetComponent<ArmyStats>().isSelected = true;
                }
            }
            userIsDragging = false;
        }       
    }
}
