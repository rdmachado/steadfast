using UnityEngine;
using System.Linq;
using System.Collections;

public class AttackProjectileController : MonoBehaviour {

    public float baseExplosionRadius = 0.3f;
    public float baseExplosionForce = 100f;
    public float chargeAmount = 0f;
    public AnimationCurve distanceXForceRatio;
    public AnimationCurve distanceYForceRatio;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Floor" || col.gameObject.tag == "Goblin")
        {
            var goblins = GameObject.FindGameObjectsWithTag("Goblin").Where(e => Vector2.Distance(e.transform.position, transform.position) < (baseExplosionRadius + (chargeAmount / 100 * 0.7f)));

            foreach (GameObject g in goblins)
            {
                Vector2 forceDirection = new Vector2(distanceXForceRatio.Evaluate(Vector2.Distance(g.transform.position, transform.position)), distanceYForceRatio.Evaluate(Vector2.Distance(g.transform.position, transform.position)));

                forceDirection.x *= Mathf.Sign(g.transform.position.x - transform.position.x);

                g.GetComponent<Rigidbody2D>().AddForce(forceDirection * (baseExplosionForce + (chargeAmount / 100 * 400)), ForceMode2D.Force);
                g.GetComponent<GoblinController>().ApplyDamage(1f);
            }
            Destroy(this.gameObject);
        }
    }
}