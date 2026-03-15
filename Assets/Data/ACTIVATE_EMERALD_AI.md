# Как активировать Emerald AI

## Текущая ситуация

[`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1) использует условную компиляцию и сейчас находится в неактивном состоянии. Это позволяет проекту компилироваться без ошибок, даже если Emerald AI не установлен.

## Что это значит

- ✅ Проект компилируется без ошибок
- ✅ Вы можете использовать [`ImprovedEnemyAI.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/ImprovedEnemyAI.cs:1) прямо сейчас
- ✅ [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1) готов к использованию, когда Emerald AI будет установлен

## Как активировать Emerald AI

### Шаг 1: Установите Emerald AI

Если вы ещё не установили Emerald AI:
1. Import Package > Emerald AI
2. Дождитесь полной установки

### Шаг 2: Дождитесь компиляции

Unity автоматически скомпилирует Emerald AI. Это может занять несколько минут.

### Шаг 3: Определите символ компиляции

1. Откройте Edit > Project Settings > Player
2. Найдите раздел "Scripting Define Symbols"
3. Добавьте символ: `EMERALD_AI_PRESENT`
4. Нажмите Apply

### Шаг 4: Перезапустите Unity

После добавления символа компиляции перезапустите Unity, чтобы изменения вступили в силу.

## Проверка активации

После активации Emerald AI:

1. Проверьте Console - не должно быть ошибок, связанных с EmeraldAIEnemy.cs
2. Создайте GameObject и добавьте компонент `EmeraldAIEnemy`
3. В Inspector вы увидите все настройки Emerald AI вместо предупреждения

## Если вы не хотите активировать Emerald AI

Это нормально! Вы можете продолжать использовать [`ImprovedEnemyAI.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/ImprovedEnemyAI.cs:1), который:

- ✅ Работает прямо сейчас
- ✅ Не требует Emerald AI
- ✅ Имеет 8 состояний ИИ
- ✅ Полностью интегрирован с NavMesh
- ✅ Имеет полную документацию

## Документация

- [ImprovedEnemyAI Documentation](Assets/Data/README_IMPROVED_ENEMY_AI.md) - полное руководство
- [Emerald AI Integration Guide](Assets/Data/EMERALD_AI_INTEGRATION_GUIDE.md) - для использования Emerald AI
- [Enemy AI Solution](Assets/Data/ENEMY_AI_SOLUTION.md) - решение и рекомендации

## Резюме

**Сейчас:** Используйте [`ImprovedEnemyAI.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/ImprovedEnemyAI.cs:1) - он работает сразу!

**В будущем:** Если захотите использовать Emerald AI, просто определите символ `EMERALD_AI_PRESENT` в Player Settings.
