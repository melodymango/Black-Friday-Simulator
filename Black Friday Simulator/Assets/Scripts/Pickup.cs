using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
    public int id;
    public string pname;
    public float price;

    [SyncVar]
    private bool hasPlayer = false;
    [SyncVar]
    private GameObject player = null;

    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.Space) && hasPlayer) {

            if (!isLocalPlayer)
            {
                return;
            }

            var playerCurrentMoney = player.gameObject.GetComponent<PlayerResources>();
            //Will not let the player pickup the item if buying it will reduce their current money below 0.
            if (playerCurrentMoney != null && playerCurrentMoney.GetCurrentMoney() - price >= 0) {
                playerCurrentMoney.DecrementAmount(price);
                Destroy(gameObject);
                hasPlayer = false;
                player = null;
            }
        }*/
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            hasPlayer = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" && player) {
            hasPlayer = false;
            player = null;
        }
    }

    //constructor
    public Pickup(int id, string name, float price) {
        this.id = id;
        this.pname = name;
        this.price = price;
    }

    //set attributes
    public void SetAttributes(int id, string name, float price) {
        this.id = id;
        this.pname = name;
        this.price = price;
    }

    /*
    //make the item get picked up
    [Command]
    public void CmdPickupItem()
    {
        Debug.Log("CmdPickupItem() is being called.");
        var playerCurrentMoney = player.gameObject.GetComponent<PlayerResources>();
        //Will not let the player pickup the item if buying it will reduce their current money below 0.
        if (playerCurrentMoney != null && playerCurrentMoney.GetCurrentMoney() - price >= 0)
        {
            playerCurrentMoney.DecrementAmount(price);
            Destroy(gameObject);
            hasPlayer = false;
            player = null;
        }
    }*/

    public override string ToString()
    {
        return pname + " ($" + price + ")";
    }

    public bool HasPlayer()
    {
        return hasPlayer;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public float GetPrice()
    {
        return price;
    }
}
