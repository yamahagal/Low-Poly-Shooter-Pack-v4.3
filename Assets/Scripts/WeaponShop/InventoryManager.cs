using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace WeaponShop
{
    /// <summary>
    /// Менеджер инвентаря для чтения и записи JSON-файла.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        #region FIELDS

        [Header("Settings")]
        [Tooltip("Путь к файлу инвентаря.")]
        [SerializeField]
        private string inventoryFilePath = "saves/inventory_test.json";

        [Tooltip("Удалять невыбранные оружия из инвентаря.")]
        [SerializeField]
        private bool removeUnselectedWeapons = true;

        [Tooltip("Задержка перед удалением оружий (в секундах).")]
        [SerializeField]
        private float weaponRemovalDelay = 0.5f;

        private InventoryData inventoryData;
        private bool weaponsProcessed = false;

        #endregion

        #region EVENTS

        /// <summary>
        /// Событие изменения инвентаря.
        /// </summary>
        public event Action OnInventoryChanged;

        #endregion

        #region UNITY METHODS

        private void Awake()
        {
            LoadInventory();
        }

        private void Start()
        {
            if (removeUnselectedWeapons && !weaponsProcessed)
            {
                StartCoroutine(RemoveUnselectedWeaponsDelayed());
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveInventory();
            }
        }

        private void OnApplicationQuit()
        {
            SaveInventory();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Получает данные инвентаря.
        /// </summary>
        public InventoryData GetInventoryData()
        {
            return inventoryData;
        }

        /// <summary>
        /// Получает список всех оружий.
        /// </summary>
        public List<WeaponInventoryItem> GetAllWeapons()
        {
            if (inventoryData == null || inventoryData.weapons == null)
            {
                return new List<WeaponInventoryItem>();
            }
            return new List<WeaponInventoryItem>(inventoryData.weapons.Values);
        }

        /// <summary>
        /// Получает текущее оружие.
        /// </summary>
        public WeaponInventoryItem GetCurrentWeapon()
        {
            if (string.IsNullOrEmpty(inventoryData.currentWeaponId) ||
                inventoryData.weapons == null)
            {
                return null;
            }

            return inventoryData.weapons.ContainsKey(inventoryData.currentWeaponId)
                ? inventoryData.weapons[inventoryData.currentWeaponId]
                : null;
        }

        /// <summary>
        /// Проверяет, есть ли оружие в инвентаре.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        /// <returns>True, если оружие есть, иначе false.</returns>
        public bool HasWeapon(string weaponId)
        {
            return inventoryData != null &&
                   inventoryData.weapons != null &&
                   inventoryData.weapons.ContainsKey(weaponId) &&
                   inventoryData.weapons[weaponId].purchased;
        }

        /// <summary>
        /// Добавляет оружие в инвентарь.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        public void AddWeapon(string weaponId)
        {
            if (string.IsNullOrEmpty(weaponId))
            {
                Debug.LogError("Идентификатор оружия не может быть пустым.");
                return;
            }

            if (inventoryData.weapons == null)
            {
                inventoryData.weapons = new Dictionary<string, WeaponInventoryItem>();
            }

            if (HasWeapon(weaponId))
            {
                Debug.LogWarning($"Оружие {weaponId} уже есть в инвентаре.");
                return;
            }

            if (inventoryData.weapons.Count >= inventoryData.maxWeapons)
            {
                Debug.LogWarning($"Достигнут максимальный лимит оружия ({inventoryData.maxWeapons}).");
                return;
            }

            inventoryData.weapons.Add(weaponId, new WeaponInventoryItem
            {
                weaponId = weaponId,
                purchased = true,
                selected = false,
                addedTime = System.DateTime.UtcNow.ToString("o")
            });
            OnInventoryChanged?.Invoke();
            SaveInventory();
        }

        /// <summary>
        /// Удаляет оружие из инвентаря.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        public void RemoveWeapon(string weaponId)
        {
            if (!HasWeapon(weaponId))
            {
                Debug.LogWarning($"Оружие {weaponId} не найдено в инвентаре.");
                return;
            }

            if (inventoryData.currentWeaponId == weaponId)
            {
                Debug.LogWarning($"Нельзя удалить текущее оружие.");
                return;
            }

            inventoryData.weapons.Remove(weaponId);
            OnInventoryChanged?.Invoke();
            SaveInventory();
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
            if (!string.IsNullOrEmpty(inventoryData.currentWeaponId) &&
                inventoryData.weapons.ContainsKey(inventoryData.currentWeaponId))
            {
                inventoryData.weapons[inventoryData.currentWeaponId].selected = false;
            }

            // Устанавливаем новое текущее оружие
            inventoryData.currentWeaponId = weaponId;
            inventoryData.weapons[weaponId].selected = true;
             
            OnInventoryChanged?.Invoke();
            SaveInventory();
        }

        /// <summary>
        /// Сбрасывает текущее оружие.
        /// </summary>
        public void UnequipWeapon()
        {
            if (!string.IsNullOrEmpty(inventoryData.currentWeaponId) &&
                inventoryData.weapons != null &&
                inventoryData.weapons.ContainsKey(inventoryData.currentWeaponId))
            {
                inventoryData.weapons[inventoryData.currentWeaponId].selected = false;
            }

            inventoryData.currentWeaponId = null;
            OnInventoryChanged?.Invoke();
            SaveInventory();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Загружает инвентарь из файла.
        /// </summary>
        private void LoadInventory()
        {
            // Сначала пробуем загрузить из Assets (для редактирования)
            string assetsPath = Path.Combine(Application.dataPath, inventoryFilePath);
            
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
                     
                    int weaponCount = inventoryData.weapons != null ? inventoryData.weapons.Count : 0;
                    Debug.Log($"✓ Инвентарь загружен из Assets. Оружий: {weaponCount}, Текущее: {inventoryData.currentWeaponId}");
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Не удалось загрузить из Assets: {e.Message}");
                }
            }

            // Пробуем загрузить из persistentDataPath
            string fullPath = Path.Combine(Application.persistentDataPath, inventoryFilePath);
            
            if (!File.Exists(fullPath))
            {
                CreateDefaultInventory();
                Debug.Log("Файл инвентаря не найден в persistentDataPath. Создан новый.");
                return;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
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
                     
                    int weaponCount = inventoryData.weapons != null ? inventoryData.weapons.Count : 0;
                    Debug.Log($"✓ Инвентарь загружен из persistentDataPath. Оружий: {weaponCount}, Текущее: {inventoryData.currentWeaponId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка загрузки инвентаря: {e.Message}");
                CreateDefaultInventory();
            }
        }

        /// <summary>
        /// Сохраняет инвентарь в файл.
        /// </summary>
        private void SaveInventory()
        {
            string fullPath = Path.Combine(Application.persistentDataPath, inventoryFilePath);
            string directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                string json = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);
                File.WriteAllText(fullPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка сохранения инвентаря: {e.Message}");
            }
        }

        /// <summary>
        /// Создаёт инвентарь по умолчанию.
        /// </summary>
        private void CreateDefaultInventory()
        {
            inventoryData = new InventoryData
            {
                playerId = "test_player",
                currencyBalance = 1000,
                weapons = new Dictionary<string, WeaponInventoryItem>(),
                currentWeaponId = null,
                maxWeapons = 5
            };
        }

        /// <summary>
        /// Корутина для удаления невыбранных оружий с задержкой.
        /// </summary>
        private System.Collections.IEnumerator RemoveUnselectedWeaponsDelayed()
        {
            yield return new WaitForSeconds(weaponRemovalDelay);
            RemoveUnselectedWeapons();
        }

        /// <summary>
        /// Удаляет невыбранные оружия из инвентаря игрока.
        /// </summary>
        private void RemoveUnselectedWeapons()
        {
            Debug.Log("=== Начинаем удаление невыбранных оружий ===");
            
            if (inventoryData == null || inventoryData.weapons == null)
            {
                Debug.LogWarning("Данные инвентаря не загружены.");
                return;
            }

            Debug.Log($"Загружено оружий в JSON: {inventoryData.weapons.Count}");

            // Ищем все WeaponBehaviour в сцене
            var allWeapons = FindObjectsOfType<InfimaGames.LowPolyShooterPack.WeaponBehaviour>();
            Debug.Log($"Найдено WeaponBehaviour в сцене: {allWeapons.Length}");
            
            int removedCount = 0;
            int checkedCount = 0;
            var weaponsToDestroy = new System.Collections.Generic.List<GameObject>();

            foreach (var weapon in allWeapons)
            {
                string weaponName = weapon.name;
                checkedCount++;
                
                Debug.Log($"Проверяем оружие: '{weaponName}'");
                
                bool isSelected = IsWeaponSelected(weaponName);
                Debug.Log($"  Результат IsWeaponSelected: {isSelected}");
                
                if (!isSelected)
                {
                    Debug.Log($"  >>> Помечаем для удаления: {weaponName}");
                    weaponsToDestroy.Add(weapon.gameObject);
                    removedCount++;
                }
            }

            Debug.Log($"=== Проверено {checkedCount} оружий, помечено для удаления {removedCount} ===");
            weaponsProcessed = true;
            
            // Сначала обновляем инвентарь, чтобы убрать ссылки на удаляемые оружия
            RefreshInventoryWeapons();
            
            // Затем уничтожаем GameObjects (в следующем кадре, после обновления инвентаря)
            StartCoroutine(DestroyWeaponsNextFrame(weaponsToDestroy));
        }

        /// <summary>
        /// Уничтожает оружия в следующем кадре.
        /// </summary>
        private System.Collections.IEnumerator DestroyWeaponsNextFrame(System.Collections.Generic.List<GameObject> weaponsToDestroy)
        {
            yield return null; // Ждем следующего кадра
            
            Debug.Log($"=== Уничтожаем {weaponsToDestroy.Count} оружий ===");
            foreach (var weaponObj in weaponsToDestroy)
            {
                if (weaponObj != null)
                {
                    Debug.Log($"  Уничтожаем: {weaponObj.name}");
                    Destroy(weaponObj);
                }
            }
        }

        /// <summary>
        /// Проверяет, выбрано ли оружие по имени.
        /// </summary>
        private bool IsWeaponSelected(string weaponName)
        {
            Debug.Log($"    IsWeaponSelected('{weaponName}')");
            
            if (inventoryData == null || inventoryData.weapons == null)
            {
                Debug.Log($"      Данные инвентаря null, возвращаем false");
                return false;
            }

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
            var playerInventory = FindObjectOfType<InfimaGames.LowPolyShooterPack.Inventory>();
             
            if (playerInventory == null)
            {
                Debug.LogWarning("Компонент Inventory не найден на игроке.");
                return;
            }

            var inventoryType = playerInventory.GetType();
            var initMethod = inventoryType.GetMethod("Init", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
             
            if (initMethod != null)
            {
                initMethod.Invoke(playerInventory, new object[] { 0 });
                Debug.Log("Инвентарь обновлен после удаления невыбранных оружий.");
            }
        }

        #endregion
    }
}
