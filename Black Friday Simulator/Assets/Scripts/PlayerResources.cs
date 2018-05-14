/* Keeps track of the player's id, money and items picked up.
 Is also in charge of rendering the UI for the player's inventory and money*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerResources : NetworkBehaviour {

    public const float startingMoney = 100f;
    public GameObject ResourceUI;
    public Text currentMoneyText; //the UI element that displays their money
    public Text shoppingListText; //UI element that shows shopping list
    public Text inventoryText; //UI element displaying player's current inventory
    public List<string> inventory = new List<string>();

    private int id = -1;
    private string shoppingList = ""; //Shopping List
    private bool shoppingListVisible = false;
    [SyncVar]
    private float currentMoney = startingMoney;

    // Use this for initialization
    void Start() {

        ResourceUI.GetComponent<Canvas>().enabled = false;
        if (isLocalPlayer)
        {
            ResourceUI.GetComponent<Canvas>().enabled = true;
            Invoke("ToggleShoppingList", 0.5f);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        //Only update/render UI for local player because each player may have differing amounts of money.
        if (isLocalPlayer) {
            //Displays player UI (shopping list and money left so far)
            DisplayUI();
        }    
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
        //Display inventory
        inventoryText.text = "Inventory:\n" + InventoryToString();
    }

    //toggle visibility of shopping list
    public void ToggleShoppingList() {
        if (!shoppingListVisible)
        {
            shoppingListText.text = "Shopping List:\n" + shoppingList;
            shoppingListVisible = true;
        }
        else
        {
            shoppingListText.text = "";
            shoppingListVisible = false;
        }
    }

    [Command]
    public void CmdAddItemToInventory(string i) {
        inventory.Add(i);
    }

    [ClientRpc]
    public void RpcSetShoppingList(string s) {
        shoppingList = s;
        Debug.Log("Player " + id + "'s shopping list: " + shoppingList);
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
        foreach (string i in inventory) {
            result += (i.ToString() + "\n");
        }
        return result;
    }

    public string GetShoppingList()
    {
        return shoppingList;
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int i)
    {
        id = i;
    }
}

