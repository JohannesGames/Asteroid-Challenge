using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPC : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] Ammo ammunition;
    [Tooltip("How many rounds per second")]
    [SerializeField] float rateofFire;
    float nextShotTime;

    Vector3 gunDirection;
    Vector3 moveDirection;


    void Start()
    {

    }


    void Update()
    {
        // point the gun where the mouse is (the heading from the PC to the mouse)
        gunDirection = GameManager.gm.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        gunDirection.y = 0;
        gunDirection.Normalize();

        // if PC is pressing "fire"
        if (GameManager.gm.inGame && Time.time >= nextShotTime && Input.GetMouseButton(0))
        {
            // fire a shot
            Ammo _ammo = Instantiate(ammunition, transform.position, Quaternion.identity);
            _ammo.AmmoStart(gunDirection);
            // set cooldown til next shot
            nextShotTime = Time.time + 1 / rateofFire;
        }
    }
}
