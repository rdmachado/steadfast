using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

    public int killCount = 0;
    public SpawnController spawn;
    public Text statusText;
    public Text tauntText1;
    public bool towerLost = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (killCount > 150)
            spawn.spawnRateInSeconds = 0.1f;
        else if (killCount > 75)
            spawn.spawnRateInSeconds = 0.5f;
        else if (killCount > 50)
            spawn.spawnRateInSeconds = 0.7f;
        else if (killCount > 15)
            spawn.spawnRateInSeconds = 0.9f;
        
        if (towerLost)
        {
            //tauntText1.text = "Thug Life™";
        }
        else
        {
            statusText.text = "Count: " + killCount + "\nRate: " + spawn.spawnRateInSeconds;
        }
	}
}
