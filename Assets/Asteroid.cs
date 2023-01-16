using UnityEngine;


public class Asteroid : MonoBehaviour
{
    public Game gameManager;

    public new Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    public float size = 1f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50f;
    public float maxLifetime = 30f;

    private void Awake()
    {
        gameManager = FindObjectOfType<Game>();

        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if(rigidbody == null)
            rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Assign random properties to make each asteroid feel unique
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));

        // Set the scale and mass of the asteroid based on the assigned size so
        // the physics is more realistic
        transform.localScale = Vector3.one * size;
        rigidbody.mass = size;
        
    }

    public void SetTrajectory(Vector2 direction)
    {
        // The asteroid only needs a force to be added once since they have no
        // drag to make them stop moving
        rigidbody.AddForce(direction * movementSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Check if the asteroid is large enough to split in half
            // (both parts must be greater than the minimum size)
            if ((size * 0.5f) >= minSize)
            {
                CreateSplit();
                CreateSplit();
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

    private Asteroid CreateSplit()
    {
        // Set the new asteroid poistion to be the same as the current asteroid
        // but with a slight offset so they do not spawn inside each other
        Vector2 position = transform.position;
        position += Random.insideUnitCircle * 0.5f;

        // Create the new asteroid at half the size of the current
        // Instantiate "this" means clone the gameobject
        Asteroid half = Instantiate(this, position, transform.rotation);
        half.size = size * 0.5f;

        // Set a random trajectory
        half.SetTrajectory(Random.insideUnitCircle.normalized);

        return half;
    }

}
