using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : EntityMovable 
{
    [Header("Patrol Points")]
    [SerializeField] protected GameObject [] patrolPoints;
    protected bool shallPatrolRight;
    protected byte moneyDropped;
    //protected Potion potionDropped;

    private void Start()
    {
        onStart();
    }
    override protected void onStart()
    {
        base.onStart();
        shallPatrolRight = true;// (1 == UnityEngine.Random.Range(0, 2));
        //Only a example
        //moneyDropped = Damage * Life * DifficultyOfTheGame * Attacks.size()/2 
    }
    private void Update()
    {
       CollisionCheck();
       
        rb.velocity = tempVelocity;
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        Walk();
        JumpCheck();
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

    virtual protected void JumpCheck()
    {
        isJumping = Physics2D.Raycast(
            new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.center.y * 1.2f),
            Vector2.right * Mathf.Sign(tempVelocity.x),
            1f,
            ~entityLayer & ~Physics2D.IgnoreRaycastLayer
        );

        //Debug
        Debug.DrawLine(
            new Vector3(
                entityCollider.bounds.center.x,
                entityCollider.bounds.center.y * 1.2f,
                entityCollider.bounds.center.z
                ),
            new Vector3(
                entityCollider.bounds.center.x + 1f * Mathf.Sign(tempVelocity.x),
                entityCollider.bounds.center.y * 1.2f,
                entityCollider.bounds.center.z
                ),
            Color.red);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == patrolPoints[0].GetComponent<Collider2D>() || collision == patrolPoints[1].GetComponent<Collider2D>())
        {
            shallPatrolRight = !shallPatrolRight;
        }
    }
}
