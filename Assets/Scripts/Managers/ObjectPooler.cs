using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int size;
        [SerializeField] private bool render = true;

        public GameObject Prefab => prefab;
        public int Size => size;
        public bool Render => render;
    }

    public static ObjectPooler Instance { get; private set; }

    [SerializeField] private List<Pool> pools = new List<Pool>();
    [SerializeField] private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {    
        if (Instance == this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject poolSend = Instantiate(pool.Prefab);
                RegisterPooledObject(pool, poolSend);

                objectPool.Enqueue(poolSend);
            }

            poolDictionary.Add(pool.Prefab, objectPool);
        }
    }

    private void RegisterPooledObject(Pool pool, GameObject poolSend)
    {
        if (!pool.Render)
        {
            if (poolSend.transform.childCount > 0)
            {
                Renderer[] renders = poolSend.GetComponentsInChildren<Renderer>();
                foreach (Renderer render in renders) render.enabled = false; 
            }
            else
            {
                Renderer render = poolSend.GetComponent<Renderer>();
                if (render != null) render.enabled = false;
            }
        }

        poolSend.SetActive(false);
        poolSend.transform.SetParent(transform);
    }

    public GameObject Spawn(GameObject prefab, bool newPos, Vector3 position = default, Quaternion rotation = default)
    {
        if (!poolDictionary.ContainsKey(prefab)) return null;

        GameObject spawnedObject = poolDictionary[prefab].Dequeue();
        spawnedObject.SetActive(false);

        if (newPos) spawnedObject.transform.SetPositionAndRotation(position, rotation);
        spawnedObject.SetActive(true);

        poolDictionary[prefab].Enqueue(spawnedObject);

        return spawnedObject;
    }
}

