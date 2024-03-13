using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningAttack
{
    public void Attack(in Transform attackPoint, in float attackRange, in LayerMask playerLayer, 
        in float damageAplied, ref Vector2 velocity, in float speedUp, in float direction)
    {
        if (Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer))
            Debug.LogWarning($"Player shall recive damageAplied ({damageAplied}), but its not yet implemented");
        velocity.x = Mathf.Sin(direction) * speedUp * Time.deltaTime;
    }
}
