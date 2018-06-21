using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CityButtonScript : MonoBehaviour {
    public Dictionary<string, Dictionary<string, int>> levelsTown; // A dictionary containing the level dictionary per town.

    // Panels
    public GameObject buildingPanel;
    public GameObject legionsPanel;
    public GameObject infantryPanel;
    public GameObject army;

    void Start()
    {
        levelsTown = new Dictionary<string, Dictionary<string, int>>();
        buildingPanel.SetActive(false);
        legionsPanel.SetActive(false);
        infantryPanel.SetActive(false);
    }

    public void ClickBuildingButton()
    {
        
        GameObject button = EventSystem.current.currentSelectedGameObject; // The button that was clicked
        GameObject city = button.transform.parent.parent.parent.gameObject;
        CityStats buttonParent = city.GetComponent<CityStats>();  // Gets the parent from the button that was clicked, aka a City

        string buildingname = "";
        for (int i = 0; i < button.name.Length; i++) // Removes "Button" from the string, leaving the name of the building. Eg, FarmButton becomes Farm.
        {
            buildingname = buildingname + button.name[i];
            if (button.name[i + 1].Equals('B'))
            {
                break;
            }
        }
        if (!levelsTown.ContainsKey(city.name))
        {
            levelsTown.Add(city.name, new Dictionary<string, int>());
        }

        if (!levelsTown[city.name].ContainsKey(buildingname)) // If the key is not yet in the dictionary it will be added.
        {
            levelsTown[city.name].Add(buildingname, 0);
        }


        if (buildingname == "Townhall")
        {
            buttonParent.popMax = 1200 * Mathf.Pow(levelsTown[city.name]["Townhall"] + 1, 2) + 5000;
        }

        if (Config.buildingconfig[buildingname]["Balance"].Count > levelsTown[city.name][buildingname] && BalanceController.balance >= (int)Config.buildingconfig[buildingname]["Balance"][levelsTown[city.name][buildingname]]) // There needs to be a level for the specific building and enough money.
        {
            // Update balance, buildingmod and popgrowth based on the buildingname config.
            Stats_update((int)Config.buildingconfig[buildingname]["Balance"][levelsTown[city.name][buildingname]],
                Config.buildingconfig[buildingname]["Buildingmod"][0],
                Config.buildingconfig[buildingname]["Popgrowth"][0], 0, buttonParent);
            if (Config.buildingconfig[buildingname]["Balance"].Count > (levelsTown[city.name][buildingname]) + 1) // Update to the price of the next level, if there is one.
            {
                button.GetComponentInChildren<Text>().text = buildingname + " " + (levelsTown[city.name][buildingname] + 2).ToString()  // Takes the buttons children text and updates the price
                    + ": " + (int)Config.buildingconfig[buildingname]["Balance"][levelsTown[city.name][buildingname] + 1];
            }
            else
            {
                button.GetComponentInChildren<Text>().text= buildingname+ " Max";
            }
            levelsTown[city.name][buildingname]++;
        }
    }

    public void ClickLegionsButton()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject; // The button that was clicked
        CityStats buttonParent = button.transform.parent.parent.parent.gameObject.GetComponent<CityStats>(); // Gets the parent from the button that was clicked, aka a City

        string legionname = "";
        for (int i = 0; i < button.name.Length; i++) // Removes "Button" from the string, leaving the name of the building. Eg, FarmButton becomes Farm.
        {
            legionname = legionname + button.name[i];
            if (button.name[i + 1].Equals('B'))
            {
                break;
            }
        }
        if (Config.legionsConfig[legionname] <= BalanceController.balance && buttonParent.population > 20)
        {
            Stats_update(Config.legionsConfig[legionname], 0, 0, 1, buttonParent);
        }
    }

    private void Stats_update(int Balance, double buildingmod, double popgrowth, int army, CityStats buttonParent) // Updating popgrowth, buildingmod and balance based on button clicked.
    {
        if (buttonParent.building_modifier < 100) // To be sure
        {
            buttonParent.building_modifier += (float)buildingmod;
        }        
        buttonParent.population_growth += (float)popgrowth;
        BalanceController.balance += -Balance;
        buttonParent.armySize += army;
        buttonParent.Update_Citystats();
    }

    public void OnBuildings() // On click buildings button, if building menu is active it will deactivate and vice versa.
    {
        if (buildingPanel.activeSelf)
        {
            buildingPanel.SetActive(false);
        }
        else
        {
            buildingPanel.SetActive(true);
        }      
        legionsPanel.SetActive(false);
    }

    public void OnLegions() // On click legions button, if legions menu is active it will deactivate and vice versa.
    {
        buildingPanel.SetActive(false);
        if (legionsPanel.activeSelf)
        {
            legionsPanel.SetActive(false);
        }
        else
        {
            legionsPanel.SetActive(true);
        }     
    }

    public void Send_Armies() // Send armies button
    {
        GameObject button = EventSystem.current.currentSelectedGameObject; // The button that was clicked
        CityStats buttonParent = button.transform.parent.parent.gameObject.GetComponent<CityStats>(); // Gets the parent from the button that was clicked, aka a City

        if (buttonParent.armySize > 0) // Makes a new army object
        {           
            StopCoroutine("InstantiateArmy");
            StartCoroutine("InstantiateArmy", buttonParent);
        }
    }

    IEnumerator InstantiateArmy(CityStats buttonParent) // Instantiates an army
    {
        bool playerClicked = false; // Only instantiate when a player clickes to move it immediately 
        Vector3 playerClickedTarget = Vector3.zero;
        float armysize = buttonParent.armySize;
        buttonParent.armySize = 0; // The city where the army spawned from now has 0 armies
        buttonParent.Update_Citystats();

        while (!playerClicked) // Waits untill the player clicks before instantiating unit
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(1) )
                {
                    playerClicked = true;
                    playerClickedTarget = new Vector3(hit.point.x, 0f, hit.point.z);
                }
            }
            yield return null;
        }
        // Make a new army
        GameObject newArmy = Instantiate(army, buttonParent.transform.position, Quaternion.identity);

        ArmyStats newArmyStats = newArmy.GetComponent<ArmyStats>();

        newArmyStats.justSpawned = true; // So that it doesnt collide with the town immediately, used in Collide armies to not collide with a town
        newArmy.transform.parent = GameObject.Find("_Dynamic").transform; // Becomes a dynamic object
        newArmyStats.armySize = armysize;
        newArmyStats.isSelected = true; // The army spawned is selected
        newArmyStats.spawnNumber = StaticLibrary.spawnArmyCounter;
        newArmyStats.playerNumber = buttonParent.playerNumber;

        newArmy.GetComponent<MoveArmies>().target = playerClickedTarget;
        newArmy.GetComponent<MoveArmies>().StartRoutine();

        StaticLibrary.DeselectTown();
        StaticLibrary.spawnArmyCounter++;

        playerClicked = false;
        yield return new WaitForSeconds(0.5f); // So that it doesnt collide with the town immediately
        newArmyStats.justSpawned = false;
    }

    public void Close_Button()
    {
        StaticLibrary.pointerOnUI = false;
        GameObject button = EventSystem.current.currentSelectedGameObject; // The button that was clicked
        button.transform.parent.gameObject.SetActive(false);           
    }

    public void OnInfantryStats()
    {
        legionsPanel.SetActive(true);
        if (infantryPanel.activeSelf)
        {
            infantryPanel.SetActive(false);
        }
        else
        {
            infantryPanel.SetActive(true);
        }
    }
}
