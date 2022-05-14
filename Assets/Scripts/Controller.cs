using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    private const float delay = 0.05f;
    private RoomUtils roomUtils;

    void Start() {
        roomUtils = GameObject.Find("GenerationScripts").GetComponent<RoomUtils>();
        StartCoroutine(roomUtils.SpawnRoomsWithDelay(delay));
    }

    void Update() {

    }
}
