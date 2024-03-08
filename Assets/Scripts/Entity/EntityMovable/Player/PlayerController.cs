using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : EntityMovable
{
    //Input
    private float horizontalInput;
    private float verticalInput;
    private bool pressedJump;
    private bool isHoldingJump; //to help with variable jump heights

    //Jump
    private float lastTimeTouchedGround = -5;
    [Header("Jump Modifiers")]
    [SerializeField] private LayerMask oneWayPlatforms;
    [SerializeField][Tooltip("Max  negative velocity that a player can achieve while falling")] private float terminalVelocity;
    [SerializeField][Range(1, 1.5f)] private float airAcceleration;
    [SerializeField][Tooltip("More is less, because it is a division")] private float airResistence;
    [SerializeField][Tooltip("Time to jump after leaving a platform")] private float coyoteTime;
    [SerializeField][Tooltip("Amount of seconds before hitting the ground that activates jump")] private float jumpBufferDuration;
    [SerializeField][Tooltip("How strong the player goes back to the ground after releasing the jump button")] private float variableJumpHeightStrength;
    [SerializeField][Tooltip("The seconds in the air to help the player to control")] private float apexModifierDuration;
    [SerializeField][Tooltip("This is the movement needed in the peak of the jump to trigger apex modifiers")] private float apexModifierTolerance;
    [SerializeField][Range(1f, 1.1f)] private float apexModifierSpeedBoost;
    private float jumpBuffer = -5; //to prevent an accidental jump at the start
    private bool apexActive;
    private bool hittedCeiling;

    //Mobility
    private float originalMaxSpeed;

    //Dash
    private Dash dash;
    private int lastDirection;
    private bool pressedDash;
    private bool canDash;

    //Hook
    private Hook hook;
    private bool pressedHook;
    private bool canHook;

    //Plunging Attack
    private PlungingAttack plgAtt;
    private bool plungingCondition;

    void Start()
    {
        onStart();
        hook = GetComponent<Hook>();
        dash = GetComponent<Dash>();
        plgAtt = GetComponent<PlungingAttack>();
        originalMaxSpeed = maxSpeed;
    }
    void Update()
    {
        //PlayerInput();
        CollisionCheck();
        JumpCheck();
        ApexModifiers();
        LastDirectionLooked();


        rb.velocity = tempVelocity; //THIS SHOULD ALWAYS BE THE LAST LINE!!!!!
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        if (!dash.isDashing)
        {
            Walk();
            PlayerJump();
            PlayerGravity();
            PlungingAttack();
        }
        Hook();
        Dash();
    }

    private void LastDirectionLooked() {

        if (lastDirection == 0) lastDirection = 1; //initialization
        if (horizontalInput == 0 && !isGrounded && Mathf.Abs(rb.velocity.x) > 0) lastDirection = (int)Mathf.Sign(rb.velocity.x); // to fix player hooking and changing direction
        if (horizontalInput > 0) lastDirection = 1;
        if (horizontalInput < 0) lastDirection = -1;
    }

    private void Hook() {
        if (canHook && pressedHook && !plungingCondition && !plgAtt.isPlunging) {
            hook.GoToAndLaunch();
            canHook = false;
        }
        if (dash.isDashing && hook.isHooking){
            hook.StopHook();
        }
        if (isGrounded) {
            canHook = true;
        }
        //if (Input.GetKey(KeyCode.E)) GetComponent<Hook>().GoToObject();
        //if (Input.GetKey(KeyCode.R)) GetComponent<Hook>().PullObject();
    }

    private void Dash() {
        if (pressedDash && canDash) {
            dash.ActivateDash(lastDirection);
            canDash = false;
        }
        if(isGrounded) canDash = true;
    }

    override protected void Walk() {
        if (horizontalInput != 0 && isGrounded)
        { //accelerates the player accordingly to the input
            tempVelocity.x = Mathf.Abs(tempVelocity.x) < maxSpeed ? tempVelocity.x + (horizontalInput * acceleration) : maxSpeed * horizontalInput;
        }
        else if (!isGrounded){
            //speed on air
            float targetSpeed = Mathf.Abs(horizontalInput) * maxSpeed * airAcceleration;
            if (Mathf.Abs(tempVelocity.x) < targetSpeed){
                tempVelocity.x += horizontalInput * acceleration / airResistence * 2; // to help it accelerate
            }
            else if (Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x)){
                //apply air deceleration when changing direction
                tempVelocity.x += horizontalInput * acceleration / airResistence;
            }
            tempVelocity.x = Mathf.Clamp(tempVelocity.x, -maxSpeed * airAcceleration, maxSpeed * airAcceleration);
        }
        if ((Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x) || horizontalInput == 0) && isGrounded) { //makes the player stop, friction
            tempVelocity.x += Mathf.Abs(tempVelocity.x) > 0 ? -tempVelocity.x  : 0;
        }

    }

    private void CollisionCheck() {
        isGrounded = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size - new Vector3(0.5f, 0f, 0f) // the 0.5 is because when flipping the gameobject the collider went inside the walls, causing this to activate
            , entityCollider.direction, 0, Vector2.down, 0.1f, ~entityLayer); //hits sends an capsule cast a little bit smaller than the player
        //it`s a little smaller to prevent collision problems

        hittedCeiling = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size - new Vector3(0.2f, 0f, 0f)
            , entityCollider.direction, 0, Vector2.up, 0.1f, ~entityLayer & ~oneWayPlatforms); //to prevent jumps that take too long after player hitting the head
        if (hittedCeiling) tempVelocity.y = Mathf.Min(0, tempVelocity.y);

    }

    private void ApexModifiers()
    {
        Vector2 magnitude = new Vector2(0, Mathf.Abs(tempVelocity.y));
        if (magnitude.magnitude < apexModifierTolerance && !isGrounded && tempVelocity.y > 0) { //if the player isnt moving much in the air anymore, they reached the peak
            apexActive = true;
        }
        if (apexActive) {
            tempVelocity.y = 0;
            maxSpeed = originalMaxSpeed * apexModifierSpeedBoost;
            tempVelocity.x = tempVelocity.x * apexModifierSpeedBoost;
            Invoke("DisableApexModifiers", apexModifierDuration);

        }
    }
    private void DisableApexModifiers() {
        maxSpeed = originalMaxSpeed;
        apexActive = false;
    }

    private void JumpCheck() {
        if (isGrounded) lastTimeTouchedGround = Time.time; //checking the last frame that the player was on the ground
        if (pressedJump) jumpBuffer = Time.time; //checking the frame that the player pressed jump

        bool jumpBufferCondition = Time.time - jumpBuffer <= jumpBufferDuration;
        bool coyoteCondition = Time.time - lastTimeTouchedGround <= coyoteTime;

        if (jumpBufferCondition && coyoteCondition)
        {
            isJumping = true;
        }
        else if (!isGrounded) {
            isJumping = false;
        }
        if (!isHoldingJump && (!isGrounded || !coyoteCondition) && tempVelocity.y > 0) { // variable jump height
            tempVelocity.y -= jumpForce * variableJumpHeightStrength * Time.deltaTime; // decelerates the player that isn't pressing jump button
        }
    }
    private void PlayerJump()
    {
        if (isJumping){tempVelocity.y = jumpForce;}
    }

    private void PlayerGravity() {
        tempVelocity.y -= !isGrounded && tempVelocity.y > -terminalVelocity ? gravity : 0;
        if (isGrounded && !isJumping) tempVelocity.y = 0;
    }

    private void PlungingAttack() {
        if (plungingCondition && !plgAtt.isPlunging) { plgAtt.Attack(); }
    }

    //player input

    public void InputJump(InputAction.CallbackContext context) {
        if (context.started) {
            pressedJump = true;
            isHoldingJump = true;
            StartCoroutine(ResetPressedJumpNextFrame());
        }
        if (context.canceled)
        {
            isHoldingJump = false;
        }
    }
    private IEnumerator ResetPressedJumpNextFrame(){
        yield return null; //wait for the end of the frame
        pressedJump = false;
    }
    public void InputPlungingAttack(InputAction.CallbackContext context){
        plungingCondition = context.performed && verticalInput < -0.5f;
    }
    public void InputHorizontalMovement(InputAction.CallbackContext context) { 
        horizontalInput = context.ReadValue<float>();
    }
    public void InputVerticallMovement(InputAction.CallbackContext context)
    {
        verticalInput = context.ReadValue<float>();
    }

    public void InputHook(InputAction.CallbackContext context)
    {
        pressedHook = context.performed;
    }
    public void InputDash(InputAction.CallbackContext context)
    {
        pressedDash = context.performed;
    }
    //old input system
    private void PlayerInput(){ //only the jump here because for some reason it doesn`t work with the new system
        //jump
        pressedJump = Input.GetKeyDown(KeyCode.Space);
        if (pressedJump)
        {
            isHoldingJump = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
        }

    }
}
