using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField][Tooltip("The game object has invunerability during dash?")] private bool invunerableDash;
    public bool isDashing;
    private bool dashLock; //to set things up before an dash starts

    private float healthBeforeDash;
    private float originalGravity;
    private float heightBeforeDash;
    private float originalMaxSpeed;
    private int originalDirection;

    private float timeSinceLastDash;
    private EntityMovable entity;

    private void Start()
    {
        entity = GetComponent<EntityMovable>();
        if (entity == null) Debug.LogWarning("Error!! You need to add the Dash script to the EntityMovable object");
        else {
            dashLock = true;
            originalGravity = entity.gravity;
            originalMaxSpeed = entity.maxSpeed;
            timeSinceLastDash = 0;
        }
        
    }
    private void Update()
    {
        timeSinceLastDash += !isDashing? Time.deltaTime : 0;
    }

    public void ActivateDash(int direction) { //the direction is -1 for left and 1 for right
        if (entity != null) {
            if (dashLock && dashCooldown < timeSinceLastDash) {
                if (invunerableDash) healthBeforeDash = entity.maxHealth; //change to actual health later when implemented
                isDashing = true;
                dashLock = false;
                entity.gravity = 0;
                heightBeforeDash = entity.transform.position.y;
                originalDirection = direction;

            }
            if (isDashing && !dashLock) {
                entity.maxSpeed = dashSpeed;
                entity.tempVelocity = new Vector2(dashSpeed * originalDirection, 0);
                entity.transform.position = new Vector2 (entity.transform.position.x,heightBeforeDash);
                Invoke("DeactivateDash", dashDuration);
            }
        }
    }
    private void DeactivateDash() {
        entity.maxHealth = healthBeforeDash; //also change when implemented please!!
        dashLock = true;
        isDashing = false;
        entity.gravity = originalGravity;
        entity.maxSpeed = originalMaxSpeed;
        entity.tempVelocity.x = originalMaxSpeed * Mathf.Sign(entity.tempVelocity.x);
        timeSinceLastDash = 0;
    }
}
