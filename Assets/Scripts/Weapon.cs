using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponState WeaponType;
    public float weaponRange = 100f;
    public LayerMask ignoreHitMask = 0;
    public ParticleSystem hitParticle;
    public bool lookTowardsCam = false;

    CameraController camController;
    
    protected Camera mainCam = null;

    protected void Start()
    {
        camController = FindObjectOfType<CameraController>();
        mainCam = Camera.main;
    }

    public void FixedUpdate()
    {
        lookTowardsCam = camController.GetCameraToMouseBool();

        if (lookTowardsCam ) { camController.SetCameraToMouse(true); }
        else { camController.SetCameraToMouse(false); }
    }

    public virtual bool Fire()
    {
        return true;
    }
}
