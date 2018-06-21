using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveArmies : MonoBehaviour {

    Ray ray;
    RaycastHit hit;
    public float moveSpeedx, moveSpeedz;
    public Vector3 target;
    public GameObject army;
    public float battleStartTime;
    
	void Start () { 
	}

	void Update ()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (transform.GetComponent<ArmyStats>().isSelected) // Move the army only if it is selected
            {
                if (Input.GetMouseButtonDown(1))
                {
                    target = new Vector3(hit.point.x, 0f, hit.point.z);

                    // wait 2 seconds before exiting a battle
                    if (Time.time - battleStartTime > 2)
                    {
                        transform.GetComponent<ArmyStats>().inBattle = false;
                    }

                    StopRoutine();
                    // only armys outside of a battle are able to move
                    if (!transform.GetComponent<ArmyStats>().inBattle)
                    {
                        StartRoutine();
                    }
                }
            }
        }
    }

    public void StopRoutine()
    {
        StopCoroutine("MoveArmyRoutine");
    }

    public void StartRoutine()
    {
        StartCoroutine("MoveArmyRoutine");
    }

    public IEnumerator MoveArmyRoutine() // Coroutine for moving armies
    {
        while (transform.position.z > target.z + 0.2f || transform.position.z < target.z - 0.2f || transform.position.x > target.x + 0.2f || transform.position.x < target.x - 0.2f)
        {
            CalculateMovespeed();
            transform.Translate(new Vector3(moveSpeedx * transform.GetComponent<ArmyStats>().armySpeed * Time.deltaTime, 0f, moveSpeedz * transform.GetComponent<ArmyStats>().armySpeed * Time.deltaTime));
            yield return new WaitForSeconds(0.02f);
        }
    }

    void CalculateMovespeed() // Calculates the movement speed of z and x for the army, so that the armies move in a straight line to the target
    {
        float relX, relZ;
        // Relative position of mouseclick to the object
        if (transform.position.z > target.z)
            relZ = -Mathf.Abs(transform.position.z - target.z);
        else
            relZ = Mathf.Abs(transform.position.z - target.z);
        if (transform.position.x > target.x)
            relX = -Mathf.Abs(transform.position.x - target.x);
        else
            relX = Mathf.Abs(transform.position.x - target.x);

        float ratioX = relX / (Mathf.Abs(relZ) + Mathf.Abs(relX));
        float ratioZ = relZ / (Mathf.Abs(relZ) + Mathf.Abs(relX));
   
        moveSpeedx = ratioX * (1 / Mathf.Sqrt(Mathf.Pow(ratioX, 2) + Mathf.Pow(ratioZ, 2)));
        moveSpeedz = ratioZ * (1 / Mathf.Sqrt(Mathf.Pow(ratioX, 2) + Mathf.Pow(ratioZ, 2)));
    }
}
