/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит UI для отображения баланса валюты в магазине

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WeaponShop
{
    /// <summary>
    /// UI компонент для отображения баланса валюты игрока.
    /// </summary>
    public class CurrencyBalanceUI : MonoBehaviour
    {
        #region FIELDS

        [Header("UI Elements")]
        [Tooltip("Текст баланса валюты.")]
        [SerializeField]
        private TextMeshProUGUI balanceText;

        [Tooltip("Иконка валюты.")]
        [SerializeField]
        private Image currencyIcon;

        [Header("Settings")]
        [Tooltip("Формат отображения баланса.")]
        [SerializeField]
        private string balanceFormat = "{0}";

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
        /// Обновляет отображение баланса.
        /// </summary>
        /// <param name="balance">Текущий баланс.</param>
        public void UpdateBalance(int balance)
        {
            if (balanceText != null)
            {
                balanceText.text = string.Format(balanceFormat, balance);
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Инициализирует компонент.
        /// </summary>
        private void Initialize()
        {
            CurrencyService currencyService = FindObjectOfType<CurrencyService>();

            if (currencyService == null)
            {
                Debug.LogError("Сервис валюты не найден в сцене.");
                return;
            }

            // Подписываемся на событие изменения баланса
            currencyService.OnBalanceChanged += HandleBalanceChanged;

            // Обновляем начальный баланс
            UpdateBalance(currencyService.GetBalance());
        }

        /// <summary>
        /// Очищает подписки.
        /// </summary>
        private void Cleanup()
        {
            CurrencyService currencyService = FindObjectOfType<CurrencyService>();

            if (currencyService != null)
            {
                currencyService.OnBalanceChanged -= HandleBalanceChanged;
            }
        }

        /// <summary>
        /// Обрабатывает изменение баланса.
        /// </summary>
        /// <param name="newBalance">Новый баланс.</param>
        private void HandleBalanceChanged(int newBalance)
        {
            UpdateBalance(newBalance);
        }

        #endregion
    }
}
*/
