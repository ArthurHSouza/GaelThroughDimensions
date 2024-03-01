using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private float hookForce;
    [SerializeField][Tooltip("The tag of the target game object")] private string desiredTag;
    [SerializeField][Tooltip("The game object will check for gameobjects to hook in this circle, so a bigger number means bigger detection")] private float collisionDetectionCircleRadius;
    private Rigidbody2D callerRB;
    private Rigidbody2D closestTargetRB;
    private CircleCollider2D circleCollider;
    public LineRenderer hookRenderer;

    private bool isHooking; // Flag to indicate if the hook is active

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
            Debug.LogWarning("Error, you forgot to reference a line renderer");
            hookRenderer.startColor = Color.yellow;
            hookRenderer.endColor = Color.yellow;
        }
        
        
    }
    private void Update()
    {
        DrawLine();
        if (closestTargetRB == null) isHooking = false; //to prevent cases where the object being pulled get away from the screen 
    }

    public void GoToObject()
    {
        if (closestTargetRB != null && !isHooking)
        {
            isHooking = true;
            StartCoroutine(HookCoroutine(callerRB,closestTargetRB));
        }
    }
    private void DrawLine() {
        if (isHooking)
        {
            hookRenderer.enabled = true;
            hookRenderer.SetPosition(0, transform.position);
            hookRenderer.SetPosition(1, closestTargetRB.transform.position);
        }
        else {
            hookRenderer.enabled = false;
        }
    }

    private IEnumerator HookCoroutine(Rigidbody2D caller, Rigidbody2D target)
    {
        while (Vector2.Distance(caller.position, target.transform.position) > 1f && isHooking) //the isHooking is in the case of pull that goes further than the radius
        {
            Vector2 direction = (target.transform.position - caller.transform.position).normalized;
            caller.AddForce(direction * hookForce, ForceMode2D.Impulse);
            yield return null;
        }

        caller.velocity = Vector2.zero;
        isHooking = false;
    }

    public void PullObject(){
        if (closestTargetRB != null && !isHooking)
        {
            isHooking = true;
            StartCoroutine(HookCoroutine(closestTargetRB, callerRB));
        }
    }

    public void GoToAndLaunch()
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(desiredTag))
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
        if (collision.CompareTag(desiredTag))
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
