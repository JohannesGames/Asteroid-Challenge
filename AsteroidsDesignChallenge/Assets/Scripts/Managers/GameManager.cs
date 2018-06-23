using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public WaveManager waveManagerPrefab;
    [HideInInspector]
    public WaveManager wm;
    public Camera mainCamera;
    public bool inGame;
    public bool duringTimeline; // can't click if in timeline

    public LayerMask asteroidLayer;

    //PC
    [Header("PC")]
    public ControlPC pcPrefab;
    [HideInInspector]
    public ControlPC pc;

    [Header("Particles")]
    public ParticleSystem pcHitParticle;
    public ParticleSystem ammoHitParticle;


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
    public TMPro.TextMeshProUGUI livesText;
    [SerializeField] Text highScore;
    public RectTransform highScorePanel;
    [Tooltip("0 if infinite")]
    public int maxLives;
    public int startingLives;
    [Tooltip("New life gained every __ points")]
    public int newLifeEvery;
    int nextLifeAt;

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

    public void BeginRound()
    {
        pc = Instantiate(pcPrefab, Vector3.zero, Quaternion.identity);
        pc.currentLives = startingLives;
        livesText.text = startingLives.ToString();
        wm = Instantiate(waveManagerPrefab);
        nextLifeAt = newLifeEvery;
        wm.SpawnWave();
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

    public void OutOfLives()
    {
        // Particle effect
        // Show score
        scoreText.gameObject.SetActive(false);
        highScorePanel.gameObject.SetActive(true);
        highScore.text = currentScore.ToString();
        // End round
        EndRound();
    }


    public void EndRound()
    {
        if (pc)
        {
            Destroy(pc.gameObject);
        }
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
        if (currentScore >= nextLifeAt)
        {
            nextLifeAt += newLifeEvery;
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
