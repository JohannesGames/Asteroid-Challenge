using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRound : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.gm.scoreText.text = "";
        GameManager.gm.mainCamera.orthographic = false;
    }
}
