# Emerald AI - Краткое руководство

## Что было сделано

✅ Исправлены ошибки компиляции в [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1)
✅ Обновлён код для использования правильных типов и методов Emerald AI
✅ Создана подробная документация по интеграции Emerald AI
✅ Добавлена реализация интерфейса `IDamageable` для получения урона

## Быстрый старт

### 1. Создание врага с Emerald AI

1. Создайте новый GameObject или префаб для врага
2. Добавьте компонент `EmeraldSystem` (он автоматически добавит все необходимые компоненты)
3. Добавьте скрипт [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1)
4. Настройте ссылки в Inspector:
   - **Emerald AI**: перетащите объект с EmeraldSystem
   - **Player Target**: перетащите игрока

### 2. Настройка параметров

В Inspector настройте параметры Emerald AI:

**Detection (Обнаружение):**
- `DetectionRadius`: 20 (радиус обнаружения)
- `FieldOfViewAngle`: 270 (угол обзора)
- `PlayerTag`: "Player" (тег игрока)

**Health (Здоровье):**
- `StartingHealth`: 100 (начальное здоровье)
- `CurrentHealth`: 100 (текущее здоровье)

**Movement (Движение):**
- `Speed`: 3.5 (скорость движения через NavMeshAgent)

### 3. Настройка NavMesh

1. Откройте Window > AI > Navigation
2. Выберите объекты сцены, по которым должен двигаться враг
3. Нажмите "Bake" для создания NavMesh
4. Убедитесь, что враг находится на NavMesh

## Основные компоненты

```csharp
EmeraldSystem emeraldAI = GetComponent<EmeraldSystem>();

// Доступ к компонентам
emeraldAI.DetectionComponent    // Обнаружение
emeraldAI.CombatComponent       // Бой
emeraldAI.MovementComponent     // Движение
emeraldAI.AnimationComponent    // Анимация
emeraldAI.HealthComponent       // Здоровье
emeraldAI.BehaviorsComponent    // Поведение
```

## Получение урона

Скрипт [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1) реализует интерфейс `IDamageable`, поэтому враг может получать урон от системы оружия Low Poly Shooter Pack:

```csharp
public void Damage(int damageAmount, Vector3 hitPosition, Transform attacker)
{
    if (emeraldAI != null && emeraldAI.HealthComponent != null)
    {
        emeraldAI.HealthComponent.CurrentHealth -= damageAmount;
    }
}
```

## Создание разных типов врагов

### Разведчик (Scout)
```csharp
emeraldAI.m_NavMeshAgent.speed = 5f;
emeraldAI.HealthComponent.StartingHealth = 50;
emeraldAI.DetectionComponent.DetectionRadius = 30;
```

### Штурмовик (Assault)
```csharp
emeraldAI.m_NavMeshAgent.speed = 3.5f;
emeraldAI.HealthComponent.StartingHealth = 100;
emeraldAI.DetectionComponent.DetectionRadius = 20;
```

### Снайпер (Sniper)
```csharp
emeraldAI.m_NavMeshAgent.speed = 2f;
emeraldAI.HealthComponent.StartingHealth = 150;
emeraldAI.DetectionComponent.DetectionRadius = 40;
```

### Поддержка (Support)
```csharp
emeraldAI.m_NavMeshAgent.speed = 2.5f;
emeraldAI.HealthComponent.StartingHealth = 200;
emeraldAI.DetectionComponent.DetectionRadius = 15;
```

## Отладка

Включите `Debug Mode` в Inspector компонента `EmeraldAIEnemy` для просмотра:

- Текущего здоровья
- Состояния врага (Dead, Attacking, Alert, Patrolling)
- Видимости игрока

Также будут отображаться Gizmos:
- Жёлтая сфера: радиус обнаружения
- Красная сфера: радиус атаки
- Линия: направление взгляда

## Интеграция с существующей системой

**Рекомендуется использовать параллельную работу:**

- Старые враги продолжают использовать [`EnemyNavigation.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EnemyNavigation.cs:1)
- Новые враги используют Emerald AI через [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1)

Это позволяет:
- ✅ Не ломать существующую систему
- ✅ Постепенно мигрировать на Emerald AI
- ✅ Сравнивать системы и выбирать лучшую для каждого случая

## Документация

- [Подробное руководство по интеграции](Assets/Data/EMERALD_AI_INTEGRATION_GUIDE.md)
- [Анализ и улучшения Enemy AI](plans/ENEMY_AI_ANALYSIS_AND_IMPROVEMENTS.md)

## Устранение проблем

### Враг не двигается
- Проверьте наличие NavMesh на сцене
- Убедитесь, что NavMeshAgent настроен правильно
- Проверьте, что враг находится на NavMesh слое

### Враг не обнаруживает игрока
- Проверьте тег игрока (должен быть "Player")
- Проверьте DetectionRadius и FieldOfViewAngle
- Убедитесь, что слои (Layers) настроены правильно

### Враг не атакует
- Проверьте, что CombatActions настроены
- Убедитесь, что AttackClass содержит атаки
- Проверьте, что цель установлена (CombatTarget)

### Враг не получает урон
- Убедитесь, что класс реализует IDamageable
- Проверьте, что метод Damage() вызывает HealthComponent.CurrentHealth
- Убедитесь, что оружие игрока вызывает Damage() на правильном объекте

## Следующие шаги

1. Создайте префаб врага с Emerald AI
2. Настройте NavMesh на вашей сцене
3. Протестируйте поведение врага
4. Создайте разные типы врагов с разными параметрами
5. Интегрируйте с системой спавна врагов

## Дополнительные ресурсы

- [Emerald AI Documentation](https://black-horizon-studios.gitbook.io/emerald-ai-wiki/)
- [Unity NavMesh Documentation](https://docs.unity3d.com/Manual/nav-BuildingNavMesh.html)
