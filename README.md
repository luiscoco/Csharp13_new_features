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

In **C# 13**, a new character literal escape sequence has been introduced:

**\e** → Represents the **ESCAPE** character (Unicode U+001B).

Previously, to represent the **ESCAPE** character, you had to use either:

**\u001b** (unicode escape)

**\x1b** (hexadecimal escape)

But these had downsides:

**\x1b** could lead to errors because if the following character(s) were valid hexadecimal digits, the compiler would include them in the escape sequence unintentionally.

**\u001b** was safer but verbose.

The new **\e** escape sequence is concise, safe, and readable.

The new **\e** escape sequence in **C# 13** provides a cleaner, safer, and more readable way to represent the **ESC** character (**U+001B**), especially useful in console applications leveraging terminal escape sequences and color formatting.

### 3.1. Basic Sample

```csharp
Console.WriteLine("This is an escape character: '\e'");
```

This will print:

```
This is an escape character: '←'  // ← represents the ESC character visually
```

### 3.2. ANSI escape codes (terminal colors)

ANSI escape sequences (commonly used for coloring and formatting terminals) start with an ESC character. Using the new \e simplifies the syntax significantly

**Old way**:

```csharp
Console.WriteLine("\u001b[32mHello in Green!\u001b[0m");
```

**New way (C# 13+)**:

```csharp
Console.WriteLine("\e[32mHello in Green!\e[0m");
```

Both examples print:

![image](https://github.com/user-attachments/assets/7569a014-242c-4f5d-8e63-89c79085e28f)

### 3.3. How to use '\e' escape sequence in a C# 13 console application 

Here's a simple yet clear example demonstrating how you can use the new \e escape sequence in a C# 13 console application to leverage ANSI escape codes for coloring text in the terminal

```csharp
using System;

class Program
{
    static void Main()
    {
        // Print text in different colors using ANSI escape codes
        Console.WriteLine("\e[31mThis text is red!\e[0m");
        Console.WriteLine("\e[32mThis text is green!\e[0m");
        Console.WriteLine("\e[33mThis text is yellow!\e[0m");
        Console.WriteLine("\e[34mThis text is blue!\e[0m");

        // Bold text
        Console.WriteLine("\e[1mThis text is bold!\e[0m");

        // Background color example
        Console.WriteLine("\e[44;97mWhite text on blue background!\e[0m");
    }
}
```

Explanation of the **ANSI** escape sequences used:

**\e[31m** → **Red** foreground color

**\e[32m** → **Green** foreground color

**\e[33m** → **Yellow** foreground color

**\e[34m** → **Blue** foreground color

**\e[1m** → **Bold** text

**\e[44;97m** → **Blue background (44)** and **white foreground (97)**

**\e[0m** → **Reset** all formatting **to default**

When running the application you see this output

![image](https://github.com/user-attachments/assets/53854cf6-ec36-4a5d-a4dc-1ba8ca05bfd5)

## 4. Method group natural type

In C#, a method group refers to the set of **methods** that share the **same name**, including all overloads. 

Previously, when resolving overloads, the compiler gathered all candidate methods from all scopes and then determined a "natural type" (like a delegate or expression type) from the entire set of candidates.

This could be inefficient or ambiguous, especially with **generic methods** or **constrained methods** that couldn't apply.

### 4.1. What's changed in C# 13?

**Previous behavior**:

The compiler always collected all methods with the matching name, even if many methods weren't suitable (e.g., due to generic arity mismatch or unsatisfied constraints).

**New behavior**:

**C# 13** introduces pruning ('poda' in spanish) during overload resolution. 

At each scope:

The compiler first eliminates (prunes) methods that clearly can't match (wrong generic parameters count, unsatisfied generic constraints, etc.).

If no suitable method is found at the current scope, it moves outward to the next scope.

If multiple candidates are still suitable, the compiler proceeds as usual.

If no candidates apply, the method group won't have a natural type.

This makes overload resolution more accurate, efficient, and aligned with intuitive expectations.

### 4.2. Sample Before (prior to C# 13)

```csharp
using System;

class Demo
{
    static void Run(Action action) => action();
    static void Run(Func<int> func) => Console.WriteLine(func());

    static void Test() => Console.WriteLine("Test method invoked.");

    static int Test<T>() => 42;

    static void Main()
    {
        // Method group "Test" includes:
        // - void Test()
        // - int Test<T>()
        
        Run(Test); 
        // Previously, compiler would gather all methods,
        // creating ambiguity or requiring explicit selection.
    }
}
```


### 4.3. Sample After C# 13

```csharp
using System;

class Demo
{
    static void Run(Action action) => action();
    static void Run(Func<int> func) => Console.WriteLine(func());

    static void Test() => Console.WriteLine("Test method invoked.");

    static int Test<T>() => 42;

    static void Main()
    {
        Run(Test);
        // Now, the generic method Test<T>() with wrong arity
        // is automatically pruned as it can't match Action or Func<int>.
        // The compiler easily selects "void Test()" as Action.
    }
}
```

Other sample:

```csharp
using System;

class Example
{
    static void Call(Action action) => action();

    static void Method() => Console.WriteLine("Non-generic method");

    static void Method<T>() where T : IDisposable
        => Console.WriteLine("Generic method with IDisposable constraint");

    static void Main()
    {
        Call(Method); 
        // Generic "Method<T>()" is pruned due to unsatisfied constraint,
        // compiler directly picks "Method()".
    }
}
```


## 5. Implicit index access



## 6. ref and unsafe in iterators and async methods


## 7. allows ref struct



## 8. ref struct interfaces



## 9. More partial members



## 10. Overload resolution priority



## 11. The field keyword


