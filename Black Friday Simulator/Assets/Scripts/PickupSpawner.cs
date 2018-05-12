using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupSpawner : NetworkBehaviour {

    public GameObject pickupPrefab;
    private const int numPickupsToSpawn = 20;
    private const int MaxNumQuantityOfEach = 3;
    private const int maxPrice = 20;
    private const int minPrice = 1;

    //I have to do this shit because Unity doesn't support tuples
    private List<Pickup> pickupList = new List<Pickup>();

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

    // Use this for initialization
    public override void OnStartServer () {

        var spawnRotation = Quaternion.Euler(
                0.0f,
                0.0f,
                0.0f);

        InitializePickups();

        foreach (Pickup p in pickupList) {
            var spawnPosition = new Vector3(
                Random.Range(-15.0f, 15.0f),
                Random.Range(-15.0f, 15.0f),
                -1.0f);

            GameObject pickup = Instantiate(pickupPrefab, spawnPosition, spawnRotation);
            pickup.GetComponent<Pickup>().SetAttributes(p.id, p.pname, p.price);
            NetworkServer.Spawn(pickup);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitializePickups() {

        int id = 0;

        for (int i = 0; i < pickupNames.Count; i++) {

            int quantity = Random.Range(0, MaxNumQuantityOfEach + 1);

            for (int q = 0; q < quantity; q++) {
                int price = Random.Range(minPrice, maxPrice + 1);
                pickupList.Add(new Pickup(id, pickupNames[i], price));
                id++;
            }
        }
    }
}
