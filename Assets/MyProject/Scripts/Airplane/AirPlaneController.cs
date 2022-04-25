using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirPlaneController : MonoBehaviour
{
    #region PlayerStat
    [SerializeField] float Horizontal_RotPower;
    [SerializeField] float Vertical_RotPower;
    [HideInInspector] public float MoveSpeed;

    float BulletAttackSpeed = 0.5f;
    float BulletMoveSpeed;
    int BulletDamage;
    [SerializeField] int AttackPlace;
    #endregion

    #region PlayerInfo
    public int WeaponLevel = 1;

    private float HorizontalInput = 0;
    private float VerticalInput = 0;

    private Vector3 StartPosition;
    private Vector3 TargetPoint;
    [HideInInspector] public bool isInvin;

    public bool NOSHOTTING = false;

    public int Level;
    public int _Level
    {
        get { return Level; }
        set
        {
            if (Level != MaxLevel)
            {
                Level++;
                BulletDamage += 1;
                BulletMoveSpeed += 1;
                BulletAttackSpeed -= 0.015f;

                if (Level == 3)
                {
                    Mini1.gameObject.SetActive(true);
                    Mini1.StartFire();
                }
                else if (Level == 6)
                {
                    Mini2.gameObject.SetActive(true);
                    Mini2.StartFire();
                }
                else if (Level == 10)
                {
                    Mini1.WeaponLevel = 2;
                    Mini2.WeaponLevel = 2;
                }

                LvText.text = "LV." + Level.ToString();
            }
            else
            {
                LvText.text = "LV.MAX";
            }

        }
    }
    public int MaxLevel;
    public float Exp;
    public float _Exp
    {
        get { return Exp; }
        set
        {
            if (value >= MaxExp)
            {
                Exp = 0;
                MaxExp += 100;
                _Level++;
                GameManager.Instance.GetItemTxtOutput("", true);
                Destroy(Instantiate(LevelUpEffect, gameObject.transform), 2f);
            }
            else if (value < MaxExp && Level < MaxLevel)
            {
                Exp = value;
            }
            LvBar.fillAmount = Exp / MaxExp;
            ExpText.text = Mathf.Round((Exp / MaxExp) * 100).ToString() + "%";
        }
    }
    public float MaxExp;
    #endregion

    #region AngleValue
    public float xAngel = 0;
    public float yAngel = 0;
    public float zAngle = 0;
    #endregion

    #region PosValue
    private float xPos = 0;

    private float _yPos = 0;
    private float yPos
    {
        get { return _yPos; }
        set
        {
            if (value < -10)
            {
                _yPos = -10;
                xAngel = 0;
            }
            else if (value > 10)
            {
                _yPos = 10;
                xAngel = 0;
            }
            else
                _yPos = value;
        }
    }
    #endregion

    #region DefComponent
    [SerializeField] Mini Mini1;
    [SerializeField] Mini Mini2;

    [SerializeField] RectTransform LockOn;
    [SerializeField] MeshRenderer invinmat;

    [SerializeField] Text LvText;
    [SerializeField] Text ExpText;
    [SerializeField] Image LvBar;
    #endregion

    #region EffectPrefabs
    public GameObject LevelUpEffect;
    public GameObject ShiledEffect;
    public GameObject HpUpEffect;
    public GameObject PPDownEffect;
    public GameObject WeaponUpEffect;
    public GameObject InvinEffect;
    #endregion

    #region Coroutine
    private IEnumerator InvinCorou;
    private IEnumerator AttackCorou;
    #endregion

    void Start()
    {
        Setting();
    }

    // �����鿡 �̸� �Ҵ��ؾ��ϴ� ���� �Ҵ� ���ݴϴ�.
    private void Setting()
    {
        MaterialSetting();
        InfoSetting();
        CoroutineSetting();
    }

    private void MaterialSetting()
    {
        invinmat.material.mainTextureScale = new Vector2(0, 1);
    }

    private void InfoSetting()
    {
        StartPosition = transform.position;
        TargetPoint = transform.position;

        JsonSystem json = JsonSystem.Instance;

        MoveSpeed = json.Information.PlayerMoveSpeed;
        BulletAttackSpeed = json.Information.PlayerBulletAttackSpeed;
        BulletMoveSpeed = json.Information.PlayerBulletMoveSpeed;
        BulletDamage = json.Information.PlayerDamage;
    }

    private void CoroutineSetting()
    {
        AttackCorou = FireBullet(false);
        StartCoroutine(AttackCorou);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WeaponUpgrade();
        }

        transformRotate();

        Move();

        HorizontalEvent();

        VerticalEvent();

        if (WeaponLevel == 5)
            LockOnSystem();
    }

    // �÷��̾ ��ǥ ������ �ε巴�� ȸ���մϴ�.
    private void transformRotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
        Quaternion.Euler(xAngel, yAngel, zAngle), Time.deltaTime * MoveSpeed);
    }

    // �÷��̾ ��ǥ �������� �ε巴�� �̵��մϴ�.
    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPoint, Time.deltaTime * MoveSpeed);
    }

    // �¿� ����Ű�� ������ �� �޴� �̺�Ʈ�Դϴ�. x���� ������Ű�� ������ �����մϴ�.
    private void HorizontalEvent()
    {
        if (HorizontalInput == 0)
        {
            zAngle = 0;
            yAngel = 0;
            return;
        }

        yAngel = HorizontalInput * Horizontal_RotPower;
        zAngle = -HorizontalInput * Horizontal_RotPower;
        xPos += HorizontalInput * Time.deltaTime * 30;
        TargetPoint = StartPosition + new Vector3(xPos, yPos, 0);
    }

    // ���� ����Ű�� ������ �� �޴� �̺�Ʈ�Դϴ�. y���� ������Ű�� ������ �����մϴ�.
    private void VerticalEvent()
    {
        if (VerticalInput == 0)
        {
            xAngel = 0;
            return;
        }

        xAngel = -VerticalInput * Vertical_RotPower;
        yPos += VerticalInput * 0.2f;
        TargetPoint = StartPosition + new Vector3(xPos, yPos, 0);
    }

    // �ڷ�ƾ���� �Ѿ��� �߻��մϴ�. �Ѿ��� �ܰ迡 ���� �������ְ� ����ź�̶�� �����ɽ�Ʈ�� �߻��Ͽ� ������Ʈ�� �Ǻ��մϴ�.
    IEnumerator FireBullet(bool isTarget, bool Raise = false)
    {
        while (true)
        {
            yield return new WaitForSeconds(BulletAttackSpeed);

            if (NOSHOTTING)
                continue;

            PBullet bullet = BulletModuleSetting(Raise);

            if (isTarget)
            {
                EnemyCheckBoxCast(Raise, bullet);
            }
            else
            {
                bullet.isTarget = false;
            }
        }
    }

    private PBullet BulletModuleSetting(bool Raise)
    {
        PBullet bullet;
        if (Raise)
        {
            bullet = ObjectPoolMgr.Instance.GetObject("PRaise", transform.position + new Vector3(0, 0.1f, 1)).GetComponent<PBullet>();
        }
        else
        {
            bullet = ObjectPoolMgr.Instance.GetObject("PBullet", transform.position + new Vector3(0, 0.1f, 1)).GetComponent<PBullet>();
            bullet.Speed = BulletMoveSpeed;
            bullet.Damage = BulletDamage;
        }

        return bullet;
    }

    private void EnemyCheckBoxCast(bool Raise, PBullet bullet)
    {
        var hitObjs = Physics.BoxCastAll(transform.position + new Vector3(0, 0.5f, 0), new Vector3(AttackPlace, AttackPlace, AttackPlace), transform.forward, transform.rotation, Mathf.Infinity, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (var hit in hitObjs)
        {
            if (hit.transform.gameObject.GetComponent<PoolObject>().key == "RedBlood_Cells")
            {
                continue;
            }

            if (Raise)
            {
                bullet.Speed = BulletMoveSpeed + 5;
                bullet.Damage = BulletDamage + 5;
                bullet.isRaise = true;
            }
            bullet.target = hit.transform.gameObject.transform;
            bullet.isTarget = true;
        }
    }

    // �̻��� ���� �ý����Դϴ�. Ư�� ��Ÿ� �ȿ� �ڽ� �����ɽ�Ʈ�� �߻��ϰ� Enemy Layer�� ������ �ִ� ������Ʈ�� �Ǻ��ȴٸ� �����ϰ� �߻��մϴ�.
    private void LockOnSystem()
    {
        if (NOSHOTTING)
            return;

        LockOnCheckBoxCast();
    }

    private void LockOnCheckBoxCast()
    {
        var hitObjs = Physics.BoxCastAll(transform.position + new Vector3(0, 0.5f, 0), new Vector3(AttackPlace, AttackPlace, AttackPlace), transform.forward, transform.rotation, Mathf.Infinity, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (var hit in hitObjs)
        {
            if (!hit.transform.gameObject.GetComponent<Enemy>().isTarget && hit.transform.gameObject.GetComponent<PoolObject>().key != "RedBlood_Cells")
            {
                Rocket rocket = ObjectPoolMgr.Instance.GetObject("PRocket", transform.position + new Vector3(Random.Range(-10, 10), 5, -10)).GetComponent<Rocket>();
                rocket.Target = hit.transform.gameObject.transform;
                rocket.NoTarget = false;

                Enemy enemy = hit.transform.gameObject.GetComponent<Enemy>();
                enemy.isTarget = true;
                enemy.RocketObj = rocket.gameObject;
                enemy.OnMark();

                enemy.TargetSetting();
            }
        }
    }

    // ���� ���� �߻�Ǵ� �̻����Դϴ�. �Ҵ�ǰ� ������ ���ư��ϴ�.
    private IEnumerator RocketFire()
    {
        while (true)
        {
            yield return new WaitForSeconds(BulletAttackSpeed * 2);

            if (NOSHOTTING)
                continue;

            RocketSpawnAndSetting();
        }
    }

    private void RocketSpawnAndSetting()
    {
        Rocket rocket = ObjectPoolMgr.Instance.GetObject("PRocket", transform.position + new Vector3(0, 0.1f, 2f)).GetComponent<Rocket>();
        rocket.NoTarget = true;
        rocket.Damage = BulletDamage * 10;
    }

    // ���� �Լ��Դϴ�. ���� ����Ʈ �ڷ�ƾ�� �����ŵ�ϴ�.
    public void InvinActive(int damage, float waitTime = 1f, bool isItem = false)
    {
        if (isInvin == false || isItem == true)
        {
            GameManager.Instance.Hp -= damage;
            isInvin = true;
            if (InvinCorou != null)
            {
                StopCoroutine(InvinCorou);
            }

            InvinCorou = InvinCoroutine(waitTime);
            StartCoroutine(InvinCorou);
        }
    }

    // ���� ����Ʈ �ڷ�ƾ �Դϴ�. ���׸���� ����Ʈ�� �����߽��ϴ�.
    private IEnumerator InvinCoroutine(float waitTime)
    {
        invinmat.material.mainTextureScale = new Vector2(20, 1);

        for (int i = 0; i < 100; i++)
        {
            invinmat.material.mainTextureScale -= new Vector2(0.2f, 0);
            yield return new WaitForSeconds(waitTime / 100);
        }
        yield return new WaitForSeconds(0.5f);
        isInvin = false;
    }

    public void WeaponUpgrade()
    {
        if (WeaponLevel < 5)
        {
            WeaponLevel++;
            switch (WeaponLevel)
            {
                case 2:
                    StopCoroutine(AttackCorou);
                    AttackCorou = FireBullet(true);
                    StartCoroutine(AttackCorou);
                    break;
                case 3:
                    AttackPlace += 3;
                    LockOn.localScale += new Vector3(3, 3, 0);
                    break;
                case 4:
                    StopCoroutine(AttackCorou);
                    AttackCorou = FireBullet(true, true);
                    StartCoroutine(AttackCorou);
                    break;
                case 5:
                    if (AttackCorou != null)
                        StopCoroutine(AttackCorou);
                    StartCoroutine(RocketFire());
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        GetAxis();
    }

    private void GetAxis()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
    }
}
