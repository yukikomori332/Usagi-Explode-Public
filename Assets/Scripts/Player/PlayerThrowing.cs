using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProject
{
    public class PlayerThrowing : MonoBehaviour
    {
        #region Inspector



        #endregion

        #region Fields

        private PlayerInput m_PlayerInput;

        private WeaponController m_WeaponController;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            TryGetComponent<PlayerInput>(out m_PlayerInput);
            TryGetComponent<WeaponController>(out m_WeaponController);

            m_PlayerInput.actions["Fire"].started += OnFire;
        }

        private void OnDestroy()
        {
            m_PlayerInput.actions["Fire"].started -= OnFire;
        }

        #endregion

        #region Methods

        public void OnFire(InputAction.CallbackContext context)
        {
            if (Time.timeScale == 0) return;

            m_WeaponController.Throw();
        }

        #endregion
    }
}
