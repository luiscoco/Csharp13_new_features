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

In previous versions of C#, when **initializing** collections or arrays within an **object initializer**, you had to specify indexes explicitly from the front.

With **C# 13**, you can now use the implicit "from-the-end" index operator (**^**) directly within an object initializer expression.

This makes code simpler and clearer, especially when you want to initialize arrays from the end, making indexing more intuitive and readable.

### 5.1. Previous C# 13 Sample

Previously, if you wanted to initialize an array's elements from the end, you had to manually calculate indices from the front.

```csharp
public class TimerRemaining
{
    public int[] buffer { get; set; } = new int[10];
}

// Pre C# 13 way:
var countdown = new TimerRemaining()
{
    buffer =
    {
        [9] = 0,
        [8] = 1,
        [7] = 2,
        [6] = 3,
        [5] = 4,
        [4] = 5,
        [3] = 6,
        [2] = 7,
        [1] = 8,
        [0] = 9
    }
};
```

### 5.2. New C# 13 way (with implicit indexing using ^)

In **C# 13**, you can directly specify indices from the end using the **^** operator.

```csharp
public class TimerRemaining
{
    public int[] buffer { get; set; } = new int[10];
}

// C# 13 way:
var countdown = new TimerRemaining()
{
    buffer =
    {
        [^1] = 0,  // last element
        [^2] = 1,
        [^3] = 2,
        [^4] = 3,
        [^5] = 4,
        [^6] = 5,
        [^7] = 6,
        [^8] = 7,
        [^9] = 8,
        [^10] = 9  // first element
    }
};
```

How the indexing with **^** works:

**[^1]** → last element (**buffer[9]**)

**[^2]** → second-to-last element (**buffer[8]**)

...

**[^10]** → first element (**buffer[0]**)

This naturally matches a **countdown** from **9 to 0**.

Another sample:

Syntax	Equivalent Index from start	Explanation	Value assigned

**[1]** → **buffer[1]** → 2nd element from star 0

**[2]** → **buffer[2]** → 3rd element from start 1

**[^3]** → **buffer[7]** → 3rd element from end 2

**[4]**	**buffer[4]** → 5th element from start 3

**[5]**	**buffer[5]** → 6th element from start 4

**[^6]** **buffer[4]** → 6th element from end 5 (overwrites previous 3)

**[^7]** → **buffer[3]** → 7th element from end 6

**[8]**	→ **buffer[8]** → 9th element from start 7

**[^9]** → **buffer[1]** → 9th element from end 8 (overwrites previous 0)

**[^10]** →	**buffer[0]** → 10th element (first) 9

**Important**: Notice that some positions are assigned twice:

**[4] = 3** and later **[^6] = 5** both refer to **buffer[4]**. The final value is 5.

**[1] = 0** and later **[^9] = 8** both refer to **buffer[1]**. The final value is 8.Syntax	Equivalent Index from start	Explanation	Value assigned

## 6. ref and unsafe in iterators and async methods

**Before C# 13**, the compiler **restricted** certain **scenarios** involving the following cases, in both **async methods** (async/await) and **iterator methods** (yield return).

a) ref locals

b) ref struct types (like Span<T> or ReadOnlySpan<T>)

c) unsafe contexts

This was due to complexity in managing lifetime and safety around asynchronous boundaries and iterator state machines.

### What has changed in C# 13?

**C# 13** relaxes these restrictions, **allowing**:

**Async methods** and **iterators** to declare **local ref** variables.

**Async methods** and **iterators** to declare locals of a ref struct type (such as **Span<T>** or **ReadOnlySpan<T>**).

**unsafe contexts** to be used within **iterators** (but still restricted near yield statements).

However, there's a key safety rule:

You **cannot** use ref locals or ref struct types **across** an **await boundary** (await expression) or **yield return boundary**.

The compiler strictly enforces these rules, ensuring safety and correctness.

### 6.1. Async method using ReadOnlySpan<T>

```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task<int> SumAsync(int[] data)
    {
        // Ref struct type: ReadOnlySpan<int>
        ReadOnlySpan<int> span = data.AsSpan();

        int sum = 0;

        // Allowed: use span locally before await
        foreach (int number in span)
        {
            sum += number;
        }

        await Task.Delay(100); // await boundary

        // ❌ span can't be accessed here, after await boundary.
        // foreach (int number in span) // Compiler Error!
        // {
        //     sum += number;
        // }

        return sum;
    }

    static async Task Main()
    {
        int result = await SumAsync(new[] { 1, 2, 3, 4 });
        Console.WriteLine($"Sum: {result}");
    }
}
```

### 6.2. Iterator method (yield return) using Span<T>

```csharp
using System;

class Example
{
    public static IEnumerable<int> GetFirstThree(int[] numbers)
    {
        Span<int> span = numbers.AsSpan();

        // Allowed to use span locally here:
        for (int i = 0; i < 3 && i < span.Length; i++)
        {
            int current = span[i]; // local ref struct usage allowed here
            yield return current;  // yield boundary
            
            // ❌ span can't be used across yield boundaries.
            // e.g., span[0] after yield return is not allowed.
        }
    }

    static void Main()
    {
        foreach (int value in GetFirstThree(new[] { 10, 20, 30, 40 }))
        {
            Console.WriteLine(value);
        }
    }
}
```

### 6.3. unsafe context in an iterator

```csharp
using System;
using System.Collections.Generic;

class UnsafeIteratorExample
{
    public static IEnumerable<int> UnsafeIterator(int[] numbers)
    {
        // unsafe context allowed:
        unsafe
        {
            fixed (int* ptr = numbers)
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    int value = *(ptr + i); // unsafe pointer access allowed here
                    yield return value;     // yield must be in safe context
                }
            }
        }
    }

    static void Main()
    {
        foreach (int num in UnsafeIterator(new[] { 100, 200, 300 }))
        {
            Console.WriteLine(num);
        }
    }
}
```

## 7. allows ref struct

**Before C# 13**, you couldn't use **ref struct** types (like **Span<T>**, **ReadOnlySpan<T>**) as type arguments for generic methods or classes.

This was due to **safety constraints** and limitations imposed by the compiler.

Now, **C# 13** introduces a special constraint called allows **ref struct** that explicitly permits generic type parameters to accept ref struct types.

This new constraint explicitly indicates:

a) You can now pass a ref struct (such as Span<T>) as a generic type parameter.

b) The compiler enforces ref safety rules strictly.

c) Enables high-performance, zero-allocation code patterns with generic code.

### 7.1. Basic Sample

```csharp
using System;

public class Container<T> where T : allows ref struct
{
    public void Display(scoped T item)
    {
        // Scoped ensures "item" respects ref-safety rules
        Console.WriteLine($"Type of T: {typeof(T)}");
        Console.WriteLine(item.ToString());
    }
}

class Program
{
    static void Main()
    {
        // Example using ReadOnlySpan<char> (ref struct type)
        ReadOnlySpan<char> textSpan = "Hello, C#13!";
        
        var spanContainer = new Container<ReadOnlySpan<char>>();
        spanContainer.Display(textSpan);

        // Also allowed with normal structs
        int number = 42;
        var intContainer = new Container<int>();
        intContainer.Display(number);
    }
}
```

**Why do you need the scoped keyword?**

When using ref struct parameters, C# enforces strict lifetime rules.

The keyword scoped explicitly tells the compiler that the parameter (item) won't escape the method scope.

This ensures safe usage of ref struct types in generic scenarios.

### 7.2. Generic method with allows ref struct

```csharp
using System;

class GenericExample
{
    public static void PrintFirstElement<T>(scoped T span) 
        where T : allows ref struct, IReadOnlySpan<int>
    {
        if (span.Length > 0)
        {
            Console.WriteLine($"First element: {span[0]}");
        }
    }

    static void Main()
    {
        ReadOnlySpan<int> numbers = stackalloc int[] { 10, 20, 30 };
        PrintFirstElement(numbers);
    }
}
```

Method accepts any **ref struct type** that implements **IReadOnlySpan<int>** (e.g., **ReadOnlySpan<int>**).

**allows ref struct** explicitly permits this usage.

## 8. ref struct interfaces

**Before C# 13**, **ref struct types** (like Span<T> or your own stack-only structures) **couldn't implement interfaces**. 

**C# 13** removes this limitation, **allowing ref struct types to implement interfaces**.

However, there are strict rules to maintain ref safety:

a) A ref struct can implement interfaces explicitly.

b) A ref struct type cannot be directly cast or converted to an interface type, as that would require boxing, violating ref safety rules.

c) Interface methods implemented by a ref struct type can only be accessed via a generic type parameter constrained with allows ref struct.

### 8.1. Key rules to remember

This is allowed:

```csharp
public ref struct MyRefStruct : IMyInterface
```

This is not allowed:

```csharp
IMyInterface instance = myRefStruct;  // ❌ Error!
```

You can only access **interface methods** through a generic constraint with **allows ref struct**:

```
void Method<T>(scoped T item) where T : allows ref struct, IMyInterface
```

### 8.2. Example demonstrating ref struct interfaces (C# 13)

**Step 1: Define an Interface**

```csharp
public interface IPrintable
{
    void Print();
}
```

**Step 2: Implement Interface with ref struct**

```csharp
public ref struct MySpanPrinter
{
    private ReadOnlySpan<char> _text;

    public MySpanPrinter(ReadOnlySpan<char> text)
    {
        _text = text;
    }

    // Explicit interface implementation
    void IPrintable.Print()
    {
        Console.WriteLine(_text.ToString());
    }
}
```

Correct Way to Access Interface methods from ref struct

```csharp
using System;

class Program
{
    static void PrintGeneric<T>(scoped T item) 
        where T : allows ref struct, IPrintable
    {
        item.Print(); // Safe: calls explicitly implemented interface method
    }

    static void Main()
    {
        MySpanPrinter printer = new MySpanPrinter("Hello from ref struct!");
        
        PrintGeneric(printer); //Valid and safe call
    }
}
```


## 9. More partial members



## 10. Overload resolution priority



## 11. The field keyword


