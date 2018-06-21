using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityStats : MonoBehaviour {
    public double popMax;
    public int income;
    public int population;
    public float armySize;
    public float building_modifier;
    public float population_growth;
    public float armyUpkeep = 5;

    [HideInInspector]
    public bool isSelected = true;
    public string cityName;
    public int playerNumber;

    private Color32 color;
    public int initIncome;
    public int initPopulation;

    public Text cityStatsText;

	void Start() {
        Config.Fill_list();
        color = GetComponent<Renderer>().material.color;
        transform.Find("CityCanvas").Find("CityStatsPanel").Find("CityNameText").gameObject.GetComponent<Text>().text = cityName;
        transform.Find("CityNameCanvas").Find("CityName").gameObject.GetComponent<Text>().text = cityName;
        StopCoroutine("CityStatsRoutine");
        StartCoroutine("CityStatsRoutine");
    }

    void Update()
    {
        Update_color(); // TODO: Wellicht dit niet elke update doen maar pas als de stad selected wordt, later aanpassen dus als we alle manieren van steden selecteren hebben.
    }

    public void Update_Citystats()
    {
        StopCoroutine("CityStatsRoutine");
        StartCoroutine("CityStatsRoutine");
    }

    IEnumerator CityStatsRoutine()
    {
        while(true)
        {
            int rate_of_growth = 4;
            population = (int)(initPopulation + (popMax - initPopulation) / (1 + Mathf.Exp((-population_growth * rate_of_growth / 60) * Time.time)) - (popMax - initPopulation) / 2) - Mathf.RoundToInt(armySize) * 20;
            income = initIncome + (int)((population - initPopulation) * 0.05) + (int)(initIncome * 2 * building_modifier);
            cityStatsText.text = "Income: " + income.ToString() + "\nPopulation: " + population.ToString() + "\nBuilding Value: " + (building_modifier * 100).ToString() + "%\nArmies: " + Mathf.RoundToInt(armySize).ToString();
            yield return new WaitForSeconds(1f);
        }    
    }

    void OnMouseEnter()
    {       
        GetComponent<Renderer>().material.color = new Color32(75, 10, 1, 6);
    }
    void OnMouseExit()
    {
        if (!isSelected)
            GetComponent<Renderer>().material.color = color;
    }

    void Update_color()
    {
        if (isSelected)
            transform.GetComponent<Renderer>().material.color = new Color32(75, 10, 1, 6);
        else
            transform.GetComponent<Renderer>().material.color = color;
    }
}
