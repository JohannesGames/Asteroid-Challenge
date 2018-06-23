using UnityEngine;
using System.Collections;

public class AsteroidHuge : AsteroidBig
{
    public bool firstEntry = true;
    Vector3 screenPos;
    void Update()
    {
        if (firstEntry)
        {
            screenPos = GameManager.gm.mainCamera.WorldToScreenPoint(transform.position);
            // check x is within screen bounds
            if (screenPos.x <= 0 && screenPos.x <= GameManager.gm.mainCamera.pixelWidth)
            {
                // check y is within screen bounds
                if (screenPos.y >= 0 && screenPos.y <= GameManager.gm.mainCamera.pixelHeight)
                {
                    // now it is on screen and screen wrap can occur as normal
                    GetComponent<WrapObject>().enabled = true;
                    firstEntry = false;
                }
            }

        }
        if (isActive)
        {
            // if round is over, delete self
            if (!GameManager.gm.inGame) Destroy(gameObject);
        }
    }

    public override void AsteroidInteraction()
    {
        isActive = true;

        // generate a random vector to use for velocity and torque (add movement fudge to avoid moving straight up or down)
        horizontalRandomValue = Random.Range(-1.0f, 1.0f);
        verticalRandomValue = Random.Range(-1.0f, 1.0f);
        randomVector = (Vector3.left * horizontalRandomValue + initialMovementFudge * (horizontalRandomValue / Mathf.Abs(horizontalRandomValue)) +
            Vector3.forward * verticalRandomValue).normalized;
        // add some rotation
        rb.AddTorque(randomVector * rotationSpeed, ForceMode.VelocityChange);
        // set velocity to a random vector at somewhere between half of max speed and max speed
        randomVector = (Vector3.zero - transform.position).normalized;   // reuse random vector
        randomVector.y = 0;
        rb.velocity = randomVector * Random.Range(maxMoveSpeed / 2, maxMoveSpeed);
    }
}
