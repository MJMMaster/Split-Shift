using System.Collections.Generic;
using UnityEngine;

public class CodeRequirementsDemo : MonoBehaviour
{
    //  LIST (Requirement)
    private List<string> collectedItems = new List<string>();

    //  DICTIONARY (Requirement)
    private Dictionary<string, int> enemyKills = new Dictionary<string, int>();

    private void Start()
    {
        // ---------------------------
        //  FOR LOOP (Requirement)
        // ---------------------------
        Debug.Log("FOR LOOP DEMO:");

        for (int i = 1; i <= 5; i++)
        {
            Debug.Log("For Loop Count: " + i);
        }

        // ---------------------------
        //  LIST + FOREACH (Requirement)
        // ---------------------------
        Debug.Log("LIST + FOREACH DEMO:");

        collectedItems.Add("Keycard");
        collectedItems.Add("Potion");
        collectedItems.Add("Battery");

        foreach (string item in collectedItems)
        {
            Debug.Log("Collected Item: " + item);
        }

        // ---------------------------
        //  DICTIONARY (Requirement)
        // ---------------------------
        Debug.Log("DICTIONARY DEMO:");

        enemyKills.Add("Robot", 2);
        enemyKills.Add("Guard", 1);
        enemyKills.Add("Drone", 3);

        foreach (KeyValuePair<string, int> pair in enemyKills)
        {
            Debug.Log(pair.Key + " Kills: " + pair.Value);
        }
    }
}