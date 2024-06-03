using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalConstantValues;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Controllers/Enemy Controller")]
public class EnemyController : MonoBehaviour
{
    #region Variable Declaration
    private GameObject player;
    private GameMaster gameMaster;
    private float speed;
    private float savedSpeed;
    private float personalityPick;
    private readonly float screenMaxX = GlobalScreenReadonly.SCREEN_MAX_X;
    private readonly float screenMaxZ = GlobalScreenReadonly.SCREEN_MAX_Z;
    private readonly float screenMinX = GlobalScreenReadonly.SCREEN_MIN_X;
    private bool spawnedIn = false;
    private Animator enemyAnimator;
    private NavMeshAgent agent;
    private PlayerController playerController;
    #endregion

    #region Game Initialization
    //Method to assign variables
    void Initialize() 
    {
        CheckPositionInBounds();    //Checks if the enemy is spawned within the allowed area
        speed = Random.Range(SpeedSelection.ENEMY_MIN, SpeedSelection.ENEMY_MAX);       //Sets the speed of the enemy
        player = GameObject.Find("Player(Clone)");      //Finds the player
        playerController = player.GetComponent<PlayerController>();     //gets the player's controller
        gameMaster = GlobalMasterCreationReadonly.GameMaster;       //Assigns the gameMaster
        enemyAnimator = GetComponent<Animator>();       //Gets the Animator
        agent = GetComponent<NavMeshAgent>();       //Gets the NavMeshAgent
        personalityPick = Random.Range(1, 200);     //Picks a personality to use
    }

    //Method thats called before the first frame
    void Start()
    {
        Initialize();       //Calls initialize to assign the values needed
        StartCoroutine(SpawnEnemy());       //Starts the SpawnEnemy Coroutine
    }
    #endregion

    #region Updating Methods
    //Updates every frame
    void Update()
    {
        UpdateAgentSpeed();     //Checks the agent speed.
        CheckGameState();       //Checks the game state.
        UpdateAnimatorSpeed();      //Refreshes the animator to change it based off speed
    }

    //Updates at a fixed pace, better for preformence for larger tasks
    void FixedUpdate() 
    {
        UpdateAgentDestination(); //Updates the place where the enemy wants to end up
        CheckPositionInBounds();        //Checks to make sure the enemy is in bounds
    }

    //Method to update the Enemy's Agent's desitination
    void UpdateAgentDestination()
    {
        if (player == null) return;     //if the player doesnt exist, end method.

        Vector3 destination = player.transform.position;        //Gets the current player's destination
        destination.x = personalityPick >= 1 && personalityPick <= 25 ? destination.x + 2 : destination.x;      //if the personality is between 1 and 25, then it will go 2 places larger from the player's 'x'
        destination.x = personalityPick >= 51 && personalityPick <= 75 ? destination.x - 2 : destination.x;      //if the personality is between 51 and 75, then it will go 2 places lower from the player's 'x'
        destination.z = personalityPick >= 26 && personalityPick <= 50 ? destination.z + 2 : destination.z;      //if the personality is between 26 and 50, then it will go 2 places larger from the player's 'z'
        destination.z = personalityPick >= 76 && personalityPick <= 100 ? destination.z - 2: destination.z;      //if the personality is between 76 and 100, then it will go 2 places lower from the player's 'z'
        agent.SetDestination(destination);      //Sets the agents destination
    }

    //Method to update the agents speed based on if the player has the Slow Down Time Powerup
    void UpdateAgentSpeed() => agent.speed = playerController.HasSlowDownTimePowerup() ? speed / 2 : speed;
    //Method to update the speed of the enemy's animator
    void UpdateAnimatorSpeed() => enemyAnimator.SetFloat("Speed_f", speed);
    #endregion

    #region Enemy Checks + Spawning
    //Method to handle the enemy spawning
    IEnumerator SpawnEnemy() 
    {
        if (!spawnedIn)
        {
            CheckPositionInBounds();        //Checks if the enemy is in bounds once again
            savedSpeed = speed;     //saves the speed in a variable to be used for later
            speed = 0;      //sets the speed to zero
            yield return new WaitForSeconds(0.5f);  //waits for .5 seconds
            speed = savedSpeed;     //turns the speed to its saved speed
            spawnedIn = true;   //turns spawned in to true to end coroutine
        }
    }
    
    //Checks the game state, if it is false, then it destroys the enemy, if not then it does nothing
    void CheckGameState() => Destroy(!gameMaster.GetGameState() ? gameObject : null);
    //Checks if the enemy collided with a object bound, if it did then it destroys the enemy, if not, it does nothing
    void OnTriggerEnter(Collider other) => Destroy(other.CompareTag("ObjectBounds") ? gameObject : null);
    //Method to make sure enemy is always in bounds
    void CheckPositionInBounds() => transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenMinX, screenMaxX), 
                                    transform.position.y, Mathf.Clamp(transform.position.z, -screenMaxZ, screenMaxZ));
    #endregion
}
