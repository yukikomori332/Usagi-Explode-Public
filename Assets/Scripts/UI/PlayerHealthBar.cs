using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject
{
    public class PlayerHealthBar : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image[] healthPoints;

        #endregion

        public virtual void UpdateHealthPoints(float health)
        {
            for (int i = 0; i < healthPoints.Length; i++)
            {
                healthPoints[i].enabled = true;
                health--;

                // PlayerのHPが0以上であれば、処理をスキップ
                if (health >= 0) continue;

                healthPoints[i].enabled = false;
            }
        }
    }
}
