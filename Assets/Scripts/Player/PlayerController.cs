using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Attributes")]
    [SerializeField] public float damage;
    [SerializeField] public float health;

    [Header("Mobility")]
    [SerializeField] public Vector2 tempVelocity; //make changes to this velocity, PLEASE don`t use the one in Rigidbody directly
    //you can get the player attributes by using tempVelocity.x for example
    [SerializeField] public float acceleration;
    [SerializeField] public float maxSpeed;

    [Header("Jump")]
    [SerializeField] public float jumpForce;
    [SerializeField] private float gravity;
    private bool isJumping;


    [Header("Collision")]
    [SerializeField] LayerMask playerLayer;
    public bool isGrounded { get; private set; }
    private CapsuleCollider2D playerCollider;

    //Input
    private float horizontalInput;
    private float verticalInput;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
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

    private void Walk() {
        if (horizontalInput != 0) { //accelerates the player accordingly to the input
            tempVelocity.x += Mathf.Abs(tempVelocity.x) < maxSpeed ? horizontalInput * acceleration : 0; 
        } 
        if(Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x) || horizontalInput == 0) { //makes the player stop, friction
            tempVelocity.x += Mathf.Abs(tempVelocity.x) > 0 ? -tempVelocity.x / 10 : 0; 
        } 

    }

    private void CollisionCheck() {
        isGrounded = Physics2D.CapsuleCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(0.2f, 0f, 0f)
            , playerCollider.direction, 0, Vector2.down, 0.1f, ~playerLayer); //hits sends an capsule cast a little bit smaller than the player
        //it`s a little smaller to prevent collision problems
    }
    private void Jump() {
        if(isJumping && isGrounded) tempVelocity.y = jumpForce;
    }
    private void Gravity() {
        tempVelocity.y -= !isGrounded && tempVelocity.y > -maxSpeed? gravity : 0; //the comparison with maxSpeed adds a terminal velocity, change it for a specific variable later if needed
        if (isGrounded && !isJumping) tempVelocity.y = 0;
    }
    private void PlayerInput(){
        //horizontal and Vertical Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //jump
        isJumping = Input.GetKey(KeyCode.Space);
    }
}
