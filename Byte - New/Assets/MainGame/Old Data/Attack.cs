using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{

    public float damageValue;
    public bool doesKnockback;
    public Vector2 knockback;
    public GameObject damageSource;



    public Attack(float _damageValue, bool _doesKnockback, GameObject _damageSource, Vector2 _knockback)
    {
        this.damageValue = _damageValue;
        this.doesKnockback = _doesKnockback;
        this.damageSource = _damageSource;
        knockback = _knockback;

    }
}

