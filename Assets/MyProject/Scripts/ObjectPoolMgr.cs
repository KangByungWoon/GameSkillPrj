using System.Collections.Generic;
using UnityEngine;
using System;

using KeyType = System.String;

public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
{
    // Ǯ���� ������Ʈ�� �����͸� �޾ƿ��� List�Դϴ�.
    [SerializeField]
    private List<PoolObjectData> poolObjectDataList = new List<PoolObjectData>();

    // Ǯ���� ������Ʈ�� ���� ������Ʈ�� Dictionary�Դϴ�
    private Dictionary<KeyType, PoolObject> sampleDict;

    // Ǯ���� ������Ʈ�� ������ Dictionary�Դϴ�.
    private Dictionary<KeyType, PoolObjectData> dataDict;

    // ������ƮǮ���� �����ϴ� Dictionary�Դϴ�
    private Dictionary<KeyType, Stack<PoolObject>> poolDict;

    private Dictionary<KeyType, GameObject> t_poolParent;

    private GameObject sampleTransform_Parent;

    private void Awake()
    {
        Init();
    }

    // ������ƮǮ�� ���� ������Ʈ�� �����͸� ���� �ٸ� Dictionary�����͸� �Ҵ����ݴϴ�.
    private void Init()
    {
        int length = poolObjectDataList.Count;
        if (length == 0) return;

        sampleTransform_Parent = new GameObject();
        sampleTransform_Parent.name = "sampleTransform_Parent";
        sampleTransform_Parent.transform.parent = gameObject.transform;

        sampleDict = new Dictionary<KeyType, PoolObject>(length);
        dataDict = new Dictionary<KeyType, PoolObjectData>(length);
        poolDict = new Dictionary<KeyType, Stack<PoolObject>>(length);
        t_poolParent = new Dictionary<KeyType, GameObject>(length);

        foreach (var data in poolObjectDataList)
        {
            Register(data);
        }
    }

    // ������ ������Ʈ Ǯ�� �����ϴ� Dictionary���� ���ڷ� ���� �������� Ű ���� �ִ��� ã���ϴ�.
    // Dictionary�� ���ڷ� ���� Ű ���� ������ �������� ���� ������ ���ÿ�����Ʈ�� ����ϴ�.
    // ������Ʈ���� ������ Stack�� ����� Ŭ���� ����Ͽ� ������Ʈ���� ���� �� �Ҵ����ݴϴ�.
    private void Register(PoolObjectData data)
    {
        if (poolDict.ContainsKey(data.key))
        {
            return;
        }

        GameObject sample = Instantiate(data.prefab, sampleTransform_Parent.transform);
        if (!sample.TryGetComponent(out PoolObject po))
        {
            po = sample.AddComponent<PoolObject>();
            po.key = data.key;
        }
        sample.SetActive(false);

        Stack<PoolObject> pool = new Stack<PoolObject>(data.MaxCreateCount);

        GameObject parent = new GameObject();
        parent.name = data.key;
        parent.transform.parent = gameObject.transform;

        for (int i = 0; i < data.InitCreateCount; i++)
        {
            PoolObject clone = po.Clone();
            clone.transform.parent = parent.transform;
            pool.Push(clone);
        }

        sampleDict.Add(data.key, po);
        dataDict.Add(data.key, data);
        poolDict.Add(data.key, pool);
        t_poolParent.Add(data.key, parent);
    }

    // ������Ʈ Ǯ���� �����ϴ� Dictionary�� ���ڷ� ���� Ű ���� �˻��մϴ�.
    // ������ null�� ��ȯ�ϰ� �ִٸ� ������Ʈ�� Ȱ��ȭ �ϰ� ��ȯ�մϴ�.
    // ���� ������Ʈ Ǯ �ȿ� ������Ʈ�� ���ٸ� ������ ��ȯ���ݴϴ�.
    public PoolObject GetObject(KeyType key)
    {
        if (!poolDict.TryGetValue(key, out var pool))
        {
            return null;
        }

        PoolObject po;

        if (pool.Count > 0)
        {
            po = pool.Pop();
        }
        else
        {
            po = sampleDict[key].Clone();
        }

        po.Activate();

        return po;
    }

    // ������Ʈ Ǯ���� �����ϴ� Dictionary�� ���ڷ� ���� Ű ���� �˻��մϴ�.
    // ������ �Լ��� ������ �ִٸ� ã�� Dictionary�� ���ڷ� ���� Ǯ ������Ʈ�� �Ҵ����ְ� ��Ȱ��ȭ�մϴ�.
    // ���� Dictionary�� Count�� ���߾��� �ִ� ���� ���� �� ���� ũ�ٸ� Destroy�� �������ݴϴ�.
    public void ReleaseObject(PoolObject po)
    {
        if (!poolDict.TryGetValue(po.key, out var pool))
        {
            return;
        }

        KeyType key = po.key;

        if (pool.Count < dataDict[key].MaxCreateCount)
        {
            pool.Push(po);
            po.Deactivate();
        }
        else
        {
            Destroy(po.gameObject);
        }
    }
}
