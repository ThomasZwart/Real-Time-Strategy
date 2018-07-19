using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollideArmies : MonoBehaviour {

    public GameObject battleGround;
    
    /* De 2 collision gameobjects kunnen hier niet, want dan als een 3e partij collide dan worden ze geupdate hier en klopt het gevecht niet meer.
      
     */

    void OnTriggerEnter(Collider collision)

    // When an other prefab army is hit they collide, based on wether it is an enemy or ally it responds accordingly.
    {
        if (collision.gameObject.CompareTag("Town"))
        {
            ArmyStats myArmyStats = transform.GetComponent<ArmyStats>();
            CityStats cityStats = collision.GetComponent<CityStats>();

            if (myArmyStats.playerNumber == cityStats.playerNumber && !myArmyStats.justSpawned) // Own city collision
            {
                cityStats.armySize += myArmyStats.armySize;
                cityStats.Update_Citystats();
                Destroy(myArmyStats.gameObject);
            }
            else // TODO enemy city collision
            {

            }
        }
        else if (collision.gameObject.CompareTag("Army")) // Collision between armies
        {
            ArmyStats myArmyStats = transform.GetComponent<ArmyStats>();
            ArmyStats enemyArmyStats = collision.GetComponent<ArmyStats>();
            
            if (enemyArmyStats.playerNumber == myArmyStats.playerNumber) // Own Team collision
            {
                // spawnNumber < spawnNumber is so that the collision only gets done once, because this method is being invoked on both objects
                if (enemyArmyStats.spawnNumber < myArmyStats.spawnNumber && !myArmyStats.inBattle && !enemyArmyStats.inBattle)
                    OwnTeamCollision(collision);

                else if (!enemyArmyStats.inBattle && myArmyStats.inBattle) // If one of the 2 is in battle
                {
                    myArmyStats.armySize += enemyArmyStats.armySize;
                    Destroy(enemyArmyStats.gameObject);
                }
            }
            else // Enemy Team collision
            {
                if (enemyArmyStats.spawnNumber < myArmyStats.spawnNumber && !myArmyStats.inBattle && !enemyArmyStats.inBattle)
                    EnemyTeamCollision(collision);

                else if (!enemyArmyStats.inBattle && myArmyStats.inBattle)
                {
                    myArmyStats.inBattleWith.GetComponent<ArmyStats>().armySize += enemyArmyStats.armySize;
                    Destroy(enemyArmyStats.gameObject);
                }
            }           
        }         
    }

    void EnemyTeamCollision(Collider collision)
    {
        StopCoroutine("FightRoutine");
        StartCoroutine("FightRoutine", collision);      
    }

    IEnumerator FightRoutine(Collider collision)
    {
        ArmyStats myArmyBattleStats = transform.GetComponent<ArmyStats>();
        ArmyStats enemyArmyBattleStats = collision.GetComponent<ArmyStats>();
        myArmyBattleStats.inBattle = true;
        enemyArmyBattleStats.inBattle = true;

        // When the reinforcements hit the other team, you need to acces the one he is in battle with via this.
        myArmyBattleStats.inBattleWith = enemyArmyBattleStats.gameObject;
        enemyArmyBattleStats.inBattleWith = myArmyBattleStats.gameObject;

        // Armies halt when they fight
        collision.GetComponent<MoveArmies>().StopRoutine();
        gameObject.GetComponent<MoveArmies>().StopRoutine();

        // Instantiate a timer
        gameObject.GetComponent<MoveArmies>().battleStartTime = Time.time;
        collision.GetComponent<MoveArmies>().battleStartTime = Time.time;

        float myStrengthFactor = myArmyBattleStats.armyForce / (myArmyBattleStats.armyForce + enemyArmyBattleStats.armyForce);

        GameObject battle = Instantiate(battleGround, (transform.position + collision.transform.position) / 2, Quaternion.identity);  // Takes the average of the 2 armies and places a battle prefab
        battle.transform.parent = GameObject.Find("_Dynamic").transform; // Becomes a dynamic object

        while (myArmyBattleStats.armySize >= 1 && enemyArmyBattleStats.armySize >= 1 && myArmyBattleStats.inBattle && enemyArmyBattleStats.inBattle)
        {
            battle.transform.Find("BattlegroundCanvas").Find("PlayerOneArmySizeText").gameObject.GetComponent<Text>().text = Mathf.RoundToInt(myArmyBattleStats.armySize).ToString();
            battle.transform.Find("BattlegroundCanvas").Find("PlayerTwoArmySizeText").gameObject.GetComponent<Text>().text = Mathf.RoundToInt(enemyArmyBattleStats.armySize).ToString();

            if (myStrengthFactor > Random.Range(0f, 1f)) // Strengthfactors decide the change of winning per time interval
                enemyArmyBattleStats.armySize -= 0.2f;
            else
                myArmyBattleStats.armySize -= 0.2f;
            myStrengthFactor = myArmyBattleStats.armyForce / (myArmyBattleStats.armyForce + enemyArmyBattleStats.armyForce);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(battle);

        // The army that didnt flee gets a morale boost
        if (!enemyArmyBattleStats.inBattle && enemyArmyBattleStats.armySize >= 1)
            myArmyBattleStats.moraleBoost += 0.1f;
        else if (!myArmyBattleStats.inBattle && myArmyBattleStats.armySize >= 1)
            enemyArmyBattleStats.moraleBoost += 0.1f;

        myArmyBattleStats.inBattle = false;
        enemyArmyBattleStats.inBattle = false;
        myArmyBattleStats.inBattleWith = null;
        enemyArmyBattleStats.inBattleWith = null;      
        
        Destroy(battle);
        
        // When armysize is 0 the army is destroyed
        if (myArmyBattleStats.armySize < 1 && enemyArmyBattleStats.armySize < 1)
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
        else if(myArmyBattleStats.armySize < 1)
        {
            enemyArmyBattleStats.moraleBoost += 0.2f;
            Destroy(gameObject);
        }
        else if (enemyArmyBattleStats.armySize < 1)
        {
            myArmyBattleStats.moraleBoost += 0.2f;
            Destroy(collision.gameObject);
        }       
    }

    void OwnTeamCollision(Collider collision) // Collision with own team
    {
        // Gameobject spawnnumber > collision spawnnumber
        transform.GetComponent<ArmyStats>().armySize += collision.GetComponent<ArmyStats>().armySize;

        transform.GetComponent<ArmyStats>().isSelected = true;

        if (collision.GetComponent<ArmyStats>().isSelected && !transform.GetComponent<ArmyStats>().inBattle) // If the one being destroyed is selected the one that he collides with takes his movespeed and location
        {
            transform.GetComponent<MoveArmies>().target = collision.GetComponent<MoveArmies>().target;

            transform.GetComponent<MoveArmies>().StopRoutine();
            transform.GetComponent<MoveArmies>().StartRoutine();
        }
        Destroy(collision.gameObject);
    }
}
