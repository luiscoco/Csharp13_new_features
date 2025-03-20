# C # version 13 New Features Samples

## 1. params collections

In **C# 13**, the **params** modifier has been expanded beyond arrays to include various other collection types. 

Previously, you could only use params with **arrays** (like params int[] numbers).

Now, it also supports types like **Span<T>**, **ReadOnlySpan<T>**, and any collection implementing **IEnumerable<T>** or related interfaces (**ICollection<T>**, **IList<T>**, **IReadOnlyCollection<T>**, etc.), provided they have an Add method.

**Before C# 13**:

```csharp
public void PrintNumbers(params int[] numbers)
{
    foreach (var number in numbers)
    {
        Console.Write($"{number} ");
    }
    Console.WriteLine();
}

PrintNumbers(1, 2, 3, 4);
```

```
// Output: 1 2 3 4
```

**In C# 13 (extended params)**:

Allows other collection types (including **Span<T>**, **ReadOnlySpan<T>**, and interfaces like **IEnumerable<T>**) as parameters.

When an interface (e.g., **IEnumerable<T>**) is specified, the compiler creates an internal storage structure for arguments automatically.

### 1.1. Example 1: Using params ReadOnlySpan<T>

```csharp
public void Concat<T>(params ReadOnlySpan<T> items)
{
    for (int i = 0; i < items.Length; i++)
    {
        Console.Write($"{items[i]} ");
    }
    Console.WriteLine();
}

Concat(1, 2, 3, 4);
```

```
// Output: 1 2 3 4
```

```csharp
Concat("Hello", "world", "!");
```

```
// Output: Hello world !
```

### 1.2. Example 2: Using params IEnumerable<T>

You can also use interfaces directly

```csharp
public void DisplayItems<T>(params IEnumerable<T> items)
{
    foreach (var item in items)
    {
        Console.Write($"{item} ");
    }
    Console.WriteLine();
}

// Using directly supplied items
DisplayItems(10, 20, 30);
```

```
// Output: 10 20 30
```

```csharp
// Or passing existing collections
List<string> fruits = ["apple", "banana", "kiwi"];
DisplayItems(fruits);
```

```
// Output: apple banana kiwi
```

### 1.3. Example 3: Custom Collection Type with params

If your custom type implements IEnumerable<T> and has an Add method, you can use it with params

```csharp
public class MyCollection<T> : IEnumerable<T>
{
    private readonly List<T> _innerList = new();

    public void Add(T item) => _innerList.Add(item);

    public IEnumerator<T> GetEnumerator() => _innerList.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public void UseMyCollection(params MyCollection<int> numbers)
{
    foreach (var num in numbers)
    {
        Console.Write($"{num} ");
    }
    Console.WriteLine();
}

UseMyCollection(1, 2, 3);
```

```
// Output: 1 2 3
```

## 2. New lock object

In **C# 13** and **.NET 9**, there's a new thread synchronization primitive: **System.Threading.Lock**.

It offers improved synchronization performance and cleaner semantics compared to the previous lock pattern, which used the traditional **System.Threading.Monitor**.

**Previously**, the **lock** statement used the **System.Threading.Monitor** class under the hood:

```csharp
private readonly object _sync = new();

lock (_sync)
{
    // critical section
}
```

Now, **.NET 9** introduces the new synchronization primitive **System.Threading.Lock**, improving clarity and performance.

**Key changes**:

Provides a clear, structured API using **EnterScope()** method returning a **ref struct**.

Uses the **Dispose()** pattern implicitly, enabling simpler and safer code through the **using** statement.

The compiler automatically detects if the **lock** target is a **Lock** type and uses its optimized **API**.

If the **lock** object is converted or cast to another type, the compiler will revert to generating the traditional Monitor-based locking code.

**Benefits of the new Lock object**:

**Performance**: Improved and optimized for modern hardware and runtime.

**Clarity**: Explicit, well-defined API makes it clear where locking begins and ends.

**Safety**: Built-in disposal pattern (IDisposable) ensures locks are released appropriately.

**Compatibility**: Automatically integrates with existing lock syntax without requiring major code changes.

**Summary**:

With the introduction of **System.Threading.Lock** in **C# 13** and **.NET 9**, you get a modern, high-performance synchronization primitive that integrates seamlessly into your existing codebase.

Simply changing the **lock** object from **object** to **Lock** gives you immediate performance and safety improvements without needing further significant refactoring.

### 2.1. Basic Usage of the new Lock (System.Threading.Lock)

```csharp
using System.Threading;

public class Counter
{
    private int _count;
    private readonly Lock _lock = new();

    public void Increment()
    {
        // Enter exclusive scope using new Lock type
        using (_lock.EnterScope())
        {
            _count++;
        }
    }

    public int GetCount()
    {
        using (_lock.EnterScope())
        {
            return _count;
        }
    }
}
```

**EnterScope()** provides a safe entry/exit model for locking.

**using** ensures the lock is properly exited (disposed) even if exceptions occur.

### 2.2 Using new Lock object with the traditional lock statement

You don't need to rewrite your code structure

The compiler recognizes if you're using a Lock object in the traditional syntax

```csharp
using System.Threading;

public class Cache
{
    private readonly Dictionary<string, string> _cache = new();
    private readonly Lock _lock = new();

    public void AddOrUpdate(string key, string value)
    {
        lock (_lock)
        {
            _cache[key] = value;
        }
    }

    public string? GetValue(string key)
    {
        lock (_lock)
        {
            return _cache.TryGetValue(key, out var value) ? value : null;
        }
    }
}
```

### 2.3. Fallback to Traditional Monitor-based Locking

If you explicitly cast or convert a **Lock** object to another type, the compiler will revert to traditional Monitor locking

```csharp
using System.Threading;

public class Demo
{
    private readonly Lock _lock = new();

    public void Execute()
    {
        object legacyLock = _lock; // implicit conversion

        lock (legacyLock)
        {
            // Compiler will use Monitor.Enter/Exit here, not the new Lock API
        }
    }
}
```

This behavior ensures backward compatibility if your codebase requires legacy synchronization behavior.


## 3. New escape sequence


## 4. Method group natural type


## 5. Implicit index access



## 6. ref and unsafe in iterators and async methods


## 7. allows ref struct



## 8. ref struct interfaces



## 9. More partial members



## 10. Overload resolution priority



## 11. The field keyword


