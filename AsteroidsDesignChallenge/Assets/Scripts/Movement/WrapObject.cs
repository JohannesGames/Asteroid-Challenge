using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapObject : MonoBehaviour
{

    Vector3 screenPos;
    Vector3 newWorldPos;
    enum WrapRequired
    {
        Top,
        Bottom,
        Left,
        Right
    }

    void Update()
    {
        IsOnScreen();
    }

    void IsOnScreen()
    {
        screenPos = GameManager.gm.mainCamera.WorldToScreenPoint(transform.position);
        // check x is within screen bounds
        if (screenPos.x < 0)
        {
            ObjectWrap(WrapRequired.Right);
        }
        if (screenPos.x > GameManager.gm.mainCamera.pixelWidth)
        {
            ObjectWrap(WrapRequired.Left);
        }
        // check y is within screen bounds
        if (screenPos.y < 0)
        {
            ObjectWrap(WrapRequired.Top);
        }
        if (screenPos.y > GameManager.gm.mainCamera.pixelHeight)
        {
            ObjectWrap(WrapRequired.Bottom);
        }
    }

    void ObjectWrap(WrapRequired wrapReq)
    {
        switch (wrapReq)
        {
            case WrapRequired.Top:
                screenPos = new Vector3(screenPos.x, GameManager.gm.mainCamera.pixelHeight, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
            case WrapRequired.Bottom:
                screenPos = new Vector3(screenPos.x, 0, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
            case WrapRequired.Left:
                screenPos = new Vector3(0, screenPos.y, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
            case WrapRequired.Right:
                screenPos = new Vector3(GameManager.gm.mainCamera.pixelWidth, screenPos.y, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
        }
    }
}
