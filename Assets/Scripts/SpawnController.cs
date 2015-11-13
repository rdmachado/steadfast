using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

    public GameObject monsterGrouper;
    public GameObject monster;
    public AnimationCurve speedMultiplierProbability;
    public float spawnRateInSeconds = 1;
    public bool Active = true;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnMonster());
	}
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator SpawnMonster()
    {
        while (true)
        {
            if (Active)
            {
                var newMonster = Instantiate(monster);
                newMonster.GetComponent<SpriteRenderer>().enabled = true;
                newMonster.GetComponent<Transform>().position = new Vector3(this.transform.position.x, this.transform.position.y);
                newMonster.transform.parent = monsterGrouper.transform;
                var speedMultiplier = speedMultiplierProbability.Evaluate(Random.value);
                newMonster.GetComponent<GoblinController>().StartWalkingLeft(speedMultiplier);
            }
            yield return new WaitForSeconds(spawnRateInSeconds);
        }
    }
}
