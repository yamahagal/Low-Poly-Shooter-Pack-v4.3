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
        private float weaponRemovalDelay = 0f;

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
            RemoveUnselectedWeapons();
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
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Не удалось загрузить из Assets: {e.Message}");
                }
            }

            // Пробуем загрузить из persistentDataPath
            string fullPath = Path.Combine(Application.persistentDataPath, inventoryFilePath);

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
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка загрузки инвентаря: {e.Message}");
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
        /// Удаляет невыбранные оружия из инвентаря игрока.
        /// </summary>
        private void RemoveUnselectedWeapons()
        {
            if (inventoryData == null || inventoryData.weapons == null)
            {
                Debug.LogWarning("Данные инвентаря не загружены.");
                return;
            }

            // Находим инвентарь игрока и получаем все оружия (включая неактивные)
            var playerInventory = FindObjectOfType<InfimaGames.LowPolyShooterPack.Inventory>();
            if (playerInventory == null)
            {
                Debug.LogWarning("Компонент Inventory не найден на игроке.");
                return;
            }

            // Получаем экипированное оружие через рефлексию
            var inventoryType = playerInventory.GetType();
            var equippedField = inventoryType.GetField("equipped", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            InfimaGames.LowPolyShooterPack.WeaponBehaviour equippedWeapon = null;
            if (equippedField != null)
            {
                equippedWeapon = equippedField.GetValue(playerInventory) as InfimaGames.LowPolyShooterPack.WeaponBehaviour;
            }

            // Получаем все дочерние WeaponBehaviour из инвентаря игрока (включая неактивные)
            var allWeapons = playerInventory.GetComponentsInChildren<InfimaGames.LowPolyShooterPack.WeaponBehaviour>(true);
            
            int removedCount = 0;
            int checkedCount = 0;
            var weaponsToDestroy = new System.Collections.Generic.List<GameObject>();

            foreach (var weapon in allWeapons)
            {
                string weaponName = weapon.name;
                checkedCount++;
                
                bool isSelected = IsWeaponSelected(weaponName);
                
                if (!isSelected)
                {
                    weaponsToDestroy.Add(weapon.gameObject);
                    removedCount++;
                    
                    // Проверяем, будет ли удалено экипированное оружие
                    if (equippedWeapon != null && weapon == equippedWeapon)
                    {
                        // Деактивируем модель оружия, чтобы убрать её из рук игрока
                        weapon.gameObject.SetActive(false);
                    }
                }
            }

            weaponsProcessed = true;
             
            // Сначала уничтожаем GameObjects (в следующем кадре)
            foreach (var weaponObj in weaponsToDestroy)
            {
                if (weaponObj != null)
                {
                    Destroy(weaponObj);
                }
            }
            //StartCoroutine(DestroyWeaponsNextFrame(weaponsToDestroy));
             
            // Затем обновляем инвентарь после удаления (задержка чтобы удаления успели выполниться)
            
            var inventory = FindObjectOfType<InfimaGames.LowPolyShooterPack.Inventory>();
            Debug.Log("-------------------Inventory----------------------");
            if (inventory != null) {
                Debug.Log("-------------------Init----------------------");
                inventory.Init(0);
            }
            var character = FindObjectOfType<InfimaGames.LowPolyShooterPack.Character>();
            character.RefreshWeaponSetup();
            //StartCoroutine(RefreshInventoryAfterDestroy());
        }

        /// <summary>
        /// Обновляет инвентарь после удаления оружий с задержкой.
        /// </summary>
        private System.Collections.IEnumerator RefreshInventoryAfterDestroy()
        {
            // Ждем 3 кадра чтобы удаления успели выполниться
            yield return null;
            yield return null;
            yield return null;
            
            RefreshInventoryWeapons();
        }

        /// <summary>
        /// Уничтожает оружия в следующем кадре.
        /// </summary>
        private System.Collections.IEnumerator DestroyWeaponsNextFrame(System.Collections.Generic.List<GameObject> weaponsToDestroy)
        {
            yield return null; // Ждем следующего кадра
            
            foreach (var weaponObj in weaponsToDestroy)
            {
                if (weaponObj != null)
                {
                    Destroy(weaponObj);
                }
            }
        }

        /// <summary>
        /// Проверяет, выбрано ли оружие по имени.
        /// </summary>
        private bool IsWeaponSelected(string weaponName)
        {
            if (inventoryData == null || inventoryData.weapons == null)
            {
                return false;
            }

            foreach (var kvp in inventoryData.weapons)
            {
                // Нечувствительная к регистру проверка - имена в сцене имеют префикс P_LPSP_WEP_ и заглавные буквы
                if (weaponName.IndexOf(kvp.Value.weaponId, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return kvp.Value.selected;
                }
            }

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

            // Получаем тип инвентаря для рефлексии
            var inventoryType = playerInventory.GetType();

            // Проверяем, есть ли ещё оружия в инвентаре
            var remainingWeapons = playerInventory.GetComponentsInChildren<InfimaGames.LowPolyShooterPack.WeaponBehaviour>(true);
             
            if (remainingWeapons == null || remainingWeapons.Length == 0)
            {
                Debug.LogWarning("В инвентаре нет оружий. Игрок остаётся без оружия.");
                // Сбрасываем состояние инвентаря через рефлексию
                var equippedField = inventoryType.GetField("equipped", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var equippedIndexField = inventoryType.GetField("equippedIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (equippedField != null)
                    equippedField.SetValue(playerInventory, null);
                if (equippedIndexField != null)
                    equippedIndexField.SetValue(playerInventory, -1);
                    
                return;
            }

            // Находим индекс выбранного оружия в JSON
            int selectedWeaponIndex = 0;
            string selectedWeaponId = inventoryData?.currentWeaponId;
            
            if (!string.IsNullOrEmpty(selectedWeaponId))
            {
                // Ищем оружие с selected: true
                foreach (var kvp in inventoryData.weapons)
                {
                    if (kvp.Value.selected)
                    {
                        selectedWeaponId = kvp.Value.weaponId;
                        break;
                    }
                }
            }
            
            // Находим индекс выбранного оружия среди оставшихся оружий
            if (!string.IsNullOrEmpty(selectedWeaponId))
            {
                Debug.LogWarning("Выбранное оружие не найдено, используем первое оружие.");
                selectedWeaponIndex = 0;
            }
            else
            {
                // Ищем индекс выбранного оружия
                for (int i = 0; i < remainingWeapons.Length; i++)
                {
                    string weaponName = remainingWeapons[i].name;
                    if (weaponName.IndexOf(selectedWeaponId, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        selectedWeaponIndex = i;
                        break;
                    }
                }
            }
            
            // Если выбранное оружие не найдено среди оставшихся, используем первое
            if (selectedWeaponIndex >= remainingWeapons.Length)
            {
                Debug.LogWarning($"Выбранное оружие {selectedWeaponId} не найдено в инвентаре, используем первое оружие.");
                selectedWeaponIndex = 0;
            }
             
            // Принудительно вызываем ForceRefreshInventory() для обновления инвентаря и экипирования выбранного оружия
            var refreshMethod = inventoryType.GetMethod("ForceRefreshInventory", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (refreshMethod != null)
            {
                refreshMethod.Invoke(playerInventory, System.Reflection.BindingFlags.InvokeMethod, null, new object[] { selectedWeaponIndex }, System.Globalization.CultureInfo.CurrentCulture);
            }
             
            // Получаем компонент Character для обновления анимации
            var character = FindObjectOfType<InfimaGames.LowPolyShooterPack.Character>();
            if (character != null)
            {
                var characterType = character.GetType();
                
                // Вызываем SetHolstered(false) для корректного состояния аниматора
                var setHolsteredMethod = characterType.GetMethod("SetHolstered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (setHolsteredMethod != null)
                {
                    setHolsteredMethod.Invoke(character, System.Reflection.BindingFlags.InvokeMethod, null, new object[] { false }, System.Globalization.CultureInfo.CurrentCulture);
                }
                
                // Получаем аниматор персонажа через рефлексию
                var characterAnimatorField = characterType.GetField("characterAnimator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (characterAnimatorField != null)
                {
                    var characterAnimator = characterAnimatorField.GetValue(character) as Animator;
                    if (characterAnimator != null)
                    {
                        // Получаем индекс слоя holster через рефлексию
                        var layerHolsterField = characterType.GetField("layerHolster", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        int layerHolster = 0;
                        if (layerHolsterField != null)
                        {
                            layerHolster = (int)layerHolsterField.GetValue(character);
                        }
                        
                        // Играем анимацию Unholster
                        characterAnimator.Play("Unholster", layerHolster, 0);
                    }
                }
                
                // Вызываем RefreshWeaponSetup для обновления контроллера анимации
                var refreshWeaponSetupMethod = characterType.GetMethod("RefreshWeaponSetup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (refreshWeaponSetupMethod != null)
                {
                    refreshWeaponSetupMethod.Invoke(character, System.Reflection.BindingFlags.InvokeMethod, null, null, System.Globalization.CultureInfo.CurrentCulture);
                }
                else
                {
                    Debug.LogWarning("Метод RefreshWeaponSetup не найден в Character.");
                }
            }
            else
            {
                Debug.LogWarning("Компонент Character не найден на объекте с Inventory.");
            }
        }

        #endregion
    }
}
