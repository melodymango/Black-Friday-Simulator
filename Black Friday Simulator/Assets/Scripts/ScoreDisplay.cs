using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreDisplay : NetworkBehaviour {

    public GameObject[] players;
    public GameObject ResourceUI;
    public bool isGamePlaying; //Boolean for if playtime is active
    public float time; //Grabs time
    public bool gameCreated; //And extra bool because GameTimer.roundHasStarted doesn't go back to false due to spaghetti code
    public string scoreString; //To get all info and then put into scoreText;
    public Text scoreText;
    public Text finalScoreText; // When timer reaches 0, then this will pop up (maybe score Text disappears)

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");
        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;
        gameCreated = false;

        ResourceUI.GetComponent<Canvas>().enabled = false;
        if (isLocalPlayer)
        {
            ResourceUI.GetComponent<Canvas>().enabled = true;
            Invoke("ToggleShoppingList", 0.5f);
        }

    }
	
	// Update is called once per frame
	void Update () {
        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;


    }

    void OnGUI()
    {
        if (!gameCreated)
        {
            scoreString = "Leaderboard: \n"; //To refresh after every call
        }

        else
        {
            scoreString = ""; //Hide leaderboard for the final results
        }

        if (time <= 0 && isGamePlaying && !gameCreated)
        {
            //In here will be where the canvas will pop up and display the result screen
            string finalScore = "FINISHED! \n";

            //players = players.sort
            foreach (GameObject p in players)
            {
                finalScore += "Player " + (p.GetComponent<PlayerResources>().GetId() + 1) + " " +
                    "Items: " + p.GetComponent<PlayerResources>().getItemAmount() + " " + 
                    "Money: $" + p.GetComponent<PlayerResources>().GetCurrentMoney() + '\n';
            }

            finalScoreText.text = finalScore;
            print("in here");
            gameCreated = true;
            scoreString = "";
        }

        if (!gameCreated)
        {
            foreach (GameObject p in players)
            {
                scoreString += "Player " + (p.GetComponent<PlayerResources>().GetId() + 1) + " " +
                    "Items: " + p.GetComponent<PlayerResources>().getItemAmount() + '\n';
            }
        }


        //isLocalPlayer seems to be not required
        if (isLocalPlayer)
        {
            scoreText.text = scoreString;
        }
    }

    //To do:
    /* When game is finished, show:
     * Place name item money
     * 
     * make a button with stophost & stopclient (test which one is correct)
     * 
     * Problem:
     * Player 1: Cannot see other players
     * Player 2+: ID, item = 0
     */
    }
