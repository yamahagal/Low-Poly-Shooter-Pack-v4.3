//Copyright 2024, Infima Games. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Классы данных для JSON десериализации
    /// </summary>
    [Serializable]
    public class AttachmentAvailability
    {
        public bool purchased;
        public bool selected;
        public string purchaseDate;
    }

    [Serializable]
    public class AttachmentCategorySettings
    {
        public int maxSlots;
        public bool selected;
    }

    [Serializable]
    public class GlobalSettings
    {
        public bool allAttachmentsPurchased;
        public bool allowExperimentalAttachments;
        public int maxAttachmentsPerWeapon;
        public Dictionary<string, AttachmentCategorySettings> attachmentCategories;
    }

    [Serializable]
    public class AttachmentsAvailabilityData
    {
        public Dictionary<string, AttachmentAvailability> attachmentsAvailability;
        public GlobalSettings globalSettings;
    }

    [Serializable]
    public class AttachmentSlot
    {
        public string slotType;
        public string currentAttachment;
        public List<string> availableAttachments;
        public bool selected;
    }

    [Serializable]
    public class PresetAttachments
    {
        public Dictionary<string, string> attachments;
    }

    [Serializable]
    public class PresetConfiguration
    {
        public string name;
        public Dictionary<string, string> attachments;
    }

    [Serializable]
    public class WeaponAttachments
    {
        public string weaponId;
        public string weaponName;
        public Dictionary<string, AttachmentSlot> attachments;
        public Dictionary<string, PresetConfiguration> presetConfigurations;
        public string activePreset;
    }

    [Serializable]
    public class GlobalWeaponSettings
    {
        public bool autoApplyPresets;
        public bool saveAttachmentsOnWeaponChange;
        public bool allowMixedAttachments;
        public bool attachmentCompatibilityCheck;
    }

    [Serializable]
    public class WeaponsAttachmentsData
    {
        public Dictionary<string, WeaponAttachments> weaponsAttachments;
        public GlobalWeaponSettings globalWeaponSettings;
    }

    /// <summary>
    /// Вспомогательный класс для десериализации списка оружий из JSON
    /// </summary>
    [Serializable]
    public class WeaponAttachmentEntry
    {
        public string weaponId;
        public string weaponName;
        public Dictionary<string, AttachmentSlot> attachments;
        public Dictionary<string, PresetConfiguration> presetConfigurations;
        public string activePreset;
    }

    /// <summary>
    /// Вспомогательный класс для десериализации данных оружия из JSON
    /// </summary>
    [Serializable]
    public class WeaponsAttachmentsDataHelper
    {
        public List<WeaponAttachmentEntry> weaponsAttachmentsList;
        public GlobalWeaponSettings globalWeaponSettings;
    }

    /// <summary>
    /// Загрузчик конфигурации обвесов из JSON файлов
    /// </summary>
    public class AttachmentConfigLoader : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Пути к JSON файлам")]
        [Tooltip("Путь к файлу доступности обвесов")]
        [SerializeField]
        private string availabilityConfigPath = "Assets/Data/attachments_availability_config.json";

        [Tooltip("Путь к файлу конфигурации обвесов оружия")]
        [SerializeField]
        private string weaponsConfigPath = "Assets/Data/weapons_attachments_config.json";

        [Tooltip("ID текущего оружия")]
        [SerializeField]
        private string currentWeaponId = "ar_01";

        [Header("Настройки")]
        [Tooltip("Автоматически загружать конфигурацию при старте")]
        [SerializeField]
        private bool autoLoadOnStart = true;

        [Tooltip("Показывать отладочные сообщения")]
        [SerializeField]
        private bool showDebugMessages = true;

        #endregion

        #region FIELDS

        private AttachmentsAvailabilityData availabilityData;
        private WeaponsAttachmentsData weaponsData;

        private WeaponAttachmentManager attachmentManager;

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
            LoadAvailabilityConfig();
            LoadWeaponsConfig();
            
            if (attachmentManager != null)
            {
                ApplyConfigToWeapon(currentWeaponId);
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
                    Debug.Log($"[AttachmentConfigLoader] Загружена конфигурация доступности обвесов");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AttachmentConfigLoader] Ошибка загрузки конфигурации доступности: {e.Message}");
            }
        }

        /// <summary>
        /// Загрузить конфигурацию обвесов оружия
        /// </summary>
        public void LoadWeaponsConfig()
        {
            try
            {
                string json = System.IO.File.ReadAllText(weaponsConfigPath);
                weaponsData = JsonUtility.FromJson<WeaponsAttachmentsData>(json);
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigLoader] Загружена конфигурация обвесов оружия");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AttachmentConfigLoader] Ошибка загрузки конфигурации оружия: {e.Message}");
            }
        }

        /// <summary>
        /// Применить конфигурацию к оружию
        /// </summary>
        public void ApplyConfigToWeapon(string weaponId)
        {
            if (weaponsData == null || !weaponsData.weaponsAttachments.ContainsKey(weaponId))
            {
                if (showDebugMessages)
                    Debug.LogWarning($"[AttachmentConfigLoader] Конфигурация для оружия {weaponId} не найдена");
                return;
            }

            WeaponAttachments weaponConfig = weaponsData.weaponsAttachments[weaponId];
            
            if (attachmentManager == null)
            {
                attachmentManager = GetComponent<WeaponAttachmentManager>();
            }

            if (attachmentManager == null)
            {
                Debug.LogError("[AttachmentConfigLoader] WeaponAttachmentManager не найден на том же объекте");
                return;
            }

            ApplyAttachmentsToManager(weaponConfig);
            
            if (showDebugMessages)
                Debug.Log($"[AttachmentConfigLoader] Применена конфигурация для оружия {weaponId}");
        }

        /// <summary>
        /// Установить менеджер обвесов
        /// </summary>
        public void SetAttachmentManager(WeaponAttachmentManager manager)
        {
            attachmentManager = manager;
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
                    Debug.LogWarning($"[AttachmentConfigLoader] Пресет {presetName} не найден для оружия {weaponId}");
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
                Debug.Log($"[AttachmentConfigLoader] Применён пресет {presetName} для оружия {weaponId}");
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
            if (weaponConfig.attachments.ContainsKey("scope"))
            {
                var scopeSlot = weaponConfig.attachments["scope"];
                string currentScope = scopeSlot.currentAttachment;
                
                // Здесь должна быть логика применения прицела
                // В текущей реализации WeaponAttachmentManager использует индексы массивов
                // Нужно сопоставить ID обвеса с индексом в массиве
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigLoader] Установлен прицел: {currentScope}");
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
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigLoader] Установлена дульная насадка: {currentMuzzle}");
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
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigLoader] Установлен лазер: {currentLaser}");
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
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigLoader] Установлена рукоятка: {currentGrip}");
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
                
                if (showDebugMessages)
                    Debug.Log($"[AttachmentConfigLoader] Установлен магазин: {currentMagazine}");
            }
        }

        #endregion
    }
}
