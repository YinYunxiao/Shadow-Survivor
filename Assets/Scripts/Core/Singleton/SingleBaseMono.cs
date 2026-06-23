using UnityEngine;

public class SingleBaseMono<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    protected static readonly object instanceLock = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    instance = GameObject.Find(typeof(T).Name)?.GetComponent<T>();
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
