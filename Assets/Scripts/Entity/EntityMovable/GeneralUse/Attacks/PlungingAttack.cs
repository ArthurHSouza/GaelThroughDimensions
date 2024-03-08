using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlungingAttack : MonoBehaviour
{
    [SerializeField][Tooltip("Speed of the fall attack")] private float plungingSpeed;
    [SerializeField][Tooltip("Time before the plunge beggings")] private float attackStartup;
    private EntityMovable entity;
    public bool isPlunging; //for control in other classes
    private bool plungeStart;

    void Start()
    {
        entity = GetComponent<EntityMovable>();
        isPlunging = false;
        plungeStart = false;
        if (plungingSpeed > 0) plungingSpeed = plungingSpeed * - 1;
        Debug.LogWarning("Plunging attack doesn't deal damage yet, please implement the damage methods");
    }

    private void Update()
    {
        if(plungeStart) entity.tempVelocity = Vector3.zero;
        if (entity.isGrounded){
            isPlunging = false;
        }
    }
    public void Attack() {
        if (!entity.isGrounded) {
            isPlunging = true;
            plungeStart = true;
            Invoke("Plunge", attackStartup);
        }
    }
    private void Plunge() {
        if (!entity.isGrounded)
        {
            plungeStart = false;
            entity.tempVelocity = Vector3.zero;
            entity.tempVelocity.y = plungingSpeed;
        }
        
        
    }
}
