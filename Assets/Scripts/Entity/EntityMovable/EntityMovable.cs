using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMovable : Entity
{

    [SerializeField] public float strenght;

    [Header("Mobility")]
    [SerializeField] public Vector2 tempVelocity; //make changes to this velocity, PLEASE don`t use the one in Rigidbody directly
    //you can get the player attributes by using tempVelocity.x for example
    [SerializeField] public float acceleration;
    [SerializeField] public float maxSpeed;

    [Header("Jump")]
    [SerializeField] public float jumpForce;
    [SerializeField] protected float gravity;
    protected bool isJumping;

    [Header("Collision")]
    [SerializeField] protected LayerMask entityLayer;

    public bool isGrounded { get; protected set; }
    //ArrayList<Buff>
    //ArrayList<Attack>;
    protected Rigidbody2D rb;

    bool isFliped = false;
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
        if (isJumping && isGrounded) tempVelocity.y = jumpForce;
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
        if ((tempVelocity.x < 0 && !isFliped) || (tempVelocity.x > 0 && isFliped))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
            isFliped = !isFliped;
        }
    }
}
