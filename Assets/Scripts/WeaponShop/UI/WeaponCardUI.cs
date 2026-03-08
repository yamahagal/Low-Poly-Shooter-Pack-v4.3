/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит UI карточки оружия для магазина

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WeaponShop
{
    /// <summary>
    /// UI элемент для отображения оружия в списке магазина.
    /// </summary>
    public class WeaponCardUI : MonoBehaviour
    {
        #region FIELDS

        [Header("UI Elements")]
        [Tooltip("Изображение оружия.")]
        [SerializeField]
        private Image weaponImage;

        [Tooltip("Текст названия оружия.")]
        [SerializeField]
        private TextMeshProUGUI nameText;

        [Tooltip("Текст типа оружия.")]
        [SerializeField]
        private TextMeshProUGUI typeText;

        [Tooltip("Текст стоимости.")]
        [SerializeField]
        private TextMeshProUGUI costText;

        [Tooltip("Кнопка покупки.")]
        [SerializeField]
        private Button buyButton;

        [Tooltip("Кнопка выбора.")]
        [SerializeField]
        private Button selectButton;

        [Tooltip("Индикатор купленного оружия.")]
        [SerializeField]
        private GameObject purchasedIndicator;

        [Tooltip("Индикатор выбранного оружия.")]
        [SerializeField]
        private GameObject selectedIndicator;

        [Header("Settings")]
        [Tooltip("Формат отображения стоимости.")]
        [SerializeField]
        private string costFormat = "{0}";

        #endregion

        #region FIELDS PRIVATE

        private WeaponData weaponData;
        private ICurrencyService currencyService;
        private IWeaponShopService weaponShopService;
        private bool isPurchased;
        private bool isSelected;

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
        /// Настраивает карточку оружия.
        /// </summary>
        /// <param name="data">Данные оружия.</param>
        public void Setup(WeaponData data)
        {
            if (data == null)
            {
                Debug.LogError("Данные оружия не могут быть null.");
                return;
            }

            weaponData = data;

            // Обновляем UI
            UpdateUI();
        }

        /// <summary>
        /// Обновляет состояние карточки.
        /// </summary>
        public void Refresh()
        {
            if (weaponData == null)
            {
                return;
            }

            UpdateUI();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Инициализирует компонент.
        /// </summary>
        private void Initialize()
        {
            // Загружаем сервисы
            currencyService = FindObjectOfType<ICurrencyService>();
            weaponShopService = FindObjectOfType<IWeaponShopService>();

            if (currencyService == null)
            {
                Debug.LogError("Сервис валюты не найден в сцене.");
            }

            if (weaponShopService == null)
            {
                Debug.LogError("Сервис магазина оружия не найден в сцене.");
            }

            // Подписываемся на события
            if (weaponShopService != null)
            {
                weaponShopService.OnWeaponPurchased += HandleWeaponPurchased;
                weaponShopService.OnWeaponSelected += HandleWeaponSelected;
                weaponShopService.OnDataChanged += HandleDataChanged;
            }

            if (currencyService != null)
            {
                currencyService.OnBalanceChanged += HandleBalanceChanged;
            }

            // Настраиваем кнопки
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyClicked);
            }

            if (selectButton != null)
            {
                selectButton.onClick.AddListener(OnSelectClicked);
            }
        }

        /// <summary>
        /// Очищает подписки.
        /// </summary>
        private void Cleanup()
        {
            if (weaponShopService != null)
            {
                weaponShopService.OnWeaponPurchased -= HandleWeaponPurchased;
                weaponShopService.OnWeaponSelected -= HandleWeaponSelected;
                weaponShopService.OnDataChanged -= HandleDataChanged;
            }

            if (currencyService != null)
            {
                currencyService.OnBalanceChanged -= HandleBalanceChanged;
            }

            if (buyButton != null)
            {
                buyButton.onClick.RemoveListener(OnBuyClicked);
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveListener(OnSelectClicked);
            }
        }

        /// <summary>
        /// Обновляет UI элементы.
        /// </summary>
        private void UpdateUI()
        {
            if (weaponData == null)
            {
                return;
            }

            // Обновляем изображение
            if (weaponImage != null && weaponData.WeaponIcon != null)
            {
                weaponImage.sprite = weaponData.WeaponIcon;
            }

            // Обновляем текст
            if (nameText != null)
            {
                nameText.text = weaponData.WeaponName;
            }

            if (typeText != null)
            {
                typeText.text = GetWeaponTypeText(weaponData.WeaponType);
            }

            if (costText != null)
            {
                costText.text = string.Format(costFormat, weaponData.BaseCost);
            }

            // Проверяем статус
            isPurchased = weaponShopService != null && weaponShopService.IsWeaponPurchased(weaponData.WeaponId);
            isSelected = weaponShopService != null && weaponShopService.IsWeaponSelected(weaponData.WeaponId);

            // Обновляем индикаторы
            if (purchasedIndicator != null)
            {
                purchasedIndicator.SetActive(isPurchased);
            }

            if (selectedIndicator != null)
            {
                selectedIndicator.SetActive(isSelected);
            }

            // Обновляем кнопки
            UpdateButtons();
        }

        /// <summary>
        /// Обновляет состояние кнопок.
        /// </summary>
        private void UpdateButtons()
        {
            if (buyButton == null || selectButton == null)
            {
                return;
            }

            if (isPurchased)
            {
                // Оружие куплено
                buyButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(true);

                // Если выбрано - отключаем кнопку
                selectButton.interactable = !isSelected;
            }
            else
            {
                // Оружие не куплено
                buyButton.gameObject.SetActive(true);
                selectButton.gameObject.SetActive(false);

                // Проверяем достаточно ли валюты
                bool canAfford = currencyService != null && currencyService.HasEnoughCurrency(weaponData.BaseCost);
                buyButton.interactable = canAfford;
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

        /// <summary>
        /// Обрабатывает нажатие кнопки покупки.
        /// </summary>
        private void OnBuyClicked()
        {
            if (weaponShopService != null && weaponData != null)
            {
                weaponShopService.BuyWeapon(weaponData.WeaponId);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки выбора.
        /// </summary>
        private void OnSelectClicked()
        {
            if (weaponShopService != null && weaponData != null)
            {
                weaponShopService.SelectWeapon(weaponData.WeaponId);
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Обрабатывает покупку оружия.
        /// </summary>
        private void HandleWeaponPurchased(string weaponId)
        {
            if (weaponData != null && weaponData.WeaponId == weaponId)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Обрабатывает выбор оружия.
        /// </summary>
        private void HandleWeaponSelected(string weaponId)
        {
            Refresh();
        }

        /// <summary>
        /// Обрабатывает изменение данных.
        /// </summary>
        private void HandleDataChanged()
        {
            Refresh();
        }

        /// <summary>
        /// Обрабатывает изменение баланса.
        /// </summary>
        private void HandleBalanceChanged(int newBalance)
        {
            UpdateButtons();
        }

        #endregion
    }
}
*/
