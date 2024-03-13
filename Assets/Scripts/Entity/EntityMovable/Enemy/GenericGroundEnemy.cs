using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A generic enemy that only moves on the ground and attack the player
public class GenericGroundEnemy : Enemy
{
    [SerializeField] Animator animator; 

    private void Start()
    {
        onStart();
    }
    protected override void onStart()
    {
        base.onStart();
    }
    private void Update()
    {
        rb.velocity = tempVelocity;
        if(Mathf.Abs(tempVelocity.x) > 0.1f){
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        CollisionCheck();
        ChaseCheck();
        if (!canMove) return;
        Walk();
        JumpCheck();
        Jump();
        Gravity();
    }

    override public void TakeDamage(float damage) {
        health -= damage;
        animator.SetTrigger("Hurt");

        if(health <= 0 ) {
            Die();
        }
    }

    public override void Die()
    {
        //Die Animation
        animator.SetBool("IsDead", true);
        
        //Disable the enemy
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
