# Plan: Consumer Execution Strategy

**Data:** 2026-02-09
**Status:** DRAFT

## 1. Opis zadania

Implementacja nowej strategii wykonywania konsumentów w `EventBus.SendAsync()`. Obecnie wykonanie wielu konsumentów zawsze odbywa się równolegle przez `Task.WhenAll`. Nowa funkcjonalność pozwoli na wybór między:
- **Sequential** - wykonanie konsumentów jeden po drugim (for each await)
- **Parallel** - wykonanie wszystkich konsumentów równolegle (Task.WhenAll) - obecne zachowanie

Wybór strategii będzie konfigurowany przez `EventBusBuilderOptions`.

## 2. Zakres zmian

| Warstwa | Zmiany | Pliki |
|---------|--------|-------|
| Core | Nowa opcja w options | `src/ReflectionEventing/EventBusBuilderOptions.cs` |
| Core | Strategia wykonania w EventBus | `src/ReflectionEventing/EventBus.cs` |
| Tests | Testy jednostkowe | `tests/ReflectionEventing.UnitTests/EventBusTests.cs` |

## 3. Szczegóły implementacji

### 3.1 EventBusBuilderOptions.cs

Dodać nową właściwość określającą tryb wykonania konsumentów:

```csharp
/// <summary>
/// Gets or sets the mode in which consumers are executed when sending events.
/// If set to <see cref="ProcessingMode.Sequential"/>, consumers are executed one at a time in sequence.
/// If set to <see cref="ProcessingMode.Parallel"/>, consumers are executed concurrently using Task.WhenAll.
/// The default value is <see cref="ProcessingMode.Parallel"/>.
/// </summary>
public ProcessingMode ConsumerExecutionMode { get; set; } = ProcessingMode.Parallel;
```

### 3.2 EventBus.cs

Zmienić konstruktor aby przyjmował `EventBusBuilderOptions` i na podstawie `ConsumerExecutionMode` wybrać strategię:
- `ProcessingMode.Sequential`: `foreach` z `await` dla każdego konsumenta
- `ProcessingMode.Parallel`: obecna implementacja z `WhenAll`

Kluczowe zmiany:
1. Dodać `EventBusBuilderOptions` jako dependency (przez konstruktor lub bezpośrednio)
2. W metodzie `SendAsync` sprawdzić `ConsumerExecutionMode`:
   - Dla Sequential: iteracja z await
   - Dla Parallel: obecna logika WhenAll

### 3.3 Propagacja Options do EventBus

Należy sprawdzić jak `EventBusBuilderOptions` jest przekazywane do `EventBus`. Obecnie `EventBus` nie ma dostępu do opcji - trzeba to dodać.

## 4. Migracje DB

- Wymagana: Nie

## 5. Testy do napisania

| Typ | Projekt | Co testuje |
|-----|---------|------------|
| Unit | ReflectionEventing.UnitTests | Sequential mode - konsumenty wywoływane sekwencyjnie |
| Unit | ReflectionEventing.UnitTests | Parallel mode - konsumenty wywoływane równolegle |
| Unit | ReflectionEventing.UnitTests | Domyślna wartość = Parallel (backward compatibility) |

## 6. Kolejność implementacji

1. [ ] Dodać `ConsumerExecutionMode` do `EventBusBuilderOptions.cs`
2. [ ] Zmodyfikować `EventBus.cs` aby przyjmował `EventBusBuilderOptions`
3. [ ] Zaktualizować `EventBusBuilder` i DI extensions aby przekazywać opcje
4. [ ] Zaimplementować logikę wyboru strategii w `SendAsync`
5. [ ] Dodać testy jednostkowe
6. [ ] Zweryfikować kompilację i testy

## 7. Ryzyka i decyzje

- [x] Domyślna wartość `Parallel` zachowuje backward compatibility
- [ ] Trzeba sprawdzić jak `DependencyInjectionEventBus` i inne implementacje mają być zaktualizowane
- [ ] Rozważyć czy użyć istniejącego `ProcessingMode` enum czy stworzyć nowy dedykowany

## 8. Aktualizacja dokumentacji

- [ ] Zaktualizować README.md z przykładem konfiguracji
- [ ] Dodać XML docs do nowej właściwości
