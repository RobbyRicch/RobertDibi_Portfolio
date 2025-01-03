using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingToLevelTrigger : MonoBehaviour
{
    [SerializeField] private CustomSceneManager _sceneManager;
    public CustomSceneManager SceneManager { get => _sceneManager; set => _sceneManager = value; }

    private const string _playerTag = "Player";
    private bool _hasPassed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_hasPassed && collision.CompareTag(_playerTag)) // need to fix
        {
            _hasPassed = true;
            SaveManager saveManager = SaveManager.Instance;
            /*saveManager.GameData.CurrentIntegrity = SaveManager.Instance.GameData.MaxIntegrity;
            if (saveManager.Player.PrimaryGun) saveManager.GameData.EquippedPrimaryId = saveManager.Player.PrimaryGun.Uid;
            if (saveManager.Player.SideArm) saveManager.GameData.EquippedSideArmId = saveManager.Player.SideArm.Uid;*/
            saveManager.SaveGame();
            //SaveDataManager.Instance.Player.LIS.ResetLink();
            _sceneManager.GoToLevel();
        }
    }
}
