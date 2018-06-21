using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceController : MonoBehaviour {

    public Text balanceText;
    public Text incomeText;
    public static int balance;
    public static float town_income;
    public static float field_army_upkeep;
    public static float city_army_upkeep;
    public static float army_upkeep;
    GameObject[] allTowns;
    GameObject[] allArmies;

    void Start()
    {
        balance = 10000;
        balanceText.text = "Balance: " + balance;
        StopCoroutine("GlobalStatsRoutine");
        StartCoroutine("GlobalStatsRoutine");
    }

    IEnumerator GlobalStatsRoutine()
    {
        while (true)
        {
            allTowns = GameObject.FindGameObjectsWithTag("Town");
            allArmies = GameObject.FindGameObjectsWithTag("Army");
            town_income = 0;
            field_army_upkeep = 0;
            foreach (GameObject town in allTowns)
            {
                town_income += town.GetComponent<CityStats>().income;
                city_army_upkeep += town.GetComponent<CityStats>().armySize * town.GetComponent<CityStats>().armyUpkeep;


            }
            foreach (GameObject army in allArmies)
            {
                field_army_upkeep += army.GetComponent<ArmyStats>().armyUpkeep * army.GetComponent<ArmyStats>().armySize;


            }
            army_upkeep = city_army_upkeep + field_army_upkeep;
            balance = (int)(balance + town_income - field_army_upkeep);
            incomeText.text = "Income: " + town_income + "\nUpkeep: " + army_upkeep;
            balanceText.text = "Balance: " + balance;
            balanceText.text = "Balance: " + balance;
            incomeText.text = "Income: " + town_income + "\nUpkeep: " + army_upkeep + "\nNet Income: " + (int)(town_income - army_upkeep);
            yield return new WaitForSeconds(5);
        }      
    }
}
