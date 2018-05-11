using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    public string pickup_name;
    public float price;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var playerCurrentMoney = other.gameObject.GetComponent<PlayerResources>();
            //Will not let the player pickup the item if buying it will reduce their current money below 0.
            if (playerCurrentMoney != null && playerCurrentMoney.GetCurrentMoney() - price >= 0)
            {
                playerCurrentMoney.DecrementAmount(price);
                Destroy(gameObject);
            }
        }
    }
}
