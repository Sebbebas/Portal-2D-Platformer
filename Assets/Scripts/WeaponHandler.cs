using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum WeaponState
{
    None = 0,
    portalGun = 1,
    gun = 2,
    Total,
}

public class WeaponHandler : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponStruct
    {
        public Weapon AvailableWeapons;
        public Image hotbar;
        public string inputKey;
    }

    [SerializeField] WeaponStruct[] state;
    [SerializeField] Weapon CurrentWeapon = null;
    [SerializeField] float mouseScrollDistance = 1f;

    private float mouseAxisDelta = 0.0f;

    private void Update()
    {
        HandleWeaponSwap();

        if (Input.GetMouseButtonDown(0) && CurrentWeapon != null)
        {
            CurrentWeapon.Fire();
        }
    }

    private void HandleWeaponSwap()
    {
        foreach (WeaponStruct weaponStruct in state)
        {
            if (weaponStruct.AvailableWeapons == CurrentWeapon)
            {
                weaponStruct.AvailableWeapons.gameObject.SetActive(true);
                weaponStruct.hotbar.gameObject.GetComponent<Image>().color = Color.white;
                CurrentWeapon = weaponStruct.AvailableWeapons;
            }
            else
            {
                weaponStruct.AvailableWeapons.gameObject.SetActive(false);
                weaponStruct.hotbar.gameObject.GetComponent<Image>().color = Color.gray;

                if (Input.GetKeyDown(weaponStruct.inputKey)) { CurrentWeapon = weaponStruct.AvailableWeapons; }
            }
        }

        //mouseAxisDelta += Input.mouseScrollDelta.y;
        //if (Mathf.Abs(mouseAxisDelta) > mouseScrollDistance)
        //{
        //    int swapDirection = (int)Mathf.Sign(mouseAxisDelta);
        //    mouseAxisDelta -= swapDirection * mouseScrollDistance;

        //    int currentWeaponIndex = (int)CurrentWeapon.WeaponType;
        //    currentWeaponIndex += swapDirection;

        //    if (currentWeaponIndex < 0)
        //    {
        //        currentWeaponIndex = (int)WeaponState.Total + currentWeaponIndex;
        //    }
        //    if (currentWeaponIndex >= (int)WeaponState.Total)
        //    {
        //        currentWeaponIndex = 0;
        //    }
        //    CurrentWeapon = weaponStruct.AvailableWeapons;
        //}
    }
}
