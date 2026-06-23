using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 子类需要有无参构造方法
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleBase<T> where T : class
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
                    // 利用反射得到私有构造函数
                    ConstructorInfo constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    if (constructor != null)
                        instance = constructor.Invoke(null) as T;
                    else
                        Debug.LogError("单例类" + typeof(T).Name + "没有无参构造函数");

                    //// 利用Activator绕过私有构造函数创建实例
                    //instance = Activator.CreateInstance(typeof(T)) as T;
                }
            }
            return instance;
        }
    }
}
