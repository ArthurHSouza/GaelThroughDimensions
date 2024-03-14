using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPCrow : Enemy
{
    [Header ("Target Properties")]
    [SerializeField] private float playerDetectionRange;
    [SerializeField] private float playerDetectionOffset;
    [SerializeField] private LayerMask playerMaskTarget;
    [SerializeField] private float? memoryChaseTimeCouterTarget = null;
    [SerializeField] private float memoryChaseTimeLimitTarget = 4f;
    [SerializeField] private const string PLAYER_TAG = "Player";

    [Header("Attack Properties")]
    [SerializeField] private float attackCooldown;
    private float attackCooldownCounter;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackKnockback;
    [SerializeField] private GameObject attackObj;


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
    }

    protected override void ChaseCheck()
    {
        bool playerViewCast = Physics2D.CapsuleCast(
            entityCollider.bounds.center,
            entityCollider.bounds.size,
            entityCollider.direction,
            0f,
            Vector2.down,
            playerDetectionRange,
            playerMaskTarget) || Physics2D.CapsuleCast(
            entityCollider.bounds.center,
            entityCollider.bounds.size,
            entityCollider.direction,
            0f,
            Vector2.right * Mathf.Sign(tempVelocity.x),
            playerDetectionRange,
            playerMaskTarget);

        if (shallChasePlayer && !playerViewCast)
        {
            if (!memoryChaseTimeCouterTarget.HasValue)
            {
                memoryChaseTimeCouterTarget = 0;
            }
            else if (memoryChaseTimeCouterTarget.Value >= memoryChaseTimeLimitTarget)
            {
                shallChasePlayer = false;
            }
            else
            {
                if(player.transform.position.y + playerDetectionOffset < transform.position.y)
                {
                    memoryChaseTimeCouterTarget += Time.deltaTime;
                }
                else
                {
                    memoryChaseTimeCouterTarget = null;
                    shallChasePlayer = false;
                    shallWaitToPatrol = true;
                    timeWhenReachedPP = 0;
                }
                
            }
        }
        else if (playerViewCast)
        {
            memoryChaseTimeCouterTarget = null;
            shallChasePlayer = true;
        }

        //Debug

        Debug.DrawLine(
         entityCollider.bounds.center,
         new Vector3(
             entityCollider.bounds.center.x,
             entityCollider.bounds.center.y - playerDetectionRange,
             entityCollider.bounds.center.z
             ),
         Color.green);
        Debug.DrawLine(
         entityCollider.bounds.center,
         new Vector3(
             entityCollider.bounds.center.x + playerDetectionRange * Mathf.Sign(tempVelocity.x),
             entityCollider.bounds.center.y,
             entityCollider.bounds.center.z
             ),
         Color.green);
    }

    private void ShootBomb()
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
                shootObj.GetComponent<BombObj>().SetBombRange(attackRange);
                shootObj.GetComponent<BombObj>().SetIgnoredName(this.gameObject.name);
                attackCooldownCounter = 0f;
            }
        }
        else
        {
            attackCooldownCounter = 0f;
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

        float direction = (target.x > rb.position.x) ? 1 : -1;

        tempVelocity.x += (Mathf.Abs(tempVelocity.x) < maxSpeed * Time.deltaTime || Mathf.Sign(tempVelocity.x) != Mathf.Sign(direction)) ?
            direction * acceleration * Time.deltaTime :
            0;
    }

}
