using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalConstantValues;

[AddComponentMenu("Modifier/Destroy On Game End")]
public class DestroyOnGameEnd : MonoBehaviour
{
    private GameMaster _gameMaster;
    void Start()
    {
        _gameMaster = GlobalMasterCreationReadonly.GameMaster;
    }
    // Update is called once per frame
    void Update()
    {
        Destroy(_gameMaster.GetGameState() ? null : gameObject);
    }

}
