using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBullet : MonoBehaviour
{
    public float Speed;
    public int Damage;
    public bool isTarget;
    public Transform target;
    public bool isRaise = false;
    public bool isDie = false;

    [SerializeField] PoolObject m_Object;

    private void OnEnable()
    {
        isDie = false;
    }

    // ����, �Ѿ� �ܰ迡 ���� ������ �̵� ����� �����մϴ�. ������� ������ Ÿ���� ���� ���� �̵����մϴ�.
    void Update()
    {
        if (!isTarget)
        {
            transform.position += Vector3.forward * Time.deltaTime * Speed * 7;

            if (transform.position.z > 200)
            {
                if (isRaise)
                {
                    ObjectPoolMgr.Instance.ReleaseObject(m_Object);
                }
                else
                {
                    ObjectPoolMgr.Instance.ReleaseObject(m_Object);
                }
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * Speed / 20);

            if (target.gameObject.activeSelf == false)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (!isDie)
        {
            isDie = true;
            ObjectPoolMgr.Instance.ReleaseObject(m_Object);

            isTarget = false;
            target = null;
            PoolObject ex = ObjectPoolMgr.Instance.GetObject("Particle", gameObject.transform.position);
            ObjectPoolMgr.Instance.ReleaseObject(ex, 2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "SEnemy" || other.gameObject.tag == "Boss")
        {
            Die();
        }
    }
}
