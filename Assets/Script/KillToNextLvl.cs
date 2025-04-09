using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillToNextLvl : MonoBehaviour
{
    public int kill = 0;
    public GameObject Wall1;
    public GameObject Wall2;

    public GameObject Boss;
    

    private bool isSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        Wall1.SetActive(true);
        Wall2.SetActive(true);
        Boss.SetActive(false);
        Debug.Log("kill " + kill);
    }

    void Update()
    {
        // Debugging kill count. Remove or comment out in production.
        Debug.Log("kill " + kill);

        // Deactivate Wall1 when kills reach 6
        if (kill >= 6 && Wall1.activeSelf)
        {
            Wall1.SetActive(false);
        }

        // Spawn Boss when kills reach 18 and Boss hasn't been spawned yet
        if (kill >= 18 && !isSpawned)
        {
            isSpawned = true;
            Wall2.SetActive(false);
            Boss.SetActive(true);
        }
    }

    // Method to add kills
    public void AddKill(int k)
    {
        kill += k;
        Debug.Log("kill " + kill);
    }
}
