using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] private float secondsBeforeLoad;
    [SerializeField] private string sceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Invoke("LoadScene", secondsBeforeLoad);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
