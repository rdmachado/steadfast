using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class WizardController : MonoBehaviour {

    private enum HandState { idle, charging, casting, reloading }

    public GameObject ChargeIndicator, CooldownIndicator;

    private ProgressBar chargeProgressIndicator, cooldownProgressIndicator;
    private GameObject attackProjectile;
    private Vector2 attackProjectileBaseScale;

    // Left hand
    private HandState leftHandState = HandState.idle;
    public float attackChargeSpeed = 40, attackReloadDuration = 2, attackProjectileSpeed = 1000;
    public Sprite idleSprite, chargingSprite, castingSprite, reloadingSprite;
    public GameObject projectile;

    private float attackCharge = 0, attackCooldown = 0;
    private bool attackReady = true;
    
    // Right hand
    private HandState rightHandState = HandState.idle;
    public List<Spell> spells = new List<Spell>();
    private int selectedSpellIndex = 0;

	// Use this for initialization
	void Start () {
        spells.Add(new VoidSpear(this.transform.position));

        leftHandState = HandState.idle;
        rightHandState = HandState.idle;

        chargeProgressIndicator = ChargeIndicator.GetComponent<ProgressBar>();
        chargeProgressIndicator.Min = 0;
        chargeProgressIndicator.Max = 100;
        chargeProgressIndicator.Value = 0;
        chargeProgressIndicator.DecimalPlaces = 0;
        chargeProgressIndicator.valueType = ProgressBar.ValueType.Percentage;

        cooldownProgressIndicator = CooldownIndicator.GetComponent<ProgressBar>();
        cooldownProgressIndicator.Value = 0;
        cooldownProgressIndicator.DecimalPlaces = 1;
        cooldownProgressIndicator.valueType = ProgressBar.ValueType.Float;
        cooldownProgressIndicator.color = Color.red;
	}
	
	void Update () {
        // check left hand
        switch (leftHandState)
        {
            case HandState.idle:
                LeftIdle();
                break;
            case HandState.charging:
                LeftCharging();
                break;
            case HandState.casting:
                LeftCasting();
                break;
            case HandState.reloading:
                LeftReloading();
                break;
            default:
                break;
        }

        // check right hand
        switch (rightHandState)
        {
            case HandState.idle:
                RightIdle();
                break;
            case HandState.charging:
                RightCharging();
                break;
            case HandState.casting:
                RightCasting();
                break;
            case HandState.reloading:
                RightReloading();
                break;
            default:
                break;
        }

        foreach (var spell in spells)
            spell.Update();
	}

    #region Left Hand Methods

#warning TO REMOVE
    IEnumerator FunTime()
    {
        int i = 10;
        while (i > 0)
        {
            float charge = UnityEngine.Random.value + 1.5f;
            var fun = Instantiate(projectile);
            fun.GetComponent<SpriteRenderer>().enabled = true;
            fun.GetComponent<Transform>().position = new Vector3(UnityEngine.Random.Range(-3f, 4f), 7);
            fun.GetComponent<Transform>().localScale *= (charge + 1);
            fun.GetComponent<Rigidbody2D>().gravityScale = 1;
            fun.GetComponent<AttackProjectileController>().chargeAmount = charge * 100;
            yield return new WaitForSeconds(0.6f);
        }
    }

    private void LeftIdle()
    {
#warning TO REMOVE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FunTime());
        }

        this.GetComponent<SpriteRenderer>().sprite = idleSprite;
        if (attackReady && Input.GetButtonDown("Fire1"))
        {
            leftHandState = HandState.charging;
            attackProjectile = Instantiate(projectile);
            attackProjectile.GetComponent<SpriteRenderer>().enabled = true;
            attackProjectile.GetComponent<Transform>().position = new Vector3(this.transform.position.x + 0.2f, this.transform.position.y);
            attackProjectileBaseScale = attackProjectile.GetComponent<Transform>().localScale;
            attackProjectile.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    private void LeftCharging()
    {
        this.GetComponent<SpriteRenderer>().sprite = chargingSprite;
        attackCharge += Time.deltaTime * attackChargeSpeed;
        if (attackCharge > 100)
            attackCharge = 100;
        chargeProgressIndicator.Value = attackCharge;
        
        attackProjectile.GetComponent<Transform>().localScale = Vector3.Lerp(attackProjectile.GetComponent<Transform>().localScale, attackProjectileBaseScale * ((attackCharge / 100) + 1), 2 * Time.deltaTime);
        
        if (Input.GetButtonUp("Fire1"))
        {
            leftHandState = HandState.casting;
            chargeProgressIndicator.Value = 0;
        }
    }

    private void LeftCasting()
    {
        this.GetComponent<SpriteRenderer>().sprite = castingSprite;
        leftHandState = HandState.casting;
        FireAttack();
        attackCharge = 0; attackReady = false; attackCooldown = attackReloadDuration;
        leftHandState = HandState.reloading;
    }

    private void LeftReloading()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            leftHandState = HandState.idle;
            cooldownProgressIndicator.Value = 0;
            attackReady = true;
        }
        else
        {
            cooldownProgressIndicator.Value = attackCooldown;
        }
    }

    private void FireAttack()
    {
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = (Input.mousePosition - sp).normalized;
        attackProjectile.GetComponent<Rigidbody2D>().AddForce(dir * (attackProjectileSpeed + (attackCharge * 2)));
        attackProjectile.GetComponent<Rigidbody2D>().gravityScale = 1;
        attackProjectile.GetComponent<AttackProjectileController>().chargeAmount = attackCharge;
    }

    #endregion

    #region Right Hand Methods

    private void RightIdle()
    {
        if (Input.GetButtonDown("Fire2") && spells[selectedSpellIndex].Ready)
        {
            rightHandState = HandState.charging;
        }
    }

    private void RightCharging()
    {
        if (spells[selectedSpellIndex].Charge())
            rightHandState = HandState.casting;
    }

    private void RightCasting()
    {
        if (spells[selectedSpellIndex].Cast())
            rightHandState = HandState.reloading;
    }

    private void RightReloading()
    {
        if (spells[selectedSpellIndex].Reload())
            rightHandState = HandState.idle;
    }

    #endregion

    #region Spell Selection

    public void SelectNextSpell()
    {
        selectedSpellIndex = (selectedSpellIndex + 1 < spells.Count ? selectedSpellIndex++ : 0);
    }

    public void SelectPreviousSpell()
    {
        selectedSpellIndex = (selectedSpellIndex - 1 >= 0 ? selectedSpellIndex-- : spells.Count - 1);
    }

    public void SelectSpellAtIndex(int index)
    {
        if (index >= 0 && index < spells.Count)
            selectedSpellIndex = index;
    }

    #endregion
}

