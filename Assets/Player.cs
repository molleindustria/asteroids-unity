using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject bulletPrefab;
    public Animator animator;
    public Game gameManager;

    //these are the constants
    public float thrustSpeed;
    public float maxSpeed;
    public float rotationSpeed;
    public float maxAngularVelocity;

    public float invulnerability;

    public Vector2 respawnPoint;

    //the name of the inputs as defined in project settings
    public string HORIZONTAL_AXIS = "Horizontal";
    public string VERTICAL_AXIS = "Vertical";
    public string FIRE = "Fire1";

    //to control engine sound and effects independently I use two audio sources
    public AudioSource engineAudioSource;
    public AudioSource effectsAudioSource;

    public AudioClip thrustSound;
    public AudioClip fireSound;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        gameManager = FindObjectOfType<Game>();

        //the thrust sound is always looping, I just turn the volume up and down
        engineAudioSource.clip = thrustSound;
        engineAudioSource.loop = true;
        engineAudioSource.Play();
        engineAudioSource.volume = 0;
    }


    private void Update()
    {
        //get the input
        float horizontalInput = Input.GetAxis(HORIZONTAL_AXIS);
        float verticalInput = Input.GetAxis(VERTICAL_AXIS);

        //phisics operations have to be multiplied by Time.deltaTime to be framerate independent
        rb.AddTorque(rotationSpeed * -horizontalInput * Time.deltaTime);

        if (verticalInput > 0)
        {
            //direction vector x speed constant x analog input
            rb.AddForce(transform.up * thrustSpeed * verticalInput * Time.deltaTime);
            
            engineAudioSource.volume = 1;
            animator.Play("thrust");
        }
        else
        {
            engineAudioSource.volume = 0;
            animator.Play("idle");
        }

        //limit the speed and angular velocity so it doesn't spin out of control
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, +maxAngularVelocity);

        //different input mode for the button
        if (Input.GetButtonDown(FIRE))
        {
            Vector3 bulletPosition = transform.position + transform.up * 0.2f;

            GameObject newBullet = Instantiate(bulletPrefab, bulletPosition, transform.rotation);

            effectsAudioSource.PlayOneShot(fireSound, 1);
        }



        //decrease the invulnerability timer
        invulnerability -= Time.deltaTime;

    }

    //this is called every time the ship is activated
    //eg after being "destroyed"
    private void OnEnable()
    {
        engineAudioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (invulnerability <= 0 && collision.gameObject.CompareTag("Asteroid"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;

            //communicate the event to the game manager
            gameManager.PlayerDeath(this);

        }
    }

}
