//Copyright 2022, Infima Games. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class Inventory : InventoryBehaviour
    {
        #region FIELDS

        /// <summary>
        /// Путь к файлу инвентаря.
        /// </summary>
        [SerializeField]
        private string inventoryFilePath = "saves/inventory_test.json";

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

        #endregion

        #region METHODS


        public override void Init(int equippedAtStart = 0) {
            RemoveUnselectedWeaponsFromJson();

            //Cache all weapons. Beware that weapons need to be parented to the object this component is on!
            weapons = GetComponentsInChildren<WeaponBehaviour>();

            //Disable all weapons. This makes it easier for us to only activate the one we need.
            foreach (WeaponBehaviour weapon in weapons) {
                weapon.gameObject.SetActive(false);
            }
                

            //Equip.
            Equip(equippedAtStart);
        }

        /// <summary>
        /// Удаляет невыбранные оружия из JSON-файла инвентаря.
        /// </summary>
        public void RemoveUnselectedWeaponsFromJson()
        {
            string fullPath = Path.Combine(Application.persistentDataPath, inventoryFilePath);
            
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"Файл инвентаря не найден: {fullPath}");
                return;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                var data = JsonConvert.DeserializeObject<InventoryData>(json);
                
                if (data == null || data.weapons == null)
                {
                    Debug.LogWarning("Данные инвентаря пусты.");
                    return;
                }

                var weaponsToRemove = new List<string>();
                
                foreach (var kvp in data.weapons)
                {
                    if (!kvp.Value.selected)
                    {
                        weaponsToRemove.Add(kvp.Key);
                    }
                }

                foreach (var weaponId in weaponsToRemove)
                {
                    data.weapons.Remove(weaponId);
                }

                string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(fullPath, updatedJson);
                
                Debug.Log($"Удалено {weaponsToRemove.Count} невыбранных оружий из JSON-файла.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при удалении невыбранных оружий: {e.Message}");
            }
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