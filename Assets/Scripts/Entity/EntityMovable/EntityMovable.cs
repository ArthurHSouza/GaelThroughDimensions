using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMovable : Entity
{

    [SerializeField] public float strength;

    [Header("Mobility")]
    [SerializeField] public Vector2 tempVelocity; //make changes to this velocity, PLEASE don`t use the one in Rigidbody directly
    //you can get the player attributes by using tempVelocity.x for example
    [SerializeField] public float acceleration;
    [SerializeField] public float maxSpeed = 1;

    [Header("Jump")]
    [SerializeField] public float jumpForce = 1;
    [SerializeField] protected float gravity = 1;
    protected bool isJumping;

    [Header("Collision")]
    [SerializeField] protected LayerMask entityLayer;

    public bool isGrounded { get; protected set; }
    //ArrayList<Buff>
    //ArrayList<Attack>;
    protected Rigidbody2D rb;

    bool isFlipped = false;
    protected override void onStart()
    {
        base.onStart();
        rb = GetComponent<Rigidbody2D>();

        if(entityLayer == LayerMask.GetMask())
        {
            Debug.LogWarning("Missing the entity layer, please select a valid one at the component window");
        }
    }

    protected abstract void Walk();

    protected void Jump()
    {
        if (isJumping && isGrounded)
        {
            tempVelocity.y = jumpForce;
        }
    }
    protected void Gravity()
    {
        //the comparison with maxSpeed adds a terminal velocity, change it for a specific variable later if needed
        tempVelocity.y -= !isGrounded && tempVelocity.y > -maxSpeed ? gravity : 0; 
        if (isGrounded && !isJumping) tempVelocity.y = 0;
    }

    private void LateUpdate()
    {
        SideCheck();
    }

    private void SideCheck()
    {
        if ((tempVelocity.x < 0 && !isFlipped) || (tempVelocity.x > 0 && isFlipped))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
            isFlipped = !isFlipped;
        }
    }

    protected void CollisionCheck()
    {
        isGrounded = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size - new Vector3(0.2f, 0f, 0f)
            , entityCollider.direction, 0, Vector2.down, 0.1f, ~entityLayer & ~Physics2D.IgnoreRaycastLayer); //hits sends an capsule cast a little bit smaller than the player
        //it`s a little smaller to prevent collision problems
    }
}
