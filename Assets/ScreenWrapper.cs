using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{

    //0.1 of the view size
    public float margin = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this is the width of the screen in world units (it depends on the camera settings)
        //add a margin so the wrapping area is slightly larger than the camera view and the asteroids
        //exit the screen before teleporting on the other side
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2 + margin;
        float screenHeight = Camera.main.orthographicSize * 2 + margin;

        //i can't assign a vector component to a transform directly so I use a temporary variable
        //even if most of the times won't be changes
        Vector2 newPosition = transform.position;

        //check all the margin 
        if (transform.position.x > screenWidth / 2)
        {
            newPosition.x = -screenWidth / 2;
        }

        if (transform.position.x < -screenWidth / 2)
        {
            newPosition.x = screenWidth / 2;
        }

        if (transform.position.y > screenHeight / 2)
        {
            newPosition.y = -screenHeight / 2;
        }

        if (transform.position.y < -screenHeight / 2)
        {
            newPosition.y = screenHeight / 2;
        }

        //assign it to the transform
        transform.position = newPosition;

    }
}
