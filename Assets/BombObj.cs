using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObj : MonoBehaviour
{
    [SerializeField] private float bombRange = 2f;
    [SerializeField] private float explosionTime = 2f;
    private string ignoreName = "Enemy";
    [SerializeField] private GameObject explosionEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name != ignoreName)
        {
            Explode();
        }
        

    }

    public void SetIgnoredName(string name)
    {
        ignoreName = name;
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
