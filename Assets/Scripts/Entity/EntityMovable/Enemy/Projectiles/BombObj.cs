using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObj : MonoBehaviour
{
    [SerializeField] private float bombRange = 2f;
    [SerializeField] private float explosionTime = 2f;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private GameObject explosionEffect;
    private bool exploded =false;

    public void Init(float bombRange)
    {
        this.bombRange = bombRange;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != ignoreLayer)
        {
            if (!exploded)
            {
                exploded = true;
                Explode();
            }
        }
    }

    public void SetBombRange(float range)
    {
        bombRange = range;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, bombRange);
        // Gizmos to show the area of the explosion
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bombRange);
        if(explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        
            foreach (Collider2D nearbyObject in colliders)
            {
                if (nearbyObject.tag == "Player")
                {
                    Debug.LogWarning("Bomb Explosion Not Implemented, Waiting For Damage Methods");
                
                }
            }
            if (explosion != null)
            {
                Destroy(explosion, explosionTime);
            }
            // Before destroying the bomb object, it should play a explosion animation or particle effect
            Destroy(gameObject);
        }
    }

    internal void AddComponent<T>(float attackRange)
    {
        throw new NotImplementedException();
    }
}
