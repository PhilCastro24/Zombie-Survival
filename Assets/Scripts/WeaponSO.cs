using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NewWeaponType", fileName = "Weapons")]

public class WeaponSO : ScriptableObject
{
    public enum WeaponType
    {
        Pistol,
        SMG,
        ShotGun
    }

    [Header("General Settings")]
    public WeaponType weaponType;
    public AmmoType ammoType;
    public float range = 100f;
    public float damage = 40;
    public float timeBetweenShots = 0.5f;

    [Header("Bullets Settings")]
    public int pelletsAmount = 10;
    public float bulletSpread = 0.1f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Audio sounds")]
    public AudioClip ShootSFX;
    public AudioClip emptyGunSFX;
}
