using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] WeaponSO weaponSO;

    [Header("References")]
    [SerializeField] Camera weaponCamera;
    [SerializeField] Ammo ammoSlot;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] AudioSource audioSource;

    [HideInInspector] public bool canShoot = true;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponentInParent<AudioSource>();
        }
        if (weaponSO == null)
        {
            Debug.Log("WeaponSO is not assigned: " + gameObject.name);
        }
    }
    void OnEnable()
    {
        canShoot = true;
    }

    void Update()
    {
        DispalyAmmo();

        if (Input.GetMouseButton(0) && canShoot)
        {
            if (weaponSO.weaponType == WeaponSO.WeaponType.SMG)
            {
                animator.SetBool("isShooting", true);
            }
            StartCoroutine(Shoot());
        }

        if (Input.GetMouseButtonUp(0) && weaponSO.weaponType == WeaponSO.WeaponType.SMG)
        {
            animator.SetBool("isShooting", false);
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        int currentAmmo = ammoSlot.GetCurrentAmmo(weaponSO.ammoType);

        if (currentAmmo > 0)
        {
            PlayMuzzleFlash();
            ProcessRaycast();
            if (weaponSO.weaponType != WeaponSO.WeaponType.SMG)
            {
                animator.SetTrigger("Shoot");
            }
            PlayShootSound();
            ammoSlot.ReduceCurrentAmmo(weaponSO.ammoType);
        }
        else
        {
            PlayEmptySound();
        }
        yield return new WaitForSeconds(weaponSO.timeBetweenShots);
        canShoot = true;
    }


    void ProcessRaycast()
    {
        RaycastHit hit;

        int pellets = weaponSO.weaponType == WeaponSO.WeaponType.ShotGun ? weaponSO.pelletsAmount : 1;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = GetBulletDirection();

            if (Physics.Raycast(weaponCamera.transform.position, direction, out hit, weaponSO.range))
            {
                CreateImpactHit(hit);
               

                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target != null)
                {
                    target.TakeDamage(weaponSO.damage);               
                }
            }
        }
        
    }

    void PlayMuzzleFlash()
    {
        if (weaponSO.muzzleFlash != null)
        {
            weaponSO.muzzleFlash.Play();
        }
    }

    void CreateImpactHit(RaycastHit hit)
    {
        if (weaponSO.impactEffect != null)
        {
            GameObject impact = Instantiate(weaponSO.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 0.1f);
        }
    }


    void DispalyAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(weaponSO.ammoType);
        ammoText.text = currentAmmo.ToString();
    }

    Vector3 GetBulletDirection()
    {
        Vector3 direction = weaponCamera.transform.forward;
        direction += new Vector3(
            Random.Range(-weaponSO.bulletSpread, weaponSO.bulletSpread),
            Random.Range(-weaponSO.bulletSpread, weaponSO.bulletSpread),
            Random.Range(-weaponSO.bulletSpread, weaponSO.bulletSpread)
        );
        return direction.normalized;
    }

    void PlayShootSound()
    {
        if (audioSource != null && weaponSO.ShootSFX != null)
        {
            audioSource.PlayOneShot(weaponSO.ShootSFX);
        }
    }

    void PlayEmptySound()
    {
        if (audioSource != null && weaponSO.emptyGunSFX != null)
        {
            audioSource.PlayOneShot(weaponSO.emptyGunSFX);
        }
    }

    void AlertEnemies(Vector3 impactPoint)
    {
        float alertRadius = 20f;

        Collider[] colliders = Physics.OverlapSphere(impactPoint, alertRadius);

        Debug.Log("AlertEnemies called. Colliders found" + colliders.Length);

        foreach (Collider collider in colliders)
        {
            EnemyController enemyController = collider.GetComponentInParent<EnemyController>();

            if (enemyController != null)
            {
                enemyController.InvestigateSound(impactPoint);
            }
        }
    }

}
