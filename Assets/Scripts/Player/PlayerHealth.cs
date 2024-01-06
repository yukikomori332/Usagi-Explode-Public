using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Inspector

        [Header("Status")]
        [SerializeField] private float initHealth = 5.0f;
        [SerializeField] private float health;

        [Header("UI")]
        [SerializeField] private PlayerHealthBar playerHealthBar;

        [Header("Effect")]
        [SerializeField] private ParticleSystem deathEffect;

        #endregion

        #region Fields

        public event Action PlayerDied;

        public bool Dead { get => dead; private set => dead = value; }

        protected bool dead = false;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            health = initHealth;
            
            playerHealthBar.UpdateHealthPoints(health);
        }

        #endregion

        #region Methods

        public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            health -= damage;
            playerHealthBar.UpdateHealthPoints(health);

            if (health <= 0 && !dead)
            {
                Die(hitPoint, hitDirection);
            }
        }

        public virtual void Die(Vector3 hitPoint, Vector3 hitDirection)
        {
            dead = true;

            // GameplayManagerへ通知
            PlayerDied?.Invoke();

            // 死亡時エフェクトを生成
            ParticleSystem effect = Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as ParticleSystem;
            effect.Play();

            //GameObject.Destroy(gameObject);
            gameObject.SetActive(false);
        }

        public virtual void AddHealth(int healthBoost)
        {
            health += healthBoost;
            playerHealthBar.UpdateHealthPoints(health);
        }

        #endregion
    }
}
