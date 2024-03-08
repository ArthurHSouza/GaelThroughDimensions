using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Hook : MonoBehaviour
{
    [SerializeField][Tooltip("Force applied to the target game object")] private float hookForce;
    [SerializeField][Tooltip("Time in the air after the launch")] private float boostTime;
    [SerializeField][Tooltip("The layer of the target game object")] private LayerMask desiredLayers;
    [SerializeField][Tooltip("The game object will check for gameobjects to hook in this circle, so a bigger number means bigger detection")] private float collisionDetectionCircleRadius;
    [SerializeField][Tooltip("Don't need to assign if enableDirectionArrow isn`t enabled")] private Sprite arrowSprite;
    [SerializeField][Tooltip("For the player, creates an arrow in the direction of the launch")] private bool enableDirectionArrow;

    private bool keepMomentum;

    private Rigidbody2D callerRB;
    private Rigidbody2D closestTargetRB;
    private CircleCollider2D circleCollider;

    public LineRenderer hookRenderer;
    private Rigidbody2D hookEndRB; //only aids the hook not to visualize the end at the closest one instead of the currently being pushed/pulled

    private Vector2 desiredDirection;
    private Vector2 currentDirection;

    public bool isHooking; //flag to indicate if the hook is active

    private GameObject arrow;

    private Coroutine hookCoroutine;
    private Coroutine boostCoroutine;

    private void Start()
    {
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        callerRB = GetComponent<Rigidbody2D>();
        circleCollider.radius = collisionDetectionCircleRadius;
        circleCollider.isTrigger = true;
        if (hookRenderer == null) {
            hookRenderer = gameObject.AddComponent<LineRenderer>();
            hookRenderer.startWidth = 0.1f;
            hookRenderer.endWidth = 0.1f;
            Debug.LogWarning("Error, you forgot to reference a line renderer, creating an default one");
            hookRenderer.startColor = Color.yellow;
            hookRenderer.endColor = Color.yellow;
        }
        if (arrowSprite != null) {
            arrow = new GameObject("HookDirectionArrow");
            SpriteRenderer spriteRenderer = arrow.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = arrowSprite;
        }
        
        
    }
    private void Update()
    {
        DrawLine();
        if (closestTargetRB == null) isHooking = false; //to prevent cases where the object being pulled get away from the screen
        CheckDirectionIndicator();
    }

    private void CheckDirectionIndicator() {
        if (enableDirectionArrow) {
            if (arrowSprite == null)
            {
                Debug.LogWarning("Error, you forgot to assign the arrowSprite, please disable the variable `enableDirectionArrow` if this game object is not the player, otherwise assign the sprite.");
            }
            else if (closestTargetRB != null)
            {
                if(!arrow.activeSelf) arrow.SetActive(true);
                if (isHooking) arrow.transform.position = closestTargetRB.position + (desiredDirection * 2);
                else { arrow.transform.position = closestTargetRB.position + (Vector2)((closestTargetRB.transform.position - transform.position).normalized * 2);}


            }
            else {
                arrow.SetActive(false);
            }
            
        }
    }

    public void GoToObject()
    {
        if (closestTargetRB != null && !isHooking)
        {
            keepMomentum = false;
            isHooking = true;
            hookCoroutine = StartCoroutine(HookCoroutine(callerRB,closestTargetRB));
        }
    }
    private void DrawLine() {
        if (isHooking)
        {
            hookRenderer.enabled = true;
            hookRenderer.SetPosition(0, transform.position);
            hookRenderer.SetPosition(1, hookEndRB.transform.position);
        }
        else {
            hookRenderer.enabled = false;
        }
    }

    private IEnumerator HookCoroutine(Rigidbody2D caller, Rigidbody2D target)
    {
        hookEndRB = closestTargetRB; //fix the issue of drawing the hook in another object end
        while (Vector2.Distance(caller.position, target.transform.position) > 1f && isHooking) //the isHooking is in the case of pull that goes further than the radius
        {   
            if(currentDirection == Vector2.zero) desiredDirection = (target.transform.position - caller.transform.position).normalized;
            currentDirection = (target.transform.position - caller.transform.position).normalized;
            caller.AddForce(currentDirection * hookForce, ForceMode2D.Impulse);

            if (!isHooking)
                yield break;

            yield return null;
        }

        if (!keepMomentum)
        {
            caller.velocity = Vector2.zero;
        }
        else {
            boostCoroutine = StartCoroutine(BoostCoroutine(0.2f,caller));
        }
        currentDirection = Vector2.zero;
        isHooking = false;
    }

    private IEnumerator BoostCoroutine(float duration, Rigidbody2D caller)
    {
        float timer = 0f;
        while (timer < duration){

            caller.GetComponent<PlayerController>().tempVelocity = desiredDirection * hookForce;

            timer += Time.deltaTime;

            yield return null;
        }
    }

    public void StopHook() {
        if (hookCoroutine != null){
            StopCoroutine(hookCoroutine);
            StopCoroutine(boostCoroutine);
            hookCoroutine = null;
            isHooking = false;
            //callerRB.velocity = Vector2.zero;
        }
    }

    public void PullObject(){
        if (closestTargetRB != null && !isHooking)
        {
            keepMomentum = false;
            isHooking = true;
            hookCoroutine = StartCoroutine(HookCoroutine(closestTargetRB, callerRB));
        }
    }

    public void GoToAndLaunch()
    {
        if (closestTargetRB != null && !isHooking)
        {
            keepMomentum = true;
            isHooking = true;
            hookCoroutine = StartCoroutine(HookCoroutine(callerRB, closestTargetRB));
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((desiredLayers.value & 1 << collision.gameObject.layer) > 0)
        {
            Rigidbody2D targetRB = collision.attachedRigidbody;
            if (closestTargetRB == null || Vector2.Distance(transform.position, targetRB.transform.position) < Vector2.Distance(transform.position, closestTargetRB.transform.position))
            {
                closestTargetRB = targetRB;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((desiredLayers.value & 1 << collision.gameObject.layer) > 0)
        {
            Rigidbody2D targetRB = collision.attachedRigidbody;
            if (targetRB == closestTargetRB)
            {
                closestTargetRB.velocity = Vector3.zero;
                closestTargetRB = null;
                isHooking = false;
            }
        }
    }

}
