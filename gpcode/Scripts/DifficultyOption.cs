using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalConstantValues;

[RequireComponent(typeof(Button))]
[AddComponentMenu("Miscellaneous/Difficulty Selector")]
public class DifficultyOption : MonoBehaviour
{
    #region Variable Declaration
    private Button difficultyButton;
    private GameMaster gameMaster;
    private SaveMaster saveMaster;
    public DifficultySelection difficulty;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    void Start() => Initialize();

    //Method to create all the Masters and Components
    void Initialize()
    {
        saveMaster = GlobalMasterCreationReadonly.SaveMaster;
        gameMaster = GlobalMasterCreationReadonly.GameMaster;
        difficultyButton = GetComponent<Button>();
        difficultyButton.onClick.AddListener(SetDifficulty);
    }
    #endregion

    #region Update Methods
    // Update is called once per frame
    void Update() => difficultyButton.interactable = saveMaster.GetGameCompletion((int)difficulty);
    #endregion

    #region Difficulty Methods
    //Starts the game with the selected difficulty
    public void SetDifficulty() => gameMaster.StartGame((int)difficulty);
    #endregion
}
