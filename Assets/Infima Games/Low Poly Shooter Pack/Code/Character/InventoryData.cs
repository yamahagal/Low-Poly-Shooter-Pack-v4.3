using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Данные об оружии для загрузки из JSON файла.
    /// </summary>
    [System.Serializable]
    public class WeaponDataJSON
    {
        [Tooltip("Уникальный идентификатор оружия.")]
        public string weaponId;

        [Tooltip("Название оружия.")]
        public string weaponName;

        [Tooltip("Тип оружия.")]
        public string weaponType;

        [Tooltip("Путь к префабу оружия.")]
        public string weaponPrefabPath;

        [Tooltip("Путь к иконке оружия.")]
        public string weaponIconPath;

        [Tooltip("Базовая стоимость оружия.")]
        public int baseCost;

        [Tooltip("Урон оружия.")]
        public float damage = 100f;

        [Tooltip("Скорострельность (выстрелов в минуту).")]
        public int fireRate = 200;

        [Tooltip("Ёмкость магазина.")]
        public int magazineCapacity = 30;

        [Tooltip("Время перезарядки (в секундах).")]
        public float reloadTime = 2.5f;

        [Tooltip("Точность (меньше = точнее).")]
        public float accuracy = 0.25f;

        [Tooltip("Дальность стрельбы.")]
        public float range = 100f;
    }

    /// <summary>
    /// Данные инвентаря игрока для тестирования.
    /// </summary>
    [System.Serializable]
    public class InventoryData
    {
        [Header("Player Info")]
        [Tooltip("Идентификатор игрока.")]
        public string playerId;

        [Tooltip("Баланс валюты.")]
        public int currencyBalance;

        [Header("Weapons")]
        [Tooltip("Словарь оружий в инвентаре (ключ - weaponId).")]
        public Dictionary<string, WeaponInventoryItem> weapons;

        [Header("Current Weapon")]
        [Tooltip("Идентификатор текущего оружия.")]
        public string currentWeaponId;

        [Header("Settings")]
        [Tooltip("Максимальное количество оружия в инвентаре.")]
        public int maxWeapons = 5;

        [Header("Weapon Definitions")]
        [Tooltip("Словарь определений всех доступных оружий (ключ - weaponId).")]
        public Dictionary<string, WeaponDataJSON> weaponDefinitions;

        #region PROPERTIES

        /// <summary>
        /// Получает словарь оружий в инвентаре.
        /// </summary>
        public Dictionary<string, WeaponInventoryItem> Weapons => weapons;

        /// <summary>
        /// Получает текущее оружие.
        /// </summary>
        public WeaponInventoryItem CurrentWeapon
        {
            get
            {
                if (string.IsNullOrEmpty(currentWeaponId) || weapons == null)
                {
                    return null;
                }

                return weapons.ContainsKey(currentWeaponId) ? weapons[currentWeaponId] : null;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Проверяет, есть ли оружие в инвентаре.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        /// <returns>True, если оружие есть в инвентаре, иначе false.</returns>
        public bool HasWeapon(string weaponId)
        {
            return weapons != null && weapons.ContainsKey(weaponId) && weapons[weaponId].purchased;
        }

        /// <summary>
        /// Добавляет оружие в инвентарь.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        public void AddWeapon(string weaponId)
        {
            if (weapons == null)
            {
                weapons = new Dictionary<string, WeaponInventoryItem>();
            }

            if (weapons.Count >= maxWeapons)
            {
                Debug.LogWarning($"Достигнут максимальный лимит оружия ({maxWeapons}).");
                return;
            }

            if (weapons.ContainsKey(weaponId))
            {
                Debug.LogWarning($"Оружие {weaponId} уже есть в инвентаре.");
                return;
            }

            weapons.Add(weaponId, new WeaponInventoryItem
            {
                weaponId = weaponId,
                purchased = true,
                selected = false,
                addedTime = System.DateTime.UtcNow.ToString("o")
            });
        }

        /// <summary>
        /// Устанавливает текущее оружие.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        public void EquipWeapon(string weaponId)
        {
            if (!HasWeapon(weaponId))
            {
                Debug.LogWarning($"Оружие {weaponId} не найдено в инвентаре.");
                return;
            }

            // Снимаем выделение с предыдущего оружия
            if (!string.IsNullOrEmpty(currentWeaponId) && weapons.ContainsKey(currentWeaponId))
            {
                weapons[currentWeaponId].selected = false;
            }

            // Устанавливаем новое текущее оружие
            currentWeaponId = weaponId;
            weapons[weaponId].selected = true;
        }

        /// <summary>
        /// Удаляет оружие из инвентаря.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        public void RemoveWeapon(string weaponId)
        {
            if (weapons == null || !weapons.ContainsKey(weaponId))
            {
                return;
            }

            weapons.Remove(weaponId);

            if (currentWeaponId == weaponId)
            {
                currentWeaponId = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// Данные об оружии в инвентаре.
    /// </summary>
    [System.Serializable]
    public class WeaponInventoryItem
    {
        [Tooltip("Идентификатор оружия.")]
        public string weaponId;

        [Tooltip("Куплено ли оружие.")]
        public bool purchased;

        [Tooltip("Выбрано ли оружие.")]
        public bool selected;

        [Tooltip("Время добавления.")]
        public string addedTime;
    }
}
