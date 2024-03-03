using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

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

    protected float nextWayPointDistance = 3f;
    private Path path;
    private int currentWayPoint = 0;
    private bool reachEndOfPath = false;
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
        
        if(currentWayPoint >= path.vectorPath.Count)
        {
            reachEndOfPath = true;
            return;
        }
        reachEndOfPath = false;

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;


        tempVelocity.x += (Mathf.Abs(tempVelocity.x) < maxSpeed * Time.deltaTime) ? Mathf.Sign(direction.x) * acceleration * Time.deltaTime : 0;

        if(Mathf.Sign(direction.x) != Mathf.Sign(tempVelocity.x) && direction.x != 0f)
        {
            tempVelocity.x += -tempVelocity.x;
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
