using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericFlyingEnemy : FlyingEnemy
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
        Fly();
    }
}
