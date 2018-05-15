/* Taken from https://answers.unity.com/questions/1179602/implementing-server-side-code-with-unet.html and modified*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameTimer : NetworkBehaviour
{
    private const float roundTime = 30.0f;
    private const float countDownTime = 8.0f;
    [SyncVar]
    public float gameTime; //The length of a game, in seconds.
    [SyncVar]
    public float timer; //How long the game has been running. -1=waiting for players, -2=game is done
    [SyncVar]
    public bool masterTimer = false; //Is this the master timer?
    [SyncVar]
    public bool roundHasStarted; //Whether the game has started or not

    public Text countdownTimerText;

    GameTimer serverTimer;

    void Start()
    {
        roundHasStarted = false;
        timer = countDownTime;
        if (isServer)
        { // For the host to do: use the timer and control the time.
            if (isLocalPlayer)
            {
                serverTimer = this;
                masterTimer = true;
            }
        }
        else if (isLocalPlayer)
        { //For all the boring old clients to do: get the host's timer.
            GameTimer[] timers = FindObjectsOfType<GameTimer>();
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].masterTimer)
                {
                    serverTimer = timers[i];
                }
            }
        }
    }

    void Update()
    {
        /*
        if (!roundHasStarted)
        {
            PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
            foreach (PlayerController p in playerControllers)
            {
                p.CmdSetCanMove(false);
            }
        }*/

        if (masterTimer)
        { //Only the MASTER timer controls the time
            timer -= Time.deltaTime;
            //Debug.Log(timer);
        }

        if (isLocalPlayer)
        { //EVERYBODY updates their own time accordingly.
            if (serverTimer)
            {
                gameTime = serverTimer.gameTime;
                timer = serverTimer.timer;
                countdownTimerText.text = Mathf.Floor(timer).ToString();
            }
            else
            { //Maybe we don't have it yet?
                GameTimer[] timers = FindObjectsOfType<GameTimer>();
                for (int i = 0; i < timers.Length; i++)
                {
                    if (timers[i].masterTimer)
                    {
                        serverTimer = timers[i];
                    }
                }
            }
        }

        if (timer < 1 && !roundHasStarted)
        {
            //Debug.Log("Time: " + timer);
            countdownTimerText.text = "START SHOPPING!!";
            //PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
            StartCoroutine(RoundStartPause(3));
            timer = roundTime;
            roundHasStarted = true;
            /*
            foreach (PlayerController p in playerControllers)
            {
                p.CmdSetCanMove(true);
            }*/

        }
    }

    IEnumerator RoundStartPause(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}