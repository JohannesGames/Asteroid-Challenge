using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] PlayableDirector openingTimeline;
    [SerializeField] PlayableDirector switchToGame;
    [SerializeField] PlayableDirector switchToMenu;

    // Press Play
    Vector3 startingPos;
    bool moveButton;
    bool growButton;

    private void Start()
    {
        startingPos = playButton.transform.localPosition;
    }

    void Update()
    {
        // cosmetics

        // grow or shrink button with mouse input
        if (!GameManager.gm.duringTimeline)
        {
            if (growButton)
            {
                playButton.transform.localScale = Vector3.Lerp(playButton.transform.localScale, new Vector3(2, 2, 2), Time.deltaTime / 10);
            }
            else
            {
                playButton.transform.localScale = Vector3.Lerp(playButton.transform.localScale, Vector3.one, Time.deltaTime * 5);
            }
        }

        // zoom button away
        if (moveButton)
        {
            playButton.transform.localPosition = Vector3.Lerp(playButton.transform.localPosition,
                new Vector3(playButton.transform.localPosition.x, playButton.transform.localPosition.y, 3000), Time.deltaTime);

            // rotate button
            playButton.transform.localRotation = Quaternion.Lerp(playButton.transform.localRotation, Quaternion.Euler(0, 0, -90), Time.deltaTime);
        }
        else
        {
            playButton.transform.localPosition = Vector3.Lerp(playButton.transform.localPosition,
                new Vector3(startingPos.x, startingPos.y, startingPos.z), Time.deltaTime * 5);

            // rotate button
            playButton.transform.localRotation = Quaternion.Lerp(playButton.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime);
        }

        if (GameManager.gm.inGame && Input.GetKeyDown(KeyCode.Escape) && !GameManager.gm.duringTimeline)
        {
            // return to main menu
            SwitchToMainMenu();
        }
    }

    public void OnPointerOver()
    {
        growButton = true;
    }

    public void OnPointerExit()
    {
        growButton = false;
    }

    public void OnPointerDown()
    {
        if (!GameManager.gm.duringTimeline)
        {
            moveButton = true;
            GameManager.gm.inGame = true;
            GameManager.gm.AddIntensity(50, 0);
            switchToGame.Play();
            openingTimeline.Stop();
            switchToMenu.Stop();
        }
    }

    public void SwitchToMainMenu()
    {
        moveButton = GameManager.gm.mainCamera.orthographic = GameManager.gm.inGame = false;
        switchToMenu.Play();
        switchToGame.Stop();
        openingTimeline.Stop();
    }
}
