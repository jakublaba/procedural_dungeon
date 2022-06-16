using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public const float delay = 0.02f;
    private RoomUtils roomUtils;

    void Start()
    {
        roomUtils = GameObject.Find("GenerationScripts").GetComponent<RoomUtils>();
        StartCoroutine(roomUtils.SpawnRoomsWithDelay());
    }
}
