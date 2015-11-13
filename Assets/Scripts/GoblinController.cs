using UnityEngine;
using System.Collections;

public class GoblinController : MonoBehaviour {

    public float baseSpeed = 0.5f;
    public float maxHealth = 1f;

    private enum GoblinState { idle, walking, frozen, dead }
    private GoblinState currentState = GoblinState.idle;
    private GoblinState previousState = GoblinState.idle;
    private float speed;
    private bool isDead = false;
    private float currentHealth;
    private float frozenCountdown = 0;


	// Use this for initialization
	void Start () {
        if (speed == 0)
            speed = baseSpeed;
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	    switch (currentState)
        {
            case GoblinState.idle:
                Idle();
                break;
            case GoblinState.frozen:
                Frozen();
                break;
            case GoblinState.walking:
                Walk();
                break;
            case GoblinState.dead:
                Dead();
                break;
        }
	}

    public void StartWalkingLeft(float speedMultiplier)
    {
        //Debug.Log("Speed multiplier: " + speedMultiplier + "\nSpeed: " + baseSpeed + "\nNew Speed: " + (baseSpeed * speedMultiplier));
        currentState = GoblinState.walking;
        speed = baseSpeed * speedMultiplier;
    }

    private void Idle()
    {

    }

    private void Frozen()
    {
        if (frozenCountdown <= 0)
            currentState = previousState;
        else
        {
            frozenCountdown -= Time.deltaTime;
        }
    }

    private void Walk()
    {
        this.transform.Translate(new Vector2(-1, 0) * speed * Time.deltaTime);
    }

    private void Dead()
    {
        if (!isDead)
        {
            this.transform.Rotate(Vector3.forward * -90);
            isDead = true;
            GameObject.Find("GameManager").GetComponent<GameManager>().killCount++;
            Destroy(this.gameObject, 5 + Random.value * 5f);
        }
    }

    public void Freeze(float durationInSeconds)
    {
        frozenCountdown = durationInSeconds;
        previousState = currentState;
        currentState = GoblinState.frozen;
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentState = GoblinState.dead;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Tower")
        {
            StartCoroutine(HitTower());
            GameObject.Find("GameManager").GetComponent<GameManager>().towerLost = true;
            currentState = GoblinState.idle;
        }
        else if (col.gameObject.tag == "AttackProjectile")
        {
            currentState = GoblinState.dead;
        }
    }

    IEnumerator HitTower()
    {
        //apply damage to tower
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
