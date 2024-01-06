using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyProject
{
    public class HealItem : MonoBehaviour, IUsable
    {
        #region Inspector

        [field: SerializeField] public UnityEvent OnUse { get; private set; }

        #endregion

        #region Fields

        protected int m_HealthBoost = 1;

        protected PlayerHealth m_PlayerHealth;

        #endregion

        #region Methods

        public void Use(Transform actor)
        {
            actor.TryGetComponent<PlayerHealth>(out m_PlayerHealth);
            if (m_PlayerHealth != null)
                m_PlayerHealth.AddHealth(m_HealthBoost);

            // PlayerHealthへ通知
            OnUse?.Invoke();

            //Destroy(gameObject);
            gameObject.SetActive(false);
        }

        #endregion
    }
}
