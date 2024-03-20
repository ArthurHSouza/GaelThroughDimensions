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
    [SerializeField] 
    private LayerMask oneWayPlatforms;
    [SerializeField][Tooltip("Max  negative velocity that a player can achieve while falling")] 
    private float terminalVelocity;
    [SerializeField][Range(1, 1.5f)] 
    private float airAcceleration;
    [SerializeField][Tooltip("More is less, because it is a division")] 
    private float airResistance;
    [SerializeField][Tooltip("Time to jump after leaving a platform")] 
    private float coyoteTime;
    [SerializeField][Tooltip("Amount of seconds before hitting the ground that activates jump")] 
    private float jumpBufferDuration;
    [SerializeField][Tooltip("How strong the player goes back to the ground after releasing the jump button")] 
    private float variableJumpHeightStrength;
    [SerializeField][Tooltip("The seconds in the air to help the player to control")] 
    private float apexModifierDuration;
    [SerializeField][Tooltip("This is the movement needed in the peak of the jump to trigger apex modifiers")] 
    private float apexModifierTolerance;
    [SerializeField][Range(1f, 1.1f)] 
    private float apexModifierSpeedBoost;
    private float jumpBuffer = -5; //to prevent an accidental jump at the start
    private bool apexActive;
    private bool hittedCeiling;
    private bool doubleTapped; //this is for getting down one way platforms
    private GameObject oneWayPlatformDescended;
    private bool lastCollisionWasOneWay;
    private bool isInsideOneWayPlatforms;

    //Mobility
    private float originalMaxSpeed;

    //Dash
    [Header("Dash")]
    [SerializeField] private bool dashEnabled;
    private Dash dash;
    private int lastDirection;
    private bool pressedDash;
    private bool canDash;

    //Hook
    [Header("Hook")]
    [SerializeField] private bool hookEnabled;
    [SerializeField] private float jumpMultiplierAfterRope;
    [SerializeField] private float hookCooldown; //you still need to touch the ground to restore hook, this is to prevent pulling objects really fast
    private float timeSinceLastHook;
    private Hook hook;
    private bool pressedHook;
    private bool canHook;

    //Plunging Attack
    [Header("Plunging Attack")]
    [SerializeField] private bool plungingAttackEnabled;
    private PlungingAttack plgAtt;
    private bool plungingCondition;

    //Wall Jump
    [Header("Wall Jump")]
    [SerializeField] private bool wallJumpEnabled;
    [SerializeField] private float wallJumpBoost;
    [SerializeField] private float wallJumpBoostDuration;
    private bool hittedWallLeft;
    private bool hittedWallRight;
    private bool isWallJumping;
    private float wallJumpSpeed;
    private string lastJumpedWall;
    private bool applyWallPenalty;

    //Double Jump
    [Header("Double Jump")]
    [SerializeField] private bool doubleJumpEnabled;
    [SerializeField] private float  doubleJumpBoost;
    private bool canDoubleJump = true;

    //Combat
    private float damageInterval = 1f;
    private bool canDamage = true;


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
        CollisionCheck();
        JumpCheck();
        ApexModifiers();
        LastDirectionLooked();
        PlatformDescent();


        rb.velocity = tempVelocity; //THIS SHOULD ALWAYS BE THE LAST LINE!!!!!
    }
    private void FixedUpdate() //put all Physics related methods here
    {
        if (!dash.isDashing)
        {
            Walk();
            if (wallJumpEnabled)WallJump();
            PlayerJump();
            if(doubleJumpEnabled)DoubleJump();
            PlayerGravity();
            if(plungingAttackEnabled)PlungingAttack();
        }
        if(dashEnabled)Dash();
        if(hookEnabled)Hook();
        if (pressedJump) { ResetJumpAfterOneLateFrame(); }
    }

    private void LastDirectionLooked() {

        if (lastDirection == 0) lastDirection = 1; //initialization
        if (horizontalInput == 0 && !isGrounded && Mathf.Abs(rb.velocity.x) > 0) lastDirection = (int)Mathf.Sign(rb.velocity.x); //to fix player hooking
                                                                                                                                 //and changing direction
        if (horizontalInput > 0) lastDirection = 1;
        if (horizontalInput < 0) lastDirection = -1;
    }
    private void WallJump() {
        if ((hittedWallRight || hittedWallLeft) && !isGrounded) {
            //tempVelocity.y += Mathf.Abs(gravity/1.2f);//makes the player fall half slower but create a bug
            tempVelocity.x = 0;//stopping the player on the wall
            if (pressedJump) {
                tempVelocity.x = hittedWallLeft ? maxSpeed : -maxSpeed;
                isWallJumping = true;
                if ((lastJumpedWall == "right" && hittedWallRight) || (lastJumpedWall == "left" && hittedWallLeft)) applyWallPenalty = true;
                lastJumpedWall = hittedWallRight? "right":"left";
                wallJumpSpeed = tempVelocity.x;
                Invoke("DisableWallJumping",wallJumpBoostDuration);
            }
        }
        if (isWallJumping) {
            tempVelocity.y = applyWallPenalty? jumpForce / 2 * wallJumpBoost: jumpForce * wallJumpBoost;
            tempVelocity.x = wallJumpSpeed;//maintaining walljump speed
        }
        if (isGrounded) {
            applyWallPenalty = false;
            lastJumpedWall = "";
            isWallJumping = false;
        }
    }
    private void DisableWallJumping() {
        isWallJumping = false;
    }
    private void DoubleJump() {
        if (canDoubleJump && pressedJump) {
            canDoubleJump = false;
            isJumping = true;
            tempVelocity.y = jumpForce * doubleJumpBoost;
        }
        if (isGrounded) {
            canDoubleJump = true;
        }
    }
    private void Hook() {
        if (canHook && pressedHook && !plungingCondition && !plgAtt.isPlunging && !hook.isHooking && timeSinceLastHook > hookCooldown) {
            switch (hook.GetTargetObjectTag()) {
                case "HookSpot":
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    hook.GoToAndLaunch();
                    break;
                case "SwingSpot":
                    hook.RopeCheck();
                    break;
                case "Thrust":
                    hook.ThrustObject();
                    break;
                case "Pullable":
                    hook.PullObject();
                    break;
            }
            timeSinceLastHook = 0;
            canHook = false;
        }
        if (dash.isDashing && hook.isHooking){
            hook.cancelRope = true; //in case the player wants to dash off the rope
            hook.RopeManager();
            hook.StopHook();
        }
        if (pressedJump && isHoldingJump && !hook.cancelRope){
            hook.cancelRope = true; //in case the player wants to jump off the rope
            hook.RopeManager();
            tempVelocity.y = jumpForce * jumpMultiplierAfterRope;
        }
        if (isGrounded) {
            canHook = true;
        }
        if (!hook.isHooking) {
            timeSinceLastHook += Time.deltaTime;
        }
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
                tempVelocity.x += horizontalInput * acceleration / airResistance * 2; // to help it accelerate
            }
            else if (Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x)){
                //apply air deceleration when changing direction
                tempVelocity.x += horizontalInput * acceleration / airResistance;
            }
            if (!hook.cancelRope && horizontalInput == 0) tempVelocity.x = 0;//to help player stop at the rope swing
            tempVelocity.x = Mathf.Clamp(tempVelocity.x, -maxSpeed * airAcceleration, maxSpeed * airAcceleration);
        }
        if ((Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x) || horizontalInput == 0) && isGrounded) { //makes the player stop, friction
            tempVelocity.x += Mathf.Abs(tempVelocity.x) > 0 ? -tempVelocity.x  : 0;
        }

    }

    protected override void CollisionCheck() {
        isGrounded = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size - new Vector3(0.5f, 0f, 0f) // the 0.5 is because when
                                                                                                                                // flipping the gameobject the
                                                                                                                                // collider went inside the
                                                                                                                                // walls, causing this to
                                                                                                                                // activate
            , entityCollider.direction, 0, Vector2.down, 0.1f, ~entityLayer);
        

        //fixing oneway platforms problem
        float offset = entityCollider.bounds.size.y * 0.05f; // 5% of the player's height
        Vector2 startPoint = entityCollider.bounds.center + new Vector3(0, offset, 0);

        isInsideOneWayPlatforms = Physics2D.CapsuleCast(startPoint, entityCollider.bounds.size
                , entityCollider.direction, 0, Vector2.down, 0.1f, oneWayPlatforms);
        if (isInsideOneWayPlatforms) tempVelocity.y = jumpForce; //player keep going up if it is still inside the platform


        hittedCeiling = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size - new Vector3(0.2f, 0f, 0f)
            , entityCollider.direction, 0, Vector2.up, 0.1f, ~entityLayer & ~oneWayPlatforms); //to prevent jumps that take too long after player hitting
                                                                                               //the head

        hittedWallLeft = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size  
            , entityCollider.direction, 0, Vector2.left, 0.1f, ~entityLayer);
        hittedWallRight = Physics2D.CapsuleCast(entityCollider.bounds.center, entityCollider.bounds.size
           , entityCollider.direction, 0, Vector2.right, 0.1f, ~entityLayer);
        if (hittedCeiling) tempVelocity.y = Mathf.Min(0, tempVelocity.y);
    }

    private void ApexModifiers()
    {
        Vector2 magnitude = new Vector2(0, Mathf.Abs(tempVelocity.y));
        if (magnitude.magnitude < apexModifierTolerance && !isGrounded && tempVelocity.y > 0) { //if the player isnt moving much in the air anymore,
                                                                                                //they reached the peak
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
        if (isJumping){
            tempVelocity.y = jumpForce;   
        }
    }

    private void PlayerGravity() {
        tempVelocity.y -= !isGrounded && tempVelocity.y > -terminalVelocity ? gravity : 0;
        if (isGrounded && !isJumping) tempVelocity.y = 0;
    }

    private void PlungingAttack() {
        if (plungingCondition && !plgAtt.isPlunging) { plgAtt.Attack(); }
    }

    private void PlatformDescent() {
        if (lastCollisionWasOneWay && doubleTapped) {
            oneWayPlatformDescended.gameObject.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(renableOneWayPlatform(oneWayPlatformDescended));
        }
        else
        {
            doubleTapped = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((oneWayPlatforms & (1 << collision.gameObject.layer)) != 0)
        {
            if (oneWayPlatformDescended == null) oneWayPlatformDescended = collision.gameObject;
            lastCollisionWasOneWay = true;
        }
        else {
            lastCollisionWasOneWay = false;
        }
        
    }

    private IEnumerator renableOneWayPlatform(GameObject collision) {
        doubleTapped = false;
        yield return new WaitForSeconds(0.5f);
        collision.gameObject.GetComponent<Collider2D>().enabled = true;
        oneWayPlatformDescended = null;
    }

    //player input

    public void InputOneWayPlatformsDescendCheck(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            doubleTapped = true;
        }
    }
    public void InputJump(InputAction.CallbackContext context) {
        if (context.started) {
            pressedJump = true;
            isHoldingJump = true;
        }
        if (context.canceled)
        {
            isHoldingJump = false;
        }
    }
    private void ResetJumpAfterOneLateFrame() { pressedJump = false; } //basically fixes interactions between the input in the update and the methods in Late
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

    public override void TakeDamage(float damage) {
        if (canDamage) {
            health -= damage;
            animator.SetTrigger("Hurt");

            if (health <= 0) {
                Die();
            }
            canDamage = false;
            Invoke("ResetDamageTimer", damageInterval);
        }
    }

    private void ResetDamageTimer() {
        // Permite causar dano novamente
        canDamage = true;
    }
}
