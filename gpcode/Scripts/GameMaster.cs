using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GlobalConstantValues;

[AddComponentMenu("Masters/Game Master")]
public class GameMaster : MonoBehaviour
{
    #region Variable Declaration
    [Tooltip("List of prefab enemy GameObjects to spawn during game")]
    [SerializeField] private  List<GameObject> enemies;
    [Space(5)]

    [Tooltip("List of prefab obstacle GameObjects to spawn during game.")]
    [SerializeField] private List<GameObject> obstacles;
    [Space(5)]

    [Tooltip("List of prefab powerup GameObjects to spawn during game.")]
    [SerializeField] private List<GameObject> powerups;
    [Space(5)]

    [Tooltip("Prefab player GameObject to spawn at the start of the game.")]
    [SerializeField] private GameObject player;

    //Bool Fields
    private bool isGameActive = false;
    private bool isTimeBonusActive = false;

    //Int Fields
    private int currentEnemyCount;
    private int maxEnemyCount;
    private int score;
    private int difficulty;

    //Float Fields
    private float timeRemaining;
    private float spawnInterval;
    private const float maxTimeScale = 1f;
    private const float minTimeScale = .2f;

    //Master Fields
    private UIMaster uiMaster;
    private SaveMaster saveMaster;

    //Vector 3 Fields
    private Vector3 enemySpawnPosition;

    //Readonly Fields
    private readonly int screenMaxX = GlobalScreenReadonly.SCREEN_MAX_X;
    private readonly int screenMinX = GlobalScreenReadonly.SCREEN_MIN_X;
    private readonly int screenMaxZ = GlobalScreenReadonly.SCREEN_MAX_Z;
    private readonly float minTimeRemaining = GlobalGameMasterReadonly.TIME_MINIMUM;
    private readonly int pointsPerEnemy = GlobalGameMasterReadonly.SCORE_PER_ENEMY;
    private readonly int pointsPerSecond = GlobalGameMasterReadonly.SCORE_PER_SECOND;
    #endregion

    #region Game Initialization
    //Method that is called before the first frame
    void Start() => InitializeGame();

    //Method to create masters and set default values
    void InitializeGame() 
    {
        uiMaster = GlobalMasterCreationReadonly.UiMaster;
        saveMaster = GlobalMasterCreationReadonly.SaveMaster;
        difficulty = 1;
        timeRemaining = 99;
    }

    //Method to handle starting the game
    public void StartGame(int selectedDifficulty) 
    {   //Sets all the default vars based on the selectedDifficulty
        score = 0;
        difficulty = selectedDifficulty;
        maxEnemyCount = 5 * difficulty;
        timeRemaining = 25 * difficulty;
        spawnInterval = 8 / difficulty;
        currentEnemyCount = 0;
        isTimeBonusActive = false;
        
        uiMaster.ShowGameUIScreen();    //Shows the game ui
        
        Vector3 playerSpawnPosition = new Vector3(Random.Range(screenMinX, screenMaxX), 0, Random.Range(-screenMaxZ, screenMaxZ));
        Instantiate(player, playerSpawnPosition, Quaternion.Euler(0, 0, 0));    //Creates player
        
        isGameActive = true;
        StartCoroutine(SpawnEnemies());     //Starts CoRoutines
        StartCoroutine(SpawnObstacles());
        StartCoroutine(SpawnPowerups());
    }
    #endregion

    #region Coroutine Methods
    //Coroutine for handling the spawning of enemies
    IEnumerator SpawnEnemies() 
    {
        while (isGameActive) 
        {
            yield return new WaitForSeconds(spawnInterval);     //Wait for the spawn interval

            if (currentEnemyCount < maxEnemyCount) //Checks to see if the current enemy count is less then the max
            {
                Vector3 spawnPosition = new Vector3(Random.Range(screenMinX + 4, screenMaxX - 4), -0.4294128f, Random.Range(-screenMaxZ + 4, screenMaxZ - 4)) + player.transform.position; //Creates the spawn position for the enemy
                Instantiate(enemies[Random.Range(0, enemies.Count)], spawnPosition, Quaternion.identity);   //Creates the enemy into the scene
                currentEnemyCount++;        //Increases the enemy count
                score += pointsPerEnemy;        //Adds the score based on the points per enemy
            } 
        }
    }

    //Method to handle the spawning on obstacles
    IEnumerator SpawnObstacles() 
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(10);        //Waits 10 seconds
            Vector3 spawnPosition = new Vector3(Random.Range(screenMinX, screenMaxX), 0, Random.Range(-screenMaxZ, screenMaxZ));    //Creates the obstacles position
            Instantiate(obstacles[Random.Range(0, obstacles.Count)], spawnPosition, Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));      //Creates the obstacle in the scene
        }
    }

    //Method to handle the spawning of powerups
    IEnumerator SpawnPowerups() 
    {
        while (isGameActive) 
        {
            yield return new WaitForSeconds(25);        //Wait for 25 seconds
            Vector3 spawnPosition = new Vector3(Random.Range(screenMinX, screenMaxX), 0, Random.Range(-screenMaxZ, screenMaxZ));        //Creats the pwrup's positon
            Instantiate(powerups[Random.Range(0, powerups.Count)], spawnPosition, Quaternion.Euler(90, 0, 0));      //Creates the pwrup in the scene
        }
    }

    //Method to manage the time bonus given near the end of the game
    IEnumerator ActivateTimeBonus() 
    {
        while (isGameActive && timeRemaining > minTimeRemaining) 
        {
            score += pointsPerSecond;       //Score is added on by the points per second
            yield return new WaitForSeconds(1);     //Wait one second
        }
    }
    #endregion

    #region Game Functions

    //Method to be ran every frame
    void Update() 
    {
        if (!isGameActive) return;  //If game is not active, then end method

        if (currentEnemyCount == maxEnemyCount && timeRemaining > minTimeRemaining) //if the current enemy count is the same as the max, and the time remaining is less then the minimum time needed, continue
        {
            timeRemaining -= Time.deltaTime;    //subtracts Time.deltaTime from timeRemaining

            if (!isTimeBonusActive) //if the time bonus is false
            {
                StartCoroutine(ActivateTimeBonus());        //start the time bonus
                isTimeBonusActive = true;
            }
        }

        if (timeRemaining <= minTimeRemaining)
        {
            EndGame();
        }
    }

    //Method to handle ending the game
    private void EndGame()
    {
        uiMaster.ShowGameOverScreen();
        saveMaster.UpdateGameCompletion(difficulty);
        timeRemaining = float.MaxValue;
        isGameActive = false;
        StopAllCoroutines();
    }

    //method to set the game state from other files 
    public void SetGameState(bool state)
    {
        isGameActive = state;
        if (!isGameActive) StopAllCoroutines();
    }

    //method to restart the game from other files
    public void RestartGame() 
    {
        SetGameState(false);
        StartGame(difficulty);
    }

    //method to pause the game from other files
    public void PauseGame() => Time.timeScale = 0;

    //method to resume the game from other files
    public void ResumeGame() => Time.timeScale = maxTimeScale;
    #endregion

    #region Get Game Information
    //method to return the current game state
    public bool GetGameState() => isGameActive;
    
    //method to return the current score
    public int GetScore() => score;

    //method to get the current enemy count
    public int GetCurrentEnemyCount() => currentEnemyCount;

    //method to get the max enemy count
    public int GetMaxEnemyCount() => maxEnemyCount;

    //method to get the currently active difficulty
    public int GetDifficulty() => difficulty;

    //method to get the time remaining
    public float GetTimeRemaining() => timeRemaining;
    #endregion
}
