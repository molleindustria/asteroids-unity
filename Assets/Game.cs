using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public Player player;
    public Asteroid asteroidPrefab;
    public ParticleSystem explosionEffect;
    public GameObject gameOverUI;

    public int score = 0;
    public int lives = 3;

    public int asteroidsPerWave = 3;
    public float spawnMargin = 1f;

    public Text scoreText;
    public Text livesText;

    public float respawnDelay = 2;
    public float respawnInvulnerability = 2;

    public AudioSource audioSource;
    public AudioClip smallExplosionSound;
    public AudioClip mediumExplosionSound;
    public AudioClip bigExplosionSound;


    //executed at the beginning
    private void Start()
    {
        //update the text
        scoreText.text = score.ToString();
        livesText.text = lives.ToString();

        gameOverUI.SetActive(false);
        SpawnPlayer();
        AsteroidWave();
        
    }

    //executed continuously
    private void Update()
    {
        //restart whole scene 0 lives and return pressed
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

   
    //called at the beginning and when the played dies
    public void SpawnPlayer()
    {
        //initial position
        player.transform.position = player.respawnPoint;
        player.invulnerability = respawnInvulnerability;
        player.gameObject.SetActive(true);
    }

    //spawn a number of asteroids
    public void AsteroidWave()
    {
        //this is the width of the screen in world units (it depends on the camera settings)
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
        float screenHeight = Camera.main.orthographicSize * 2;

        for (int i = 0; i < asteroidsPerWave; i++)
        {
            
            //randomize the side in which the asteroid spawns
            //since it's in integer it will be only 0, 1, 2, 3 
            int side = Random.Range(0, 4);
            
            Vector2 spawnPoint;

            //top
            if (side == 0)
            {
                //random x point on the top edge + add a margin so it's right out of the screen
                spawnPoint = new Vector2(Random.Range(-screenWidth / 2, screenWidth / 2), screenHeight / 2 + spawnMargin);
            }
            else if (side == 1)
            {
                //bottom
                spawnPoint = new Vector2(Random.Range(-screenWidth / 2, screenWidth / 2), -screenHeight / 2 - spawnMargin);
                //trajectory = new Vector2(Random.Range(-1f, 1f), 1);
            }
            else if (side == 2)
            {
                //left
                spawnPoint = new Vector2(-screenWidth / 2 - spawnMargin, Random.Range(-screenHeight / 2, screenHeight / 2));
                
            }
            else 
            {
                //right
                spawnPoint = new Vector2(screenWidth / 2 +spawnMargin, Random.Range(-screenHeight / 2, screenHeight / 2));
                
            }

            
            //create the instance from the prefab
            Asteroid newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.transform.position = spawnPoint;
            
            //add a force in the defined trajectory
            newAsteroid.SetTrajectory(Random.insideUnitCircle);
            
        }
    }


    //called by the asteroid
    public void AsteroidDestroyed(Asteroid asteroid)
    {
        //move the particle effect to the point and play it
        //the particles exist and die independently so I just need one spawner for everything
        explosionEffect.transform.position = asteroid.transform.position;
        explosionEffect.Play();

        //score based on size
        if (asteroid.size < 0.7f)
        {
            score += 100; // small asteroid
            audioSource.PlayOneShot(smallExplosionSound, 1);
        }
        else if (asteroid.size < 1.4f)
        {
            score += 50; // medium asteroid
            audioSource.PlayOneShot(mediumExplosionSound, 1);
        }
        else
        {
            score += 25; // large asteroid
            audioSource.PlayOneShot(bigExplosionSound, 1);
        }

        //update the score text
        scoreText.text = score.ToString();

        //check the winning condition
        //get all the asteroids in the scene
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        //problem a: Destroy() takes effect at the end of the frame
        //so I can't check when there are no asteroids from this function called upon destruction
        //instead I set size to 0 to the destroyed asteroids and check that value for all asteroids
        bool stageClear = true;
        
        //cycle through all the asteroids put each in variable a
        foreach (Asteroid a in asteroids)
        {
            //check the size var
            if (a.size > 0)
                stageClear = false;
        }

        //if stage clear is still true there are no asteroids around
        if(stageClear)
        {
            print("Wave over");
            //increase the asteroids spawned until death
            asteroidsPerWave++;
            AsteroidWave();
        }
    }
    

    //called by the player
    public void PlayerDeath(Player player)
    {
        explosionEffect.transform.position = player.transform.position;
        explosionEffect.Play();

        //disable the game object until respawn
        player.gameObject.SetActive(false);

        //subtract lifes
        lives--;

        //visualize on the interface
        livesText.text = lives.ToString();

        //I need to play the sound from here because the spaceship and its audio source 
        //is being destroyed
        audioSource.PlayOneShot(bigExplosionSound, 1);

        //check game over condition
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            //invoke calls a function by name after a delay of x seconds
            Invoke("SpawnPlayer", respawnDelay);
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }


}
