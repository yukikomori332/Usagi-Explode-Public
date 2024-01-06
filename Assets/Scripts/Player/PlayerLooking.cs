using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class PlayerLooking : MonoBehaviour
    {
        #region Inspector

        [Header("Cameras")]
        [SerializeField] private Camera aimCamera;

        #endregion

        #region Fields



        #endregion

        #region MonoBehaviour

        private void Update()
        {
            if (Time.timeScale == 0) return;
#if UNITY_ANDROID

#else
            Ray ray = aimCamera.ScreenPointToRay(/*Fix*/Input.mousePosition);
#endif
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                Look(point);

                Debug.DrawRay(ray.origin, point, Color.red);
            }
        }

        #endregion

        #region Methods

        private void Look(Vector3 lookPoint)
        {
            Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
            transform.LookAt(heightCorrectedPoint);
        }

        #endregion
    }
}
