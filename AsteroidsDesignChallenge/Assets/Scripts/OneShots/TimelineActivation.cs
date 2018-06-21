using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineActivation : MonoBehaviour
{
    public GameObject[] objectsToEnable;
    public GameObject[] objectsToDeactivate;

    void OnEnable()
    {
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }
    }
}
