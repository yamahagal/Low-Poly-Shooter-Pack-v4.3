//Copyright 2022, Infima Games. All Rights Reserved.

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class Inventory : InventoryBehaviour
    {
        #region FIELDS

        /// <summary>
        /// Array of all weapons. These are gotten in the order that they are parented to this object.
        /// </summary>
        [SerializeField]        
        private WeaponBehaviour[] weapons;

        /// <summary>
        /// Currently equipped WeaponBehaviour.
        /// </summary>
        [SerializeField] 
        private WeaponBehaviour equipped;
        /// <summary>
        /// Currently equipped index.
        /// </summary>
        private int equippedIndex = -1;

        public SaveSystem.PlayerData data;
        public Material[] materials;

        #endregion

        #region METHODS

        public void Awake() {
            var saveSystem = new SaveSystem();
            data = saveSystem.Load("C:\\Unity\\Low Poly Shooter Pack v4.3\\Saves");
        }

        public override void Init(int equippedAtStart = 0)
        {
            

            bool destroy = true;
            Weapon[] _weap = GetComponentsInChildren<Weapon>(true);
            foreach (Weapon w in _weap) {
                destroy = true;
                foreach (WeaponShopSettings WSS in data.weapons) {
                    if (WSS.weaponName == w.weaponName) {
                        if (WSS.hasPlayerWeapon) {
                            destroy = false;
                            new AutoMaterial().MaterialChange(materials[WSS.currentSkinIndex], w.gameObject);
                            w.gameObject.SetActive(true);
                        }
                    }
                }
                if (destroy) {
                    Destroy(w.gameObject);
                }
            }

            //Cache all weapons. Beware that weapons need to be parented to the object this component is on!
            weapons = GetComponentsInChildren<WeaponBehaviour>();

            //Disable all weapons. This makes it easier for us to only activate the one we need.
            foreach (WeaponBehaviour weapon in weapons) {
                
                weapon.gameObject.SetActive(false);
            }
                

            //Equip.
            Equip(equippedAtStart);
        }

        public override WeaponBehaviour Equip(int index)
        {
            //If we have no weapons, we can't really equip anything.
            if (weapons == null)
                return equipped;
            
            //The index needs to be within the array's bounds.
            if (index > weapons.Length - 1)
                return equipped;

            //No point in allowing equipping the already-equipped weapon.
            if (equippedIndex == index)
                return equipped;
            
            //Disable the currently equipped weapon, if we have one.
            if (equipped != null)
                equipped.gameObject.SetActive(false);

            //Update index.
            equippedIndex = index;
            //Update equipped.
            equipped = weapons[equippedIndex];
            //Activate the newly-equipped weapon.
            equipped.gameObject.SetActive(true);

            //Return.
            return equipped;
        }
        
        #endregion

        #region Getters

        public override int GetLastIndex()
        {
            //Get last index with wrap around.
            int newIndex = equippedIndex - 1;
            if (newIndex < 0)
                newIndex = weapons.Length - 1;

            //Return.
            return newIndex;
        }

        public override int GetNextIndex()
        {
            //Get next index with wrap around.
            int newIndex = equippedIndex + 1;
            if (newIndex > weapons.Length - 1)
                newIndex = 0;

            //Return.
            return newIndex;
        }

        public override WeaponBehaviour GetEquipped() => equipped;
        public override int GetEquippedIndex() => equippedIndex;

        #endregion
    }
}