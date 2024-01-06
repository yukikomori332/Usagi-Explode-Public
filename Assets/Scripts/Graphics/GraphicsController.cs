using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class GraphicsController : MonoBehaviour
    {
        public void UpdateQuality(int qualityIndex)
        {
            #region Methods

            QualitySettings.SetQualityLevel(qualityIndex);

            #endregion
        }
    }
}
