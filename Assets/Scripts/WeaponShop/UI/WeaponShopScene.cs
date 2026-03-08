/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит основной скрипт для управления сценой магазина оружия

using UnityEngine;
using WeaponShop;

namespace WeaponShop
{
    /// <summary>
    /// Основной скрипт для управления сценой магазина оружия.
    /// </summary>
    public class WeaponShopScene : MonoBehaviour
    {
        #region FIELDS

        [Header("UI References")]
        [Tooltip("Ссылка на UI списка оружия.")]
        [SerializeField]
        private WeaponListUI weaponListUI;

        [Tooltip("Ссылка на UI предпросмотра оружия.")]
        [SerializeField]
        private WeaponPreview3D weaponPreview3D;

        [Tooltip("Ссылка на UI панели параметров.")]
        [SerializeField]
        private WeaponStatsPanel weaponStatsPanel;

        [Tooltip("Ссылка на UI баланса валюты.")]
        [SerializeField]
        private CurrencyBalanceUI currencyBalanceUI;

        [Header("Scene Settings")]
        [Tooltip("Кнопка выхода из магазина.")]
        [SerializeField]
        private UnityEngine.UI.Button exitButton;

        #endregion

        #region FIELDS PRIVATE

        private IWeaponShopService weaponShopService;
        private ICurrencyService currencyService;

        #endregion

        #region UNITY METHODS

        private void Awake()
        {
            InitializeServices();
        }

        private void Start()
        {
            SetupUI();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Инициализирует сервисы.
        /// </summary>
        private void InitializeServices()
        {
            // Ищем IWeaponShopService (WeaponInventoryManager)
            weaponShopService = FindObjectOfType<IWeaponShopService>();
             
            currencyService = FindObjectOfType<ICurrencyService>();

            if (weaponShopService == null)
            {
                Debug.LogError("Сервис магазина оружия не найден в сцене.");
            }

            if (currencyService == null)
            {
                Debug.LogError("Сервис валюты не найден в сцене.");
            }
        }

        /// <summary>
        /// Настраивает UI.
        /// </summary>
        private void SetupUI()
        {
            // Настраиваем кнопку выхода
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }

            // Инициализируем UI компоненты
            if (weaponListUI != null)
            {
                weaponListUI.RefreshWeaponList();
            }

            if (weaponPreview3D != null)
            {
                // Предпросмотр автоматически обновится при выборе оружия
            }

            if (weaponStatsPanel != null)
            {
                // Панель параметров автоматически обновится при выборе оружия
            }

            if (currencyBalanceUI != null)
            {
                // Баланс автоматически обновится через событие
            }

            Debug.Log("UI магазина оружия инициализирован.");
        }

        /// <summary>
        /// Очищает подписки.
        /// </summary>
        private void Cleanup()
        {
            if (exitButton != null)
            {
                exitButton.onClick.RemoveListener(OnExitClicked);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки выхода.
        /// </summary>
        private void OnExitClicked()
        {
            Debug.Log("Выход из магазина оружия.");

            // Здесь можно добавить логику выхода из магазина
            // Например, загрузка предыдущей сцены или закрытие магазина
        }

        #endregion
    }
}
*/
