using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerRB;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRB = GetComponentInParent<Rigidbody2D>();
    }
    private void LateUpdate()
    {
        CheckDirection();
    }
    private void CheckDirection() {
        spriteRenderer.flipX = playerRB.velocity.x > 0? true : false;

    }
}
