using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    //detect what door the player used, also assign an position to each entrance

    private string GetActiveRoom() {
        return SceneManager.GetActiveScene().name;
    }
}
