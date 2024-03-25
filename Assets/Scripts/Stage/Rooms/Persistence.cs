using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistence : MonoBehaviour
{
    public static Persistence instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
}
