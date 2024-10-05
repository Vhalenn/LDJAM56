using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError($"Tried to get Singleton {nameof(T)} but it was NULL");
                // _instance = new T(); -> Not possible for MonoBehaviour
            }

            return _instance;
        }

        set
        {
            if (_instance != null)
            {
                Debug.Log($"{typeof(T)} already have an instanced called {_instance}");
                //throw new NotImplementedException();
            }

            _instance = value;
        }
    }

    public virtual void OnDestroy()
    {
        //_instance = null;
    }
}
