using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// 19-5-2018
public class ArmyStats : MonoBehaviour {
    public bool isSelected;
    public bool inBattle;
    public bool justSpawned;

    public float armySize;
    public float armySpeed;
    public float armyForce;
    public float moraleBoost = 0.1f;
    public float armyUpkeep = 5;
    public float positionBonus = 1.2f;

    [HideInInspector]
    public GameObject inBattleWith;
    //[HideInInspector]
    public int spawnNumber;
    //[HideInInspector]
    public int playerNumber;
    [HideInInspector]
    public Text armySizeText;

    private Color color;


    // Use this for initialization
    void Start () {
        color = GetComponent<Renderer>().material.color;
    }

	// Update is called once per frame
	void Update () {
        armyUpkeep = armySize; // TODO: formule voor upkeep aan de hand van armysize en soort troepen
        armySizeText.text = Mathf.RoundToInt(armySize).ToString();
        armyForce = ArmyForce(); // formule voor armyforce
        Update_color();
	}

    private float ArmyForce()
    {
        float tempForce = armySize;

        for (int i = 0; i < armySize; i++)
        {
            tempForce += moraleBoost;
        }

        tempForce *= positionBonus;

        return tempForce;
    }

    void Update_color()
    {
        if (isSelected)
            transform.GetComponent<Renderer>().material.color = new Color32(75, 10, 1, 6);
        else
            transform.GetComponent<Renderer>().material.color = color;
    }
}
