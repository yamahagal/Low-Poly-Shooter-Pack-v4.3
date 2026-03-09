using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Game;
using Newtonsoft.Json;

namespace WeaponShop
{
    /// <summary>
    /// Менеджер для фильтрации оружия в инвентаре.
    /// Загружает данные из JSON файла и удаляет невыбранные оружия из инвентаря игрока.
    /// </summary>
    public class WeaponInventoryManager : MonoBehaviour
    {
        #region FIELDS

        [Header("Settings")]
        [Tooltip("Путь к файлу инвентаря.")]
        [SerializeField]
        private string inventoryFilePath = "saves/inventory_test.json";

        [Tooltip("Задержка перед удалением оружий (в секундах).")]
        [SerializeField]
        private float initializationDelay = 0.5f;

        private InventoryData inventoryData;
        private bool weaponsProcessed = false;

        #endregion

        #region UNITY METHODS

        private void Awake()
        {
            LoadInventoryData();
        }

        private void Start()
        {
            // Удаляем невыбранные оружия с задержкой после инициализации инвентаря
            if (!weaponsProcessed)
            {
                StartCoroutine(RemoveUnselectedWeaponsDelayed());
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Загружает данные инвентаря из JSON.
        /// </summary>
        private void LoadInventoryData()
        {
            Debug.Log("=== Начинаем загрузку инвентаря ===");
            
            // Сначала пробуем загрузить из Assets (для редактирования)
            string assetsPath = Path.Combine(Application.dataPath, inventoryFilePath);
            Debug.Log($"Путь к Assets: {assetsPath}");
            Debug.Log($"Файл существует в Assets: {File.Exists(assetsPath)}");
             
            if (File.Exists(assetsPath))
            {
                try
                {
                    string json = File.ReadAllText(assetsPath);
                    inventoryData = JsonConvert.DeserializeObject<InventoryData>(json);
                     
                    // Инициализируем словари если null
                    if (inventoryData.weapons == null)
                    {
                        inventoryData.weapons = new Dictionary<string, WeaponInventoryItem>();
                    }
                    if (inventoryData.weaponDefinitions == null)
                    {
                        inventoryData.weaponDefinitions = new Dictionary<string, WeaponDataJSON>();
                    }
                     
                    int weaponCount = inventoryData.weaponDefinitions != null ? inventoryData.weaponDefinitions.Count : 0;
                    Debug.Log($"✓ Инвентарь загружен из Assets. Оружий: {weaponCount}");
                    return;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Не удалось загрузить из Assets: {e.Message}");
                }
            }

            // Пробуем загрузить из persistentDataPath
            string persistentPath = Path.Combine(Application.persistentDataPath, inventoryFilePath);
            Debug.Log($"Путь к persistentDataPath: {persistentPath}");
            Debug.Log($"Файл существует в persistentDataPath: {File.Exists(persistentPath)}");
             
            if (File.Exists(persistentPath))
            {
                try
                {
                    string json = File.ReadAllText(persistentPath);
                    inventoryData = JsonConvert.DeserializeObject<InventoryData>(json);
                     
                    // Инициализируем словари если null
                    if (inventoryData.weapons == null)
                    {
                        inventoryData.weapons = new Dictionary<string, WeaponInventoryItem>();
                    }
                    if (inventoryData.weaponDefinitions == null)
                    {
                        inventoryData.weaponDefinitions = new Dictionary<string, WeaponDataJSON>();
                    }
                     
                    int weaponCount = inventoryData.weaponDefinitions != null ? inventoryData.weaponDefinitions.Count : 0;
                    Debug.Log($"✓ Инвентарь загружен из persistentDataPath. Оружий: {weaponCount}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Не удалось загрузить инвентарь: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Файл инвентаря не найден: {persistentPath}");
            }
            
            Debug.Log($"=== Загрузка инвентаря завершена. inventoryData == null: {inventoryData == null} ===");
        }

        /// <summary>
        /// Корутина для удаления невыбранных оружий с задержкой.
        /// </summary>
        private System.Collections.IEnumerator RemoveUnselectedWeaponsDelayed()
        {
            yield return new WaitForSeconds(initializationDelay);
            RemoveUnselectedWeapons();
        }

        /// <summary>
        /// Удаляет невыбранные оружия из инвентаря игрока.
        /// </summary>
        private void RemoveUnselectedWeapons()
        {
            Debug.Log("=== Начинаем удаление невыбранных оружий ===");
            
            if (inventoryData == null)
            {
                Debug.LogError("inventoryData == null");
                return;
            }
            
            if (inventoryData.weapons == null)
            {
                Debug.LogError("inventoryData.weapons == null");
                return;
            }

            Debug.Log($"Загружено оружий в JSON: {inventoryData.weapons.Count}");
            foreach (var kvp in inventoryData.weapons)
            {
                Debug.Log($"  {kvp.Key}: selected={kvp.Value.selected}");
            }

            // Ищем все WeaponBehaviour в сцене
            WeaponBehaviour[] allWeapons = FindObjectsOfType<WeaponBehaviour>();
            Debug.Log($"Найдено WeaponBehaviour в сцене: {allWeapons.Length}");
            
            int removedCount = 0;
            int checkedCount = 0;

            foreach (WeaponBehaviour weapon in allWeapons)
            {
                // Получаем имя оружия
                string weaponName = weapon.name;
                checkedCount++;
                
                Debug.Log($"Проверяем оружие: '{weaponName}'");
                
                // Проверяем, выбрано ли оружие
                bool isSelected = IsWeaponSelected(weaponName);
                Debug.Log($"  Результат IsWeaponSelected: {isSelected}");
                
                if (!isSelected)
                {
                    // Удаляем оружие из сцены
                    Debug.Log($"  >>> УДАЛЯЕМ невыбранное оружие: {weaponName}");
                    Destroy(weapon.gameObject);
                    removedCount++;
                }
            }

            Debug.Log($"=== Проверено {checkedCount} оружий, удалено {removedCount} ===");
            weaponsProcessed = true;
            
            // Пересоздаем массив оружий в инвентаре
            RefreshInventoryWeapons();
        }

        /// <summary>
        /// Проверяет, выбрано ли оружие по имени.
        /// </summary>
        private bool IsWeaponSelected(string weaponName)
        {
            Debug.Log($"    IsWeaponSelected('{weaponName}')");
            
            if (inventoryData == null)
            {
                Debug.Log($"      inventoryData == null, возвращаем false");
                return false;
            }
            
            if (inventoryData.weapons == null)
            {
                Debug.Log($"      inventoryData.weapons == null, возвращаем false");
                return false;
            }

            // Ищем оружие в словаре по имени
            foreach (var kvp in inventoryData.weapons)
            {
                Debug.Log($"      Проверяем: weaponId='{kvp.Value.weaponId}', selected={kvp.Value.selected}");
                
                if (kvp.Value.weaponId == weaponName || weaponName.Contains(kvp.Value.weaponId))
                {
                    Debug.Log($"      ✓ Совпадение найдено! Возвращаем {kvp.Value.selected}");
                    return kvp.Value.selected;
                }
            }

            Debug.Log($"      Совпадений не найдено, возвращаем false");
            return false;
        }

        /// <summary>
        /// Обновляет массив оружий в инвентаре после удаления невыбранных.
        /// </summary>
        private void RefreshInventoryWeapons()
        {
            // Ищем компонент Inventory на игроке
            var playerInventoryComponent = FindObjectOfType<InfimaGames.LowPolyShooterPack.Inventory>();
             
            if (playerInventoryComponent == null)
            {
                Debug.LogWarning("Компонент Inventory не найден на игроке.");
                return;
            }

            // Вызываем Init для обновления массива оружий
            var inventoryType = playerInventoryComponent.GetType();
            var initMethod = inventoryType.GetMethod("Init", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
             
            if (initMethod != null)
            {
                // Переинициализируем инвентарь
                initMethod.Invoke(playerInventoryComponent, new object[] { 0 });
                Debug.Log("Инвентарь обновлен после удаления невыбранных оружий.");
            }
        }

        #endregion
    }
}
