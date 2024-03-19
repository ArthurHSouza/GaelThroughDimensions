using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;
using Unity.VisualScripting;

//A abstract enemy that moves only at the ground and in the air
public abstract class FlyingEnemy : Enemy
{
    protected float nextWayPointDistance = 3f;

    private Path path;
    private int currentWayPoint = 0;
    private Seeker seeker;

    override protected void onStart()
    {
        base.onStart();

        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 0.8f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    virtual protected void Fly()
    {
        if (path == null) return;
        if (shallChasePlayer)
        {
            shallWaitToPatrol = false;
            Chase();
        }
        else
        {
            if (!shallWaitToPatrol)
                Patrol();
            else if (timeWhenReachedPP + waitPatrolTime < Time.time)
                shallWaitToPatrol = false;
            else
            {
                tempVelocity = new Vector2();
                return;
            }

        }


        //Reached the end of the path calculated at the moment
        if (currentWayPoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        tempVelocity = direction * maxSpeed * Time.deltaTime;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }
}