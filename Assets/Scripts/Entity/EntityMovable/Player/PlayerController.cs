using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityMovable
{
    //Input
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        onStart();
    }
    void Update()
    {
        PlayerInput();
        CollisionCheck();
        rb.velocity = tempVelocity; //THIS SHOULD ALWAYS BE THE LAST LINE!!!!!
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        Walk();
        Jump();
        Gravity();
    }

    override protected void Walk() {
        if (horizontalInput != 0) { //accelerates the player accordingly to the input
            tempVelocity.x += Mathf.Abs(tempVelocity.x) < maxSpeed ? horizontalInput * acceleration : 0; 
        } 
        if(Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x) || horizontalInput == 0) { //makes the player stop, friction
            tempVelocity.x += Mathf.Abs(tempVelocity.x) > 0 ? -tempVelocity.x / 10 : 0; 
        } 

    }

    private void CollisionCheck() {
        isGrounded = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size - new Vector3(0.2f, 0f, 0f)
            , entityCollider.direction, 0, Vector2.down, 0.1f, ~entityLayer); //hits sends an capsule cast a little bit smaller than the player
        //it`s a little smaller to prevent collision problems
    }

    private void PlayerInput(){
        //horizontal and Vertical Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //jump
        isJumping = Input.GetKey(KeyCode.Space);
    }
}
