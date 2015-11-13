using UnityEngine;
using System.Collections;

public abstract class Spell
{
    public readonly string spellName;
    public readonly float cooldown;
    
    private float _cooldown;

    protected Spell(string spellName, float cooldownInSeconds)
    {
        this.spellName = spellName;
        this.cooldown = cooldownInSeconds;
    }
    
    public bool Ready
    {
        get { return _cooldown <= 0; }
    }

    protected void BeginCooldown()
    {
        _cooldown = cooldown;
    }

    public virtual void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.deltaTime;
    }

    public virtual bool Charge()
    {
        return true;
    }

    public virtual bool Cast()
    {
        return true;
    }

    public virtual bool Reload()
    {
        return true;
    }
}

public class VoidSpear : Spell
{
    private readonly float chargeTime, castTime;
    private float _chargeTime = 0, _castTime = 0;
    public Vector3 origin;

    public VoidSpear(Vector3 originCoordinates) : base("Void Spear", 10f)
    {
        chargeTime = 5f;
        _chargeTime = chargeTime;
        castTime = 2f;
        origin = originCoordinates;
    }

    public override bool Charge()
    {
        _chargeTime -= Time.deltaTime;
        if (_chargeTime > 0)
        {
            var direction = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) - origin).normalized;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, 50f, 1 << 9);
            if (hit)
            {
                Debug.DrawLine(origin, hit.point, Color.white);
            }
            else
            {
                Debug.DrawRay(origin, direction, Color.white);
            }

            //update charge
            return false;
        }
        else
        {
            Debug.Log("All Charged Up!");
            _chargeTime = chargeTime;
            _castTime = castTime;
            return true;
        }
    }

    public override bool Cast()
    {
        _castTime -= Time.deltaTime;
        if (_castTime > 0)
        {
            // do casting stuff
            return false;
        }
        else
        {
            Debug.Log("I've just finished casting! I'm a good boy!");
            //casting done, move on!
            base.BeginCooldown();
            return true;
        }
    }
}