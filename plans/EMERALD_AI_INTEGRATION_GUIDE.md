# Руководство по интеграции Emerald AI с существующей системой врагов

## 📋 Обзор ситуации

### Текущая система:
- **EnemyNavigation.cs** - существующая система AI врагов (простая FSM с if/else)
- **Emerald AI** - новая мощная система AI (FSM, Behaviour Trees, Sensory System)

### Проблема:
- Две системы используют разные подходы
- Нет коммуникации между ними
- Разные системы анимации
- Разные системы навигации

---

## 🎯 Стратегия интеграции

### Вариант 1: Полная замена (Рекомендуется)

**Зачем:** Заменить существующую систему EnemyNavigation.cs на Emerald AI

**Преимущества:**
- ✅ Мощная и проверенная система AI
- ✅ Меньше кода для поддержки
- ✅ Единая архитектура
- ✅ Лучшее поведение врагов

**Недостатки:**
- ❌ Требует переписывания всего кода врагов
- ❌ Требует перестройки всех префабов врагов
- ❌ Может сломать существующие механики

**Когда использовать:**
- Если вы только начинаете разработку
- Если готовы полностью переписать систему врагов

---

### Вариант 2: Параллельная работа (Гибридный подход)

**Зачем:** Использовать обе системы параллельно

**Преимущества:**
- ✅ Сохранить существующий код
- ✅ Плавный переход к новой системе
- ✅ Возможность сравнивать подходы

**Недостатки:**
- ❌ Сложнее в поддержке
- ❌ Возможны конфликты поведения
- ❌ Двойная нагрузка на производительность

**Когда использовать:**
- Если хотите сохранить существующую систему
- Если хотите использовать Emerald AI для определённых типов врагов

---

### Вариант 3: Адаптерный паттерн (Рекомендуется для текущей ситуации)

**Зачем:** Создать адаптер, который позволяет Emerald AI работать с существующими компонентами

**Преимущества:**
- ✅ Минимальные изменения в существующем коде
- ✅ Гибкость - можно использовать разные AI для разных врагов
- ✅ Легко отключить/включить новую систему

**Недостатки:**
- ❌ Требует дополнительного слоя абстракции
- ❌ Может быть сложнее в отладке

**Когда использовать:**
- Если хотите быстро интегрировать Emerald AI
- Если хотите протестировать новую систему без удаления старой

---

## 🏗️ Архитектура интеграции (Вариант 3 - Адаптер)

### Структура компонентов:

```
GameObject (Enemy)
├── EnemyNavigation (старая система - отключена)
├── EmeraldAIAdapter (новый адаптер)
│   ├── EmeraldAIComponent (ссылка на Emerald AI)
│   ├── EnemyBridge (мост к старой системе)
│   └── AIController (управление какой системой использовать)
└── Ragdoll (общий компонент)
```

---

## 📝 Реализация адаптера

### 1. Создание интерфейса адаптера

```csharp
public interface IEnemyAI {
    // Общие методы для обеих систем
    void SetTarget(Transform target);
    void SetState(EnemyState state);
    void TakeDamage(float damage, Vector3 hitPosition);
    void Die();
    Transform GetTransform();
    bool IsDead();
    
    // События
    event Action<Transform> OnTargetSpotted;
    event Action<Transform> OnTargetLost;
    event Action<float> OnHealthChanged;
}

public enum EnemyState {
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    Reloading,
    TakingCover,
    Investigating,
    Fleeing,
    Dead
}
```

### 2. Создание адаптера

```csharp
using UnityEngine;
using EmeraldAI;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Адаптер для интеграции Emerald AI с существующей системой врагов
    /// </summary>
    public class EmeraldAIAdapter : MonoBehaviour, IEnemyAI
    {
        [Header("Settings")]
        [SerializeField] private bool useEmeraldAI = true;
        [SerializeField] private bool debugMode = false;
        
        [Header("References")]
        [SerializeField] private EmeraldAIComponent emeraldAIComponent;
        [SerializeField] private EnemyNavigation legacyNavigation;
        
        [Header("State")]
        private EnemyState currentState = EnemyState.Idle;
        private EnemyState previousState = EnemyState.Idle;
        
        // Ссылки на компоненты старой системы
        private NavMeshAgent agent;
        private Animator animator;
        private HealthSystem healthSystem;
        
        private void Start()
        {
            // Получаем ссылки на старые компоненты
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            healthSystem = GetComponent<HealthSystem>();
            
            // Подписываемся на события Emerald AI
            if (emeraldAIComponent != null)
            {
                emeraldAIComponent.OnTargetSpotted += HandleTargetSpotted;
                emeraldAIComponent.OnTargetLost += HandleTargetLost;
                emeraldAIComponent.OnAttack += HandleAttack;
                emeraldAIComponent.OnDamageTaken += HandleDamageTaken;
            }
            
            // Отключаем старую систему если используется Emerald AI
            if (useEmeraldAI && legacyNavigation != null)
            {
                legacyNavigation.enabled = false;
            }
        }
        
        private void Update()
        {
            // Определяем, какую систему использовать
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                // Используем Emerald AI
                UpdateEmeraldAI();
            }
            else if (!useEmeraldAI && legacyNavigation != null)
            {
                // Используем старую систему
                UpdateLegacyAI();
            }
            
            // Проверяем переходы состояний
            CheckStateTransitions();
            
            // Отладка
            if (debugMode)
            {
                DebugState();
            }
        }
        
        private void UpdateEmeraldAI()
        {
            // Синхронизируем состояние с Emerald AI
            if (currentState != emeraldAIComponent.CurrentState)
            {
                currentState = emeraldAIComponent.CurrentState;
                
                // Уведомляем старую систему о новом состоянии
                if (legacyNavigation != null)
                {
                    legacyNavigation.status = GetLegacyStatus(currentState);
                }
            }
        }
        
        private void UpdateLegacyAI()
        {
            // Старая логика (существующая)
            // Оставляем без изменений
        }
        
        private void CheckStateTransitions()
        {
            // Проверяем изменения состояния
            if (currentState != previousState)
            {
                OnStateChanged(currentState);
                previousState = currentState;
            }
        }
        
        // Реализация интерфейса IEnemyAI
        public void SetTarget(Transform target)
        {
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                emeraldAIComponent.SetTarget(target);
            }
            else if (!useEmeraldAI && legacyNavigation != null)
            {
                legacyNavigation.Target = target;
            }
        }
        
        public void SetState(EnemyState state)
        {
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                emeraldAIComponent.SetState(state);
            }
            else if (!useEmeraldAI && legacyNavigation != null)
            {
                legacyNavigation.status = GetLegacyStatus(state);
            }
        }
        
        public void TakeDamage(float damage, Vector3 hitPosition)
        {
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                emeraldAIComponent.TakeDamage(damage, hitPosition);
            }
            else if (!useEmeraldAI && legacyNavigation != null)
            {
                legacyNavigation.CheckHit(damage, hitPosition);
            }
        }
        
        public void Die()
        {
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                emeraldAIComponent.Die();
            }
            else if (!useEmeraldAI && legacyNavigation != null)
            {
                // Старая логика смерти
                current_health = 0;
                isDead = true;
            }
        }
        
        public Transform GetTransform()
        {
            return transform;
        }
        
        public bool IsDead()
        {
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                return emeraldAIComponent.IsDead;
            }
            else
            {
                return isDead;
            }
        }
        
        // Обработчики событий Emerald AI
        private void HandleTargetSpotted(Transform target)
        {
            if (debugMode)
                Debug.Log($"[EmeraldAIAdapter] Цель обнаружена: {target.name}");
            
            OnTargetSpotted?.Invoke(target);
        }
        
        private void HandleTargetLost(Transform target)
        {
            if (debugMode)
                Debug.Log($"[EmeraldAIAdapter] Цель потеряна: {target.name}");
            
            OnTargetLost?.Invoke(target);
        }
        
        private void HandleAttack(Transform target)
        {
            if (debugMode)
                Debug.Log($"[EmeraldAIAdapter] Атака на: {target.name}");
            
            // Уведомляем старую систему об атаке
            if (legacyNavigation != null)
            {
                legacyNavigation.status = "attacking";
            }
        }
        
        private void HandleDamageTaken(float damage)
        {
            if (debugMode)
                Debug.Log($"[EmeraldAIAdapter] Получен урон: {damage}");
            
            OnHealthChanged?.Invoke(damage);
        }
        
        private void OnStateChanged(EnemyState newState)
        {
            if (debugMode)
                Debug.Log($"[EmeraldAIAdapter] Состояние изменено: {currentState} -> {newState}");
            
            // Вызываем события старой системы для совместимости
            if (legacyNavigation != null)
            {
                switch (newState)
                {
                    case EnemyState.Patrolling:
                        legacyNavigation.status = "patrolling";
                        break;
                    case EnemyState.Chasing:
                        legacyNavigation.status = "attacking";
                        break;
                    case EnemyState.Attacking:
                        // Уже атакуем
                        break;
                    case EnemyState.Reloading:
                        legacyNavigation.status = "patrolling";
                        break;
                }
            }
        }
        
        private string GetLegacyStatus(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                case EnemyState.Patrolling:
                case EnemyState.Reloading:
                case EnemyState.Investigating:
                    return "patrolling";
                case EnemyState.Chasing:
                case EnemyState.Attacking:
                case EnemyState.TakingCover:
                    return "attacking";
                case EnemyState.Fleeing:
                    return "patrolling";
                case EnemyState.Dead:
                    return "dead";
                default:
                    return "patrolling";
            }
        }
        
        private void DebugState()
        {
            if (useEmeraldAI && emeraldAIComponent != null)
            {
                Debug.Log($"[EmeraldAIAdapter] Используется: Emerald AI");
                Debug.Log($"[EmeraldAIAdapter] Состояние: {currentState}");
                Debug.Log($"[EmeraldAIAdapter] Цель: {emeraldAIComponent.Target?.name ?? "None"}");
            }
            else if (!useEmeraldAI && legacyNavigation != null)
            {
                Debug.Log($"[EmeraldAIAdapter] Используется: Старая система");
                Debug.Log($"[EmeraldAIAdapter] Состояние: {currentState}");
                Debug.Log($"[EmeraldAIAdapter] Цель: {legacyNavigation.Target?.name ?? "None"}");
            }
        }
    }
}
```

---

## 🎨 Настройка Emerald AI

### 1. Создание Behaviour Tree

**Шаги:**
1. Откройте Emerald AI в Unity
2. Перейдите в `Assets/Emerald AI/Demo/`
3. Откройте сцену `Demo Source`
4. Найдите объект `Demo Source/Emerald AI`
5. В инспекторе найдите компонент `Emerald AI`
6. Нажмите на `Open Behaviour Editor`
7. Создайте Behaviour Tree для врага

**Пример Behaviour Tree:**

```
Behaviour Tree
├── Selector (Проверка условий)
│   ├── Sequence (Последовательное выполнение)
│   │   ├── Condition (Проверка видимости)
│   │   ├── Condition (Проверка расстояния)
│   │   └── Action (Патрулирование)
│   ├── Sequence (Ближний бой)
│   │   ├── Action (Атака)
│   │   └── Action (Перезарядка)
│   └── Sequence (Дальний бой)
│       ├── Action (Стрельба)
│       └── Action (Отступление)
└── Decorator (Повторение)
    └── Sequence (Патрулирование)
```

### 2. Настройка параметров Emerald AI

**Основные параметры:**
- **Movement Speed** - скорость движения
- **Stopping Distance** - дистанция остановки
- **Attack Range** - дальность атаки
- **Detection Range** - дальность обнаружения
- **Reaction Time** - время реакции
- **Accuracy** - точность

**Где настроить:**
- В компоненте `Emerald AI` на префабе врага
- Или в `Emerald Combat Manager` (глобальные настройки)

---

## 🔧 Интеграция с существующими системами

### 1. Настройка префабов врагов

**Шаги:**
1. Откройте префаб врага (например, `Enemy.prefab`)
2. Добавьте компонент `Emerald AI Adapter`
3. Убедитесь, что компонент `Enemy Navigation` отключен
4. Настройте параметры адаптера:
   - `Use Emerald AI` = true
   - `Debug Mode` = false (для продакшена)
5. Сохраните префаб

### 2. Настройка Emerald AI

**Шаги:**
1. Откройте компонент `Emerald AI` на префабе
2. Настройте параметры:
   - `Movement Speed` = 3.5
   - `Stopping Distance` = 2.0
   - `Attack Range` = 15.0
   - `Detection Range` = 20.0
   - `Reaction Time` = 0.5
   - `Accuracy` = 0.7

### 3. Создание профилей врагов

**Для разных типов врагов:**
- **Разведчик** - высокая скорость, низкий урон, высокое восприятие
- **Штурмовик** - средняя скорость, средний урон, среднее восприятие
- **Снайпер** - низкая скорость, высокий урон, высокое восприятие
- **Поддержка** - низкая скорость, низкий урон, низкое восприятие, приоритет союзников

---

## 📊 Тестирование

### 1. Тестирование адаптера

**Шаги:**
1. Создайте тестовую сцену с одним врагом
2. Добавьте компонент `Emerald AI Adapter`
3. Установите `Use Emerald AI` = true
4. Запустите игру
5. Проверьте поведение врага
6. Проверьте консоль Unity на сообщения отладки

**Ожидаемые результаты:**
- Враг патрулирует по точкам
- Враг обнаруживает игрока и преследует
- Враг атакует игрока
- Враг использует укрытия
- Переходы между состояниями плавные

### 2. Сравнение производительности

**Метрики:**
- FPS с использованием старой системы
- FPS с использованием Emerald AI
- Использование CPU
- Использование памяти

---

## 🚨 Решение проблем

### Проблема 1: Конфликт анимаций

**Решение:**
```csharp
// В адаптере добавьте синхронизацию анимаций
private void SyncAnimations()
{
    if (useEmeraldAI && emeraldAIComponent != null && animator != null)
    {
        // Получаем текущее состояние анимации из Emerald AI
        string currentAnimation = emeraldAIComponent.GetCurrentAnimation();
        
        // Устанавливаем анимацию в старой системе
        if (!string.IsNullOrEmpty(currentAnimation))
        {
            animator.Play(currentAnimation);
        }
    }
}
```

### Проблема 2: Разные системы навигации

**Решение:**
```csharp
// В адаптере добавьте переключение навигации
private void SwitchNavigation()
{
    if (useEmeraldAI && emeraldAIComponent != null)
    {
        // Отключаем NavMeshAgent старой системы
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }
    else
    {
        // Включаем NavMeshAgent старой системы
        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
}
```

### Проблема 3: Система здоровья

**Решение:**
```csharp
// В адаптере синхронизируйте здоровье
public void SyncHealth()
{
    if (useEmeraldAI && emeraldAIComponent != null && healthSystem != null)
    {
        // Получаем здоровье из Emerald AI
        float emeraldHealth = emeraldAIComponent.GetHealth();
        
        // Устанавливаем в старой системе
        current_health = emeraldHealth;
        HealthBar.fillAmount = current_health / start_health;
    }
}
```

---

## 📚 Документация

### 1. Создание README

**Создайте файл:** `Assets/Emerald AI/README_INTEGRATION.md`

**Содержание:**
- Обзор интеграции
- Инструкция по настройке
- Примеры использования
- Решение типичных проблем

### 2. Обновление существующей документации

**Обновите файл:** `Assets/Data/ENEMY_AI_ANALYSIS_AND_IMPROVEMENTS.md`

**Добавьте раздел:**
- Интеграция с Emerald AI
- Адаптерный паттерн
- Примеры кода

---

## 🎯 Рекомендуемый план внедрения

### Фаза 1: Подготовка (1 день)
- [ ] Изучить документацию Emerald AI
- [ ] Создать интерфейс адаптера `IEnemyAI`
- [ ] Создать класс адаптера `EmeraldAIAdapter`
- [ ] Создать тестовую сцену
- [ ] Настроить Behaviour Tree для одного врага

### Фаза 2: Интеграция (2-3 дня)
- [ ] Добавить адаптер на префаб врага
- [ ] Настроить параметры Emerald AI
- [ ] Протестировать поведение врага
- [ ] Решить проблемы с анимациями
- [ ] Решить проблемы с навигацией
- [ ] Решить проблемы со здоровьем

### Фаза 3: Тестирование и балансировка (3-5 дней)
- [ ] Создать профили для разных типов врагов
- [ ] Настроить сложность для каждого типа
- [ ] Балансировать урон и здоровье
- [ ] Протестировать боевые сценарии
- [ ] Оптимизировать производительность

### Фаза 4: Развёртывание (6-7 дней)
- [ ] Добавить Behaviour Trees для всех типов врагов
- [ ] Создать сложные сценарии поведения
- [ ] Добавить систему коммуникации между врагами
- [ ] Настроить адаптивную сложность
- [ ] Создать систему событий

---

## 💡 Дополнительные рекомендации

### 1. Используйте ScriptableObjects

Создайте `ScriptableObject` для параметров врагов:

```csharp
[CreateAssetMenu(fileName = "EnemyProfiles")]
public class EnemyProfile : ScriptableObject
{
    public string enemyName;
    public float movementSpeed;
    public float stoppingDistance;
    public float attackRange;
    public float detectionRange;
    public float reactionTime;
    public float accuracy;
    public float health;
    public float damage;
    public EnemyType enemyType;
    
    public enum EnemyType
    {
        Scout,
        Grunt,
        Sniper,
        Support
    }
}
```

### 2. Создайте систему событий

Используйте Unity Events для коммуникации между врагами:

```csharp
public class EnemyEventManager : MonoBehaviour
{
    public static event Action<Transform> OnEnemySpotted;
    public static event Action<Transform> OnEnemyDown;
    public static event Action<Transform> OnEnemyAttacking;
    
    public static void NotifyEnemySpotted(Transform enemy)
    {
        OnEnemySpotted?.Invoke(enemy);
    }
    
    public static void NotifyEnemyDown(Transform enemy)
    {
        OnEnemyDown?.Invoke(enemy);
    }
    
    public static void NotifyEnemyAttacking(Transform enemy)
    {
        OnEnemyAttacking?.Invoke(enemy);
    }
}
```

### 3. Оптимизация производительности

- Используйте Object Pooling для врагов
- Оптимизируйте Raycast проверки
- Используйте корутины вместо Update где возможно
- Ограничьте количество проверок видимости

---

## 🎨 Заключение

**Рекомендация:** Используйте **адаптерный паттерн** для интеграции Emerald AI с существующей системой врагов.

**Преимущества адаптерного подхода:**
- ✅ Минимальные изменения в существующем коде
- ✅ Возможность быстрого переключения между системами
- ✅ Гибкость - можно использовать разные AI для разных врагов
- ✅ Легкое тестирование и отладка
- ✅ Сохранение существующих механик

**Следующие шаги:**
1. Создайте интерфейс адаптера `IEnemyAI`
2. Создайте класс адаптера `EmeraldAIAdapter`
3. Добавьте адаптер на префаб врага
4. Создайте Behaviour Tree для врага в Emerald AI
5. Настройте параметры Emerald AI
6. Протестируйте и настройте поведение

Это позволит вам использовать мощную систему Emerald AI, сохраняя при этом существующую логику игры!
