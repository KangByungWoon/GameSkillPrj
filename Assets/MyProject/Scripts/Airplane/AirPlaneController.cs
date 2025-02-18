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

    // 변수들에 미리 할당해야하는 값을 할당 해줍니다.
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

    // 플레이어가 목표 각도로 부드럽게 회전합니다.
    private void transformRotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
        Quaternion.Euler(xAngel, yAngel, zAngle), Time.deltaTime * MoveSpeed);
    }

    // 플레이어가 목표 지점으로 부드럽게 이동합니다.
    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPoint, Time.deltaTime * MoveSpeed);
    }

    // 좌우 방향키를 눌렀을 때 받는 이벤트입니다. x축을 증감시키고 각도를 지정합니다.
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

    // 상하 방향키를 눌렀을 때 받는 이벤트입니다. y축을 증감시키고 각도를 지정합니다.
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

    // 코루틴으로 총알을 발사합니다. 총알의 단계에 따라 지정해주고 유도탄이라면 레이케스트를 발사하여 오브젝트를 판별합니다.
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

    // 미사일 락온 시스템입니다. 특정 사거리 안에 박스 레이케스트를 발사하고 Enemy Layer을 가지고 있는 오브젝트가 판별된다면 지정하고 발사합니다.
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

    // 락온 없이 발사되는 미사일입니다. 할당되고 앞으로 나아갑니다.
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

    // 무적 함수입니다. 무적 이펙트 코루틴을 실행시킵니다.
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

    // 무적 이펙트 코루틴 입니다. 메테리얼로 이펙트를 구현했습니다.
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
