using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : Weapon
{
    [Space]

    [SerializeField] GameObject prodjectile;
    [SerializeField] GameObject gunPos;

    public new void Start()
    {
        base.Start();
    }

    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        HitScanFire();
        return true;
    }

    private void HitScanFire()
    {
        Instantiate(prodjectile, transform.position, Quaternion.identity);
    }
}
