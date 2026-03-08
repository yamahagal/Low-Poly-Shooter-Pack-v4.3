/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит интерфейс сервиса магазина оружия для UI

using System;
using System.Collections.Generic;

namespace WeaponShop
{
    /// <summary>
    /// Интерфейс сервиса магазина оружия для UI.
    /// Содержит только необходимые методы для отображения данных.
    /// </summary>
    public interface IWeaponShopService
    {
        /// <summary>
        /// Покупает оружие. Возвращает true, если покупка успешна.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        /// <returns>True, если покупка успешна, иначе false.</returns>
        bool BuyWeapon(string weaponId);

        /// <summary>
        /// Выбирает оружие.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        void SelectWeapon(string weaponId);

        /// <summary>
        /// Получает данные оружия по идентификатору.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        /// <returns>Данные оружия или null, если не найдено.</returns>
        WeaponData GetWeaponData(string weaponId);

        /// <summary>
        /// Получает список всех доступных оружий.
        /// </summary>
        /// <returns>Список всех оружий.</returns>
        List<WeaponData> GetAllWeapons();

        /// <summary>
        /// Получает список купленных оружий.
        /// </summary>
        /// <returns>Список купленных оружий.</returns>
        List<WeaponData> GetPurchasedWeapons();

        /// <summary>
        /// Получает выбранное оружие.
        /// </summary>
        /// <returns>Данные выбранного оружия или null.</returns>
        WeaponData GetSelectedWeapon();

        /// <summary>
        /// Проверяет, куплено ли оружие.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        /// <returns>True, если оружие куплено, иначе false.</returns>
        bool IsWeaponPurchased(string weaponId);

        /// <summary>
        /// Проверяет, выбрано ли оружие.
        /// </summary>
        /// <param name="weaponId">Идентификатор оружия.</param>
        /// <returns>True, если оружие выбрано, иначе false.</returns>
        bool IsWeaponSelected(string weaponId);

        /// <summary>
        /// Событие покупки оружия.
        /// </summary>
        event Action<string> OnWeaponPurchased;

        /// <summary>
        /// Событие выбора оружия.
        /// </summary>
        event Action<string> OnWeaponSelected;

        /// <summary>
        /// Событие изменения данных.
        /// </summary>
        event Action OnDataChanged;
    }
}
*/
