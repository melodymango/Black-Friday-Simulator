/* Keeps track of the player's money and items picked up.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerResources : NetworkBehaviour {

    public const float startingMoney = 100f;
    public Text currentMoneyText; //the UI element that displays their money

    [SyncVar]
    private float currentMoney = startingMoney;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        
        //Only update/render UI for local player because each player may have differing amounts of money.
        if (!isLocalPlayer) {
            return;
        }
        //Displays current money amount for the player
        currentMoneyText.text = "Money remaining: $" + currentMoney;
    }

    //Decrements the amount of money the player has
    public void DecrementAmount(float amount) {
        //Only the server shoud have the authority to do this
        if (!isServer) {
            return;
        }

        currentMoney -= amount;
    }

    public float GetCurrentMoney() {
        return currentMoney;
    }
}
