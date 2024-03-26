using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Initializer 
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Execute() {
        Debug.LogWarning("Loaded the PersistentObjects from the Initializer Script!");
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("PersistentObjects")));
    }
}
