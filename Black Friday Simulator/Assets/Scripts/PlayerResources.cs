/* Keeps track of the player's id, money and items picked up.
 Is also in charge of rendering the UI for the player's inventory and money*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerResources : NetworkBehaviour {

    public const float startingMoney = 100f;
    public int itemAmount; //Currently shows how many items a player has. 
                           //Eventually will change so it counts how many items are in inventory
    public GameObject ResourceUI;
    public Text currentMoneyText; //the UI element that displays their money
    public Text shoppingListText; //UI element that shows shopping list
    public Text inventoryText; //UI element displaying player's current inventory
    public SyncListString inventory = new SyncListString();
    public GameObject pickupPrefab;

    private int id = -1;
    private string shoppingList = ""; //Shopping List
    private bool shoppingListVisible = false;
    [SyncVar]
    private float currentMoney = startingMoney;

    // Use this for initialization
    void Start() {
        itemAmount = 0;

        ResourceUI.GetComponent<Canvas>().enabled = false;
        if (isLocalPlayer)
        {
            ResourceUI.GetComponent<Canvas>().enabled = true;
            Invoke("ToggleShoppingList", 0.5f);
        }
    }

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            //This GUILayout block displays the player's inventory
            GUILayout.BeginArea(new Rect(Screen.width - 250, 50, 200, Screen.height));
            GUILayout.Label("Inventory");
            for (var i = 0; i < inventory.Count; i++)
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button(inventory[i]))
                {
                    //Debug.Log("Dropping " + inventory[i]);
                    CmdDropItem(i);
                    //GetPickupAttributes(inventory[i]);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
            
            //Displays player UI (shopping list and money left so far)
            DisplayUI();
        }
    }

    // Update is called once per frame
    void Update () {
        /*
        //Only update/render UI for local player because each player may have differing amounts of money.
        if (isLocalPlayer) {
            //Displays player UI (shopping list and money left so far)
            DisplayUI();
        } */   
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
    public void DisplayUI()
    {
        //Display how much money the player has left
        currentMoneyText.text = "$" + currentMoney + " Remaining";
        //Display inventory
        //inventoryText.text = "Inventory:\n" + InventoryToString();
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
        itemAmount++;
    }

    [Command]
    public void CmdDropItem(int i)
    {
        string[] item = GetPickupAttributes(inventory[i]);
        GameObject pickup = Instantiate(pickupPrefab, transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        pickup.GetComponent<Pickup>().SetAttributes(item[0], float.Parse(item[1]), item[0]);
        currentMoney += float.Parse(item[1]);
        NetworkServer.Spawn(pickup);
        inventory.RemoveAt(i);
        itemAmount--;
    }

    [ClientRpc]
    public void RpcSetShoppingList(string s) {
        shoppingList = s;
        Debug.Log("Player " + id + "'s shopping list: " + shoppingList);
    }

    public string[] GetPickupAttributes(string p)
    {
        string[] separatingChars = { " ($", ")" };
        string[] result = p.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < result.Length; i++)
        {
            Debug.Log(result[i]);
        }
        return result;
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

    public int getItemAmount(){

        return itemAmount;
    }
}

