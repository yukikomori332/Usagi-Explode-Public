using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyProject
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        #region Enum

        public enum State { Idle, Chasing, Attacking };
        private State m_CurrentState;

        #endregion

        #region Inspector

        [Header("Distination")]
        [SerializeField] private Transform targetTransform;

        [Header("Status")]
        [SerializeField] private float initHealth = 1f;
        [SerializeField] private float health;

        [SerializeField] private float attackDistanceThreshold = 0.5f;
        [SerializeField] private float timeBetweenAttacks = 1.0f;

        [Header("Damage")]
        [SerializeField] private float damage = 1.0f;

        [Header("Effect")]
        [SerializeField] private ParticleSystem deathEffect;

        #endregion

        #region Fields

        public event Action DiedEnemy;

        public bool Dead { get => dead; private set => dead = value; }

        protected bool dead = false;

        private float m_NextAttackTime;

        private CapsuleCollider m_CapsuleCollider;

        private CapsuleCollider m_TargetCapsuleCollider;

        private NavMeshAgent m_NavMeshAgent;

        private PlayerHealth m_PlayerHealth;

        #endregion

        #region MonoBehaviour

        protected virtual void Update()
        {
            // transformが取得できていなければ
            if (targetTransform == null) return;

            // チェイス状態なら
            if (m_CurrentState == State.Chasing)
            {
                Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
                Vector3 targetPosition = targetTransform.position - dirToTarget * (m_CapsuleCollider.radius + m_TargetCapsuleCollider.radius + attackDistanceThreshold / 2);

                if (!dead)
                    m_NavMeshAgent.SetDestination(targetPosition);
                else
                    m_NavMeshAgent.enabled = false;
            }

            // プレイヤーへ攻撃可能なら
            m_NextAttackTime += Time.deltaTime;
            if (m_NextAttackTime > timeBetweenAttacks)
            {
                float sqrDstToTarget = (targetTransform.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + m_CapsuleCollider.radius + m_TargetCapsuleCollider.radius, 2))
                {
                    m_NextAttackTime = 0;

                    // Play Sound
                    // - ここに処理を書く -

                    StartCoroutine(Attack());
                }
            }
        }

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            TryGetComponent<CapsuleCollider>(out m_CapsuleCollider);
            TryGetComponent<NavMeshAgent>(out m_NavMeshAgent);

            // HPの初期化
            health = initHealth;

            // transformの取得
            if (targetTransform == null)
            {
                foreach (PlayerController playerController in PlayerController.playerControllers)
                {
                    if (playerController.name == "Player")
                    {
                        targetTransform = playerController.transform;
                        targetTransform.TryGetComponent<CapsuleCollider>(out m_TargetCapsuleCollider);

                        // プレイヤーのHPを取得
                        m_PlayerHealth = playerController.PlayerHealth;
                    }
                }
            }

            m_CurrentState = State.Chasing;
        }

        public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            health -= damage;

            if (health <= 0 && !dead)
            {
                Die(hitPoint, hitDirection);
            }
        }

        protected virtual void Die(Vector3 hitPoint, Vector3 hitDirection)
        {
            dead = true;

            // EnemySpawnerへイベントを通知
            DiedEnemy?.Invoke();

            // 死亡時エフェクトを生成
            ParticleSystem effect = Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as ParticleSystem;
            effect.Play();

            gameObject.SetActive(false);
        }

        protected virtual IEnumerator Attack()
        {
            // 攻撃状態
            m_CurrentState = State.Attacking;
            m_NavMeshAgent.enabled = false;

            Vector3 originalPosition = transform.position + Vector3.back / 2;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            Vector3 attackPosition = targetTransform.position - dirToTarget * (m_CapsuleCollider.radius + m_TargetCapsuleCollider.radius　/* 本来、m_CapsuleCollider.radiusのみ */);

            float attackSpeed = 3;
            float percent = 0;

            while (percent <= 1)
            {
                // Playerを攻撃
                m_PlayerHealth.TryGetComponent<IDamageable>(out IDamageable damageableObject);
                if (damageableObject != null)
                    damageableObject.TakeDamage(damage, transform.position, transform.forward);

                percent += Time.deltaTime * attackSpeed;
                float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

                yield return null;
            }

            // チェイス状態
            m_CurrentState = State.Chasing;
            m_NavMeshAgent.enabled = true;
        }

        #endregion
    }
}
