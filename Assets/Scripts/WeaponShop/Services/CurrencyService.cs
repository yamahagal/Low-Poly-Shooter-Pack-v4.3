/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит сервис управления валютой для магазина

using System;
using UnityEngine;

namespace WeaponShop
{
    /// <summary>
    /// Сервис управления валютой игрока.
    /// </summary>
    public class CurrencyService : MonoBehaviour, ICurrencyService
    {
        #region FIELDS

        [Header("Settings")]
        [Tooltip("Начальный баланс валюты.")]
        [SerializeField]
        private int startingBalance = 1000;

        [Tooltip("Путь к файлу сохранения.")]
        [SerializeField]
        private string savePath = "saves/currency_save.json";

        private int balance;

        #endregion

        #region EVENTS

        /// <summary>
        /// Событие изменения баланса валюты.
        /// </summary>
        public event Action<int> OnBalanceChanged;

        #endregion

        #region UNITY METHODS

        private void Awake()
        {
            LoadBalance();
        }

        private void OnDestroy()
        {
            SaveBalance();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Получает текущий баланс валюты.
        /// </summary>
        public int GetBalance()
        {
            return balance;
        }

        /// <summary>
        /// Добавляет валюту игроку.
        /// </summary>
        /// <param name="amount">Количество валюты для добавления.</param>
        public void AddCurrency(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"Попытка добавить отрицательное или нулевое количество валюты: {amount}");
                return;
            }

            balance += amount;
            OnBalanceChanged?.Invoke(balance);
            SaveBalance();
        }

        /// <summary>
        /// Тратит валюту. Возвращает true, если операция успешна.
        /// </summary>
        /// <param name="amount">Количество валюты для траты.</param>
        /// <returns>True, если валюты достаточно, иначе false.</returns>
        public bool SpendCurrency(int amount)
        {
            if (!HasEnoughCurrency(amount))
            {
                Debug.LogWarning($"Недостаточно валюты. Требуется: {amount}, Доступно: {balance}");
                return false;
            }

            balance -= amount;
            OnBalanceChanged?.Invoke(balance);
            SaveBalance();
            return true;
        }

        /// <summary>
        /// Проверяет, достаточно ли валюты.
        /// </summary>
        /// <param name="amount">Количество валюты для проверки.</param>
        /// <returns>True, если валюты достаточно, иначе false.</returns>
        public bool HasEnoughCurrency(int amount)
        {
            return balance >= amount;
        }

        /// <summary>
        /// Устанавливает баланс валюты.
        /// </summary>
        /// <param name="amount">Новое значение баланса.</param>
        public void SetBalance(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Попытка установить отрицательный баланс: {amount}");
                return;
            }

            balance = amount;
            OnBalanceChanged?.Invoke(balance);
            SaveBalance();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Загружает баланс из файла сохранения.
        /// </summary>
        private void LoadBalance()
        {
            string fullPath = System.IO.Path.Combine(Application.persistentDataPath, savePath);

            if (!System.IO.File.Exists(fullPath))
            {
                balance = startingBalance;
                Debug.Log($"Файл сохранения не найден. Установлен начальный баланс: {balance}");
                return;
            }

            try
            {
                string json = System.IO.File.ReadAllText(fullPath);
                CurrencySaveData data = JsonUtility.FromJson<CurrencySaveData>(json);
                balance = data.balance;
                Debug.Log($"Баланс загружен: {balance}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки баланса: {e.Message}");
                balance = startingBalance;
            }
        }

        /// <summary>
        /// Сохраняет баланс в файл.
        /// </summary>
        private void SaveBalance()
        {
            string fullPath = System.IO.Path.Combine(Application.persistentDataPath, savePath);
            string directory = System.IO.Path.GetDirectoryName(fullPath);

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            try
            {
                CurrencySaveData data = new CurrencySaveData { balance = balance };
                string json = JsonUtility.ToJson(data, true);
                System.IO.File.WriteAllText(fullPath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка сохранения баланса: {e.Message}");
            }
        }

        #endregion

        #region SAVE DATA

        /// <summary>
        /// Данные сохранения валюты.
        /// </summary>
        [System.Serializable]
        private class CurrencySaveData
        {
            public int balance;
        }

        #endregion
    }
}
*/
