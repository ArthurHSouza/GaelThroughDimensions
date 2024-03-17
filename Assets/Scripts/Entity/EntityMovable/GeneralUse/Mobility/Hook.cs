using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Hook : MonoBehaviour
{
    [Header("Hook")]
    [SerializeField]
    [Tooltip("Force applied to the target game object")]
    private float hookForce;
    [SerializeField]
    [Tooltip("The distance to stop hooking before getting to the target")]
    private float hookStopDistance;
    [SerializeField]
    [Tooltip("Time in the air after the launch")]
    private float boostTime;
    [SerializeField]
    [Tooltip("The layer of the target game object")]
    private LayerMask desiredLayers;
    [SerializeField]
    [Tooltip("The game object will check for gameobjects to hook in this circle, so a bigger number means bigger detection")]
    private float collisionDetectionCircleRadius;
    [SerializeField]
    [Tooltip("Don't need to assign if enableDirectionArrow isn`t enabled")]
    private Sprite arrowSprite;
    [SerializeField]
    [Tooltip("For the player, creates an arrow in the direction of the launch")]
    private bool enableDirectionArrow;
    [SerializeField]
    [Tooltip("Layers that will be ignored for the hook check")]
    private LayerMask rayIgnoreLayers;

    private bool keepMomentum;
    private bool isPulling;
    private bool canPool;
    private bool canThrust;
    private bool isOnThrust;
    public bool isHooking; //flag to indicate if the hook is active

    private Rigidbody2D callerRB;
    private Rigidbody2D currentlyBeingMovedRB;
    private Rigidbody2D closestTargetRB;
    private CircleCollider2D circleCollider;

    [SerializeField] private LineRenderer hookRenderer;
    private Rigidbody2D hookEndRB; //only aids the hook not to visualize the end at the closest one instead of the currently being pushed/pulled

    private Vector2 desiredDirection;
    private Vector2 currentDirection;


    private GameObject arrow;

    private Coroutine hookCoroutine;
    private Coroutine boostCoroutine;

    private bool noObjectBetweenTarget; //check if theres an object between caller and hook and it is not rayIgnoreLayers

    [Header("Rope")][SerializeField] private float ropeSize;
    private DistanceJoint2D distanceJoint;
    public bool cancelRope;

    private void Start()
    {
        GameObject colliderObject = new GameObject("HookDetectionCollider"); //creates a game object for the circle collider
        colliderObject.transform.parent = transform;
        colliderObject.transform.localPosition = Vector3.zero;
        colliderObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        circleCollider = colliderObject.AddComponent<CircleCollider2D>();
        circleCollider.tag = "Untagged";

        callerRB = GetComponent<Rigidbody2D>();
        circleCollider.radius = collisionDetectionCircleRadius;
        circleCollider.isTrigger = true;

        if (hookRenderer == null)
        {
            hookRenderer = gameObject.AddComponent<LineRenderer>();
            hookRenderer.startWidth = 0.1f;
            hookRenderer.endWidth = 0.1f;
            Debug.LogWarning("Error, you forgot to reference a line renderer, creating an default one");
            hookRenderer.startColor = Color.yellow;
            hookRenderer.endColor = Color.yellow;
        }
        if (arrowSprite != null)
        {
            arrow = new GameObject("HookDirectionArrow");
            SpriteRenderer spriteRenderer = arrow.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = arrowSprite;
        }
        if (distanceJoint == null)
        {
            distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
            distanceJoint.maxDistanceOnly = true;
            distanceJoint.distance = ropeSize;
            distanceJoint.autoConfigureDistance = false;
            distanceJoint.enabled = false;
            distanceJoint.connectedBody = null;
        }
        cancelRope = true;
        canThrust = false;
        canPool = false;
        arrow.SetActive(false);

    }
    private void Update()
    {

        DrawLine();

        if (closestTargetRB == null)
        {
            isHooking = false; //to prevent cases where the object being pulled get away from the screen
        }
        else
        {
            noObjectBetweenTarget = ObstructionRaycastCheck();
            if (!noObjectBetweenTarget) arrow.SetActive(false);

        }
        CheckDirectionIndicator();
        PullEngine(); //the method check conditions before running, don`t worry
        ThrustEngine(); //the method check conditions before running, don`t worry
    }
    private bool ObstructionRaycastCheck()
    {
        Vector2 direction = (closestTargetRB.transform.position - callerRB.transform.position).normalized;
        Debug.DrawRay(transform.position, direction * collisionDetectionCircleRadius, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, collisionDetectionCircleRadius, ~rayIgnoreLayers);

        if (hit.collider != null && hit.collider.gameObject.layer != 0) //if its not the default layer
        {
            return true; 
        }

        return false;
    }

    private void CheckDirectionIndicator()
    {
        if (enableDirectionArrow && noObjectBetweenTarget)
        {
            if (arrowSprite == null)
            {
                Debug.LogWarning("Error, you forgot to assign the arrowSprite, please disable the variable " +
                    "`enableDirectionArrow` if this game object is not the player, otherwise assign the sprite.");
            }
            else if (closestTargetRB != null)
            {
                if (!arrow.activeSelf) arrow.SetActive(true);
                if (isHooking) arrow.transform.position = closestTargetRB.position + (desiredDirection * 2);
                else
                {
                    arrow.transform.position = closestTargetRB.position +
                        (Vector2)((closestTargetRB.transform.position - transform.position).normalized * 2);
                }


            }
            else
            {
                arrow.SetActive(false);
            }

        }
    }
    private void DrawLine()
    {
        if (isHooking)
        {
            hookRenderer.enabled = true;
            hookRenderer.SetPosition(0, transform.position);
            hookRenderer.SetPosition(1, hookEndRB.transform.position);
        }
        else
        {
            hookRenderer.enabled = false;
        }
    }

    private IEnumerator HookCoroutine(Rigidbody2D caller, Rigidbody2D target)
    {
        hookEndRB = closestTargetRB; //fix the issue of drawing the hook in another object end

        while (Vector2.Distance(caller.position, target.transform.position) > 1f && isHooking && noObjectBetweenTarget)
        //the isHooking is in the case of pull that goes further than the radius
        {
            if (currentDirection == Vector2.zero) desiredDirection = (target.transform.position - caller.transform.position).normalized;
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
        else
        {
            if (noObjectBetweenTarget) //to prevent the canceled hook from running the boost
                boostCoroutine = StartCoroutine(BoostCoroutine(0.2f, caller));
        }
        currentDirection = Vector2.zero;
        isHooking = false;
    }

    public void RopeCheck()
    {
        if (closestTargetRB != null && !isHooking)
        {
            cancelRope = false;
            hookEndRB = closestTargetRB;
            isHooking = true;
            if (distanceJoint.connectedBody == null)
            {
                distanceJoint.connectedBody = closestTargetRB;

            }
        }
        RopeManager();

    }

    public void RopeManager()
    {
        if (!cancelRope && noObjectBetweenTarget)
        {
            distanceJoint.enabled = true;
            isHooking = true;
        }
        if (cancelRope)
        {
            isHooking = false;
            distanceJoint.connectedBody = null;
            distanceJoint.enabled = false;

        }
    }

    public string GetTargetObjectTag()
    {
        if (closestTargetRB != null)
        {
            return closestTargetRB.gameObject.tag;
        }
        else return "There`s no target object nearby";
    }


    private IEnumerator BoostCoroutine(float duration, Rigidbody2D caller)
    {
        float timer = 0f;
        while (timer < duration)
        {

            caller.GetComponent<PlayerController>().tempVelocity = desiredDirection * hookForce;

            timer += Time.deltaTime;

            yield return null;
        }
    }

    public void StopHook()
    {
        if (hookCoroutine != null)
        {
            StopCoroutine(hookCoroutine);
            StopCoroutine(boostCoroutine);
            hookCoroutine = null;
            isHooking = false;
            //callerRB.velocity = Vector2.zero;
        }
    }
    public void PullObject()
    {
        canPool = true;
        PullEngine();
    }
    private void PullEngine()
    {
        if (canPool)
        {
            if (!isPulling && closestTargetRB != null)
            {
                hookEndRB = closestTargetRB;
                currentlyBeingMovedRB = closestTargetRB;
                isPulling = true;
                isHooking = true;
            }
            float callerRadius = callerRB.GetComponent<Collider2D>().bounds.extents.magnitude;
            float targetRadius = currentlyBeingMovedRB.GetComponent<Collider2D>().bounds.extents.magnitude;
            float distance = Vector2.Distance(callerRB.transform.position, currentlyBeingMovedRB.transform.position) - (callerRadius + targetRadius);
            //hookCoroutine = StartCoroutine(HookCoroutine(closestTargetRB, callerRB)); old implementation
            if (distance > hookStopDistance && distance < collisionDetectionCircleRadius && noObjectBetweenTarget)
            {


                Vector2 direction = (callerRB.transform.position - currentlyBeingMovedRB.transform.position).normalized;
                if (!currentlyBeingMovedRB.GetComponent<EntityMovable>())//this is to change how it moves to temp velocity on entities
                    currentlyBeingMovedRB.AddForce(hookForce / 20 * direction * currentlyBeingMovedRB.mass, ForceMode2D.Impulse);
                else currentlyBeingMovedRB.GetComponent<EntityMovable>().tempVelocity = hookForce / 20 * direction * currentlyBeingMovedRB.mass;
            }
            else
            {
                currentlyBeingMovedRB.velocity = Vector2.zero;
                currentlyBeingMovedRB = null;
                isPulling = false;
                isHooking = false;
                canPool = false;
            }

            //if (distance > collisionDetectionCircleRadius)
            //{
            //    Debug.Log("Exited");
            //    isPulling = false;
            //    isHooking = false;
            //    canPool = false;
            //}
        }
    }
    public void ThrustObject()
    {
        canThrust = true;
        ThrustEngine();
    }
    private void ThrustEngine()
    {
        if (canThrust)
        {
            if (!isOnThrust && closestTargetRB != null)
            {
                hookEndRB = closestTargetRB;
                currentlyBeingMovedRB = closestTargetRB;
                isOnThrust = true;
                isHooking = true;
            }
            float callerRadius = callerRB.GetComponent<Collider2D>().bounds.extents.magnitude;
            float targetRadius = currentlyBeingMovedRB.GetComponent<Collider2D>().bounds.extents.magnitude;
            float distance = Vector2.Distance(callerRB.transform.position, currentlyBeingMovedRB.transform.position) - (callerRadius + targetRadius);
            //hookCoroutine = StartCoroutine(HookCoroutine(closestTargetRB, callerRB)); old implementation
            if (distance > hookStopDistance && distance < collisionDetectionCircleRadius && noObjectBetweenTarget)
            {
                Vector2 direction = (currentlyBeingMovedRB.transform.position - callerRB.transform.position).normalized;
                //the next 2 lines are to change how it behaves with an object based on velocity and entities that use temp velocity
                if (!callerRB.GetComponent<EntityMovable>()) callerRB.AddForce(hookForce / 20 * direction * callerRB.mass, ForceMode2D.Impulse);
                else callerRB.GetComponent<EntityMovable>().tempVelocity = hookForce * direction * callerRB.mass;
            }
            else
            {
                callerRB.velocity = Vector2.zero;
                currentlyBeingMovedRB = null;
                isOnThrust = false;
                isHooking = false;
                canThrust = false;
            }
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

    //functions to assign the closestTargetRB
    private void ExitRB(Collider2D collision)
    {
        if ((desiredLayers.value & 1 << collision.gameObject.layer) > 0) //if the collision is a desired layer
        {
            Rigidbody2D targetRB = collision.attachedRigidbody;
            if (targetRB == closestTargetRB && cancelRope)
            {
                if (closestTargetRB.bodyType != RigidbodyType2D.Static) closestTargetRB.velocity = Vector3.zero;
                closestTargetRB = null;
                isHooking = false;
            }
        }
    }
    private void StayRB(Collider2D collision)
    {
        if ((desiredLayers.value & 1 << collision.gameObject.layer) > 0) //if the collision is a desired layer
        {
            Rigidbody2D targetRB = collision.attachedRigidbody;
            if (closestTargetRB == null ||
                Vector2.Distance(transform.position,
                targetRB.transform.position) < Vector2.Distance(transform.position, closestTargetRB.transform.position))
            {
                closestTargetRB = targetRB;
                if (closestTargetRB.CompareTag("Pullable") || closestTargetRB.CompareTag("Thrust") || closestTargetRB.CompareTag("SwingSpot"))
                {
                    enableDirectionArrow = false;
                }
                if (closestTargetRB.CompareTag("HookSpot"))
                {
                    enableDirectionArrow = true;
                }
            }
        }
    }

    //collision case for enemies and pulling player
    private void OnCollisionStay2D(Collision2D collision)
    {
        StayRB(collision.collider);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        ExitRB(collision.collider);
    }


    //trigger cases for hookspot and swingspot
    private void OnTriggerStay2D(Collider2D collision)
    {
        StayRB(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ExitRB(collision);
    }

}