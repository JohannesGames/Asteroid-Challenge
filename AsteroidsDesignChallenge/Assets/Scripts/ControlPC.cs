using System.Collections;
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

    // damage
    [Header("Damage")]
    [Tooltip("How long after being hit is PC invulnerable")]
    [SerializeField] float damageGracePeriod;
    float gracePeriodTimer;
    bool wasHit;
    Collider[] hitBy;
    AsteroidChunk chunkHit;

    void Start()
    {

    }


    void Update()
    {
        if (GameManager.gm.inGame)
        {
            GetPlayerInput();

            // if was hit, make invulnerable for some time
            if (wasHit)
            {
                CheckGracePeriod();
            }
        }
        MovePC();
    }

    private void FixedUpdate()
    {
        if (GameManager.gm.inGame && !wasHit)
        {
            CheckCollision();
        }
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

    public bool RemoveLife()
    {
        currentLives--;

        // if lives remain keep playing
        if (currentLives < 0)
        {
            currentLives = 0;
            return false;
        }
        else return true;
    }

    void CheckCollision()
    {
        // check for asteroid collision
        hitBy = Physics.OverlapCapsule(transform.position + Vector3.up * 10,
            transform.position + Vector3.down * 10, .4f, GameManager.gm.asteroidLayer);

        if (hitBy.Length > 0)
        {
            wasHit = true;
            chunkHit = hitBy[0].GetComponent<AsteroidChunk>();
            // if any collision detected
            Debug.Log("PC hit chunk");
            // if PC still has lives remaining
            if (chunkHit && RemoveLife())
            {
                GameManager.gm.livesText.text = currentLives.ToString();
                // if attached to big asteroid
                if (chunkHit.mainAsteroid)
                {
                    chunkHit.mainAsteroid.RemoveChunk(chunkHit);
                    chunkHit.mainAsteroid.rb.AddTorque(chunkHit.mainAsteroid.randomVector + chunkHit.randomVector * 100);
                }
            }
            else
            {
                // if out of lives
                GameManager.gm.OutOfLives();
            }
            Destroy(chunkHit.gameObject);
            chunkHit = null;
            Instantiate(GameManager.gm.pcHitParticle, transform.position, Quaternion.identity);
        }
    }

    void CheckGracePeriod()
    {
        gracePeriodTimer += Time.deltaTime;
        if (gracePeriodTimer >= damageGracePeriod)
        {
            gracePeriodTimer = 0;
            wasHit = false;
        }
    }
}
