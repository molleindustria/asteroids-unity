using UnityEngine;


public class Asteroid : MonoBehaviour
{
    public Game gameManager;

    public new Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    public int asteroidFragments = 2;

    public float size = 3; //size is 3 2 1 
    public float scaleMultiplier = 0.5f;

    public float movementSpeed = 50f;
    public float maxLifetime = 30f;

    private void Awake()
    {
        gameManager = FindObjectOfType<Game>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        // Assign random rotation
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));

        // Set the scale and mass of the asteroid based on the assigned size
        transform.localScale = Vector3.one * scaleMultiplier * size;

        //greater mass > greater size
        //rigidbody.mass = size;

    }

    public void Push(Vector2 direction, float movementSpeed)
    {
        rigidbody.AddForce(direction * movementSpeed);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Check if the asteroid is large enough to split in half
            if (size > 1)
            {
                //for each fragment
                for (int i = 0; i < asteroidFragments; i++)
                {
                    Asteroid newAsteroid = CreateFragment(size-1);

                    // Set a random trajectory 
                    newAsteroid.Push(Random.insideUnitCircle.normalized, movementSpeed);
                }

            }
            else
            {
                // setting the size to zero to check the clearing condition
                size = 0;
            }

            //notify the game manager
            gameManager.AsteroidDestroyed(this);

            // Destroy the current asteroid since it is either replaced by two
            // new asteroids or small enough to be destroyed by the bullet
            Destroy(gameObject);
        }
    }

    private Asteroid CreateFragment(float newSize)
    {
        // Set the new asteroid poistion to be the same as the current asteroid
        // but with a slight offset so they do not spawn inside each other
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;

        //randomize the prefab to instantiate
        Asteroid prefab = gameManager.asteroidPrefabs[Random.Range(0, gameManager.asteroidPrefabs.Length - 1)];

        //create the instance from the prefab
        Asteroid newAsteroid = Instantiate(prefab, position, transform.rotation);

        newAsteroid.size = newSize;

        return newAsteroid;
    }

}
