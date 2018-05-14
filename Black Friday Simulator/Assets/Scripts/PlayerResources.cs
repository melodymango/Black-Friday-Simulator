/* Keeps track of the player's money and items picked up.
 Is also in charge of rendering the UI for the player's inventory and money*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerResources : NetworkBehaviour {

    public const float startingMoney = 100f;
    public Text currentMoneyText; //the UI element that displays their money
    public Text shoppingListText; //UI element that shows shopping list
    public Text inventoryText; //UI element displaying player's current inventory
    private List<string> shoppingList = new List<string>();
    private List<Pickup> inventory = new List<Pickup>();

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
        //Displays player UI (shopping list and money left so far)
        DisplayUI();
    }

    //Decrements the amount of money the player has
    public void DecrementAmount(float amount) {
        //Only the server shoud have the authority to do this
        if (!isServer) {
            return;
        }
        currentMoney -= amount;
    }

    //Handles UI for the player
    public void DisplayUI() {
        //Display how much money the player has left
        currentMoneyText.text = "$" + currentMoney + " Remaining";
        //Display the shopping list for the round
        shoppingListText.text = "Shopping List:\n" + ListToString(shoppingList);
        //Display inventory
        inventoryText.text = "Inventory:\n" + InventoryToString();
    }

    public void AddItemToInventory(Pickup p) {
        inventory.Add(p);
    }

    public void SetShoppingList(List<string> l) {
        shoppingList = new List<string>();
        shoppingList = l;
        Debug.Log("Player " + GetComponent<NetworkIdentity>().playerControllerId + "'s shopping list: " + ListToString(shoppingList));
    }

    public float GetCurrentMoney() {
        return currentMoney;
    }

    public string ListToString(List<string> l) {
        string result = "";
        foreach (string i in l) {
            result += (i + "\n");
        }
        return result;
    }

    public string InventoryToString() {
        string result = "";
        foreach (Pickup i in inventory) {
            result += (i.ToString() + "\n");
        }
        return result;
    }
}
