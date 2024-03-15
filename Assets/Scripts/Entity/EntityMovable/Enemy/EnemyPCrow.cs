using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPCrow : Enemy
{
    [Header ("Target Properties")]
    [SerializeField] private float playerDetectionOffset; // Offset to detect the player above the enemy
    [SerializeField] private const string PLAYER_TAG = "Player";
    [SerializeField] private float playerRangeOffset = 4f; // Offset to detect if the player is bellow the enemy
    [SerializeField] private float dizzyTime = 4f; // The time the enemy will be dizzy after the player gets above it
    private float curDizzyTime = 0f;
    private bool stopFlying = false; // Stops the enemy from flying when it's true

    [Header("Attack Properties")]
    [SerializeField] private float attackCooldown;
    private float attackCooldownCounter;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackKnockback;
    [SerializeField] private GameObject attackObj;
    private bool isDizzy = false;


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
        ShootBomb();
        Dizzy();

    }

    protected override void ChaseCheck()
    {
        bool playerViewCast = Physics2D.CapsuleCast(
            entityCollider.bounds.center,
            entityCollider.bounds.size,
            entityCollider.direction,
            0f,
            Vector2.down,
            detectionPlayerRange,
            playerMask) || Physics2D.CapsuleCast(
            entityCollider.bounds.center,
            entityCollider.bounds.size,
            entityCollider.direction,
            0f,
            Vector2.right * Mathf.Sign(tempVelocity.x),
            detectionPlayerRange,
            playerMask);



        if (shallChasePlayer && !playerViewCast)
        {
            if (!memoryChaseTimeCouter.HasValue)
            {
                memoryChaseTimeCouter = 0;
            }
            else if (memoryChaseTimeCouter.Value >= memoryChaseTimeLimit)
            {
                shallChasePlayer = false;
            }
        }
        else if (playerViewCast)
        {
            memoryChaseTimeCouter = null;
            shallChasePlayer = true;

            
        }


        
        

        if (shallChasePlayer)
        {
            bool playerRangeCast = Physics2D.CapsuleCast(
               entityCollider.bounds.center,
               entityCollider.bounds.size/playerRangeOffset,
               entityCollider.direction,
               0f,
               Vector2.down,
               detectionPlayerRange,
               playerMask);

            if (player.transform.position.y + playerDetectionOffset >= transform.position.y)
            {
                if (!isDizzy)
                {
                    isDizzy = true;
                }
            }

            if (playerRangeCast)
            {
                    stopFlying = true;
            }
            else
            {
                if(!isDizzy)
                {
                    stopFlying = false;

                }
            }

            
        }

        //Debug

        Debug.DrawLine(
         entityCollider.bounds.center,
         new Vector3(
             entityCollider.bounds.center.x,
             entityCollider.bounds.center.y - detectionPlayerRange,
             entityCollider.bounds.center.z
             ),
         Color.green);
        Debug.DrawLine(
         entityCollider.bounds.center,
         new Vector3(
             entityCollider.bounds.center.x + detectionPlayerRange * Mathf.Sign(tempVelocity.x),
             entityCollider.bounds.center.y,
             entityCollider.bounds.center.z
             ),
         Color.green);
    }

    private void Dizzy()
    {
        if(isDizzy == true)
        {
            Debug.Log("Dizzy");
            stopFlying = true;
            curDizzyTime += Time.fixedDeltaTime;

            if (curDizzyTime >= dizzyTime)
            {
                shallChasePlayer = false;
                stopFlying = false;
                isDizzy = false;
                curDizzyTime = 0f;
            }
        }
        
    }

    private void ShootBomb()
    {
        if (!isDizzy)
        {
            if (shallChasePlayer)
            {
                if (attackCooldownCounter < attackCooldown)
                {
                    attackCooldownCounter += Time.deltaTime;
                }
                else
                {
                    GameObject shootObj = Instantiate(attackObj, rb.position, Quaternion.identity);
                    shootObj.AddComponent<BombObj>().Init(attackRange);
                    attackCooldownCounter = 0f;
                }
            }
            else
            {
                attackCooldownCounter = 0f;
            }
        }
    }


    private void Fly()
    {
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

        if (stopFlying == false)
        {
            float direction = (target.x > rb.position.x) ? 1 : -1;

            tempVelocity.x += (Mathf.Abs(tempVelocity.x) < maxSpeed * Time.deltaTime || Mathf.Sign(tempVelocity.x) != Mathf.Sign(direction)) ?
                direction * acceleration * Time.deltaTime :
                0;
        }else
        {
            tempVelocity.x = 0;
        }
    }

}
