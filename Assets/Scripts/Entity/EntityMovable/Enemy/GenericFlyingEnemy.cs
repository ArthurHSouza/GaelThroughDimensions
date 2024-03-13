using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericFlyingEnemy : FlyingEnemy
{
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
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        CollisionCheck();
        ChaseCheck();
        if (!canMove) return;
        Fly();
    }

    override public void TakeDamage(float damage) {
        health -= damage;

        if(health <= 0 ) {
            Die();
        }
    }

    public override void Die()
    {
        //Die Animation

        //Disable the entity
    }
}
