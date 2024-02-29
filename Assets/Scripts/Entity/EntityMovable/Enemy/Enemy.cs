using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : EntityMovable 
{
    [Header("Patrol Points")]
    [SerializeField] protected GameObject [] patrolPoints;
    protected bool shallPatrolRight;
    private byte moneyDropped;
    [Header("Chase Information")]
    [SerializeField] protected GameObject player;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float memoryChaseTimeLimit = 4f;
    private float? memoryChaseTimeCouter = null;
    bool shallChasePlayer;
    //protected Potion potionDropped;
    
    private void Start()
    {
        onStart();
    }
    override protected void onStart()
    {
        base.onStart();
        shallPatrolRight = (1 == UnityEngine.Random.Range(0, 2));
        shallChasePlayer = false;
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
        ChaseCheck();
        Walk();
        JumpCheck();
        Jump();
        Gravity();
    }

    virtual protected void ChaseCheck()
    {
        bool playerViewCast = Physics2D.CapsuleCast(
            entityCollider.bounds.center, 
            entityCollider.bounds.size, 
            entityCollider.direction, 
            0f, 
            Vector2.right * Mathf.Sign(tempVelocity.x),
            5f,
            playerMask);
        
        if(shallChasePlayer && !playerViewCast)
        {
            if(!memoryChaseTimeCouter.HasValue)
            {
                memoryChaseTimeCouter = 0;
            }
            else if (memoryChaseTimeCouter.Value >= memoryChaseTimeLimit)
            {
                shallChasePlayer = false;
            }
            else
            {
                memoryChaseTimeCouter += Time.deltaTime;
            }
        }
        else if (playerViewCast)
        {
            memoryChaseTimeCouter = null;
            shallChasePlayer = true;
        }

        //Debug

        if(memoryChaseTimeCouter.HasValue)
        {
            Debug.Log(memoryChaseTimeCouter);
        }


        Debug.DrawLine(
         entityCollider.bounds.center,
         new Vector3(
             entityCollider.bounds.center.x + 5f * Mathf.Sign(tempVelocity.x),
             entityCollider.bounds.center.y,
             entityCollider.bounds.center.z
             ),
         Color.green);
    }

    override protected void Walk()
    {
        if (shallChasePlayer)
            Chase();
        else
            Patrol();
    }

    virtual protected void Patrol()
    {
        if (shallPatrolRight)
        {
            tempVelocity.x = maxSpeed;
        }
        else
        {
            tempVelocity.x = -maxSpeed;
        }
    }

    virtual protected void Chase()
    {
        if (Mathf.Approximately(player.transform.position.x, rb.transform.position.x))
            return;
        tempVelocity.x = maxSpeed * ((player.transform.position.x > rb.transform.position.x) ? 1 : -1);
    }

    virtual protected void JumpCheck()
    {
        isJumping = Physics2D.Raycast(
            new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.center.y * 1.2f),
            Vector2.right * Mathf.Sign(tempVelocity.x),
            1f,
            ~entityLayer & ~playerMask & ~Physics2D.IgnoreRaycastLayer
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
        if ((collision == patrolPoints[0].GetComponent<Collider2D>() && !shallPatrolRight) || 
            collision == patrolPoints[1].GetComponent<Collider2D>() && shallPatrolRight)
        {
            shallPatrolRight = !shallPatrolRight;
        }
    }
}
