using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public Camera mainCamera;
    public bool inGame;
    public bool duringTimeline; // can't click if in timeline

    //PC
    [Header("PC")]
    public ControlPC pcPrefab;
    [HideInInspector]
    public ControlPC pc;

    [Header("Asteroid Spawning")]
    public Asteroid[] asteroidPrefabs;
    Asteroid spawnedAsteroid;
    int ranSpawnChoice;
    Vector3 asteroidSpawnLocation;
    public LayerMask asteroidLayer;

    [Header("Camera Shake")]
    Cinemachine.CinemachineBrain mainCameraBrain;
    [HideInInspector]
    public Cinemachine.CinemachineVirtualCamera currentVCam;
    public float amplitudeIntensity;
    public float frequencyIntensity;
    float newAmplitudeIntensity;
    float newFrequencyIntensity;
    [HideInInspector]
    public float shakeAmplitude = .34f;
    [HideInInspector]
    public float shakeFrequency = .45f;

    // Points
    [Header("Points & Lives")]
    public int currentScore;
    public TMPro.TextMeshProUGUI scoreText;
    [SerializeField] int pointsChunkHit;
    [SerializeField] int pointsChunkDestroyed;
    [SerializeField] int pointsAsteroidSplit;
    [SerializeField] TMPro.TextMeshProUGUI livesText;
    [Tooltip("0 if infinite")]
    public int maxLives;
    public int startingLives;
    [Tooltip("New life gained every __ points")]
    public int newLifeEvery;

    public enum PointEvent
    {
        chunkHit,
        chunkDestroyed,
        asteroidSplit
    }

    void Awake()
    {
        if (gm)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gm = this;
        }
    }

    private void Update()
    {
        IntensityManager();

        ScoreAnimation();
    }

    void IntensityManager()
    {
        // manage intensity (mainly for camera shake)

        // amplitude
        newAmplitudeIntensity = Mathf.Lerp(newAmplitudeIntensity, 0, Time.deltaTime * 2);
        amplitudeIntensity = Mathf.Lerp(amplitudeIntensity, newAmplitudeIntensity, Time.deltaTime * 2);

        if (amplitudeIntensity > 1)
        {
            // Cam shake
            currentVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude * amplitudeIntensity;
        }
        else amplitudeIntensity = 1;

        // frequency
        newFrequencyIntensity = Mathf.Lerp(newFrequencyIntensity, 0, Time.deltaTime * 2);
        frequencyIntensity = Mathf.Lerp(frequencyIntensity, newFrequencyIntensity, Time.deltaTime * 2);

        if (frequencyIntensity > 1)
        {
            // Cam shake
            currentVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = shakeFrequency * frequencyIntensity;
        }
        else frequencyIntensity = 1;


    }

    public void AddIntensity(float addAmplitude, float addFrequency)
    {
        newAmplitudeIntensity = newAmplitudeIntensity + addAmplitude;
        newFrequencyIntensity = newFrequencyIntensity + addFrequency;
    }

    public void SpawnAsteroid()
    {
        // Instantiate asteroids
        for (int i = 0; i < 5; i++)
        {
            spawnedAsteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)]);
            spawnedAsteroid.transform.position = ChooseAsteroidSpawnLocation();
            spawnedAsteroid.AsteroidHit();
        }

        // Spawn PC
        pc = Instantiate(pcPrefab, Vector3.zero, Quaternion.identity);
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
                asteroidSpawnLocation = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, mainCamera.pixelWidth), mainCamera.pixelHeight, 0));
                asteroidSpawnLocation.y = 0;
                break;
            case 1:
                // spawn bottom
                asteroidSpawnLocation = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, mainCamera.pixelWidth), 0, 0));
                asteroidSpawnLocation.y = 0;
                break;
            case 2:
                // spawn left
                asteroidSpawnLocation = mainCamera.ScreenToWorldPoint(new Vector3(0, Random.Range(0, mainCamera.pixelHeight), 0));
                asteroidSpawnLocation.y = 0;
                break;
            case 3:
                // spawn right
                asteroidSpawnLocation = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth, Random.Range(0, mainCamera.pixelHeight), 0));
                asteroidSpawnLocation.y = 0;
                break;
            default:
                break;
        }

        return asteroidSpawnLocation;
    }

    public void EndRound()
    {
        Destroy(pc.gameObject);
    }

    #region Points
    public void AddPoints(PointEvent pe)
    {
        switch (pe)
        {
            case PointEvent.chunkHit:
                currentScore += pointsChunkHit;
                break;
            case PointEvent.chunkDestroyed:
                currentScore += pointsChunkDestroyed;
                break;
            case PointEvent.asteroidSplit:
                currentScore += pointsAsteroidSplit;
                break;
        }

        scoreText.text = currentScore.ToString();
        scoreText.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // check if enough points for new life
        if (currentScore % newLifeEvery == 0)
        {
            pc.AddLife();
            livesText.text = pc.currentLives.ToString();
        }
    }

    void ScoreAnimation()
    {
        if (scoreText.transform.localScale.y > 1.05)
        {
            scoreText.transform.localScale = Vector3.Lerp(scoreText.transform.localScale, Vector3.one, Time.deltaTime * 3);
        }
        else scoreText.transform.localScale = Vector3.one;
    }
    #endregion
}
