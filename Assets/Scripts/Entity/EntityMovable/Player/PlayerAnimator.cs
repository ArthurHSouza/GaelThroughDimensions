using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerRB;

    [SerializeField] Animator animator;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRB = GetComponentInParent<Rigidbody2D>();
    }

    private void Update() {
        IsMoving();
    }

    private void IsMoving() {
        if(Mathf.Abs(playerRB.velocity.x) > 0.1f) {
            animator.SetBool("isWalking", true);
        }
        else {
            animator.SetBool("isWalking", false);
        }
    }
}
