using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mobility")]
    [SerializeField] public Vector2 tempVelocity; //make changes to this velocity, PLEASE don`t use the one in Rigidbody directly
    //you can get the player attributes by using tempVelocity.x for example
    [SerializeField] public float acceleration;
    [SerializeField] public float maxSpeed;

    [Header("Jump")]
    [SerializeField] public float jumpForce;
    [SerializeField] public float gravity;

    //Input
    private float horizontalInput;
    private float verticalInput;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {

        PlayerInput();
        rb.velocity = tempVelocity; //THIS SHOULD ALWAYS BE THE LAST LINE!!!!!
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        Walk();
        if(verticalInput> 0)Jump();
        Gravity();
    }

    private void Walk() {
        tempVelocity.x += Mathf.Abs(tempVelocity.x) < maxSpeed ? horizontalInput * acceleration : 0; //accelerates the player accordingly to the input
        tempVelocity.x += Mathf.Abs(tempVelocity.x) > 0 ? -tempVelocity.x / 10 : 0; //makes the player stop, friction

    }

    private void Jump() {
        tempVelocity.y = jumpForce;
    }
    private void Gravity() {
        tempVelocity.y -= gravity;
    }
    private void PlayerInput(){
        //horizontal and Vertical Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //jump
    }
}
