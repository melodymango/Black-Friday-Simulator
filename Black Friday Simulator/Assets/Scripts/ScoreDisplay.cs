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
    public bool gameCreated;
    public string scoreString; //To get all info and then put into scoreText;
    public Text scoreText;

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

        if(time <= 0 && isGamePlaying && !gameCreated) {
            print("in here");
            gameCreated = true;
        }
    }

    void OnGUI()
    {
        scoreString = "Leaderboard: \n"; //To refresh after every call
        
        foreach(GameObject p in players) {
            //print();
            scoreString += "Player " + (p.GetComponent<PlayerResources>().GetId() + 1) + " " +
                "Items: " + p.GetComponent<PlayerResources>().getItemAmount() + '\n';
        }

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
     */
    }
