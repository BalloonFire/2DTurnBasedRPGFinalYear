using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance => instance;

    // Subclasses can override this to prevent persistence
    protected virtual bool ShouldPersist => true;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;

        // Respect ShouldPersist flag!
        if (ShouldPersist && transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
