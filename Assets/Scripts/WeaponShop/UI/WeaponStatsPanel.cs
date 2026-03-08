/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит UI панели параметров оружия для магазина

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeaponShop;

namespace WeaponShop
{
    /// <summary>
    /// Панель для отображения параметров выбранного оружия.
    /// </summary>
    public class WeaponStatsPanel : MonoBehaviour
    {
        #region FIELDS

        [Header("Basic Info")]
        [Tooltip("Текст названия оружия.")]
        [SerializeField]
        private TextMeshProUGUI weaponNameText;

        [Tooltip("Текст типа оружия.")]
        [SerializeField]
        private TextMeshProUGUI weaponTypeText;

        [Header("Stats")]
        [Tooltip("Текст урона.")]
        [SerializeField]
        private TextMeshProUGUI damageText;

        [Tooltip("Полоска урона.")]
        [SerializeField]
        private Image damageBar;

        [Tooltip("Текст скорострельности.")]
        [SerializeField]
        private TextMeshProUGUI fireRateText;

        [Tooltip("Полоска скорострельности.")]
        [SerializeField]
        private Image fireRateBar;

        [Tooltip("Текст ёмкости магазина.")]
        [SerializeField]
        private TextMeshProUGUI magazineCapacityText;

        [Tooltip("Полоска ёмкости магазина.")]
        [SerializeField]
        private Image magazineCapacityBar;

        [Tooltip("Текст времени перезарядки.")]
        [SerializeField]
        private TextMeshProUGUI reloadTimeText;

        [Tooltip("Полоска времени перезарядки.")]
        [SerializeField]
        private Image reloadTimeBar;

        [Tooltip("Текст точности.")]
        [SerializeField]
        private TextMeshProUGUI accuracyText;

        [Tooltip("Полоска точности.")]
        [SerializeField]
        private Image accuracyBar;

        [Tooltip("Текст дальности.")]
        [SerializeField]
        private TextMeshProUGUI rangeText;

        [Tooltip("Полоска дальности.")]
        [SerializeField]
        private Image rangeBar;

        [Header("Settings")]
        [Tooltip("Максимальные значения для полосок.")]
        [SerializeField]
        private float maxDamage = 500f;

        [SerializeField]
        private int maxFireRate = 1200;

        [SerializeField]
        private int maxMagazineCapacity = 100;

        [SerializeField]
        private float maxReloadTime = 10f;

        [SerializeField]
        private float maxRange = 500f;

        #endregion

        #region FIELDS PRIVATE

        private IWeaponShopService weaponShopService;
        private WeaponData currentWeapon;

        #endregion

        #region UNITY METHODS

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Показывает параметры оружия.
        /// </summary>
        /// <param name="weaponData">Данные оружия.</param>
        public void ShowWeaponStats(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogError("Данные оружия не могут быть null.");
                return;
            }

            currentWeapon = weaponData;
            UpdateStats();
        }

        /// <summary>
        /// Скрывает панель.
        /// </summary>
        public void HidePanel()
        {
            currentWeapon = null;
            ClearStats();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Инициализирует компонент.
        /// </summary>
        private void Initialize()
        {
            weaponShopService = FindObjectOfType<WeaponShopService>();

            if (weaponShopService == null)
            {
                Debug.LogError("Сервис магазина оружия не найден в сцене.");
                return;
            }

            // Подписываемся на события
            weaponShopService.OnWeaponSelected += HandleWeaponSelected;
            weaponShopService.OnDataChanged += HandleDataChanged;

            // Показываем выбранное оружие
            WeaponData selectedWeapon = weaponShopService.GetSelectedWeapon();
            if (selectedWeapon != null)
            {
                ShowWeaponStats(selectedWeapon);
            }
        }

        /// <summary>
        /// Очищает подписки.
        /// </summary>
        private void Cleanup()
        {
            if (weaponShopService != null)
            {
                weaponShopService.OnWeaponSelected -= HandleWeaponSelected;
                weaponShopService.OnDataChanged -= HandleDataChanged;
            }
        }

        /// <summary>
        /// Обновляет отображение параметров.
        /// </summary>
        private void UpdateStats()
        {
            if (currentWeapon == null)
            {
                return;
            }

            // Обновляем базовую информацию
            if (weaponNameText != null)
            {
                weaponNameText.text = currentWeapon.WeaponName;
            }

            if (weaponTypeText != null)
            {
                weaponTypeText.text = GetWeaponTypeText(currentWeapon.WeaponType);
            }

            // Обновляем параметры
            UpdateStatDisplay(damageText, damageBar, currentWeapon.Damage, maxDamage, "{0}");
            UpdateStatDisplay(fireRateText, fireRateBar, currentWeapon.FireRate, maxFireRate, "{0}/мин");
            UpdateStatDisplay(magazineCapacityText, magazineCapacityBar, currentWeapon.MagazineCapacity, maxMagazineCapacity, "{0}");
            UpdateStatDisplay(reloadTimeText, reloadTimeBar, currentWeapon.ReloadTime, maxReloadTime, "{0:F1}с");
            UpdateStatDisplay(accuracyText, accuracyBar, (1f - currentWeapon.Accuracy) * 100f, 100f, "{0:F0}%");
            UpdateStatDisplay(rangeText, rangeBar, currentWeapon.Range, maxRange, "{0}м");
        }

        /// <summary>
        /// Обновляет отображение параметра.
        /// </summary>
        private void UpdateStatDisplay(TextMeshProUGUI text, Image bar, float value, float maxValue, string format)
        {
            if (text != null)
            {
                text.text = string.Format(format, value);
            }

            if (bar != null)
            {
                bar.fillAmount = Mathf.Clamp01(value / maxValue);
            }
        }

        /// <summary>
        /// Очищает отображение параметров.
        /// </summary>
        private void ClearStats()
        {
            if (weaponNameText != null)
            {
                weaponNameText.text = "";
            }

            if (weaponTypeText != null)
            {
                weaponTypeText.text = "";
            }

            if (damageText != null)
            {
                damageText.text = "";
            }

            if (damageBar != null)
            {
                damageBar.fillAmount = 0f;
            }

            if (fireRateText != null)
            {
                fireRateText.text = "";
            }

            if (fireRateBar != null)
            {
                fireRateBar.fillAmount = 0f;
            }

            if (magazineCapacityText != null)
            {
                magazineCapacityText.text = "";
            }

            if (magazineCapacityBar != null)
            {
                magazineCapacityBar.fillAmount = 0f;
            }

            if (reloadTimeText != null)
            {
                reloadTimeText.text = "";
            }

            if (reloadTimeBar != null)
            {
                reloadTimeBar.fillAmount = 0f;
            }

            if (accuracyText != null)
            {
                accuracyText.text = "";
            }

            if (accuracyBar != null)
            {
                accuracyBar.fillAmount = 0f;
            }

            if (rangeText != null)
            {
                rangeText.text = "";
            }

            if (rangeBar != null)
            {
                rangeBar.fillAmount = 0f;
            }
        }

        /// <summary>
        /// Получает текст типа оружия.
        /// </summary>
        private string GetWeaponTypeText(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Handgun:
                    return "Пистолет";
                case WeaponType.SMG:
                    return "ПП";
                case WeaponType.Shotgun:
                    return "Дробовик";
                case WeaponType.AssaultRifle:
                    return "Автомат";
                case WeaponType.Sniper:
                    return "Снайперская винтовка";
                case WeaponType.RocketLauncher:
                    return "РПГ";
                case WeaponType.GrenadeLauncher:
                    return "Гранатомёт";
                default:
                    return "Неизвестно";
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Обрабатывает выбор оружия.
        /// </summary>
        private void HandleWeaponSelected(string weaponId)
        {
            if (weaponShopService != null)
            {
                WeaponData weaponData = weaponShopService.GetWeaponData(weaponId);
                ShowWeaponStats(weaponData);
            }
        }

        /// <summary>
        /// Обрабатывает изменение данных.
        /// </summary>
        private void HandleDataChanged()
        {
            if (currentWeapon != null)
            {
                UpdateStats();
            }
        }

        #endregion
    }
}
*/
