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

            GetWeaponList(equippedAtStart);
        }


        public void GetWeaponList(int equippedAtStart)
        {
            //Cache all weapons. Beware that weapons need to be parented to the object this component is on!
            weapons = GetComponentsInChildren<WeaponBehaviour>(true);

            //Disable all weapons. This makes it easier for us to only activate the one we need.
            foreach (WeaponBehaviour weapon in weapons) {
                weapon.gameObject.SetActive(false);
            }
                 
            //Equip.
            Equip(equippedAtStart);
        }

        /// <summary>
        /// Удаляет невыбранные оружия из инвентаря игрока на основе данных из JSON-файла.
        /// </summary>
        public void RemoveUnselectedWeaponsFromJson()
        {
            string fullPath = Path.Combine(Application.dataPath, inventoryFilePath);
            
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

                // Получаем все оружия в инвентаре игрока
                var allWeapons = GetComponentsInChildren<WeaponBehaviour>(true);
                
                int removedCount = 0;
                var weaponsToDestroy = new System.Collections.Generic.List<GameObject>();

                foreach (var weapon in allWeapons)
                {
                    string weaponName = weapon.name;
                    bool isSelected = false;
                    
                    // Проверяем, выбрано ли оружие в JSON
                    foreach (var kvp in data.weapons)
                    {
                        if (weaponName.IndexOf(kvp.Value.weaponId, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (kvp.Value.selected)
                            {
                                isSelected = true;
                            }
                            break;
                        }
                    }
                    
                    // Если оружие не выбрано, добавляем в список на удаление
                    if (!isSelected)
                    {
                        weaponsToDestroy.Add(weapon.gameObject);
                        removedCount++;
                    }
                }

                // Удаляем GameObjects мгновенно
                foreach (var weaponObj in weaponsToDestroy)
                {
                    if (weaponObj != null)
                    {
                        DestroyImmediate(weaponObj);
                    }
                }
                
                Debug.Log($"Удалено {removedCount} невыбранных оружий из инвентаря. Осталось: {allWeapons.Length - removedCount}");
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