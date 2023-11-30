using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalGun : Weapon
{
    public new void Start()
    {
        hitParticle.gameObject.SetActive(false);
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
        Ray weaponRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit = new();
        if (Physics.Raycast(weaponRay, out hit, weaponRange, ignoreHitMask))
        {
            hitParticle.transform.position = hit.point;
            hitParticle.gameObject.SetActive(true);
        }
    }
    private void RotateTowardsCamera()
    {
        
    }
}
