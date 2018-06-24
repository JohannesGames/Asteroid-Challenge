using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveMakeup
{
    public int bigCount;
    public int hugeCount;
}

public class WaveManager : MonoBehaviour
{
    public List<WaveMakeup> allWaves;
    [Header("Asteroid Spawning")]
    public AsteroidBig[] asteroidPrefabsBig;
    public AsteroidHuge[] asteroidPrefabsHuge;
    AsteroidBig spawnedAsteroid;
    int ranSpawnChoice;
    Vector3 asteroidSpawnLocation;
    bool spawning;

    List<AsteroidChunk> chunksRemaining = new List<AsteroidChunk>();
    List<AsteroidBig> asteroidsRemaining = new List<AsteroidBig>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnWave()
    {
        if (allWaves.Count > 0)
        {
            asteroidsRemaining.Clear();
            chunksRemaining.Clear();

            // Instantiate big asteroids
            for (int i = 0; i < allWaves[0].bigCount; i++)
            {
                spawnedAsteroid = Instantiate(asteroidPrefabsBig[Random.Range(0, asteroidPrefabsBig.Length)]);
                spawnedAsteroid.transform.position = ChooseAsteroidSpawnLocation();
                spawnedAsteroid.AsteroidInteraction();
                asteroidsRemaining.Add(spawnedAsteroid);
            }

            // Instantiate huge asteroids
            for (int i = 0; i < allWaves[0].hugeCount; i++)
            {
                spawnedAsteroid = Instantiate(asteroidPrefabsHuge[Random.Range(0, asteroidPrefabsHuge.Length)]);
                spawnedAsteroid.transform.position = ChooseAsteroidSpawnLocation();
                spawnedAsteroid.AsteroidInteraction();
                asteroidsRemaining.Add(spawnedAsteroid);
            }

            allWaves.RemoveAt(0);
        }

        spawning = false;
    }

    Vector3 ChooseAsteroidSpawnLocation()
    {
        asteroidSpawnLocation = Vector3.zero;
        // choose a side of the screen
        ranSpawnChoice = Random.Range(0, 4);
        switch (ranSpawnChoice)
        {
            case 0:
                // spawn top
                asteroidSpawnLocation = GameManager.gm.mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, GameManager.gm.mainCamera.pixelWidth),
                    spawnedAsteroid.asteroidSize == Asteroid.AsteroidSize.Big ? GameManager.gm.mainCamera.pixelHeight : GameManager.gm.mainCamera.pixelHeight * 1.5f, 0));  // if huge spawn further outside the screen bounds
                asteroidSpawnLocation.y = 0;
                break;
            case 1:
                // spawn bottom
                asteroidSpawnLocation = GameManager.gm.mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, GameManager.gm.mainCamera.pixelWidth),
                    spawnedAsteroid.asteroidSize == Asteroid.AsteroidSize.Big ? 0 : -GameManager.gm.mainCamera.pixelHeight * .5f, 0));                          // if huge spawn further outside the screen bounds
                asteroidSpawnLocation.y = 0;
                break;
            case 2:
                // spawn left
                asteroidSpawnLocation = GameManager.gm.mainCamera.ScreenToWorldPoint(new Vector3(0, Random.Range(spawnedAsteroid.asteroidSize == Asteroid.AsteroidSize.Big ? 0 : -GameManager.gm.mainCamera.pixelWidth * .5f,   // if huge spawn further outside the screen bounds
                    GameManager.gm.mainCamera.pixelHeight), 0));
                asteroidSpawnLocation.y = 0;
                break;
            case 3:
                // spawn right
                asteroidSpawnLocation = GameManager.gm.mainCamera.ScreenToWorldPoint(new Vector3(GameManager.gm.mainCamera.pixelWidth * (spawnedAsteroid.asteroidSize == Asteroid.AsteroidSize.Big ? 1 : 1.5f),     // if huge spawn further outside the screen bounds
                    Random.Range(0, GameManager.gm.mainCamera.pixelHeight), 0));
                asteroidSpawnLocation.y = 0;
                break;
            default:
                break;
        }

        return asteroidSpawnLocation;
    }

    public void RemoveAsteroid(AsteroidBig _ast)
    {
        asteroidsRemaining.Remove(_ast);

        // if no asteroids or chunks left, move to next wave
        if (!spawning && asteroidsRemaining.Count == 0 && chunksRemaining.Count == 0)
        {
            spawning = true;
            SpawnWave();
        }
    }

    public void AddChunk(AsteroidChunk _chunk)
    {
        chunksRemaining.Add(_chunk);
    }

    public void RemoveChunk(AsteroidChunk _chunk)
    {
        if (chunksRemaining.Count != 0)
        {
            chunksRemaining.Remove(_chunk);

            // if no asteroids or chunks left, move to next wave
            if (!spawning && asteroidsRemaining.Count == 0 && chunksRemaining.Count == 0)
            {
                spawning = true;
                SpawnWave();
            }
        }
    }
}
