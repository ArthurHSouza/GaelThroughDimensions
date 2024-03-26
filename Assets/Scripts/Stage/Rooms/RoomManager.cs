using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    private bool isChangingRooms;
    private GameObject player;
    private bool wasInitialized = false; 

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!wasInitialized) { StartCoroutine(Initialization()); }
         //in the case of loading scenes in unity itself
    }

    private IEnumerator Initialization() {
        while(!GameObject.Find("DefaultSpawnPoint")){
            if (SceneManager.GetActiveScene().isLoaded) {
                Debug.LogError("Error!! You're trying to load a scene from the editor without assigning a `DefaultSpawnPoint` game object, " +
                    "please create a game object with this exact name to where you want the player to spawn");
                yield break;
            }
            yield return null;  
        }
        SpawnPlayer("DefaultSpawnPoint");
        
    }
    public void LoadRoom(string sceneName, string targetDoorName){
        StartCoroutine(LoadRoomCoroutine(sceneName, targetDoorName));
    }
    public IEnumerator LoadRoomCoroutine(string sceneName, string targetDoorName ) {
        //fading out
        SceneTransition.instance.StartFadeOut();
        Vector2 playerVelocityInTransition = player.GetComponent<PlayerController>().tempVelocity;
        while (SceneTransition.instance.isFadingOut) {
            player.GetComponent<PlayerController>().tempVelocity = playerVelocityInTransition;
            yield return null;
        }
        //loading the next room
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        isChangingRooms = true;
        SpawnPlayer(targetDoorName); //spawning the player

        //fading in in the new room
        SceneTransition.instance.StartFadeIn();
        while (SceneTransition.instance.isFadingIn)
        {
            player.GetComponent<PlayerController>().tempVelocity = playerVelocityInTransition;
            yield return null;
        }
    }

    private void SpawnPlayer(string doorToSpawnTo) {
        GameObject targetDoor = GameObject.Find(doorToSpawnTo);
        if (targetDoor != null){
            player.transform.position = targetDoor.transform.position;
            isChangingRooms = false;
        }
        else {
            Debug.LogWarning("Couldn't find the target door, check the name again");
        }
        
    }
}
