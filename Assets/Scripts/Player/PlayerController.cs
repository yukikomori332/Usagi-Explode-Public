using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class PlayerController : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private PlayerMovement playerMovement; public PlayerMovement PlayerMovement { get => playerMovement; }

        [SerializeField] private PlayerLooking playerLooking;

        [SerializeField] private PlayerHealth playerHealth; public PlayerHealth PlayerHealth { get => playerHealth; }

        [SerializeField] private WeaponController weaponController;

        [SerializeField] private PlayerThrowing playerThrowing;

        #endregion

        #region Fields

        public static List<PlayerController> playerControllers = new List<PlayerController>();

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            GameplayManager.StartedGame += OnStartedGame;
        }

        private void OnDisable()
        {
            GameplayManager.StartedGame -= OnStartedGame;
        }

        private void OnDestroy()
        {
            playerControllers.Remove(this);
        }

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            playerControllers.Add(this);

            // ゲーム開始前の処理
            // - ここに処理 -
        }

        private void OnStartedGame()
        {
            // ゲーム開始時の処理
            // - ここに処理 -
        }

        #endregion
    }
}
