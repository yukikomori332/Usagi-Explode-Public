using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProject
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Inspector

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5.0f;

        [Header("LayerMask")]
        [SerializeField] private LayerMask forwardCollisionMask;
        [SerializeField] private LayerMask backwardCollisionMask;

        #endregion

        #region Fields

        public event Action<float> UpdatedMoveSpeed;

        private const float k_SpeedBoostModifier = 0.5f;

        private float m_CurrentSpeedBoost;

        private Vector2 m_MoveDirection;

        private Rigidbody m_Rigidbody;

        private PlayerInput m_PlayerInput;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            TryGetComponent<Rigidbody>(out m_Rigidbody);
            TryGetComponent<PlayerInput>(out m_PlayerInput);
        }

        private void Update()
        {
            if (m_PlayerInput != null)
                m_MoveDirection = m_PlayerInput.actions["Move"].ReadValue<Vector2>();

            float moveDistance = moveSpeed * Time.deltaTime;

            CheckCollisions(moveDistance);
        }

        private void FixedUpdate()
        {
            Move(m_MoveDirection);
        }

        #endregion

        #region Methods

        private void Move(Vector2 direction)
        {
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            Vector3 moveVelocity = moveDirection.normalized * moveSpeed;

            // 【メモ書き】
            //#if UNITY_EDITOR
#if UNITY_ANDROID
            //if (moveDirection != Vector3.zero)
            //{
            //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.15f);
            //}
#endif
            m_Rigidbody.MovePosition(m_Rigidbody.position + moveVelocity * Time.deltaTime);
        }

        protected virtual void CheckCollisions(float moveDistance)
        {
            Ray forwardRay = new Ray(transform.position + Vector3.up, transform.forward);
            Ray backwardRay = new Ray(transform.position + Vector3.up, -transform.forward * 0.5f);
            RaycastHit hit;

            // 前方にrayを作成
            if (Physics.Raycast(forwardRay, out hit, moveDistance, forwardCollisionMask, QueryTriggerInteraction.Collide))
            {
                HitObject(hit);
            }
            // 後方にrayを作成
            else if (Physics.Raycast(backwardRay, out hit, moveDistance, backwardCollisionMask, QueryTriggerInteraction.Collide))
            {
                HitObject(hit);
            }
        }

        protected virtual void HitObject(RaycastHit hit)
        {
            hit.collider.TryGetComponent<IUsable>(out IUsable usableObject);
            if (usableObject != null)
            {
                usableObject.Use(transform);
            }
        }

        public void AddSpeed(float speedBoost)
        {
            float totalSpeedBoost = k_SpeedBoostModifier * speedBoost;
            m_CurrentSpeedBoost += totalSpeedBoost;
            moveSpeed += m_CurrentSpeedBoost;

            // GUIManagerにイベントを通知
            UpdatedMoveSpeed?.Invoke(m_CurrentSpeedBoost);
        }

        #endregion
    }
}
