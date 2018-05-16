/* Player movement and interaction with items (picking up and dropping items) */

using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public float playerSpeed = 4f;
    public Text itemPopUp;
    [SyncVar]
    private bool canMove = true;
    [SyncVar]
    private bool canPickUp = false;
    [SyncVar]
    public GameObject itemToPickUp = null;


    //Only do this for the local player
    public override void OnStartLocalPlayer() {
        //moves the player's Z position to -1 so they can collide with other objects 
        transform.Translate(0, 0, -1);
        //sets the camera to track the local player
        Camera.main.GetComponent<CameraFollow>().SetTarget(gameObject.transform);
    }

    void Update() {
        //only move the local player
        if (!isLocalPlayer) {
            return;
        }

        if (canMove)
        {
            //Player needs rigidbody to collide with other stuff. Sets the rigidbody's velocity, no acceleration so the player doesn't slide or anything
            Vector2 targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            GetComponent<Rigidbody2D>().velocity = targetVelocity * playerSpeed;

            if (Input.GetKeyDown(KeyCode.Space) && itemToPickUp && canPickUp)
            {
                CmdPickupItem();
            }
        }

        //old movement code, didn't work for collisions but I'm keeping this here just in case
        /*var x = Input.GetAxis("Horizontal") * Time.deltaTime * 10.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 10.0f;

        transform.Translate(x, 0, 0);
        transform.Translate(0, y, 0);*/
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Pickup") {
            canPickUp = true;
            itemToPickUp = collision.gameObject;
            //Debug.Log("Player " + GetComponent<PlayerResources>().GetId() + " standing on " + itemToPickUp.GetComponent<Pickup>().ToString());
            //Debug.Log("Can pick up item.");

            if (isLocalPlayer)
            {
                itemPopUp = collision.gameObject.GetComponentInChildren<Text>();
                itemPopUp.enabled = true;
                //display name and price
                itemPopUp.text = itemToPickUp.GetComponent<Pickup>().ToString();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Pickup") {
            canPickUp = false;
            //Debug.Log("Can no longer pick up item.");
            itemPopUp = collision.gameObject.GetComponentInChildren<Text>();
            itemPopUp.enabled = false;
            itemToPickUp = null;
        }
    }

    [Command]
    public void CmdPickupItem() {
        //Debug.Log("CmdPickupItem() is being called.");
        var playerCurrentMoney = GetComponent<PlayerResources>();
        //Will not let the player pickup the item if buying it will reduce their current money below 0.
        if (playerCurrentMoney != null && playerCurrentMoney.GetCurrentMoney() - itemToPickUp.GetComponent<Pickup>().GetPrice() >= 0) {
            //subtract the cost of the item being picked up from the player's remaining currency
            playerCurrentMoney.DecrementAmount(itemToPickUp.GetComponent<Pickup>().GetPrice());
            //add the picked up item to the player's inventory
            GetComponent<PlayerResources>().CmdAddItemToInventory(itemToPickUp.GetComponent<Pickup>().ToString());
            //whomst picked up what
            //Debug.Log("Player " + GetComponent<PlayerResources>().GetId() + " picked up " + itemToPickUp.GetComponent<Pickup>().ToString());
            //Debug.Log("Player " + GetComponent<PlayerResources>().GetId() + "'s inventory:\n" + GetComponent<PlayerResources>().InventoryToString());
            //destroy the item from the level
            Destroy(itemToPickUp);
            itemToPickUp = null;
            canPickUp = false;
        }
    }

    public void SetCanMove(bool b)
    {
        canMove = b;
    }
}