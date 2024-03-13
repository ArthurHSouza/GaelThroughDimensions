using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Basic Attributes")]
    [SerializeField] public float maxHealth;
    protected float health;

    protected CapsuleCollider2D entityCollider;
    protected virtual void onStart()
    {
        health = maxHealth;
        entityCollider = GetComponent<CapsuleCollider2D>();
    }

    public abstract void TakeDamage(float damage);

    public abstract void Die();
}
