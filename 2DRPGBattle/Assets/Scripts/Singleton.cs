using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

<<<<<<< HEAD
    protected virtual void Awake()
=======
    protected virtual void Awake ()
>>>>>>> parent of cee922b3 (updated)
    {
        if (instance != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
<<<<<<< HEAD
        }
        else
=======
        } else
>>>>>>> parent of cee922b3 (updated)
        {
            instance = (T)this;
        }

        if (!gameObject.transform.parent)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
