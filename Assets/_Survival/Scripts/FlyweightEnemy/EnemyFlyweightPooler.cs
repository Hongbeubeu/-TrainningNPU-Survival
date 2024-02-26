using System.Collections.Generic;

public class EnemyFlyweightPooler<T> where T : new()
{
    private readonly Stack<T> _pools;

    public EnemyFlyweightPooler(int capacity)
    {
        _pools = new Stack<T>(capacity);
    }

    public T GetEnemy()
    {
        return _pools.Count > 0 ? _pools.Pop() : new T();
    }
    
    public void ReturnEnemy(T item)
    {
        _pools.Push(item);
    }
}