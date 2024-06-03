using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalConstantValues;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Controllers/Player Controller")]
public class PlayerController : MonoBehaviour
{
    #region Variable Declaration
    private float forwardInput, turningInput, movementSpeed, rotationSpeed, distanceFromMouse;
    private readonly int screenMaxX = GlobalScreenReadonly.SCREEN_MAX_X;
    private readonly int screenMinX = GlobalScreenReadonly.SCREEN_MIN_X;
    private readonly int screenMaxZ = GlobalScreenReadonly.SCREEN_MAX_Z;
    private bool hasSlowDownTimePowerup, hasPowerup, isMouseControlEnabled;
    private UIMaster uiMaster;
    private GameMaster gameMaster;
    private Animator playerAnimator;
    [SerializeField] private ParticleSystem deathParticleEffect;
    [SerializeField] private ParticleSystem powerupPickupEffect;
    #endregion

    #region Player Initialization
    //Method to start before the first frame
    void Start() => InitializePlayer();

    //Method to create default values, and masters
    void InitializePlayer() 
    {
        uiMaster = GlobalMasterCreationReadonly.UiMaster;
        gameMaster = GlobalMasterCreationReadonly.GameMaster;
        playerAnimator = GetComponent<Animator>();
        movementSpeed = GlobalPlayerControllerReadonly.PLAYER_MAX;
        rotationSpeed = GlobalPlayerControllerReadonly.PLAYER_TURNING_SPEED;
        hasPowerup = false;
    }
    #endregion

    #region Player Update Methods
    //Method to call every frame
    void Update()
    {
        CheckPositionWithinBounds();    //Checks if the player is within bounds
        isMouseControlEnabled = uiMaster.IsPlayerMouseControlEnabled();

        if (!isMouseControlEnabled)
        {
            HandleKeyboardInput();  //Method for wasd input
        } 
        else 
        {
            HandleMouseInput();     //Method for mouse control
        }

        Destroy(!gameMaster.GetGameState() ? gameObject : null);    //Conditional to destory player if game has ended

    }

    void HandleKeyboardInput() 
    {
        forwardInput = Input.GetKey("s") || Input.GetKey("down") ? -1 : Input.GetKey("w") || Input.GetKey("up") ? 1 : 0;    //Conditionals to handle the w/up and s/down inputs
        turningInput = Input.GetKey("d") || Input.GetKey("right") ? 1 : Input.GetKey("a") || Input.GetKey("left") ? -1 : 0;     //Conditionals to hangle the a/left, and the d/right inputs

        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed * forwardInput);   //Moves the player 
        transform.Rotate(Vector3.up, rotationSpeed * turningInput * Time.deltaTime);    //Rotates the player
        playerAnimator.SetFloat("Speed_f", movementSpeed * forwardInput);       //sets the running animation
    }

    void HandleMouseInput()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);      //"always" moves the player
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //Gets the ray from the camera to the mouse position
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.LookAt(hit.point);    //Make the player look at the ray hitpoint
            distanceFromMouse = Vector3.Distance(transform.position, hit.point);    //Checks the distance between the player and the mouse position

            movementSpeed = distanceFromMouse switch
            {
                < GlobalPlayerControllerReadonly.PLAYER_MINIMUM_DISTANCE_FROM_MOUSE => -GlobalPlayerControllerReadonly.PLAYER_MAX,  //moves the player backward if mouse moves to close to the player
                < GlobalPlayerControllerReadonly.PLAYER_STOP_DISTANCE_FROM_MOUSE => GlobalPlayerControllerReadonly.PLAYER_MIN,  //stops the player once it reaches a certian spot away from the player
                < GlobalPlayerControllerReadonly.PLAYER_APPROACH_DISTANCE_FROM_MOUSE => GlobalPlayerControllerReadonly.PLAYER_MAX / 2,  //slows the player once it reaches a certian spot away from the player
                _ => GlobalPlayerControllerReadonly.PLAYER_MAX      //moves the player at a normal speed
            };
        }

        playerAnimator.SetFloat("Speed_f", movementSpeed);  //sets the running animation
    }
    #endregion

    #region Player Functionality Methods
    //Method to handle object triggers
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Enemy"))  //if the trigger's tag is a enemy
        {
            Instantiate(deathParticleEffect, transform.position, deathParticleEffect.transform.rotation);   //Create the death particle
            Destroy(gameObject);    //destory the player
            uiMaster.ShowGameOverScreen();      //show the game over screen
        }
        else if (other.CompareTag("SpeedPowerup") && !hasPowerup)   //if the trigger is a speed powerup and player doesnt have a powerup
        {
            Instantiate(powerupPickupEffect, other.transform.position, powerupPickupEffect.transform.rotation); //Create pickup particle
            hasPowerup = true;
            Destroy(other.gameObject);  //destroy the powerup from the scene
            StartCoroutine(ActivateSpeedPowerup());     //start the speed powerup coroutine
        }
        else if (other.CompareTag("SDTPowerup") && !hasPowerup)     //if the trigger is a slow down time powerup and player doesnt have a powerup
        {
            Instantiate(powerupPickupEffect, other.transform.position, powerupPickupEffect.transform.rotation);     //Create the pickup particle
            hasPowerup = true;
            Destroy(other.gameObject);  //destory the powerup
            StartCoroutine(ActivateSlowDownTimePowerup());      //start the slow down time powerup
        }
    }

    //Method to handle the speed powerup
    IEnumerator ActivateSpeedPowerup() 
    {
        return ExecutePowerup(GlobalPlayerControllerReadonly.PLAYER_SPEED_POWERUP_DURATION, //The duration of the powerup
        () =>
            {
                movementSpeed += GlobalPlayerControllerReadonly.PLAYER_SPEED_POWERUP_SPEED_MODIFIER;    //Increases the movement speed
                uiMaster.ToggleSpeedIndicator(true);    //turns the speed indicator on
            },
        () =>
            {
                movementSpeed -= GlobalPlayerControllerReadonly.PLAYER_SPEED_POWERUP_SPEED_MODIFIER;       //Returns speed to normal
                uiMaster.ToggleSpeedIndicator(false);       //turns off the speed indicator
            });     //Calls ExecutePowerup to handle all the logic
    }

    IEnumerator ActivateSlowDownTimePowerup() 
    {
        return ExecutePowerup(GlobalPlayerControllerReadonly.PLAYER_SLOW_DOWN_TIME_POWERUP_DURATION,    //the duration of the powerup
        () => 
            {
                hasSlowDownTimePowerup = true;
                uiMaster.ToggleSlowDownTimeIndicator(true);     //toggle the slow down time indicator to true
            }, 
        () =>
            {
                hasSlowDownTimePowerup = false;
                uiMaster.ToggleSlowDownTimeIndicator(false);    //toggle the slow down time indicator to false
            }); //Calls ExecutePowerup to handle all the logic
            
    }

    //Method to handle the powerup executions
    private IEnumerator ExecutePowerup(float duration, Action doBeforeStart = null, Action doAfterStart = null)
    {
        while(hasPowerup)
        {
            doBeforeStart?.Invoke();    //If the doBeforeStart is not null, do whatever that action was
            yield return new WaitForSeconds(duration);  //waits whatever the duration was
            doAfterStart?.Invoke(); //If the doAfterStart is not null, do whatever that action was
            hasPowerup = false;
        }
    }

    //Method to return if the player has the hasSlowDownTimePowerup
    public bool HasSlowDownTimePowerup() => hasSlowDownTimePowerup;

    //Method to insure player is in bounds
    public void CheckPositionWithinBounds() 
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenMinX, screenMaxX),
        transform.position.y,
        Mathf.Clamp(transform.position.z, -screenMaxZ, screenMaxZ));
    }
    #endregion
}
