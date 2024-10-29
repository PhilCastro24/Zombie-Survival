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
        ShotGun,
        GrenadeLauncher
    }

    [Header("General Settings")]
    public WeaponType weaponType;
    public AmmoType ammoType;
    public float range = 100f;
    public float damage = 40;
    public float timeBetweenShots = 0.5f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float explosionRadius = 10f;
    public float explosionForce = 10f;
    public float fuseTime = 2f;

    [Header("Bullets Settings")]
    public int pelletsAmount = 10;
    public float bulletSpread = 0.1f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Audio sounds")]
    public AudioClip ShootSFX;
    public AudioClip emptyGunSFX;
    public AudioClip explosionSFX;
}
