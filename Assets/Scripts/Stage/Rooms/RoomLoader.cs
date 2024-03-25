using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField]
    [Tooltip("Target door name to spawn to, it can be an empty gameobject just with transform")]
    private string targetDoorName;
    private RoomManager roomManager;

    private void Awake()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Invoke("LoadSceneWithDelay",0);
    }

    private void LoadSceneWithDelay()
    {
        roomManager.LoadRoom(sceneName,targetDoorName);
    }
}
