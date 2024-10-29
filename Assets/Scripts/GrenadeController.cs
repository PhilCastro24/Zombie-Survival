using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float explosionRadius;
    [HideInInspector] public float explosionForce;
    [HideInInspector] public float fuseTime;
    [HideInInspector] public GameObject explosionEffect;
    [HideInInspector] public AudioClip explosionSound;

    float countdown;
    bool hasExploded = false;

    private AudioSource audiosource;

    void Start()
    {
        countdown = fuseTime;

        audiosource = Camera.main.GetComponent<AudioSource>();

        audiosource.spatialBlend = 1f;
        audiosource.rolloffMode=AudioRolloffMode.Linear;
        audiosource.maxDistance = 50f;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        PlayExplosionSound();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            EnemyHealth enemyHealth = nearbyObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void PlayExplosionSound()
    {
        if (explosionSound != null && audiosource != null)
        {
            audiosource.PlayOneShot(explosionSound);
        }
        else
        {
            Debug.LogWarning("Explosion Sound or Audiosource is missing on Grenade");
        }
    }
}
