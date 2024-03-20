using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask enemyLayers;

    [SerializeField] protected float attackRange = 0.5f;
    [SerializeField] protected float attackDamage = 20;

    void Update() {
        Attack();
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            //Player attack animation
            animator.SetTrigger("Attack");

            //Detect entities in range of attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            //Damages entities
            foreach(Collider2D enemy in hitEnemies) {
                enemy.GetComponent<Entity>().TakeDamage(attackDamage);
            }
        }
    }

    //Attack point of the melee weapon
    void OnDrawGizmosSelected() { 
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}