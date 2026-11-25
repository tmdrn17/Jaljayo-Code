using UnityEngine;

// 매니저 공통 싱글톤. 처음 부를 때 씬에서 찾고, 중복은 파괴하고, 종료 중에는 찾지 않는다.
// 자식은 Awake 대신 OnInitialize()를 override 한다.
public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    private static T instance;
    private static bool isQuitting;

    public static T Instance
    {
        get
        {
            if (isQuitting) return null;

            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    Debug.LogWarning($"[{typeof(T).Name}] 씬에 싱글톤 오브젝트가 없습니다.");
                }
            }
            return instance;
        }
    }

    /// <summary>true면 씬이 바뀌어도 이 오브젝트를 파괴하지 않는다.</summary>
    protected virtual bool IsPersistent => false;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (IsPersistent) DontDestroyOnLoad(gameObject);

        OnInitialize();
    }

    /// <summary>중복이 아닐 때만 Awake에서 한 번 불린다.</summary>
    protected virtual void OnInitialize()
    {
    }

    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
