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



## 3. New escape sequence


## 4. Method group natural type


## 5. Implicit index access



## 6. ref and unsafe in iterators and async methods


## 7. allows ref struct



## 8. ref struct interfaces



## 9. More partial members



## 10. Overload resolution priority



## 11. The field keyword


