/*Script taken from http://answers.unity.com/answers/1268915/view.html 
  This script is for centering the camera on the player at all times.*/

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public int depth = -20; //don't worry about this number lol

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {   
            //centers the camera on the player every frame
            transform.position = playerTransform.position + new Vector3(0, 0, depth);
        }
    }

    //Set which player the camera is going to follow
    public void SetTarget(Transform target)
    {
        playerTransform = target;
    }
}
