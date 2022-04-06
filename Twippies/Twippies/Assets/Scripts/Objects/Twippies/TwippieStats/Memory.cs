using System;
using System.Collections.Generic;

public enum MemoryType
{
    People,
    Places
}

public class Memory
{
    public List<Guid> Data { get; set; }
    public MemoryType Type { get; set; }
    public int Capacity { get; private set; }

    public Memory(MemoryType memoryType)
    {
        Type = memoryType;
        Data = new List<Guid>();
    }

    public Memory(MemoryType memoryType, int capacity)
    {
        Type = memoryType;
        Capacity = capacity;
        Data = new List<Guid>();
    }

    public Memory(MemoryType memoryType, List<Guid> data, int capacity)
    {
        Type = memoryType;
        Capacity = capacity;
        Data = data;
    }

    public void AddOrRefresh(Guid entity)
    {
        // On ne peut rien retenir si la capacité de mémorisation est nulle
        if (Capacity == 0)
            return;

        if (Data.Contains(entity))
        {
            Data.Remove(entity);
        }

        Data.Add(entity);
    }

    private void ForgetIfNeeded()
    {
        if (Capacity >= Data.Count)
            return;

        Data.RemoveRange(0, Data.Count - Capacity);
    }

    public void SetCapacity(int capacity)
    {
        if (capacity < Capacity)
        {
            ForgetIfNeeded();
        }

        Capacity = capacity;
    }
}
