using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponState WeaponType;
    [SerializeField] protected LayerMask ignoreHitMask = 0;
    [SerializeField] protected ParticleSystem hitParticle;
    [SerializeField] protected bool lookTowardsCam = false;

    [Space]
    
    CameraController camController;
    
    protected Camera mainCam = null;


    protected void Start()
    {
        camController = FindObjectOfType<CameraController>();
        mainCam = Camera.main;
    }

    public void FixedUpdate()
    {
        RotateTowardsCamera();
    }

    public virtual bool Fire()
    {
        return true;
    }

    private void RotateTowardsCamera()
    {
        //CAMERA CONTROLLER
        lookTowardsCam = camController.GetCameraToMouseBool();

        if (lookTowardsCam) { camController.SetCameraToMouse(true); }
        else { camController.SetCameraToMouse(false); }
    }
}
