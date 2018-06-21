using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

public class SetCamPriority : MonoBehaviour
{
    public CinemachineVirtualCamera givePriority;
    public CinemachineVirtualCamera[] otherCameras;

    void OnEnable()
    {
        GameManager.gm.currentVCam = givePriority;
        givePriority.Priority = 20;
        foreach (var cam in otherCameras)
        {
            cam.Priority = 1;
        }
    }
}
