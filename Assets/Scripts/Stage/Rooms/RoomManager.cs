using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    private bool isChangingRooms;


    public void LoadRoom(string sceneName, string targetDoorName){
        StartCoroutine(LoadRoomCoroutine(sceneName, targetDoorName));
    }
    public IEnumerator LoadRoomCoroutine(string sceneName, string targetDoorName ) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        isChangingRooms = true;
        SpawnPlayer(targetDoorName);
    }

    private void SpawnPlayer(string doorToSpawnTo) {
        GameObject targetDoor = GameObject.Find(doorToSpawnTo);
        if (targetDoor != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = targetDoor.transform.position;
            isChangingRooms = false;
        }
        else {
            Debug.LogWarning("Couldn't find the target door, check the name again");
        }
        
    }
}
