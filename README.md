# C # version 13 New Features Samples

## 1. params collections

In **C# 13**, the **params** modifier has been expanded beyond arrays to include various other collection types. 

Previously, you could only use params with **arrays** (like params int[] numbers).

Now, it also supports types like **Span<T>**, **ReadOnlySpan<T>**, and any collection implementing **IEnumerable<T>** or related interfaces (**ICollection<T>**, **IList<T>**, **IReadOnlyCollection<T>**, etc.), provided they have an Add method.

