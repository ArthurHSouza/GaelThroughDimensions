using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityMovable
{
    //Input
    private float horizontalInput;
    private float verticalInput;

    [Header("Item Properties")]
    [Range(1.0f,100.0f)]
    public float itemPickableRadius = 10.0f;
    [Range(1.0f,20.0f)]
    public float forceMultiplyer = 5.0f;

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
        CollectablesVerifier();
    }

    void OnDrawGizmos()
    {
        // APENAS PARA DEBUG
        // Ciculo de itens coletáveis
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, itemPickableRadius);
    }

    void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if(otherCollider.transform.CompareTag("Item"))
        {
            Debug.Log($"{otherCollider.transform.name} deve ser adicionado no inventário");
            Destroy(otherCollider.gameObject);
        }
    }

    override protected void Walk() {
        if (horizontalInput != 0) { //accelerates the player accordingly to the input
            tempVelocity.x += Mathf.Abs(tempVelocity.x) < maxSpeed ? horizontalInput * acceleration : 0; 
        } 
        if(Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x) || horizontalInput == 0) { //makes the player stop, friction
            tempVelocity.x += Mathf.Abs(tempVelocity.x) > 0 ? -tempVelocity.x / 10 : 0; 
        } 

    }

    private void PlayerInput(){
        //horizontal and Vertical Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //jump
        isJumping = Input.GetKey(KeyCode.Space);
    }

    private void CollectablesVerifier()
    {
        //Vizualização do raycast de circulo
        RaycastHit2D ItemPickableCircle = Physics2D.CircleCast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), itemPickableRadius, new Vector2(0.0f, 0.0f));
        if (ItemPickableCircle.transform.CompareTag("Item"))
        {
            //Pull item logic
            Rigidbody2D ItemCollider = ItemPickableCircle.collider.attachedRigidbody;
            if(ItemCollider == null)
            {
                Debug.LogWarning($"O item {ItemPickableCircle.transform.name} não possui rigidbody.Esse componente é essencial!");
                return;
            }
            
            // Vetor entre os pontos A(Posição do item) até B(Player) {Vector2 = (B.x - A.x),(B.y - A.y)}
            float deltaX = (gameObject.transform.position.x - ItemCollider.transform.position.x) * forceMultiplyer;
            float deltaY = (gameObject.transform.position.y - ItemCollider.transform.position.y) * forceMultiplyer;
            Vector2 deltaForce = new Vector2(deltaX,deltaY);
            ItemCollider.AddForce(deltaForce);
        }
    }

}
