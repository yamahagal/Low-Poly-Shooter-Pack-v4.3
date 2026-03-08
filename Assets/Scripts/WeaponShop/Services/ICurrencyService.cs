/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит интерфейс сервиса валюты для магазина

using System;

namespace WeaponShop
{
    /// <summary>
    /// Интерфейс сервиса валюты.
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Получает текущий баланс валюты.
        /// </summary>
        int GetBalance();

        /// <summary>
        /// Добавляет валюту игроку.
        /// </summary>
        /// <param name="amount">Количество валюты для добавления.</param>
        void AddCurrency(int amount);

        /// <summary>
        /// Тратит валюту. Возвращает true, если операция успешна.
        /// </summary>
        /// <param name="amount">Количество валюты для траты.</param>
        /// <returns>True, если валюты достаточно, иначе false.</returns>
        bool SpendCurrency(int amount);

        /// <summary>
        /// Проверяет, достаточно ли валюты.
        /// </summary>
        /// <param name="amount">Количество валюты для проверки.</param>
        /// <returns>True, если валюты достаточно, иначе false.</returns>
        bool HasEnoughCurrency(int amount);

        /// <summary>
        /// Устанавливает баланс валюты.
        /// </summary>
        /// <param name="amount">Новое значение баланса.</param>
        void SetBalance(int amount);

        /// <summary>
        /// Событие изменения баланса валюты.
        /// </summary>
        event Action<int> OnBalanceChanged;
    }
}
*/
