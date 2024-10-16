using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] CinemachineVirtualCamera CVCamera;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactEffect;
    [SerializeField] Ammo ammoSlot;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] AmmoType ammoType;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 40;
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] int pelletsAmount = 10;
    [SerializeField] float bulletSpread = 0.1f;
    [SerializeField] AudioSource audiosource;

    [Header("Crouching")]
    [SerializeField] float crouchHeight = 1.0f;
    [SerializeField] float crouchSpeed = 2.0f;   
    [SerializeField] float normalSpeed = 5.0f;
    [SerializeField] float originalHeight = 1f;
    [SerializeField] float originalCameraHeight = 1f;



    [Header("Gun sounds")]
    [SerializeField] AudioClip pistolShootSFX;
    [SerializeField] AudioClip emptyGunSFX;

    [Header("SMG sounds")]
    [SerializeField] AudioClip smgShootSFX;
    [SerializeField] AudioClip emptySmgSFX;

    [Header("Shotgun sounds")]
    [SerializeField] AudioClip shotgunShootSFX;
    [SerializeField] AudioClip emptyShotgunSFX;

    [HideInInspector] public bool canShoot = true;

    bool isCrouching = false;
    Vector3 originalCenter;


    CharacterController characterController;
    Animator animator;

    public enum WeaponType
    {
        Pistol,
        SMG,
        ShotGun
    }

    public WeaponType weaponType;

    void Start()
    {
        animator = GetComponent<Animator>();
        CVCamera = GetComponent<CinemachineVirtualCamera>();

        if (audiosource != null)
        {
            audiosource = GetComponentInParent<AudioSource>();
        }

        characterController = GetComponentInParent<CharacterController>();
        if (characterController != null)
        {
            originalHeight = characterController.height;
            originalCenter = characterController.center;
        }
        else
        {
            Debug.Log("CharacterController not found in Parent objects");
        }
    }


    void OnEnable()
    {
        canShoot = true;
    }

    void Update()
    {
        DispalyAmmo();

        if (Input.GetMouseButtonDown(0) && canShoot == true)
        {
            StartCoroutine(Shoot());
        }

        DisableAnimations();
        Crouch();
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);

        switch (weaponType)
        {
            case WeaponType.Pistol:
                if (currentAmmo > 0)
                {
                    PlayMuzzleFlash();
                    ProcessRaycast();
                    animator.SetTrigger("Shoot");
                    audiosource.PlayOneShot(pistolShootSFX);
                    ammoSlot.ReduceCurrentAmmo(ammoType);
                }
                else
                {
                    audiosource.PlayOneShot(emptyGunSFX);
                }
                break;

            case WeaponType.SMG:
                if (currentAmmo > 0)
                {
                    PlayMuzzleFlash();
                    ProcessRaycast();
                    animator.SetBool("isShooting", true);
                    audiosource.PlayOneShot(smgShootSFX);
                    ammoSlot.ReduceCurrentAmmo(ammoType);
                }
                else
                {
                    audiosource.PlayOneShot(emptySmgSFX);
                }
                break;

            case WeaponType.ShotGun:
                if (currentAmmo > 0)
                {
                    PlayMuzzleFlash();
                    animator.SetTrigger("Shoot");
                    audiosource.PlayOneShot(shotgunShootSFX);
                    for (int i = 0; i < pelletsAmount; i++)
                    {
                        ProcessRaycast();
                    }
                    ammoSlot.ReduceCurrentAmmo(ammoType);
                }
                else
                {
                    audiosource.PlayOneShot(emptyShotgunSFX);
                }
                break;

                // You can probably use something like this instead of "else" in each switch case

                /*default:
                    if (WeaponType.Pistol)
                    {
                        audiosource.PlayOneShot(emptyGunSFX);
                    }
                    else if (WeaponType.SMG)
                    {
                        audiosource.PlayOneShot(emptyGunSFX);
                    }
                    else if (WeaponType.ShotGun)
                    {
                        audiosource.PlayOneShot(emptyGunSFX);
                    }
                    break;*/
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }


    void ProcessRaycast()
    {
        RaycastHit hit;

        Vector3 direction = GetShotgunSpread();

        Debug.DrawRay(FPCamera.transform.position, direction * range, Color.red, 5f);

        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            //Debug.Log("I hit this thing: " + hit.transform.name);

            CreateImpactHit(hit);

            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();

            if (target == null)
            {
                return;
            }

            target.TakeDamage(damage);
        }
        else
        {
            return;
        }
    }

    void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    void CreateImpactHit(RaycastHit hit)
    {
        GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 0.1f);
    }

    void DispalyAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
    }

    Vector3 GetShotgunSpread()
    {
        Vector3 direction = FPCamera.transform.forward;
        direction.x += Random.Range(-bulletSpread, bulletSpread);
        direction.y += Random.Range(-bulletSpread, bulletSpread);
        return direction;
    }

    void DisableAnimations()
    {
        if (Input.GetMouseButtonUp(0) && weaponType == WeaponType.SMG)
        {
            animator.SetBool("isShooting", false);
        }

        /* else if (Input.GetMouseButtonUp(0) && weaponType == WeaponType.Shotgun)
        {
            animator.SetBool("isShotgunShooting", false);
        } */
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C) && characterController != null)
        {
            isCrouching = !isCrouching;
            

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                characterController.center = new Vector3(0, crouchHeight / 2, 0);

                FPCamera.transform.localPosition = new Vector3(
                    FPCamera.transform.localPosition.x,
                    originalCameraHeight - (originalHeight - crouchHeight) / 2,
                    FPCamera.transform.localPosition.z);
            }
            else
            {
                characterController.height = originalHeight;
                characterController.center = originalCenter;

                FPCamera.transform.localPosition = new Vector3(
                    FPCamera.transform.localPosition.x,
                    originalCameraHeight,
                    FPCamera.transform.localPosition.z);
            }
        }
    }
}
