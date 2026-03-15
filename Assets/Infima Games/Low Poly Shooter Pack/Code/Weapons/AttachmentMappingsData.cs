//Copyright 2024, Infima Games. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Данные сопоставления одного обвеса
    /// </summary>
    [Serializable]
    public class AttachmentMapping
    {
        [Tooltip("ID обвеса из JSON конфигурации")]
        public string attachmentId;

        [Tooltip("Индекс в массиве WeaponAttachmentManager")]
        public int arrayIndex;

        [Tooltip("Название обвеса для отображения")]
        public string name;
    }

    /// <summary>
    /// Полные данные сопоставлений обвесов (глобальные для всех оружий)
    /// </summary>
    [Serializable]
    public class AttachmentMappingsData
    {
        [Tooltip("Версия формата файла")]
        public string version;

        [Tooltip("Описание файла")]
        public string description;

        [Tooltip("Сопоставления для каждого типа обвесов")]
        public Dictionary<string, List<AttachmentMapping>> mappings;
    }
}
