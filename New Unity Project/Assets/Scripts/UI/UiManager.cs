﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public static UiManager Instance { get { return instance; } }
    private static UiManager instance;

    public GameObject titleContainer, startContainer, readyContainer, GameOverContainer, BlueTeamWin, RedTeamWin;


    public GameObject playerIDcontainer;

    public CameraActionBoxFollower CameraBoxFollower;

    public Animator FadeToWhite;

    public List<Text> playerId = new List<Text>();

    public static bool isGameStarted { get { return gameStarted; } }
    static bool gameStarted;

    public GameObject mainOrb;
    public Transform[] mainOrbSpawnPoints;
    public Transform Everything;
    public GameObject Sakuras;


    // Use this for initialization
    void Start()
    {

        instance = this;
        OnResetProperties();

        if (WwiseManager.isWwiseEnabled)
            AkSoundEngine.PostEvent("GAME_OPEN", gameObject);
    }

    public static void onSetGameStartedState(bool state)
    {
        gameStarted = state;
    }


    public static void OnResetProperties()
    {
        UIEffectManager.OnResetProperties();
        gameStarted = false;
        instance.playerIDcontainer.SetActive(false);
    }


    public void EndCinematic()
    {
        Sakuras.SetActive(false);
        CameraBoxFollower.enabled = true;
        onSpawnOrb();
        StartCoroutine(OnBeginGame());
    }

    IEnumerator OnBeginGame()
    {
        
        readyContainer.SetActive(true);
        yield return new WaitForSeconds(3.2f);
        readyContainer.SetActive(false);
        gameStarted = true;
        yield break;

    }


    public void onSpawnOrb()
    {
        mainOrb.SetActive(true);
        mainOrb.transform.position = mainOrbSpawnPoints[Random.Range(0, mainOrbSpawnPoints.Length-1)].position;
    }


    IEnumerator makeIDAppear()
    {
        yield return new WaitForSeconds(6f);
        playerIDcontainer.SetActive(true);

        foreach (Text g in playerId)
        {
            g.CrossFadeAlpha(1.0f, 2.0f, false);
            g.CrossFadeAlpha(0.0f, 2.0f, false);
        }

        yield break;
    }

    public void onStartGame()
    {
        Sakuras.SetActive(true);

        if (WwiseManager.isWwiseEnabled)
            AkSoundEngine.PostEvent("UI_SELECT", gameObject);

        if (WwiseManager.isWwiseEnabled)
            AkSoundEngine.PostEvent("GAME_PLAY", gameObject);

        gameObject.GetComponent<Animator>().enabled = true;
        FadeToWhite.enabled = true;
        StartCoroutine(makeIDAppear());
    }

    public static void onGameOverScreen(bool state)
    {
        instance.GameOverContainer.SetActive(state);
    }





}
