using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTest : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))particles.Play();
    }
}
