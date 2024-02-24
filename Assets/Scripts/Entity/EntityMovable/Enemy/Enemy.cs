using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : EntityMovable 
{
    [Header("Patrol Points")]
    [SerializeField] protected GameObject [] patrolPoints;
    protected byte moneyDropped;
    protected bool shallPatrolRight;
    //protected Potion potionDropped;

    private void Start()
    {
        onStart();
    }
    override protected void onStart()
    {
        base.onStart();
        shallPatrolRight = (1 == UnityEngine.Random.Range(0, 2));
        //Only a example
        //moneyDropped = Damage * Life * DifficultyOfTheGame * Attacks.size()/2 
    }
    private void Update()
    {
        rb.velocity = tempVelocity;
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        Walk();
        Jump();
        Gravity();
    }

    protected override void Walk()
    {
        if(shallPatrolRight)
        {
            tempVelocity.x = maxSpeed;
        }
        else
        {
            tempVelocity.x = -maxSpeed;
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PatrolPointTag"))
        {
            shallPatrolRight = !shallPatrolRight;
        }
    }
}
