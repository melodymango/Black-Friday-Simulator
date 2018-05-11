using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float playerSpeed = 4f;

    //Only do this for the local player
    public override void OnStartLocalPlayer()
    {
        //moves the player's Z position to -1 so they can collide with other objects 
        transform.Translate(0, 0, -1);
        //sets the camera to track the local player
        Camera.main.GetComponent<CameraFollow>().SetTarget(gameObject.transform);
    }

    void Update()
    {
        //only move the local player
        if (!isLocalPlayer)
        {
            return;
        }

        //Player needs rigidbody to collide with other stuff. Sets the rigidbody's velocity, no acceleration so the player doesn't slide or anything
        Vector2 targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        GetComponent<Rigidbody2D>().velocity = targetVelocity * playerSpeed;

        //old movement code, didn't work for collisions but I'm keeping this here just in case
        /*var x = Input.GetAxis("Horizontal") * Time.deltaTime * 10.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 10.0f;

        transform.Translate(x, 0, 0);
        transform.Translate(0, y, 0);*/

        //To be finished lol
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        throw new NotImplementedException();
    }
}