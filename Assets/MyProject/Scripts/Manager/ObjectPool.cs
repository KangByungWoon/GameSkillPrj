using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public enum PoolType
    {
        Bacteria,
        Germ,
        Virus,
        Cancer_Cells,
        Leukocyte,
        RedBlood_Cells,
        PRocket,
        Particle
    };

    [Header("Pools")]
    public Queue<GameObject> TestQueue = new Queue<GameObject>();
    public Queue<GameObject> Bacterias = new Queue<GameObject>();
    public Queue<GameObject> Germs = new Queue<GameObject>();
    public Queue<GameObject> Viruses = new Queue<GameObject>();
    public Queue<GameObject> Cancer_Cellses = new Queue<GameObject>();
    public Queue<GameObject> Leukocytes = new Queue<GameObject>();
    public Queue<GameObject> RedBlood_Cellses = new Queue<GameObject>();

    public Queue<GameObject> BacteriaRockets = new Queue<GameObject>();
    public Queue<GameObject> GermRockets = new Queue<GameObject>();
    public Queue<GameObject> VirusRockets = new Queue<GameObject>();
    public Queue<GameObject> Cancer_CellsRockets = new Queue<GameObject>();
    public Queue<GameObject> PRockets = new Queue<GameObject>();

    public Queue<GameObject> Particles = new Queue<GameObject>();

    [Header("Prefabs")]
    public GameObject TestObj;
    [SerializeField] private GameObject Bacteria;
    [SerializeField] private GameObject Germ;
    [SerializeField] private GameObject Virus;
    [SerializeField] private GameObject Cancer_Cells;
    [SerializeField] private GameObject Leukocyte;
    [SerializeField] private GameObject RedBlood_Cells;

    [SerializeField] private GameObject BacteriaRocket;
    [SerializeField] private GameObject GermRocket;
    [SerializeField] private GameObject VirusRocket;
    [SerializeField] private GameObject Cancer_CellsRocket;
    [SerializeField] private GameObject PRocket;

    [SerializeField] private GameObject Particle;

    [Header("PoolParent")]
    public Transform TestPool;
    [SerializeField] private Transform BacteriaPool;
    [SerializeField] private Transform GermPool;
    [SerializeField] private Transform VirusPool;
    [SerializeField] private Transform Cancer_CellsPool;
    [SerializeField] private Transform LeukocytePool;
    [SerializeField] private Transform RedBlood_CellsPool;

    [SerializeField] private Transform BacteriaRocketPool;
    [SerializeField] private Transform GermRocketPool;
    [SerializeField] private Transform VirusRocketPool;
    [SerializeField] private Transform Cancer_CellsRocketPool;
    [SerializeField] private Transform PRocketPool;

    [SerializeField] private Transform ParticlePool;

    private void Awake()
    {
        Instance = this;
        Setting();
    }

    private void Setting()
    {
        AddObject(Bacteria, Bacterias, BacteriaPool);
        AddObject(Germ, Germs, GermPool);
        AddObject(Virus, Viruses, VirusPool);
        AddObject(Cancer_Cells, Cancer_Cellses, Cancer_CellsPool);
        AddObject(Leukocyte, Leukocytes, LeukocytePool, 25);
        AddObject(RedBlood_Cells, RedBlood_Cellses, RedBlood_CellsPool, 25);
        AddObject(BacteriaRocket, BacteriaRockets, BacteriaRocketPool);
        AddObject(GermRocket, GermRockets, GermRocketPool);
        AddObject(VirusRocket, VirusRockets, VirusRocketPool);
        AddObject(Cancer_CellsRocket, Cancer_CellsRockets, Cancer_CellsRocketPool);
        AddObject(PRocket, PRockets, PRocketPool, 1000);
        AddObject(Particle, Particles, ParticlePool, 200);
    }

    private void AddObject(GameObject obj, Queue<GameObject> objectPool, Transform pool_parent, int addValue = 100)
    {
        for (int i = 0; i < addValue; i++)
        {
            GameObject tempObj = Instantiate(obj, pool_parent);
            objectPool.Enqueue(tempObj);
            tempObj.SetActive(false);
        }
    }

    public GameObject GetObject(Queue<GameObject> objectPool, Vector3 pos)
    {
        GameObject obj = objectPool.Dequeue();
        obj.transform.position = pos;
        obj.SetActive(true);
        return obj;
    }


    public void ReleaseObject(Queue<GameObject> objectPool, GameObject m_go, float waitsecond = 0)
    {
        StartCoroutine(ReleaseObject_Coroutine(objectPool, m_go, waitsecond));
    }
    public IEnumerator ReleaseObject_Coroutine(Queue<GameObject> objectPool, GameObject m_go, float waitsecond)
    {
        yield return new WaitForSeconds(waitsecond);

        objectPool.Enqueue(m_go);
        m_go.SetActive(false);
    }

}