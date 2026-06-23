using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private Stack<GameObject> stack = new Stack<GameObject>();
    private int maxCount;
    public int Count => stack.Count;
    public GameObjectPool(int maxCount)
    {
        this.maxCount = maxCount;
    }
    public GameObject Pop()
    {
        return stack.Count > 0 ? stack.Pop() : null;
    }
    public void Push(GameObject obj)
    {
        if (stack.Count < maxCount)
            stack.Push(obj);
        else
            Object.Destroy(obj);
    }
}

public class PoolManager : SingleBase<PoolManager>
{
    private PoolManager() { }

    private Dictionary<string, GameObjectPool> pool = new Dictionary<string, GameObjectPool>();

    public GameObject PopGameObject(string resourcePath, string name, int maxCount)
    {
        if (!pool.ContainsKey(name))
            pool[name] = new GameObjectPool(maxCount);

        GameObject gameObject = pool[name].Pop();
        if (gameObject == null)
        {
            gameObject = GameObject.Instantiate(Resources.Load<GameObject>(resourcePath));
            gameObject.name = name;
        }
        gameObject.SetActive(true);
        return gameObject;
    }

    public void PushGameObject(GameObject gameObject)
    {
        if (pool.ContainsKey(gameObject.name))
        {
            gameObject.SetActive(false);
            pool[gameObject.name].Push(gameObject);
        }
    }
}
