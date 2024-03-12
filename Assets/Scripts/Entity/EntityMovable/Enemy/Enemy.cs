using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

//A abstract enemy that moves only at the ground
public class Enemy : EntityMovable 
{
    [Header("Patrol Points")]
    [SerializeField] protected GameObject [] patrolPoints;
    [Header("Wait time after reach patrol point")]
    [SerializeField] [Range(1,10)] protected float waitPatrolTime = 1f;
    protected float timeWhenReachedPP = 0f;
    protected bool shallPatrolRight;
    protected bool shallWaitToPatrol = false;
    

    private byte moneyDropped;
    
    [Header("Chase Information")]
    [SerializeField] protected GameObject player;
    [SerializeField] protected LayerMask playerMask;
    [SerializeField] private float memoryChaseTimeLimit = 4f;
    [SerializeField] private float detectionPlayerRange = 5f; 
    private float? memoryChaseTimeCouter = null;
    protected bool shallChasePlayer;
    public bool canMove { get; set; } = true;
    //protected Potion potionDropped;
    protected Vector2 target;

    override protected void onStart()
    {
        base.onStart();
        shallPatrolRight = (1 == UnityEngine.Random.Range(0, 2));
        shallChasePlayer = false;

        //Only a example
        //moneyDropped = Damage * Life * DifficultyOfTheGame * Attacks.size()/2 
    }

    virtual protected void ChaseCheck()
    {
        bool playerViewCast = Physics2D.CapsuleCast(
            entityCollider.bounds.center, 
            entityCollider.bounds.size, 
            entityCollider.direction, 
            0f, 
            Vector2.right * Mathf.Sign(tempVelocity.x),
            detectionPlayerRange,
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

        Debug.DrawLine(
         entityCollider.bounds.center,
         new Vector3(
             entityCollider.bounds.center.x + detectionPlayerRange * Mathf.Sign(tempVelocity.x),
             entityCollider.bounds.center.y,
             entityCollider.bounds.center.z
             ),
         Color.green);
    }
    override protected void Walk()
    {
        if (shallChasePlayer)
        {
            shallWaitToPatrol = false;
            Chase();
        }
        else
        {
            if(!shallWaitToPatrol)
                Patrol();
            else if(timeWhenReachedPP + waitPatrolTime < Time.time)
                shallWaitToPatrol = false;
            else
            {
                tempVelocity = new Vector2();
                return;
            }
                
        }

        float direction = (target.x > rb.position.x) ? 1 : -1;

        tempVelocity.x += (Mathf.Abs(tempVelocity.x) < maxSpeed * Time.deltaTime || Mathf.Sign(tempVelocity.x) != Mathf.Sign(direction)) ? 
            direction * acceleration * Time.deltaTime : 
            0;
    }

    virtual protected void Patrol()
    {
        if (shallPatrolRight)
        {
            target = patrolPoints[1].GetComponent<Transform>().position;
        }
        else
        {
            target = patrolPoints[0].GetComponent<Transform>().position;
        }
    }

    virtual protected void Chase()
    {
        target = player.GetComponent<Transform>().position;
    }

    virtual protected void JumpCheck()
    {
        //Verifing if something will block his feet
        isJumping = Physics2D.Raycast(
            new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.center.y * 1.3f),
            Vector2.right * Mathf.Sign(tempVelocity.x),
            1f,
            ~entityLayer & ~playerMask & ~Physics2D.IgnoreRaycastLayer
        );

        //Debug
        Debug.DrawLine(
            new Vector3(
                entityCollider.bounds.center.x,
                entityCollider.bounds.center.y * 1.3f,
                entityCollider.bounds.center.z
                ),
            new Vector3(
                entityCollider.bounds.center.x + 1f * Mathf.Sign(tempVelocity.x),
                entityCollider.bounds.center.y * 1.3f,
                entityCollider.bounds.center.z
                ),
            Color.red);
        //

        //if blocked than nothing must be on his ahead of his head
        if (isJumping)
        {
            isJumping = !Physics2D.Raycast(
            new Vector2(entityCollider.bounds.center.x, entityCollider.bounds.max.y),
            Vector2.right * Mathf.Sign(tempVelocity.x),
            1f,
            ~entityLayer & ~playerMask & ~Physics2D.IgnoreRaycastLayer
            );
            return;
        }

        //Verifing if have a gap on the ground
        isJumping = isGrounded &&
            !Physics2D.Raycast(
            new Vector2(entityCollider.bounds.center.x + entityCollider.size.x * 1.1f * Mathf.Sign(tempVelocity.x), entityCollider.bounds.min.y),
            Vector2.down,
            5f,
            ~entityLayer & ~playerMask & ~Physics2D.IgnoreRaycastLayer
        );
        
        //Debug
        Debug.DrawLine(
        new Vector3(
                entityCollider.bounds.center.x + entityCollider.size.x * 1.1f * Mathf.Sign(tempVelocity.x),
                entityCollider.bounds.min.y,
                entityCollider.bounds.center.z
                ),
            new Vector3(
                entityCollider.bounds.center.x + entityCollider.size.x * 1.1f * Mathf.Sign(tempVelocity.x),
                entityCollider.bounds.min.y - 5f,
                entityCollider.bounds.center.z
                ),
            Color.red);
        //
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision == patrolPoints[0].GetComponent<Collider2D>() && !shallPatrolRight) || 
            collision == patrolPoints[1].GetComponent<Collider2D>() && shallPatrolRight)
        {
            shallPatrolRight = !shallPatrolRight;
            shallWaitToPatrol = true;
            timeWhenReachedPP = Time.time;
        }
    }
}
