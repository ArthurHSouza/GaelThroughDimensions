using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SprinterWolf : Enemy
{
    [Header("Attack Information")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointRange;
    [SerializeField] private float speedUp;
    private RunningAttack runAttack;

    private void Start()
    {
        onStart();
    }
    override protected void onStart()
    {
        base.onStart();
        runAttack = new RunningAttack();
    }
    private void Update()
    {
        rb.velocity = tempVelocity;
    }
    private void FixedUpdate()
    {
        CollisionCheck();
        ChaseCheck();
        if (!canMove) return;
        if(!shallChasePlayer)
        {
            Walk();
        }
        else
        {
            runAttack.Attack(
                attackPoint, 
                attackPointRange, 
                playerMask, 
                damage*strength, 
                ref tempVelocity,
                (player.GetComponent<Rigidbody2D>().position.x < rb.position.x) ? -speedUp : speedUp
                );
        }
        //Only do the jumpCheck if is patrolling or is chasing but isn't seeing the player
        if(!shallChasePlayer || memoryChaseTimeCouter.HasValue)
        {
            JumpCheck();
            Jump();
        }
        Gravity();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackPointRange);
    }
}