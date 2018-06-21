using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceController : MonoBehaviour {

    public Text balanceText;
    public Text incomeText;
    public static int balance;
    private float townIncome = 0;
    private float fieldArmyUpkeep = 0;
    private float cityArmyUpkeep = 0;
    private readonly float armyUpkeep = 1;
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
            townIncome = 0;
            cityArmyUpkeep = 0;
            fieldArmyUpkeep = 0;

            foreach (GameObject town in allTowns)
            {
                townIncome += town.GetComponent<CityStats>().income;
                cityArmyUpkeep += town.GetComponent<CityStats>().armySize * town.GetComponent<CityStats>().armyUpkeep;
            }
            foreach (GameObject army in allArmies)
            {
                fieldArmyUpkeep += army.GetComponent<ArmyStats>().armyUpkeep * army.GetComponent<ArmyStats>().armySize;
            }

            balance = (int)(balance + townIncome - cityArmyUpkeep - fieldArmyUpkeep);
            incomeText.text = "Income: " + townIncome;
            balanceText.text = "Balance: " + balance;

            yield return new WaitForSeconds(5f); // Every 5 seconds the balances increases by income - total army upkeep
        }      
    }
}
