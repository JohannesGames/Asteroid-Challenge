using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] float speed;

    Vector3 moveDirection;
    RaycastHit hit;
    bool hitAsteroid;
    Collider[] asteroidsHit;

    public void AmmoStart(Vector3 dir)
    {
        moveDirection = dir;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // move the bullet over time
        transform.Translate(moveDirection * Time.deltaTime * speed);
    }

    private void FixedUpdate()
    {
        asteroidsHit = Physics.OverlapCapsule(transform.position + Vector3.up * 20, transform.position + Vector3.down * 20, .25f, GameManager.gm.asteroidLayer);
        if (asteroidsHit.Length > 0)
        {
            asteroidsHit[0].GetComponent<AsteroidChunk>().AsteroidInteraction();
            Instantiate(GameManager.gm.ammoHitParticle, asteroidsHit[0].transform.position + Vector3.up, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
