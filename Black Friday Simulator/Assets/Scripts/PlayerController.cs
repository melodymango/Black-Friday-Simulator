using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public float playerSpeed = 4f;
    [SyncVar]
    private bool canPickUp = false;
    [SyncVar]
    public GameObject itemToPickUp = null;
    private NetworkIdentity objNetId;


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

        //Player needs rigidbody to collide with other stuff. Sets the rigidbody's velocity, no acceleration so the player doesn't slide or anything
        Vector2 targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        GetComponent<Rigidbody2D>().velocity = targetVelocity * playerSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && canPickUp) {
            itemToPickUp.gameObject.GetComponent<Pickup>().CmdPickupItem();
        }

        //old movement code, didn't work for collisions but I'm keeping this here just in case
        /*var x = Input.GetAxis("Horizontal") * Time.deltaTime * 10.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 10.0f;

        transform.Translate(x, 0, 0);
        transform.Translate(0, y, 0);*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pickup")
        {
            collision.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
            canPickUp = true;
            itemToPickUp = collision.gameObject;
            Debug.Log("Can pick up item.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pickup")
        {
            collision.gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
            canPickUp = false;
            itemToPickUp = null;
            Debug.Log("Can no longer pick up item.");
        }
    }
}