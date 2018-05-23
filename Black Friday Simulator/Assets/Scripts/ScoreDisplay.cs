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
    public Button exitToLobby;
    public string[] shoppingList; //The items that the players need to win
    public string[] inventoryList; //The items that the player currently has in their inventory
    public Dictionary<GameObject, int> idAmount = new Dictionary<GameObject, int>(); //Correlates player to their final score
    private int numItems; //Number of items that player collected were on the shopping list

    private Dictionary<string, int> numInventory = new Dictionary<string, int>(); //All items on shopping list with # amount of items
    private string shoppingListString;
    private string inventoryListString;

    // Use this for initialization
    void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            idAmount.Add(player, -1); //The reason it's -1 is because splitting items will have a " ", 0 key/value.
        }

        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;
        gameCreated = false;
        shoppingListString = "";

        //shoppingList = GameObject.FindGameObjectWithTag("PickupSpawner").GetComponent<PickupSpawner>().shoppingList;
        //test = players[0].GetComponent<PlayerResources>().GetShoppingList();
        
        ResourceUI.GetComponent<Canvas>().enabled = false;
        if (isLocalPlayer)
        {
            ResourceUI.GetComponent<Canvas>().enabled = true;
            Invoke("ToggleShoppingList", 0.5f);
        }
        exitToLobby.gameObject.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
        time = players[0].GetComponent<GameTimer>().timer;
        isGamePlaying = players[0].GetComponent<GameTimer>().roundHasStarted;

        //There's a delay for the shoppping list in PlayerResources to be made, so
        //this is here to keep updating until the list is created.
        if (shoppingListString == "") 
        {
            shoppingListString = players[0].GetComponent<PlayerResources>().GetShoppingList();
            shoppingList = shoppingListString.Split( '\n' );

            //Once the list has finally been split (janky I know)
            if (shoppingListString != "")
            {
                //print("IN HERE BABY");
                foreach (string s in shoppingList)
                {
                    //print(s);
                    numInventory.Add(s, 0);
                    //print(s + " " + numInventory[s]);
                }
            }
        }

    }

    void OnGUI()
    {
        if (!gameCreated)
        {
            scoreString = "Current Items Holding: \n"; //To refresh after every call
        }

        else
        {
            scoreString = ""; //Hide leaderboard for the final results
        }

        //Here I will do the calculation

        if (time <= 0 && isGamePlaying && !gameCreated)
        {
            //In here will be where the canvas will pop up and display the result screen
            string finalScore = "FINISHED! \n";

            
            exitToLobby.gameObject.SetActive(true);

			//Update player array because host didn't get it at start
			players = GameObject.FindGameObjectsWithTag("Player");

            //Go through player's items
            //Increment the dict if item is in dict
            //Go through dict and add up add the # that are non-zero
            string[] splitNameFromPrice;
            string unsplitName;
            foreach (GameObject p in players)
            {
                numItems = 0;

                //Create an array of player's inventory
                inventoryListString = p.GetComponent<PlayerResources>().InventoryToString();
                inventoryList = inventoryListString.Split('\n');
                foreach(string item in inventoryList)
                {
                    print(item);
                    splitNameFromPrice = item.Split('(');
                    unsplitName = splitNameFromPrice[0];
                    unsplitName = unsplitName.TrimEnd();
                    print(unsplitName);
                    print(numInventory.ContainsKey(unsplitName));
                    if (numInventory.ContainsKey(unsplitName))
                    {
                        //print("HERE");
                        numInventory[unsplitName]++;
                    }
                    
                }

                
                //Will count how many items are in shopping list
                
                foreach(string entry in numInventory.Keys)
                {
                    if(numInventory[entry] > 0)
                    {
                        idAmount[p]++;
                    }
                  
                }

                //YOU CANNOT CHANGE VALUE TO 0 DURING ITERATING OVER EACH KEY. WILL CAUSE OUT OF SYNC ERROR
                foreach(string s in shoppingList)
                {
                    numInventory[s] = 0;
                }
            }

            GameObject bestPlayer = null;
            int mostItems = 0;
            float mostMoney = 0;

            foreach (GameObject p in players)
            {
                if(bestPlayer == null || mostItems < idAmount[p])
                {
                    bestPlayer = p;
                    mostItems = idAmount[p];
                    mostMoney = p.GetComponent<PlayerResources>().GetCurrentMoney();
                }

                else if(mostItems == idAmount[p])
                {
                    if(mostMoney < p.GetComponent<PlayerResources>().GetCurrentMoney())
                    {
                        bestPlayer = p;
                        mostItems = idAmount[p];
                        mostMoney = p.GetComponent<PlayerResources>().GetCurrentMoney();
                    }
                }

                finalScore += "Player " + (p.GetComponent<PlayerResources>().GetId() + 1) + " " +
                    "Items: " + idAmount[p] + " " + 
                    "Money: $" + p.GetComponent<PlayerResources>().GetCurrentMoney() + '\n';
            }

            //Here I'll post the winner
            finalScore += "Player " + (bestPlayer.GetComponent<PlayerResources>().GetId() + 1) + " wins!";
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

    public void ExitGame()
    {
        if(isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
    //To do:
    /* 
     */

    //Compares a player inventory list with the shopping list
    public int CompareLists(List<string> shopping)
    {
      
        SyncListString i = GetComponent<PlayerResources>().GetInventory();
        foreach(string s in i)
        {
            Debug.Log("inventory: " + s);
        }
        return 0;
    }
}
