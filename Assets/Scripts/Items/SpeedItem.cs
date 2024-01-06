using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyProject
{
    public class SpeedItem : MonoBehaviour, IUsable
    {
        #region Inspector

        [field: SerializeField] public UnityEvent OnUse { get; private set; }

        #endregion

        #region Fields

        protected float m_SpeedBoost = 1.0f;

        protected PlayerMovement m_PlayerMovement;

        #endregion

        #region Methods

        public void Use(Transform actor)
        {
            actor.TryGetComponent<PlayerMovement>(out m_PlayerMovement);
            if (m_PlayerMovement != null)
                m_PlayerMovement.AddSpeed(m_SpeedBoost);

            // PlayerMovementへ通知
            OnUse?.Invoke();

            //Destroy(gameObject);
            gameObject.SetActive(false);
        }

        #endregion
    }
}
