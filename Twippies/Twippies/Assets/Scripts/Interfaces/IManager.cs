using System;
using System.Collections.Generic;

public interface IManager<T>
{
    /// <summary>
    /// Adds object to manager collection
    /// </summary>
    /// <param name="object"></param>
    void Add(T @object);

    /// <summary>
    /// Find object by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    T FindById(Guid id);

    /// <summary>
    /// Find object by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IList<T> FindAll();

    /// <summary>
    /// Tries to find object by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="object"></param>
    /// <returns></returns>
    bool TryGetById(Guid id, out T @object);
}
