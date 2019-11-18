using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static object _lock = new object();

    public static T instance
    {
        get
        {
            if (appIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                }
                return _instance;
            }
        }
    }

    private static bool appIsQuitting = false;

    public void OnApplicationQuit()
    {
        appIsQuitting = true;
    }
}


public abstract class DDOLSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static object _lock = new object();

    public static T instance
    {
        get
        {
            if (appIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }


    private static bool appIsQuitting = false;

    public void OnApplicationQuit()
    {
        appIsQuitting = true;
    }
}
