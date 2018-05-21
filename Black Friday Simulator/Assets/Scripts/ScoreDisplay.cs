using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreDisplay : NetworkBehaviour {

    public GameObject[] players;
    public GameObject ResourceUI;
    public string scoreString; //To get all info and then put into scoreText;
    public Text scoreText;

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        ResourceUI.GetComponent<Canvas>().enabled = false;
        if (isLocalPlayer)
        {
            ResourceUI.GetComponent<Canvas>().enabled = true;
            Invoke("ToggleShoppingList", 0.5f);
        }

    }
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnGUI()
    {
        scoreString = "Leaderboard: \n"; //To refresh after every call
        
        foreach(GameObject p in players) {
            //print();
            scoreString += "Player " + (p.GetComponent<PlayerResources>().GetId() + 1) + " " +
                "Items: " + p.GetComponent<PlayerResources>().getItemAmount() + " " +
                "$" + p.GetComponent<PlayerResources>().GetCurrentMoney() + '\n';
        }

        if (isLocalPlayer)
        {
            //This GUILayout block displays the player's inventory
            GUILayout.BeginArea(new Rect(Screen.width - 250, 50, 200, Screen.height));
            GUILayout.Label("Scoreboard");
            GUILayout.EndArea();
            scoreText.text = scoreString;
        }
    }

    //To do:
    /*
     * X Show something on screen
     * X Grab components from PlayerController
     * X Show 
     * X Make it update
     * Reset once things are done (probably will happen when game resets anyways)
     */
    }
