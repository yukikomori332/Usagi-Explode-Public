using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class WeaponController : MonoBehaviour
    {
        #region Inspector

        [Header("In Hand")]
        [SerializeField] private Transform inHand; public Transform InHand { get => inHand; set => inHand = value; }

        [Header("Throw Force")]
        [SerializeField, Range(0.0f, 50.0f)] private float throwForce = 5.0f; //Gun 20.0f //Grenade 5.0f

        [Header("Throwable Weapon")]
        [SerializeField] private Grenade grenade; public Grenade Grenade { get => grenade; set => grenade = value; }

        #endregion

        #region Fields

        public Grenade EquippedGrenade { get; private set; }

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            EquipWeapon(grenade, typeof(Grenade));
        }

        #endregion

        #region Methods

        public void EquipWeapon<T>(T weaponToEquip, System.Type objectType) where T : IThrowable
        {
            // すでに武器を装備していれば
            if (EquippedGrenade != null)
                Destroy(EquippedGrenade.gameObject);

            // タイプによる武器を装備
            if (typeof(Grenade) == objectType)
            {
                Grenade grenade = weaponToEquip as Grenade;

                EquippedGrenade = Instantiate(grenade, inHand) as Grenade;
                EquippedGrenade.transform.SetParent(inHand);
            }
        }

        public void Throw()
        {
            if (EquippedGrenade != null)
            {
                EquippedGrenade.transform.SetParent(null);

                EquippedGrenade.Throw(throwForce);
                EquippedGrenade = null;
            }

            EquipWeapon(grenade, typeof(Grenade));
        }

        //public void RespawnWeapon()
        //{

        //}

        #endregion
    }
}
