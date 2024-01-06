using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class DashedLineRenderer : MonoBehaviour
    {

        #region Inspector

        [Header("Dashed Line")]
        [SerializeField] private Transform origin;

        [SerializeField, Range(2, 50)] private int maxPoints = 20;

        [SerializeField] private LayerMask collisionMask;

        [SerializeField] private Transform point;
        
        [SerializeField, Range(0.5f, 2f)] private float spaceBetweenPoints = 0.75f;

        [SerializeField, Range(1.05f, 2f)] private float rayOverlap = 1.1f;

        #endregion

        #region Fields

        private List<Transform> m_Points;

        private List<SpriteRenderer> m_SpriteRenderers;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            // 破線の数の要素を指定
            m_Points = new List<Transform>(maxPoints);
            m_SpriteRenderers = new List<SpriteRenderer>(maxPoints);

            Vector3 startPosition = origin.position;
            Vector3 nextPosition;
            // 破線インスタンスを生成
            for (int i = 0; i < maxPoints; i++)
            {
                Transform instance = Instantiate(point, startPosition, point.rotation) as Transform;
                m_Points.Add(instance);

                instance.TryGetComponent(out SpriteRenderer spriteRenderer);
                if (m_SpriteRenderers != null)
                    m_SpriteRenderers.Add(spriteRenderer);

                m_Points[i].SetParent(origin);
                nextPosition = startPosition + origin.forward * spaceBetweenPoints;
                startPosition = nextPosition;
            }
        }

        private void Update()
        {
            // ポーズ中なら
            if (Time.timeScale == 0) return;

            Vector3 nextPosition;
            RaycastHit hit;
            float overlap;
            bool collided = false;
            // 破線インスタンスを表示・非表示
            for (int i = 0; i < m_Points.Count; i++)
            {
                nextPosition = m_Points[i].position + transform.forward * spaceBetweenPoints;
                overlap = Vector3.Distance(m_Points[i].position, nextPosition) * rayOverlap;

                if (collided)
                    m_SpriteRenderers[i].enabled = false;
                else if (!collided && Physics.Raycast(m_Points[i].position, transform.forward, out hit, overlap, collisionMask))
                {
                    collided = true;
                    m_SpriteRenderers[i].enabled = false;
                }
                else if (!collided)
                    m_SpriteRenderers[i].enabled = true;
            }
        }

        #endregion
    }
}
