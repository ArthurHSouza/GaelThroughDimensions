using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A generic enemy that only moves on the ground and attack the player
public class GenericGroundEnemy : Enemy
{
    private void Start()
    {
        onStart();
    }
    override protected void onStart()
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
        Walk();
        JumpCheck();
        Jump();
        Gravity();
    }
}
