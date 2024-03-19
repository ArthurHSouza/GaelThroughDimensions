using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
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

    [SerializeField]
    private LayerMask itemLayer;

    [SerializeField]
    public int ItemPullMaxQuantity = 10;

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
        Collider2D[] ItemPickableCircleCollider = Physics2D.OverlapCircleAll(gameObject.transform.position, itemPickableRadius, itemLayer);
        for(int i = 0; i < ItemPickableCircleCollider.Length && i < ItemPullMaxQuantity; i++)
        {
            var otherColl = ItemPickableCircleCollider[i];
            if (otherColl.transform.CompareTag("Item"))
            {
                //Pull item logic
                Rigidbody2D ItemCollider = otherColl.attachedRigidbody;
                if(ItemCollider == null)
                {
                    Debug.LogWarning($"O item {ItemCollider.transform.name} não possui rigidbody.Esse componente é essencial!");
                   continue;
                }
                ItemCollider.AddForce((gameObject.transform.position - ItemCollider.transform.position) * forceMultiplyer);
            }
            else
            {
               Debug.LogWarning($"O item {otherColl.transform.name} está na layer de items e ele não é item. Favor alterar para layer correta!"); 
            }
        }
    }
}
