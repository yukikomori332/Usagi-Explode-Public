using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MyProject
{
    public class Grenade : MonoBehaviour, IThrowable
    {
        #region Inspector

        [Header("LayerMask")]
        [SerializeField] private LayerMask forwardCollisionMask;
        [SerializeField] private LayerMask backwardCollisionMask;

        [Header("Damage")]
        [SerializeField] private float damage = 1.0f;

        [Header("Effect")]
        [SerializeField] ParticleSystem explosionEffect;

        [Header("Status")]
        [SerializeField] private bool isThrowing = false;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        #endregion

        #region Fields

        private float m_MoveSpeed = 5.0f;

        private SphereCollider m_SphereCollider;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            TryGetComponent<SphereCollider>(out m_SphereCollider);
        }

        private void Update()
        {
            if (!isThrowing) return;

            float moveDistance = m_MoveSpeed * Time.deltaTime;

            CheckCollisions(moveDistance);

            transform.Translate(Vector3.forward * moveDistance);
        }

        #endregion

        #region Methods

        public virtual void Throw(float force)
        {
            // 移動速度を決定
            m_MoveSpeed = force;

            // Sphereコライダを有効にする
            if (m_SphereCollider != null)
                m_SphereCollider.enabled = true;

            isThrowing = true;
        }

        protected virtual void CheckCollisions(float moveDistance)
        {
            Ray forwardRay = new Ray(transform.position, transform.forward);
            Ray backwardRay = new Ray(transform.position, -transform.forward * 0.5f);
            RaycastHit hit;

            // 前方にrayを作成
            if (Physics.Raycast(forwardRay, out hit, moveDistance, forwardCollisionMask, QueryTriggerInteraction.Collide))
            {
                HitObject(hit);
                Explode();
            }
            // 後方にrayを作成
            else if (Physics.Raycast(backwardRay, out hit, moveDistance, backwardCollisionMask, QueryTriggerInteraction.Collide))
            {
                HitObject(hit);
                Explode();
            }
        }

        protected virtual void HitObject(RaycastHit hit)
        {
            hit.collider.TryGetComponent<IDamageable>(out IDamageable damageableObject);
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(damage, hit.point, transform.forward);
            }
        }

        protected virtual void Explode()
        {
            // 爆発エフェクトを生成
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();

            // Play Sound
            AudioSource audio = Instantiate(audioSource, transform.position, Quaternion.identity);
            audio.Play();

            gameObject.SetActive(false);
        }

        #endregion
    }
}
