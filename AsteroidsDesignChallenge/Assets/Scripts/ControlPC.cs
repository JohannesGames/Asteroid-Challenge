﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WrapObject))]
public class ControlPC : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] Ammo ammunition;
    [Tooltip("How many rounds per second")]
    [SerializeField] float rateofFire;
    float nextShotTime;

    [HideInInspector]
    public int currentLives;

    Vector3 gunDirection;

    // movement
    [Header("Movement")]
    public float movementSpeed;
    float currentSpeed;
    [Tooltip("How long until the PC's velocity reaches 0 after releasing movement keys")]
    public float timeTilMotionStop;
    float stopTimer;
    float inputHorizontal;
    float inputVertical;
    Vector3 moveDirection;


    void Start()
    {

    }


    void Update()
    {
        if (GameManager.gm.inGame) GetPlayerInput();
        MovePC();
    }

    void GetPlayerInput()
    {
        // point the gun where the mouse is (the heading from the PC to the mouse)
        gunDirection = GameManager.gm.mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        gunDirection.y = 0;
        gunDirection.Normalize();

        // if player is pressing "fire"
        if (GameManager.gm.inGame && Time.time >= nextShotTime && Input.GetMouseButton(0))
        {
            // fire a shot
            Ammo _ammo = Instantiate(ammunition, transform.position, Quaternion.identity);
            _ammo.AmmoStart(gunDirection);
            // set cooldown til next shot
            nextShotTime = Time.time + 1 / rateofFire;
        }

        // if player is using the movement keys
        inputHorizontal = inputVertical = 0;
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        if (inputHorizontal != 0 || inputVertical != 0)
        {
            moveDirection.x = inputHorizontal;
            moveDirection.z = inputVertical;
            moveDirection.Normalize();

            currentSpeed = movementSpeed;
            stopTimer = 0; // reset timer on input
        }
        else
        {
            // if no movement input slow down PC
            stopTimer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(movementSpeed, 0, stopTimer / timeTilMotionStop);
        }
    }

    void MovePC()
    {
        if (currentSpeed > 0)
        {
            transform.Translate(moveDirection * Time.deltaTime * currentSpeed);
        }
    }

    public void AddLife()
    {
        currentLives++;
    }

    public void RemoveLife()
    {
        currentLives--;
    }
}
