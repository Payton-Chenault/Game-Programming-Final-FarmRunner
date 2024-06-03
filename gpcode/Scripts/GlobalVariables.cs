using UnityEngine;
using System;

namespace GlobalConstantValues 
{
    //CLASS TO HOLD THE MASTER CREATIONS TO BE USED THROUGHOUT THE FILES
    public static class GlobalMasterCreationReadonly
    {
        private static readonly Lazy<UIMaster> _uiMaster = new(() => GameObject.Find("UI Manager").GetComponent<UIMaster>());
        private static readonly Lazy<GameMaster> _gameMaster = new(() => GameObject.Find("Game Manager").GetComponent<GameMaster>());
        private static readonly Lazy<AudioMaster> _audioMaster = new(() => GameObject.Find("Audio Manager").GetComponent<AudioMaster>());
        private static readonly Lazy<SaveMaster> _saveMaster = new(() => GameObject.Find("Save Manager").GetComponent<SaveMaster>());

        public static UIMaster UiMaster => _uiMaster.Value;
        public static GameMaster GameMaster => _gameMaster.Value;
        public static AudioMaster AudioMaster => _audioMaster.Value;
        public static SaveMaster SaveMaster => _saveMaster.Value;
    }
    //CLASS TO HOLD THE SCREEN PARAMETERS
    public static class GlobalScreenReadonly
    {
        public const int SCREEN_MAX_X = -5;
        public const int SCREEN_MIN_X = -45;
        public const int SCREEN_MAX_Z = 11;
    }

    //CLASS TO HOLD CONSTANTS FROM THE GAME MASTER
    public static class GlobalGameMasterReadonly
    {
        public static readonly int SCORE_PER_ENEMY = 2;
        public static readonly int SCORE_PER_SECOND = 5;
        public static readonly float TIME_MINIMUM = 1f;
    }

    //CLASS TO HOLD THE CONSTANTS FROM THE SAVE MASTER
    public static class GlobalSaveMasterReadonly
    {
        public static readonly string SAVE_DATA_PATH = $"{Application.persistentDataPath}/PlayerSaveData/FarmRunnerPlayerData.save";
        public static readonly string SAVE_DATA_DIRECTORY = $"{Application.persistentDataPath}/PlayerSaveData";
        public static readonly string SAVE_DATA_FILE = "/FarmRunnerPlayerData.save";
    }

    //CLASS TO HOLD THE CONSTANTS FROM THE PLAYER CONTROLLER
    public static class GlobalPlayerControllerReadonly
    {
        public static readonly float PLAYER_MIN = 0.0f;
        public static readonly float PLAYER_MAX = 10f;
        public static readonly float PLAYER_TURNING_SPEED = 250f; 
        public static readonly float PLAYER_SPEED_POWERUP_DURATION = 5f;
        public static readonly float PLAYER_SPEED_POWERUP_SPEED_MODIFIER = 5;
        public static readonly float PLAYER_SLOW_DOWN_TIME_POWERUP_DURATION = 2.5f;
        public const float PLAYER_MINIMUM_DISTANCE_FROM_MOUSE = 2f;
        public const float PLAYER_STOP_DISTANCE_FROM_MOUSE = 3f;
        public const float PLAYER_APPROACH_DISTANCE_FROM_MOUSE = 4f;
    }

    public static class SpeedSelection 
    {

        public static readonly float ENEMY_MIN = 7f;
        public static readonly float ENEMY_MAX = 9f;
    }

    //ENUMS FOR DIFFICULTY SELECTION
    public enum DifficultySelection 
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Insane = 4,
    }
}
