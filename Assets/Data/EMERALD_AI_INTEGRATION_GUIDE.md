# Emerald AI Integration Guide

## Обзор

Этот документ описывает интеграцию Emerald AI с существующей системой игры Low Poly Shooter Pack.

## Архитектура Emerald AI

Emerald AI состоит из нескольких компонентов, которые работают вместе:

### Основные компоненты

1. **EmeraldSystem** - главный компонент, который координирует все остальные компоненты
2. **EmeraldDetection** - отвечает за обнаружение целей и препятствий
3. **EmeraldCombat** - управляет боевой системой и атаками
4. **EmeraldMovement** - управляет движением через NavMeshAgent
5. **EmeraldAnimation** - управляет анимациями и состоянием смерти
6. **EmeraldHealth** - управляет здоровьем и получением урона
7. **EmeraldBehaviors** - управляет поведением ИИ (патрулирование, преследование и т.д.)
8. **EmeraldSounds** - управляет звуковыми эффектами

### Доступ к компонентам

Все компоненты доступны через EmeraldSystem:

```csharp
EmeraldSystem emeraldAI = GetComponent<EmeraldSystem>();

// Доступ к компонентам
emeraldAI.DetectionComponent
emeraldAI.CombatComponent
emeraldAI.MovementComponent
emeraldAI.AnimationComponent
emeraldAI.HealthComponent
emeraldAI.BehaviorsComponent
emeraldAI.SoundComponent
```

## Интеграция с существующей системой

### Вариант 1: Полная замена

Замените существующую систему EnemyNavigation.cs на Emerald AI.

**Плюсы:**
- Мощная система ИИ из коробки
- Много встроенных функций
- Активная поддержка и обновления

**Минусы:**
- Требует полной переработки существующего кода
- Может быть сложной в настройке
- Потенциальная несовместимость с существующими системами

### Вариант 2: Параллельная работа (Рекомендуется)

Используйте Emerald AI для новых врагов, сохраняя существующую систему для старых врагов.

**Плюсы:**
- Не ломает существующую систему
- Позволяет постепенно мигрировать на Emerald AI
- Можно сравнивать системы и выбирать лучшую для каждого случая

**Минусы:**
- Две системы для поддержки
- Требует больше памяти

### Вариант 3: Адаптер

Создайте адаптер, который позволяет использовать Emerald AI вместе с существующей системой.

**Плюсы:**
- Единый интерфейс для всех врагов
- Плавная миграция
- Гибкость в выборе системы

**Минусы:**
- Требует дополнительного кода
- Может быть сложным в реализации

## Пример интеграции

### Создание нового врага с Emerald AI

```csharp
using UnityEngine;
using EmeraldAI;

namespace InfimaGames.LowPolyShooterPack
{
    public class EmeraldAIEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private EmeraldSystem emeraldAI;
        [SerializeField] private Transform playerTarget;
        
        private void Start()
        {
            // Получаем ссылку на Emerald AI
            if (emeraldAI == null)
            {
                emeraldAI = GetComponent<EmeraldSystem>();
            }
            
            // Настраиваем параметры
            SetupEmeraldAI();
        }
        
        private void SetupEmeraldAI()
        {
            // Устанавливаем цель
            if (playerTarget != null)
            {
                emeraldAI.CombatTarget = playerTarget;
            }
            
            // Настраиваем дальность обнаружения
            if (emeraldAI.DetectionComponent != null)
            {
                emeraldAI.DetectionComponent.DetectionRadius = 20;
            }
            
            // Настраиваем здоровье
            if (emeraldAI.HealthComponent != null)
            {
                emeraldAI.HealthComponent.StartingHealth = 100;
                emeraldAI.HealthComponent.CurrentHealth = 100;
            }
        }
        
        // Реализация IDamageable для получения урона
        public void Damage(int damageAmount, Vector3 hitPosition, Transform attacker)
        {
            if (emeraldAI != null && emeraldAI.HealthComponent != null)
            {
                emeraldAI.HealthComponent.CurrentHealth -= damageAmount;
            }
        }
    }
}
```

## Основные свойства и методы

### EmeraldSystem

```csharp
// Цели
emeraldAI.CombatTarget          // Текущая боевая цель
emeraldAI.TargetToFollow        // Цель для следования
emeraldAI.LookAtTarget          // Цель для взгляда

// Компоненты
emeraldAI.DetectionComponent    // Компонент обнаружения
emeraldAI.CombatComponent       // Компонент боя
emeraldAI.MovementComponent     // Компонент движения
emeraldAI.AnimationComponent    // Компонент анимации
emeraldAI.HealthComponent       // Компонент здоровья
emeraldAI.BehaviorsComponent    // Компонент поведения
emeraldAI.SoundComponent        // Компонент звуков

// NavMeshAgent
emeraldAI.m_NavMeshAgent        // NavMeshAgent для движения
```

### EmeraldDetection

```csharp
// Обнаружение
emeraldAI.DetectionComponent.DetectionRadius        // Радиус обнаружения
emeraldAI.DetectionComponent.FieldOfViewAngle        // Угол обзора
emeraldAI.DetectionComponent.PlayerTag               // Тег игрока
emeraldAI.DetectionComponent.CurrentDetectionState   // Текущее состояние обнаружения

// Состояния обнаружения
EmeraldDetection.DetectionStates.Alert    // Тревога
EmeraldDetection.DetectionStates.Unaware // Не подозревает
```

### EmeraldCombat

```csharp
// Боевое состояние
emeraldAI.CombatComponent.CombatState    // Находится ли в бою

// Атаки
emeraldAI.CombatComponent.Type1Attacks   // Атаки типа 1
emeraldAI.CombatComponent.Type2Attacks   // Атаки типа 2

// Оружие
emeraldAI.CombatComponent.CurrentWeaponType    // Текущий тип оружия
```

### EmeraldHealth

```csharp
// Здоровье
emeraldAI.HealthComponent.CurrentHealth   // Текущее здоровье
emeraldAI.HealthComponent.StartingHealth  // Начальное здоровье
emeraldAI.HealthComponent.Health           // Свойство для получения/установки здоровья
emeraldAI.HealthComponent.StartHealth     // Свойство для получения/установки начального здоровья

// События
emeraldAI.HealthComponent.OnTakeDamage    // Событие при получении урона
emeraldAI.HealthComponent.OnDeath         // Событие при смерти
```

### EmeraldAnimation

```csharp
// Состояние анимации
emeraldAI.AnimationComponent.IsDead    // Мертв ли персонаж
```

## Настройка врага в Unity

### Шаг 1: Добавьте EmeraldSystem

1. Выберите префаб врага
2. Добавьте компонент `EmeraldSystem`
3. Emerald AI автоматически добавит все необходимые компоненты

### Шаг 2: Настройте параметры Emerald AI

1. В Inspector настройте параметры для каждого компонента:
   - **Detection**: DetectionRadius, FieldOfViewAngle, PlayerTag
   - **Health**: StartingHealth, CurrentHealth
   - **Combat**: CombatActions, AttackClass
   - **Movement**: Speed, Acceleration
   - **Behaviors**: WanderType, ChaseType

### Шаг 3: Добавьте скрипт интеграции

1. Добавьте скрипт `EmeraldAIEnemy` к префабу врага
2. Настройте ссылки в Inspector:
   - Emerald AI: перетащите объект с EmeraldSystem
   - Player Target: перетащите игрока

### Шаг 4: Настройте NavMesh

1. Убедитесь, что на сцене есть NavMesh
2. Добавьте NavMeshAgent к врагу (если его нет)
3. Настройте параметры NavMeshAgent

## Интеграция с системой оружия

### Получение урона от игрока

Emerald AI использует интерфейс `IDamageable` для получения урона:

```csharp
public interface IDamageable
{
    void Damage(int damageAmount, Vector3 hitPosition, Transform attacker);
}
```

Чтобы ваш враг мог получать урон от системы оружия Low Poly Shooter Pack, реализуйте этот интерфейс:

```csharp
public class EmeraldAIEnemy : MonoBehaviour, IDamageable
{
    public void Damage(int damageAmount, Vector3 hitPosition, Transform attacker)
    {
        if (emeraldAI != null && emeraldAI.HealthComponent != null)
        {
            emeraldAI.HealthComponent.CurrentHealth -= damageAmount;
        }
    }
}
```

## Создание разных типов врагов

### Разведчик (Scout)

```csharp
// Высокая скорость, низкое здоровье, большой радиус обнаружения
emeraldAI.m_NavMeshAgent.speed = 5f;
emeraldAI.HealthComponent.StartingHealth = 50;
emeraldAI.DetectionComponent.DetectionRadius = 30;
```

### Штурмовик (Assault)

```csharp
// Средняя скорость, среднее здоровье, средний радиус обнаружения
emeraldAI.m_NavMeshAgent.speed = 3.5f;
emeraldAI.HealthComponent.StartingHealth = 100;
emeraldAI.DetectionComponent.DetectionRadius = 20;
```

### Снайпер (Sniper)

```csharp
// Низкая скорость, высокое здоровье, большой радиус обнаружения
emeraldAI.m_NavMeshAgent.speed = 2f;
emeraldAI.HealthComponent.StartingHealth = 150;
emeraldAI.DetectionComponent.DetectionRadius = 40;
```

### Поддержка (Support)

```csharp
// Низкая скорость, высокое здоровье, малый радиус обнаружения
emeraldAI.m_NavMeshAgent.speed = 2.5f;
emeraldAI.HealthComponent.StartingHealth = 200;
emeraldAI.DetectionComponent.DetectionRadius = 15;
```

## Отладка

### Включение отладки в Emerald AI

1. В Inspector найдите компонент `EmeraldDebugger`
2. Включите опцию `Debug Mode`
3. Вы увидите отладочную информацию в Scene View и Game View

### Пользовательская отладка

```csharp
private void OnGUI()
{
    GUILayout.Label($"Health: {emeraldAI.HealthComponent.CurrentHealth}");
    GUILayout.Label($"State: {GetCurrentState()}");
    GUILayout.Label($"Can See Player: {CheckPlayerVisibility()}");
}

private void OnDrawGizmos()
{
    // Рисуем сферу обнаружения
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, 
        emeraldAI.DetectionComponent.DetectionRadius);
    
    // Рисуем сферу атаки
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
```

## Советы и лучшие практики

1. **Используйте ScriptableObjects** для хранения конфигураций разных типов врагов
2. **Создайте базовый класс** для всех врагов с Emerald AI, чтобы избежать дублирования кода
3. **Используйте события** Emerald AI (OnTakeDamage, OnDeath и т.д.) для интеграции с другими системами
4. **Тестируйте на NavMesh**, убедитесь, что враги могут перемещаться по вашей сцене
5. **Настройте слои** (Layers) правильно для системы обнаружения Emerald AI
6. **Используйте теги** для идентификации игрока и других целей

## Устранение проблем

### Враг не двигается

- Проверьте, есть ли NavMesh на сцене
- Проверьте, что NavMeshAgent настроен правильно
- Убедитесь, что враг находится на NavMesh слое

### Враг не обнаруживает игрока

- Проверьте, что у игрока правильный тег
- Проверьте DetectionRadius и FieldOfViewAngle
- Убедитесь, что слои (Layers) настроены правильно
- Проверьте ObstructionDetectionLayerMask

### Враг не атакует

- Проверьте, что CombatActions настроены
- Убедитесь, что AttackClass содержит атаки
- Проверьте, что цель установлена (CombatTarget)

### Враг не получает урон

- Убедитесь, что класс реализует IDamageable
- Проверьте, что метод Damage() вызывает HealthComponent.CurrentHealth
- Убедитесь, что оружие игрока вызывает Damage() на правильном объекте

## Дополнительные ресурсы

- [Emerald AI Documentation](https://black-horizon-studios.gitbook.io/emerald-ai-wiki/)
- [Unity NavMesh Documentation](https://docs.unity3d.com/Manual/nav-BuildingNavMesh.html)
- [Low Poly Shooter Pack Documentation](https://assetstore.unity.com/packages/tools/integration/low-poly-shooter-pack-191269)

## Заключение

Emerald AI - мощная система ИИ, которая может значительно улучшить поведение врагов в вашей игре. Рекомендуется начать с параллельной работы (Вариант 2), чтобы не ломать существующую систему и постепенно мигрировать на Emerald AI.
