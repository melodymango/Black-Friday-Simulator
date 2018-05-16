using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
    [SyncVar]
    public string pname;
    [SyncVar]
    public float price;
    [SyncVar]
    public string image;
    [SyncVar]
    private bool hasPlayer = false;
    [SyncVar]
    private GameObject player = null;

    public SpriteRenderer spriteR;

    private void Start()
    {
        spriteR = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update() {
        RenderSpriteString();
    }

    //constructor
    public Pickup(string name, float price, string image)
    {
        this.pname = name;
        this.price = price;
        this.image = image;
    }

    //set attributes
    public void SetAttributes(string name, float price, string image)
    {
        this.pname = name;
        this.price = price;
        this.image = image;
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

    public string GetName()
    {
        return pname;
    }

    public void RenderSpriteString()
    {
        spriteR.sprite = Resources.Load<Sprite>("Sprites/"+image) as Sprite;
    }
}
