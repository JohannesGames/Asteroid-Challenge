using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

public class BeginRound : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.gm.mainCamera.orthographic = true;
        Invoke("Spawn", 1);
    }

    void Spawn()
    {
        GameManager.gm.SpawnAsteroid();
    }
}
