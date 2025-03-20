

PrintNumbers(1, 2, 3, 4);

Concat(1, 2, 3, 4);

Concat("Hello", "world", "!");

// Using directly supplied items
DisplayItems(10, 20, 30);

// Or passing existing collections
List<string> fruits = ["apple", "banana", "kiwi"];
DisplayItems(fruits);

UseMyCollection(1, 2, 3);


//---------------------------------------------------------------------------------------------------------
//
//
//
//---------------------------------------------------------------------------------------------------------

// Print text in different colors using ANSI escape codes
Console.WriteLine("\e[31mThis text is red!\e[0m");
Console.WriteLine("\e[32mThis text is green!\e[0m");
Console.WriteLine("\e[33mThis text is yellow!\e[0m");
Console.WriteLine("\e[34mThis text is blue!\e[0m");

// Bold text
Console.WriteLine("\e[1mThis text is bold!\e[0m");

// Background color example
Console.WriteLine("\e[44;97mWhite text on blue background!\e[0m");

//---------------------------------------------------------------------------------------------------------
//
//
//
//---------------------------------------------------------------------------------------------------------
void PrintNumbers(params int[] numbers)
{
    foreach (var number in numbers)
    {
        Console.Write($"{number} ");
    }
    Console.WriteLine();
}

void Concat<T>(params ReadOnlySpan<T> items)
{
    for (int i = 0; i < items.Length; i++)
    {
        Console.Write($"{items[i]} ");
    }
    Console.WriteLine();
}

void DisplayItems<T>(params IEnumerable<T> items)
{
    foreach (var item in items)
    {
        Console.Write($"{item} ");
    }
    Console.WriteLine();
}

void UseMyCollection(params MyCollection<int> numbers)
{
    foreach (var num in numbers)
    {
        Console.Write($"{num} ");
    }
    Console.WriteLine();
}

public class MyCollection<T> : IEnumerable<T>
{
    private readonly List<T> _innerList = new();

    public void Add(T item) => _innerList.Add(item);

    public IEnumerator<T> GetEnumerator() => _innerList.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

