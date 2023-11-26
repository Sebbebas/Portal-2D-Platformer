using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        return true;
    }
}
