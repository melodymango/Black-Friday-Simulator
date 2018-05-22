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
	private float slideTime = 0f;				//Remaining time until regaining control after being bashed
	public float maxSlideTime = 1f;				//Time that players should lose control when bashed
	private bool isPressingPickup = false;
	private float bashCoolDown = 0f;			//Remaining time until player can bash again
	public float maxBashCoolDown = 2f;			//What the cooldown should be set to after pressing bash
	private bool isPressingBash = false;
	public float bashRadius = 2f;				//How close other players have to be in order to be bashed
	public float bashForce = 10f;				//How strong the knockback is on bashed players

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
            Vector2 targetVelocity;
			//If not sliding, can walk
			if(slideTime <= 0)
			{
				targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			}
			else
			{
				slideTime -= Time.deltaTime;
				targetVelocity = GetComponent<Rigidbody2D>().velocity*0.95f/playerSpeed;
			}
            GetComponent<Rigidbody2D>().velocity = targetVelocity * playerSpeed;

            if (Input.GetAxisRaw("Pickup") == 1 && !isPressingPickup && itemToPickUp && canPickUp)
            {
                isPressingPickup = true;
				CmdPickupItem();
            }
			
			//Make sure player can't just hold down the pickup button
			else if(Input.GetAxisRaw("Pickup") == 1)
			{
				isPressingPickup = true;
			}			
			else
			{
				isPressingPickup = false;
			}
			
			//Whack the shit out of each other
			if (Input.GetAxisRaw("Bash") == 1  && !isPressingBash && bashCoolDown <= 0)
			{
				//Set cooldown to 2 seconds
				bashCoolDown = maxBashCoolDown;
				
				//make it so you can't hold bash
				isPressingBash = true;
				
				//Bash
				CmdBash();
			}
			
			//Make sure player can't spam bash
			else if(bashCoolDown > 0){
				bashCoolDown -= Time.deltaTime;
			}
			
			//No holding bash
			if(Input.GetAxisRaw("Bash") == 0){
				isPressingBash = false;
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

	[Command]
	public void CmdBash()
	{
		//Get a list of players
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		//Loop through all players
		for(int i = 0; i < players.Length; i++)
		{
			//You can't bash yourself, idiot
			if(players[i] != gameObject)
			{
				float distanceToPlayer = Vector2.Distance(players[i].transform.position,transform.position);
				//Debug.Log(distanceToPlayer);
				if(distanceToPlayer < bashRadius)
				{
					//Debug.Log("Bashing player " + players[i].GetComponent<PlayerResources>().GetId());
					Vector2 slideVector = players[i].transform.position-transform.position;
					slideVector.Normalize();
					slideVector *= bashForce;
					players[i].GetComponent<PlayerController>().RpcSlide(slideVector,maxSlideTime); //Slide time should be less than bash cooldown
				}
			}
		}
	}
	
	[ClientRpc]
	public void RpcSlide(Vector2 slideVector, float st){
		//Debug.Log(slideVector);
		GetComponent<Rigidbody2D>().velocity = slideVector;
		slideTime = st;
		GetComponent<PlayerResources>().CmdDropRandomItem();
	}
	
    public void SetCanMove(bool b)
    {
        canMove = b;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }
}