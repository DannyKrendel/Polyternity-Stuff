using UnityEngine;

namespace PolyternityStuff
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        protected bool IsDestroying;
    
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                IsDestroying = true;
                return;
            }

            _instance = GetComponent<T>();
        
            DontDestroyOnLoad(gameObject);
        }
    }
}