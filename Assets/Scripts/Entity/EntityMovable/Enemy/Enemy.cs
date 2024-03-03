using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;
using Unity.VisualScripting;

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
    protected bool shallChasePlayer;
    //protected Potion potionDropped;

    protected float nextWayPointDistance = 3f;
    private Path path;
    private int currentWayPoint = 0;
    private Vector2 target;

    private Seeker seeker;

    private void Start()
    {
        onStart();
    }
    override protected void onStart()
    {
        base.onStart();
        shallPatrolRight = (1 == UnityEngine.Random.Range(0, 2));
        shallChasePlayer = false;

        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdatePath", 0f, 0.8f);
        //Only a example
        //moneyDropped = Damage * Life * DifficultyOfTheGame * Attacks.size()/2 
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rb.position, target, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
    private void Update()
    {  
       rb.velocity = tempVelocity;
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        CollisionCheck();
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

        if (path == null) return;
        
        //Reached the end of the path calculated at the moment
        if(currentWayPoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        if(Mathf.Abs(direction.x) > 0.3f)
        {
            tempVelocity.x += (Mathf.Abs(tempVelocity.x) < maxSpeed * Time.deltaTime) ? Mathf.Sign(direction.x) * acceleration * Time.deltaTime : 0;

            if(Mathf.Sign(direction.x) != Mathf.Sign(tempVelocity.x) && direction.x != 0f)
            {
                tempVelocity.x += -tempVelocity.x;
            }
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if(distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
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
        isJumping = !Physics2D.Raycast(
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
        }
    }
}
