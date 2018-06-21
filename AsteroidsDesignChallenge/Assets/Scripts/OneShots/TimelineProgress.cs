using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineProgress : MonoBehaviour
{
    public bool isInProgress;

    void OnEnable()
    {
        GameManager.gm.duringTimeline = isInProgress;
    }
}
