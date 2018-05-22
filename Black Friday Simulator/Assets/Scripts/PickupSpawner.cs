/* In charge of determining/spawning pickups, randomly generating a shopping list for each round*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupSpawner : NetworkBehaviour {

    public GameObject pickupPrefab;
    public float floorWidth;
    public float floorHeight;
    public Sprite[] pickupImages;
    private GameObject[] players;
    private const int numPickupsToSpawn = 20;
    private const int MaxNumQuantityOfEach = 3;
    private const int shoppingListLength = 10;
    private const int maxPrice = 20;
    private const int minPrice = 1;

    //Stores id, name, price of each pickup that is spawned in a current round
    private List<Pickup> pickupList = new List<Pickup>();

    //Names of the merchandise on sale; can be changed if you wish
    private List<string> pickupNames = new List<string> {
        "Instant Cooker",
        "Laptop",
        "Television",
        "Graphics Card",
        "Vacuum Cleaner",
        "Washing Machine",
        "Digital Camera",
        "Speakers",
        "Katana",
        "Water Rifle",
        "Bookshelf",
        "Ice Cream Maker",
        "Smartphone",
        "Fat Bird Plush",
        "Tent",
        "Piano",
        "Chemistry Set",
        "Drum Set",
        "Chocolate Cake",
        "Sushi"
    };

    //List of merchandise to shop for in a round
    private List<string> shoppingList = new List<string>();

    // Use this for initialization
    public override void OnStartServer() {

        var spawnRotation = Quaternion.Euler(
                0.0f,
                0.0f,
                0.0f);

        InitializePickups();

        //Randomly spawn pickups in a specified square area.
        foreach (Pickup p in pickupList) {
            var spawnPosition = new Vector3(0f,0f,0f);
            Collider2D[] hitColliders;

            do
            {
                spawnPosition = new Vector3(
                Random.Range(-floorWidth / 2 + 1, floorWidth / 2 - 1),
                Random.Range(-floorHeight / 2 + 1, floorHeight / 2 - 1),
                -1.0f);
                hitColliders = Physics2D.OverlapCircleAll(spawnPosition, 1.1f);
                //Debug.Log(hitColliders.Length);
            } while (hitColliders.Length > 0);

            GameObject pickup = Instantiate(pickupPrefab, spawnPosition, spawnRotation);
            pickup.GetComponent<Pickup>().SetAttributes(p.pname, p.price, p.image);
            NetworkServer.Spawn(pickup);
        }

        //Generate shopping list for this round
        GenerateShoppingList();

        //Assign players unique ids
        Invoke("AssignPlayerIds", 0.5f);

        //Distribute the shopping list
        Invoke("DistributeShoppingList", 0.6f);
    }
	
	// Update is called once per frame
	void Update () {

    }

    //Initializes pickups
    private void InitializePickups() {
        //Cycle through every pickup name in the list
        for (int i = 0; i < pickupNames.Count; i++) {

            //Randomly generate how many of the current pickup to spawn
            int quantity = Random.Range(1, MaxNumQuantityOfEach + 1);

            for (int q = 0; q < quantity; q++) {
                //Randomly generate a price for the pickup
                int price = Random.Range(minPrice, maxPrice + 1);
                //Sprite image = pickupImages[i];
                pickupList.Add(new Pickup(pickupNames[i], price, pickupNames[i]));
                //Assign unique id to pickup for identification purposes
            }
        }
    }

    /*Generates a list of items all players need to hunt for.
     Apparently choosing x items out a list of length y is actually
     really hard so I just went with shuffling the list and then choosing
     the first 10 elements.*/
    private void GenerateShoppingList() {
        //Reset the shopping list
        shoppingList = new List<string>();
        //Shuffle the list of pickup names
        ShufflePickupNames();
        //Add the first x (shoppingListLength) items of PickupNames to the current round's shopping list.
        for(int i = 0; i < shoppingListLength; i++) {
            shoppingList.Add(pickupNames[i]);
        }
        //Debug.Log(ListToString(shoppingList));
    }

    //Shuffles the list of names
    private void ShufflePickupNames() {
        for (int i = 0; i < pickupNames.Count; i++) {
            string temp = pickupNames[i];
            int randomIndex = Random.Range(i, pickupNames.Count);
            pickupNames[i] = pickupNames[randomIndex];
            pickupNames[randomIndex] = temp;
        }
    }

    //Converts the list of pickup names to a string
    public string ListToString(List<string> l) {
        string result = "";
        foreach (string i in l) {
            result += (i + "\n");
        }
        return result;
    }

    public List<string> GetShoppingList() {
        return shoppingList;
    }

    /*Finds all players currently on the server, and assigns the shopping list to each of them*/
    private void DistributeShoppingList() { 
        foreach (GameObject player in players) 
            player.GetComponent<PlayerResources>().RpcSetShoppingList(ListToString(shoppingList));
    }

    //Assigns unique id to player. Not sure if I actually need this but I'm keeping it here
    private void AssignPlayerIds()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players: " + players.Length);
        int startingId = 0;
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerResources>().SetId(startingId);
            startingId++;
            Debug.Log("Player " + player.GetComponent<PlayerResources>().GetId() + "has joined the game.");
        }
    }
}
