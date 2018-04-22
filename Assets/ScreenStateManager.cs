using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenStateManager : MonoBehaviour {

    public enum ServerScreenState {
        Match, Waiting, Title, GameOver 
    }

    public enum ClientScreenState {
        Title, Finder, Match, GameOver
    }

    public static ScreenStateManager instance;



    public Text[] scoreMeters;
    public Text timer;

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        
    }

    void Update () {
        
    }
}
