using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GlobalConstantValues;

[AddComponentMenu("Masters/UI Master")]
public class UIMaster : MonoBehaviour
{
    #region Variable Declarations
    [Header("UI Scenes: ")]
    [SerializeField] private GameObject titleUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject resetUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject speedIndicator;
    [SerializeField] private GameObject slowDownTimeIndicator;
    [Space(20)]

    [Header("Interactable User Items: ")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Toggle playerMouseControlToggle;
    [Space(20)]

    [Header("Modifiable Text Items: ")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gameInfoText;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI pauseMenuScoreText;
    [Space(20)]

    private GameMaster gameMaster;
    private AudioMaster audioMaster;
    private SaveMaster saveMaster;
    private bool isPlayerMouseControlEnabled;

    private Dictionary<UIScreen, GameObject> uiScreens;
    #endregion
    
    #region Initialization
    //Method called before first frame
    void Start()
    {
        InitializeMasters();        //Initializes the masters
        InitializeUIScreenDictionary();     //Creates the UIScreen Dictionary
    }

    //Method to create all game masters used in this master
    void InitializeMasters()
    {
        gameMaster = GlobalMasterCreationReadonly.GameMaster;
        audioMaster = GlobalMasterCreationReadonly.AudioMaster;
        saveMaster = GlobalMasterCreationReadonly.SaveMaster;
    }

    //Creates the UIScreen Dictionary
    void InitializeUIScreenDictionary() 
    {
        uiScreens = new Dictionary<UIScreen, GameObject>
        {
            { UIScreen.Title, titleUI },
            { UIScreen.Game, gameUI },
            { UIScreen.GameOver, gameOverUI },
            { UIScreen.Options, optionsUI },
            { UIScreen.Tutorial, tutorialUI },
            { UIScreen.Reset, resetUI },
            { UIScreen.Pause, pauseUI }
        };
    }
    #endregion

    #region UI Switching Methods
    //Method to change the current UI State
    public void DisplayUI(UIScreen screenToShow, Action additionalAction = null)
    {
        ActivateUI(screenToShow);       //Method that changed the UI to the one wanted from screenToShow
        additionalAction?.Invoke();     //starts whatever action the user assined in additionalAction IF it is not null, marked by the '?'
    }

    //Calls DisplayUI to show the game over screen
    public void ShowGameOverScreen() => DisplayUI(UIScreen.GameOver, () =>
    {
        finalScoreText.text = gameMaster.GetScore().ToString();     //Gets the score from the gameMaster, and sets it as the final score text for the end screen.
        highScoreText.text = saveMaster.GetHighscore(gameMaster.GetDifficulty()).ToString();        //Gets the highscore from the saveMaster, and sets it in the highscore text field for the end screen.
        gameMaster.SetGameState(false); //Sets the game's state to false, destroying all items on the screen
    });

    //Method to show the game UI screen during gameplay, this shows things such as time left, the pause button, etc.
    public void ShowGameUIScreen() => DisplayUI(UIScreen.Game, () => gameMaster.SetGameState(true));

    //Method to show the tutorial UI when the button is clicked in the Title Screen.
    public void ShowTutorialScreen() => DisplayUI(UIScreen.Tutorial, () =>
    {
        //Tutorial text gets set here instead of inside the unity editior to allow tutorial to show the score per enemy/second without having to change it in the editor also.
        tutorialText.text = $"- Goal: Avoid getting caught.\n" +
        $"- Scoring: +{GlobalGameMasterReadonly.SCORE_PER_ENEMY} per new enemy spawned, " +
        $"+{GlobalGameMasterReadonly.SCORE_PER_SECOND} per second survived after last wave.\n" +
        $"- Controls: WASD or Arrow keys for movement. Alternatively, use mouse control. Space to restart, ESC to pause.\n" +
        $"- Have Fun!";
    });

    //Method to show the Pause UI from the GameUI screen
    public void ShowPauseMenu() => DisplayUI(UIScreen.Pause, () =>
    {
        pauseMenuScoreText.text = $"Score: {gameMaster.GetScore()}";    //Sets the score in the pause menu from gameMaster
        gameMaster.PauseGame();     //Calls the gameMaster to pause the game
    });

    //Method to call the Main Menu or "Title Screen" 
    public void ShowMainMenu() => DisplayUI(UIScreen.Title, () =>
    {
        gameMaster.SetGameState(false); //Sets the game's state to false
        gameMaster.ResumeGame();    //Calls the gameMaster's resume game to make sure that game runs again next time its started
    });

    //Method to call the Reset Screen UI from the Options menu
    public void ShowResetScreen() => DisplayUI(UIScreen.Reset);

    //Method to call the Options Screen UI from the Title Menu
    public void ShowOptionsMenu() => DisplayUI(UIScreen.Options);
    #endregion

    #region UI Functionality Methods
    //Method to switch from the pause menu back to the game UI
    public void ResumeGame() => DisplayUI(UIScreen.Game, () => gameMaster.ResumeGame());

    //Method to go back to the Title screen UI after data reset has completed
    public void ResetGameData()
    {
        DisplayUI(UIScreen.Title, () =>
        {
            saveMaster.ResetData();     //Calls saveMaster to reset the save data
            Debug.LogWarning("Data Reset Completed");       //Prints out warning to the console
        });
    }

    //Method to quit the game when it is ran as a standalone executable.
    public void QuitGame()
    {
        saveMaster.SaveGameToJSON();        //Calls saveMaster to save the current game state
        Application.Quit();     //Calls unity's built in method to quit an application
    }

    //Method to change the volume of the game audio, send the volume slider to the audioMaster
    public void AdjustVolume() => audioMaster.ChangeVolume(musicVolumeSlider.value);

    //Method to restart the game inside the GameUI
    public void RestartGame() => gameMaster.RestartGame();

    //Method to toggle the speed powerup indicator
    public void ToggleSpeedIndicator(bool isEnabled) => speedIndicator.SetActive(isEnabled);

    //Method to toggle the slow down time indicator
    public void ToggleSlowDownTimeIndicator(bool isEnabled) => slowDownTimeIndicator.SetActive(isEnabled);

    //Method to return if the user has selected the mouse control button found in settings.
    public bool IsPlayerMouseControlEnabled() => isPlayerMouseControlEnabled;
    #endregion

    #region UI Updating Methods
    void Update()
    {
        CheckKeyBindings();     //Calls the CheckKeyBindings to constantly check key presses
        UpdateUIText();
    }

    //Method to check for key presses, and do actions according to them
    void CheckKeyBindings()
    {
        isPlayerMouseControlEnabled = playerMouseControlToggle.isOn;    //Assigns the isPlayerMouseControlEnabled based on the mouse conrol toggle found inside options
        if (!gameMaster.GetGameState() && Input.GetKey("space")) RestartGame(); //Checks if the space key is pressed, and the game's state is false, and then calls the RestartGame method if those conditions are met
        else if (gameMaster.GetGameState() && Input.GetKey("escape")) ShowPauseMenu(); //Checks if the esc key is pressed, and and the game's state is true, and then calls the ShowPauseMenu if those conditions are met
    }

    //Method to update the UI's text based on certian conditions
    void UpdateUIText()
    {
        if (gameMaster.GetGameState()) scoreText.text = $"Score: {gameMaster.GetScore()}";  //Checks if the game's state is true, and then assigns the scoreText based off of the gameMaster's returned GetScore int.

        int enemiesAmount = gameMaster.GetCurrentEnemyCount();  //Assigns the enimiesAmount and maxEnemiesAmount based on the gameMasters current count of both.
        int maxEnemiesAmount = gameMaster.GetMaxEnemyCount();   // ^^^^
        gameInfoText.text = enemiesAmount != maxEnemiesAmount   //Checks if the enemiesAmount is not the same as the max amount, if it is not, then it displays the enemeies amount over the max amount (6/12)
            ? $"{enemiesAmount}/{maxEnemiesAmount}"
            : $"Time Left: {Math.Floor(gameMaster.GetTimeRemaining())}";        //If they are, then it shows the time left, called from the gameMaster
    }

    //Method to only show certian UI Screens
    void ActivateUI(params UIScreen[] screensToActivate)
    {
        DeactivateAllUI();      //Disables all UI Screens before the UI Screens are enabled
        foreach (var screen in screensToActivate)
        {
            if (uiScreens.TryGetValue(screen, out var uiElement))       //Calls the dictionary to try to get the value of the inputed screensToActivate, and returns the uiElement GameObject to be used.
            {
                uiElement.SetActive(true);      //Sets the returned uiElements to true.
            }
            else
            {
                throw new ApplicationException("Invalid UI Screen trying to be activated");     //Throws a ApplicationError if it could'nt find the screen to activate
            }
        }
    }

    //Method to deactivate all screens
    void DeactivateAllUI()
    {
        foreach (var uiElement in uiScreens.Values)
        {
            uiElement.SetActive(false);     //Gets the GameObjects from the dictionary to set to false. 
        }
    }
    #endregion
}

//Enum values for all screens used
public enum UIScreen
{
    Title,
    Game,
    GameOver,
    Options,
    Tutorial,
    Reset,
    Pause
}
