//Copyright 2024, Infima Games. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Менеджер конфигурации обвесов. Связывает JSON конфигурацию с WeaponAttachmentManager.
    /// </summary>
    public class AttachmentConfigManager : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Пути к JSON файлам")]
        [Tooltip("Путь к файлу доступности обвесов")]
        [SerializeField]
        private string availabilityConfigPath = "Assets/Data/attachments_availability_config.json";

        [Tooltip("Путь к файлу конфигурации обвесов оружия")]
        [SerializeField]
        private string weaponsConfigPath = "Assets/Data/weapons_attachments_config.json";

        [Header("Настройки")]
        [Tooltip("ID текущего оружия (оставьте пустым для автоматического определения)")]
        [SerializeField]
        private string manualWeaponId = "";

        [Tooltip("Автоматически определять текущее оружие")]
        [SerializeField]
        private bool autoDetectWeapon = true;

        [Tooltip("Автоматически загружать конфигурацию при старте")]
        [SerializeField]
        private bool autoLoadOnStart = true;

        [Tooltip("Показывать отладочные сообщения")]
        [SerializeField]
        private bool showDebugMessages = true;

        [Header("Сопоставление ID обвесов")]
        [Tooltip("Сопоставление ID прицелов с индексами")]
        [SerializeField]
        private List<AttachmentIdMapping> scopeMappings = new List<AttachmentIdMapping>();

        [Tooltip("Сопоставление ID дульных насадок с индексами")]
        [SerializeField]
        private List<AttachmentIdMapping> muzzleMappings = new List<AttachmentIdMapping>();

        [Tooltip("Сопоставление ID лазеров с индексами")]
        [SerializeField]
        private List<AttachmentIdMapping> laserMappings = new List<AttachmentIdMapping>();

        [Tooltip("Сопоставление ID рукояток с индексами")]
        [SerializeField]
        private List<AttachmentIdMapping> gripMappings = new List<AttachmentIdMapping>();

        [Tooltip("Сопоставление ID магазинов с индексами")]
        [SerializeField]
        private List<AttachmentIdMapping> magazineMappings = new List<AttachmentIdMapping>();

        #endregion

        #region FIELDS

        private AttachmentsAvailabilityData availabilityData;
        private WeaponsAttachmentsData weaponsData;

        private WeaponAttachmentManager attachmentManager;
        private string currentWeaponId = "ar_01";
        private Weapon weaponBehaviour;

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            if (autoLoadOnStart)
            {
                LoadAllConfigs();
            }
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Загрузить все конфигурации из JSON файлов
        /// </summary>
        public void LoadAllConfigs()
        {
            // Автоматически определяем текущее оружие
            AutoDetectCurrentWeapon();

            if (showDebugMessages)
                Debug.Log($"[AttachmentConfigManager] Начало загрузки конфигураций. Текущее оружие: {currentWeaponId}");

            LoadAvailabilityConfig();
            LoadWeaponsConfig();
            
            attachmentManager = GetComponent<WeaponAttachmentManager>();
            
            if (attachmentManager == null)
            {
                Debug.LogError("[AttachmentConfigManager] WeaponAttachmentManager не найден на том же объекте!");
                return;
            }

            if (showDebugMessages)
                Debug.Log("[AttachmentConfigManager] WeaponAttachmentManager найден успешно");
            
            if (!string.IsNullOrEmpty(currentWeaponId))
            {
                ApplyConfigToWeapon(currentWeaponId);
            }
            else
            {
                Debug.LogWarning("[AttachmentConfigManager] currentWeaponId не указан!");
            }
        }

        /// <summary>
        /// Автоматически определить текущее оружие
        /// </summary>
        private void AutoDetectCurrentWeapon()
        {
            // Если указан manualWeaponId, используем его
            if (!string.IsNullOrEmpty(manualWeaponId))
            {
                currentWeaponId = manualWeaponId;
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigManager] Используется вручную заданный ID оружия: {currentWeaponId}");
                return;
            }

            // Если автоматическое определение отключено, используем значение по умолчанию
            if (!autoDetectWeapon)
            {
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigManager] Автоматическое определение отключено. Используется ID по умолчанию: {currentWeaponId}");
                return;
            }

            // Пытаемся найти компонент Weapon на том же объекте
            weaponBehaviour = GetComponent<Weapon>();
            
            if (weaponBehaviour != null)
            {
                // Получаем имя оружия из поля weaponName
                string detectedWeaponName = weaponBehaviour.weaponName;
                
                if (!string.IsNullOrEmpty(detectedWeaponName))
                {
                    // Преобразуем имя оружия в ID
                    currentWeaponId = ConvertWeaponNameToId(detectedWeaponName);
                    
                    if (showDebugMessages)
                        Debug.Log($"[AttachmentConfigManager] Автоматически определено оружие: {detectedWeaponName} -> {currentWeaponId}");
                }
                else
                {
                    if (showDebugMessages)
                        Debug.LogWarning("[AttachmentConfigManager] weaponName не указан в компоненте Weapon");
                }
            }
            else
            {
                if (showDebugMessages)
                    Debug.LogWarning("[AttachmentConfigManager] Компонент Weapon не найден на том же объекте");
            }
        }

        /// <summary>
        /// Преобразовать имя оружия в ID для JSON
        /// </summary>
        private string ConvertWeaponNameToId(string weaponName)
        {
            // Преобразование имени оружия в ID для JSON файла
            // Это нужно настроить в соответствии с вашими оружиями
            
            string lowerName = weaponName.ToLower();
            
            // Примеры преобразований (настройте под ваши оружия)
            if (lowerName.Contains("ar") || lowerName.Contains("assault") || lowerName.Contains("rifle"))
            {
                if (lowerName.Contains("ar_01") || lowerName.Contains("ar01"))
                    return "ar_01";
                if (lowerName.Contains("ar_02") || lowerName.Contains("ar02"))
                    return "ar_02";
                if (lowerName.Contains("ar_03") || lowerName.Contains("ar03"))
                    return "ar_03";
                return "ar_01"; // По умолчанию
            }
            
            if (lowerName.Contains("handgun") || lowerName.Contains("pistol"))
            {
                if (lowerName.Contains("hg_01") || lowerName.Contains("hg01"))
                    return "handgun_01";
                if (lowerName.Contains("hg_02") || lowerName.Contains("hg02"))
                    return "handgun_02";
                if (lowerName.Contains("hg_03") || lowerName.Contains("hg03"))
                    return "handgun_03";
                if (lowerName.Contains("hg_04") || lowerName.Contains("hg04"))
                    return "handgun_04";
                return "handgun_01"; // По умолчанию
            }
            
            if (lowerName.Contains("smg") || lowerName.Contains("submachine"))
            {
                if (lowerName.Contains("smg_01") || lowerName.Contains("smg01"))
                    return "smg_01";
                if (lowerName.Contains("smg_02") || lowerName.Contains("smg02"))
                    return "smg_02";
                if (lowerName.Contains("smg_03") || lowerName.Contains("smg03"))
                    return "smg_03";
                if (lowerName.Contains("smg_04") || lowerName.Contains("smg04"))
                    return "smg_04";
                if (lowerName.Contains("smg_05") || lowerName.Contains("smg05"))
                    return "smg_05";
                return "smg_01"; // По умолчанию
            }
            
            if (lowerName.Contains("gl") || lowerName.Contains("grenade") || lowerName.Contains("launcher"))
            {
                if (lowerName.Contains("gl_01") || lowerName.Contains("gl01"))
                    return "gl_01";
                return "gl_01"; // По умолчанию
            }
            
            if (lowerName.Contains("rl") || lowerName.Contains("rocket") || lowerName.Contains("launcher"))
            {
                if (lowerName.Contains("rl_01") || lowerName.Contains("rl01"))
                    return "rl_01";
                return "rl_01"; // По умолчанию
            }
            
            if (lowerName.Contains("shotgun") || lowerName.Contains("shot") || lowerName.Contains("sg"))
            {
                if (lowerName.Contains("shotgun_01") || lowerName.Contains("shotgun01"))
                    return "shotgun_01";
                return "shotgun_01"; // По умолчанию
            }
            
            if (lowerName.Contains("sniper") || lowerName.Contains("rifle"))
            {
                if (lowerName.Contains("sn_01") || lowerName.Contains("sn01"))
                    return "sniper_01";
                if (lowerName.Contains("sn_02") || lowerName.Contains("sn02"))
                    return "sniper_02";
                if (lowerName.Contains("sn_03") || lowerName.Contains("sn03"))
                    return "sniper_03";
                return "sniper_01"; // По умолчанию
            }
            
            // Если не удалось определить, возвращаем имя как есть
            if (showDebugMessages)
                Debug.LogWarning($"[AttachmentConfigManager] Не удалось определить ID для оружия: {weaponName}");
            
            return weaponName.ToLower().Replace(" ", "_");
        }

        /// <summary>
        /// Установить текущее оружие и применить конфигурацию
        /// </summary>
        public void SetWeapon(string weaponId)
        {
            currentWeaponId = weaponId;
            
            if (weaponsData != null && attachmentManager != null)
            {
                ApplyConfigToWeapon(weaponId);
            }
        }

        /// <summary>
        /// Загрузить конфигурацию доступности обвесов
        /// </summary>
        public void LoadAvailabilityConfig()
        {
            try
            {
                string json = System.IO.File.ReadAllText(availabilityConfigPath);
                availabilityData = JsonUtility.FromJson<AttachmentsAvailabilityData>(json);
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigManager] Загружена конфигурация доступности обвесов");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AttachmentConfigManager] Ошибка загрузки конфигурации доступности: {e.Message}");
            }
        }

        /// <summary>
        /// Загрузить конфигурацию обвесов оружия
        /// </summary>
        public void LoadWeaponsConfig()
        {
            try
            {
                if (!System.IO.File.Exists(weaponsConfigPath))
                {
                    Debug.LogError($"[AttachmentConfigManager] Файл не найден: {weaponsConfigPath}");
                    return;
                }

                string json = System.IO.File.ReadAllText(weaponsConfigPath);
                
                // Используем Newtonsoft.Json для десериализации Dictionary
                weaponsData = JsonConvert.DeserializeObject<WeaponsAttachmentsData>(json);
                
                if (showDebugMessages)
                {
                    Debug.Log($"[AttachmentConfigManager] Конфигурация загружена успешно");
                    Debug.Log($"[AttachmentConfigManager] Оружий в конфигурации: {weaponsData.weaponsAttachments.Count}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[AttachmentConfigManager] Ошибка загрузки конфигурации оружия: {e.Message}");
                Debug.LogError($"[AttachmentConfigManager] Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Применить конфигурацию к оружию
        /// </summary>
        public void ApplyConfigToWeapon(string weaponId)
        {
            if (showDebugMessages)
                Debug.Log($"[AttachmentConfigManager] Применение конфигурации для оружия: {weaponId}");

            if (weaponsData == null)
            {
                Debug.LogError("[AttachmentConfigManager] Данные конфигурации не загружены!");
                return;
            }

            if (!weaponsData.weaponsAttachments.ContainsKey(weaponId))
            {
                Debug.LogError($"[AttachmentConfigManager] Конфигурация для оружия {weaponId} не найдена!");
                Debug.Log($"[AttachmentConfigManager] Доступные оружия: {string.Join(", ", weaponsData.weaponsAttachments.Keys)}");
                return;
            }

            WeaponAttachments weaponConfig = weaponsData.weaponsAttachments[weaponId];
            
            if (attachmentManager == null)
            {
                attachmentManager = GetComponent<WeaponAttachmentManager>();
            }

            if (attachmentManager == null)
            {
                Debug.LogError("[AttachmentConfigManager] WeaponAttachmentManager не найден на том же объекте");
                return;
            }

            if (showDebugMessages)
                Debug.Log($"[AttachmentConfigManager] Применение обвесов для {weaponConfig.weaponName}");

            ApplyAttachmentsToManager(weaponConfig);
            
            if (showDebugMessages)
                Debug.Log($"[AttachmentConfigManager] Конфигурация применена для оружия {weaponId}");
        }

        /// <summary>
        /// Получить статус покупки обвеса
        /// </summary>
        public bool IsAttachmentPurchased(string attachmentId)
        {
            if (availabilityData == null || 
                !availabilityData.attachmentsAvailability.ContainsKey(attachmentId))
                return false;

            return availabilityData.attachmentsAvailability[attachmentId].purchased;
        }

        /// <summary>
        /// Получить статус выбора обвеса
        /// </summary>
        public bool IsAttachmentSelected(string attachmentId)
        {
            if (availabilityData == null || 
                !availabilityData.attachmentsAvailability.ContainsKey(attachmentId))
                return false;

            return availabilityData.attachmentsAvailability[attachmentId].selected;
        }

        /// <summary>
        /// Получить доступные обвесы для оружия
        /// </summary>
        public List<string> GetAvailableAttachmentsForWeapon(string weaponId, string slotType)
        {
            if (weaponsData == null || 
                !weaponsData.weaponsAttachments.ContainsKey(weaponId))
                return new List<string>();

            var weaponConfig = weaponsData.weaponsAttachments[weaponId];
            
            foreach (var slot in weaponConfig.attachments)
            {
                if (slot.Value.slotType == slotType && slot.Value.selected)
                {
                    return slot.Value.availableAttachments;
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Получить текущий обвес для слота
        /// </summary>
        public string GetCurrentAttachmentForSlot(string weaponId, string slotName)
        {
            if (weaponsData == null || 
                !weaponsData.weaponsAttachments.ContainsKey(weaponId))
                return null;

            var weaponConfig = weaponsData.weaponsAttachments[weaponId];
            
            if (weaponConfig.attachments.ContainsKey(slotName))
            {
                return weaponConfig.attachments[slotName].currentAttachment;
            }

            return null;
        }

        /// <summary>
        /// Получить все пресеты для оружия
        /// </summary>
        public Dictionary<string, PresetConfiguration> GetPresetsForWeapon(string weaponId)
        {
            if (weaponsData == null || 
                !weaponsData.weaponsAttachments.ContainsKey(weaponId))
                return new Dictionary<string, PresetConfiguration>();

            return weaponsData.weaponsAttachments[weaponId].presetConfigurations;
        }

        /// <summary>
        /// Применить пресет к оружию
        /// </summary>
        public void ApplyPreset(string weaponId, string presetName)
        {
            if (weaponsData == null || 
                !weaponsData.weaponsAttachments.ContainsKey(weaponId))
                return;

            var weaponConfig = weaponsData.weaponsAttachments[weaponId];
            
            if (!weaponConfig.presetConfigurations.ContainsKey(presetName))
            {
                if (showDebugMessages)
                    Debug.LogWarning($"[AttachmentConfigManager] Пресет {presetName} не найден для оружия {weaponId}");
                return;
            }

            var preset = weaponConfig.presetConfigurations[presetName];
            
            foreach (var attachment in preset.attachments)
            {
                if (weaponConfig.attachments.ContainsKey(attachment.Key))
                {
                    weaponConfig.attachments[attachment.Key].currentAttachment = attachment.Value;
                }
            }

            weaponConfig.activePreset = presetName;
            ApplyConfigToWeapon(weaponId);
            
            if (showDebugMessages)
                Debug.Log($"[AttachmentConfigManager] Применён пресет {presetName} для оружия {weaponId}");
        }

        /// <summary>
        /// Добавить сопоставление ID обвеса с индексом
        /// </summary>
        public void AddMapping(string slotType, string attachmentId, int index)
        {
            var mapping = new AttachmentIdMapping
            {
                attachmentId = attachmentId,
                arrayIndex = index
            };

            switch (slotType.ToLower())
            {
                case "scope":
                    if (!scopeMappings.Exists(m => m.attachmentId == attachmentId))
                        scopeMappings.Add(mapping);
                    break;
                case "muzzle":
                    if (!muzzleMappings.Exists(m => m.attachmentId == attachmentId))
                        muzzleMappings.Add(mapping);
                    break;
                case "laser":
                    if (!laserMappings.Exists(m => m.attachmentId == attachmentId))
                        laserMappings.Add(mapping);
                    break;
                case "grip":
                    if (!gripMappings.Exists(m => m.attachmentId == attachmentId))
                        gripMappings.Add(mapping);
                    break;
                case "magazine":
                    if (!magazineMappings.Exists(m => m.attachmentId == attachmentId))
                        magazineMappings.Add(mapping);
                    break;
            }
        }

        /// <summary>
        /// Получить индекс обвеса по ID
        /// </summary>
        public int GetAttachmentIndex(string slotType, string attachmentId)
        {
            List<AttachmentIdMapping> mappings = null;

            switch (slotType.ToLower())
            {
                case "scope":
                    mappings = scopeMappings;
                    break;
                case "muzzle":
                    mappings = muzzleMappings;
                    break;
                case "laser":
                    mappings = laserMappings;
                    break;
                case "grip":
                    mappings = gripMappings;
                    break;
                case "magazine":
                    mappings = magazineMappings;
                    break;
            }

            if (mappings != null)
            {
                if (showDebugMessages)
                {
                    Debug.Log($"[AttachmentConfigManager] Поиск индекса для {slotType}.{attachmentId}");
                    Debug.Log($"[AttachmentConfigManager] Доступные сопоставления: {mappings.Count}");
                    foreach (var m in mappings)
                    {
                        Debug.Log($"[AttachmentConfigManager]   - {m.attachmentId} -> {m.arrayIndex}");
                    }
                }

                var mapping = mappings.Find(m => m.attachmentId == attachmentId);
                if (mapping != null)
                {
                    if (showDebugMessages)
                        Debug.Log($"[AttachmentConfigManager] Найден индекс: {mapping.arrayIndex}");
                    return mapping.arrayIndex;
                }
                else
                {
                    Debug.LogWarning($"[AttachmentConfigManager] Сопоставление не найдено для {attachmentId}");
                }
            }

            return -1;
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Применить обвесы к менеджеру
        /// </summary>
        private void ApplyAttachmentsToManager(WeaponAttachments weaponConfig)
        {
            // Применяем настройки для каждого типа обвесов
            ApplyScopeAttachments(weaponConfig);
            ApplyMuzzleAttachments(weaponConfig);
            ApplyLaserAttachments(weaponConfig);
            ApplyGripAttachments(weaponConfig);
            ApplyMagazineAttachments(weaponConfig);
        }

        /// <summary>
        /// Применить настройки прицела
        /// </summary>
        private void ApplyScopeAttachments(WeaponAttachments weaponConfig)
        {
            if (showDebugMessages)
                Debug.Log("[AttachmentConfigManager] Обработка прицела...");

            if (weaponConfig.attachments.ContainsKey("scope"))
            {
                var scopeSlot = weaponConfig.attachments["scope"];
                string currentScope = scopeSlot.currentAttachment;
                
                if (showDebugMessages)
                {
                    Debug.Log($"[AttachmentConfigManager] Текущий прицел из JSON: {currentScope}");
                    Debug.Log($"[AttachmentConfigManager] Слот выбран: {scopeSlot.selected}");
                }
                
                if (!string.IsNullOrEmpty(currentScope))
                {
                    int scopeIndex = GetAttachmentIndex("scope", currentScope);
                    
                    if (showDebugMessages)
                    {
                        Debug.Log($"[AttachmentConfigManager] Полученный индекс прицела: {scopeIndex}");
                        Debug.Log($"[AttachmentConfigManager] Индекс валиден: {(scopeIndex >= 0)}");
                    }
                    
                    if (scopeIndex >= 0)
                    {
                        // Используем рефлексию для установки приватного поля
                        var scopeIndexField = typeof(WeaponAttachmentManager).GetField("scopeIndex",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (scopeIndexField != null)
                        {
                            // Получаем текущее значение
                            int currentValue = (int)scopeIndexField.GetValue(attachmentManager);
                            if (showDebugMessages)
                                Debug.Log($"[AttachmentConfigManager] Текущее значение scopeIndex: {currentValue}");

                            // Устанавливаем новое значение
                            scopeIndexField.SetValue(attachmentManager, scopeIndex);
                            
                            if (showDebugMessages)
                                Debug.Log($"[AttachmentConfigManager] Установлено новое значение scopeIndex: {scopeIndex}");
                            
                            // Отключаем рандомизацию
                            var scopeRandomField = typeof(WeaponAttachmentManager).GetField("scopeIndexRandom",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (scopeRandomField != null)
                            {
                                scopeRandomField.SetValue(attachmentManager, false);
                                if (showDebugMessages)
                                    Debug.Log("[AttachmentConfigManager] Рандомизация прицела отключена");
                            }
                            
                            // Пересобираем обвесы
                            if (showDebugMessages)
                                Debug.Log("[AttachmentConfigManager] Вызов SetAttachments()...");
                            
                            attachmentManager.SetAttachments();
                            
                            // Проверяем результат
                            var equippedScope = attachmentManager.GetEquippedScope();
                            if (showDebugMessages)
                            {
                                Debug.Log($"[AttachmentConfigManager] Установленный прицел: {equippedScope?.name ?? "NULL"}");
                                Debug.Log($"[AttachmentConfigManager] Установлен прицел: {currentScope} (индекс: {scopeIndex})");
                            }
                        }
                        else
                        {
                            Debug.LogError("[AttachmentConfigManager] Не удалось найти поле scopeIndex через рефлексию!");
                        }
                    }
                    else
                    {
                        Debug.LogError($"[AttachmentConfigManager] Недопустимый индекс прицела: {scopeIndex}");
                    }
                }
                else
                {
                    // Устанавливаем индекс -1 (без прицела)
                    if (showDebugMessages)
                        Debug.Log("[AttachmentConfigManager] Прицел не указан в JSON, устанавливаем индекс -1 (без прицела)");
                    
                    var scopeIndexField = typeof(WeaponAttachmentManager).GetField("scopeIndex",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (scopeIndexField != null)
                    {
                        scopeIndexField.SetValue(attachmentManager, -1);
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Установлено значение scopeIndex: -1 (без прицела)");
                        
                        // Отключаем рандомизацию
                        var scopeRandomField = typeof(WeaponAttachmentManager).GetField("scopeIndexRandom",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (scopeRandomField != null)
                        {
                            scopeRandomField.SetValue(attachmentManager, false);
                            if (showDebugMessages)
                                Debug.Log("[AttachmentConfigManager] Рандомизация прицела отключена");
                        }
                        
                        // Пересобираем обвесы
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Вызов SetAttachments()...");
                        
                        attachmentManager.SetAttachments();
                        
                        // Проверяем результат
                        var equippedScope = attachmentManager.GetEquippedScope();
                        if (showDebugMessages)
                        {
                            Debug.Log($"[AttachmentConfigManager] Установленный прицел: {equippedScope?.name ?? "NULL"}");
                            Debug.Log("[AttachmentConfigManager] Прицел отключён (индекс: -1)");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("[AttachmentConfigManager] Слот 'scope' не найден в конфигурации");
            }
        }

        /// <summary>
        /// Применить настройки дульной насадки
        /// </summary>
        private void ApplyMuzzleAttachments(WeaponAttachments weaponConfig)
        {
            if (weaponConfig.attachments.ContainsKey("muzzle"))
            {
                var muzzleSlot = weaponConfig.attachments["muzzle"];
                string currentMuzzle = muzzleSlot.currentAttachment;
                
                if (!string.IsNullOrEmpty(currentMuzzle))
                {
                    int muzzleIndex = GetAttachmentIndex("muzzle", currentMuzzle);
                    if (muzzleIndex >= 0)
                    {
                        var muzzleIndexField = typeof(WeaponAttachmentManager).GetField("muzzleIndex",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (muzzleIndexField != null)
                        {
                            muzzleIndexField.SetValue(attachmentManager, muzzleIndex);
                            
                            var muzzleRandomField = typeof(WeaponAttachmentManager).GetField("muzzleIndexRandom",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (muzzleRandomField != null)
                            {
                                muzzleRandomField.SetValue(attachmentManager, false);
                            }
                            
                            attachmentManager.SetAttachments();
                            
                            if (showDebugMessages)
                                Debug.Log($"[AttachmentConfigManager] Установлена дульная насадка: {currentMuzzle} (индекс: {muzzleIndex})");
                        }
                    }
                }
                else
                {
                    // Устанавливаем индекс 0 (без дульной насадки)
                    if (showDebugMessages)
                        Debug.Log("[AttachmentConfigManager] Дульная насадка не указана в JSON, устанавливаем индекс 0 (без насадки)");
                    
                    var muzzleIndexField = typeof(WeaponAttachmentManager).GetField("muzzleIndex",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (muzzleIndexField != null)
                    {
                        muzzleIndexField.SetValue(attachmentManager, 0);
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Установлено значение muzzleIndex: 0 (без насадки)");
                        
                        var muzzleRandomField = typeof(WeaponAttachmentManager).GetField("muzzleIndexRandom",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (muzzleRandomField != null)
                        {
                            muzzleRandomField.SetValue(attachmentManager, false);
                        }
                        
                        attachmentManager.SetAttachments();
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Дульная насадка отключена (индекс: 0)");
                    }
                }
            }
        }

        /// <summary>
        /// Применить настройки лазера
        /// </summary>
        private void ApplyLaserAttachments(WeaponAttachments weaponConfig)
        {
            if (weaponConfig.attachments.ContainsKey("laser"))
            {
                var laserSlot = weaponConfig.attachments["laser"];
                string currentLaser = laserSlot.currentAttachment;
                
                if (!string.IsNullOrEmpty(currentLaser))
                {
                    int laserIndex = GetAttachmentIndex("laser", currentLaser);
                    if (laserIndex >= 0)
                    {
                        var laserIndexField = typeof(WeaponAttachmentManager).GetField("laserIndex",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (laserIndexField != null)
                        {
                            laserIndexField.SetValue(attachmentManager, laserIndex);
                            
                            var laserRandomField = typeof(WeaponAttachmentManager).GetField("laserIndexRandom",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (laserRandomField != null)
                            {
                                laserRandomField.SetValue(attachmentManager, false);
                            }
                            
                            attachmentManager.SetAttachments();
                            
                            if (showDebugMessages)
                                Debug.Log($"[AttachmentConfigManager] Установлен лазер: {currentLaser} (индекс: {laserIndex})");
                        }
                    }
                }
                else
                {
                    // Устанавливаем индекс -1 (без лазера)
                    if (showDebugMessages)
                        Debug.Log("[AttachmentConfigManager] Лазер не указан в JSON, устанавливаем индекс -1 (без лазера)");
                    
                    var laserIndexField = typeof(WeaponAttachmentManager).GetField("laserIndex",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (laserIndexField != null)
                    {
                        laserIndexField.SetValue(attachmentManager, -1);
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Установлено значение laserIndex: -1 (без лазера)");
                        
                        var laserRandomField = typeof(WeaponAttachmentManager).GetField("laserIndexRandom",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (laserRandomField != null)
                        {
                            laserRandomField.SetValue(attachmentManager, false);
                        }
                        
                        attachmentManager.SetAttachments();
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Лазер отключён (индекс: -1)");
                    }
                }
            }
        }

        /// <summary>
        /// Применить настройки рукоятки
        /// </summary>
        private void ApplyGripAttachments(WeaponAttachments weaponConfig)
        {
            if (weaponConfig.attachments.ContainsKey("grip"))
            {
                var gripSlot = weaponConfig.attachments["grip"];
                string currentGrip = gripSlot.currentAttachment;
                
                if (!string.IsNullOrEmpty(currentGrip))
                {
                    int gripIndex = GetAttachmentIndex("grip", currentGrip);
                    if (gripIndex >= 0)
                    {
                        var gripIndexField = typeof(WeaponAttachmentManager).GetField("gripIndex",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (gripIndexField != null)
                        {
                            gripIndexField.SetValue(attachmentManager, gripIndex);
                            
                            var gripRandomField = typeof(WeaponAttachmentManager).GetField("gripIndexRandom",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (gripRandomField != null)
                            {
                                gripRandomField.SetValue(attachmentManager, false);
                            }
                            
                            attachmentManager.SetAttachments();
                            
                            if (showDebugMessages)
                                Debug.Log($"[AttachmentConfigManager] Установлена рукоятка: {currentGrip} (индекс: {gripIndex})");
                        }
                    }
                }
                else
                {
                    // Устанавливаем индекс -1 (без рукоятки)
                    if (showDebugMessages)
                        Debug.Log("[AttachmentConfigManager] Рукоятка не указана в JSON, устанавливаем индекс -1 (без рукоятки)");
                    
                    var gripIndexField = typeof(WeaponAttachmentManager).GetField("gripIndex",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (gripIndexField != null)
                    {
                        gripIndexField.SetValue(attachmentManager, -1);
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Установлено значение gripIndex: -1 (без рукоятки)");
                        
                        var gripRandomField = typeof(WeaponAttachmentManager).GetField("gripIndexRandom",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (gripRandomField != null)
                        {
                            gripRandomField.SetValue(attachmentManager, false);
                        }
                        
                        attachmentManager.SetAttachments();
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Рукоятка отключена (индекс: -1)");
                    }
                }
            }
        }

        /// <summary>
        /// Применить настройки магазина
        /// </summary>
        private void ApplyMagazineAttachments(WeaponAttachments weaponConfig)
        {
            if (weaponConfig.attachments.ContainsKey("magazine"))
            {
                var magazineSlot = weaponConfig.attachments["magazine"];
                string currentMagazine = magazineSlot.currentAttachment;
                
                if (!string.IsNullOrEmpty(currentMagazine))
                {
                    int magazineIndex = GetAttachmentIndex("magazine", currentMagazine);
                    if (magazineIndex >= 0)
                    {
                        var magazineIndexField = typeof(WeaponAttachmentManager).GetField("magazineIndex",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (magazineIndexField != null)
                        {
                            magazineIndexField.SetValue(attachmentManager, magazineIndex);
                            
                            var magazineRandomField = typeof(WeaponAttachmentManager).GetField("magazineIndexRandom",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (magazineRandomField != null)
                            {
                                magazineRandomField.SetValue(attachmentManager, false);
                            }
                            
                            attachmentManager.SetAttachments();
                            
                            if (showDebugMessages)
                                Debug.Log($"[AttachmentConfigManager] Установлен магазин: {currentMagazine} (индекс: {magazineIndex})");
                        }
                    }
                }
                else
                {
                    // Устанавливаем индекс 0 (стандартный магазин)
                    if (showDebugMessages)
                        Debug.Log("[AttachmentConfigManager] Магазин не указан в JSON, устанавливаем индекс 0 (стандартный магазин)");
                    
                    var magazineIndexField = typeof(WeaponAttachmentManager).GetField("magazineIndex",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (magazineIndexField != null)
                    {
                        magazineIndexField.SetValue(attachmentManager, 0);
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Установлено значение magazineIndex: 0 (стандартный магазин)");
                        
                        var magazineRandomField = typeof(WeaponAttachmentManager).GetField("magazineIndexRandom",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (magazineRandomField != null)
                        {
                            magazineRandomField.SetValue(attachmentManager, false);
                        }
                        
                        attachmentManager.SetAttachments();
                        
                        if (showDebugMessages)
                            Debug.Log("[AttachmentConfigManager] Установлен стандартный магазин (индекс: 0)");
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Класс для сопоставления ID обвеса с индексом в массиве
    /// </summary>
    [Serializable]
    public class AttachmentIdMapping
    {
        [Tooltip("ID обвеса из JSON")]
        public string attachmentId;

        [Tooltip("Индекс в массиве WeaponAttachmentManager")]
        public int arrayIndex;
    }
}
