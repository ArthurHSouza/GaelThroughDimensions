using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Basic Attributes")]
    [SerializeField] public float maxHealth;
    [SerializeField] protected Animator animator; 
    protected float health;

    protected CapsuleCollider2D entityCollider;
    protected virtual void onStart()
    {
        health = maxHealth;
        entityCollider = GetComponent<CapsuleCollider2D>();
    }

    public virtual void TakeDamage(float damage) {
        health -= damage;
        animator.SetTrigger("Hurt");

        if (health <= 0) {
            Die();
        }
    }

    // Entity dies
    protected virtual void Die() {
        //Die Animation
        animator.SetBool("isDead", true);
        
        //Disable the entity
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
