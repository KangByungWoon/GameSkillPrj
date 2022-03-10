using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlaneController : MonoBehaviour
{
    [Header("�̵� ���� ���� ����")]
    [SerializeField] Vector2 XpositionRange;
    [SerializeField] Vector2 ZpositionRange;

    [Header("ĳ���� ȸ�� ����")]
    [SerializeField] [Range(0, 10)] private float Horizontal_RotateDegree;
    [SerializeField] [Range(0, 10)] private float Vertical_RotateDegree;

    [Header("ĳ���� �̵� �ӵ�")]
    [SerializeField] [Range(0, 10)] private float Speed;

    private float HorizontalInput;
    private float VerticalInput;
    private float angle = 0;

    void Start()
    {
        InitRocalRotation();
    }

    // ����� ���� 0���� �ʱ�ȭ
    private void InitRocalRotation()
    {
        transform.eulerAngles = Vector3.zero;
    }

    private void Update()
    {
        Character_Move();
        Character_RotatePerFrame();
    }

    private void Character_Move()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Speed *= 2;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            Speed /= 2;
        }

        Vector3 TargetPlusPosition = transform.position + new Vector3(HorizontalInput, 0, VerticalInput);
        if (TargetPlusPosition.x >= XpositionRange.x && TargetPlusPosition.x <= XpositionRange.y)
        {
            transform.position += (new Vector3(HorizontalInput, 0, 0) * Speed);
        }
        if (TargetPlusPosition.z >= ZpositionRange.x && TargetPlusPosition.z <= ZpositionRange.y)
        {
            transform.position += (new Vector3(0, 0, VerticalInput) * Speed);
        }
    }

    // �����Ӹ��� ȸ���� ������ �ִٸ� ȸ���ϱ�
    private void Character_RotatePerFrame()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
        Quaternion.Euler(0, 0, Mathf.Clamp(angle, -30f, 30f)), Time.deltaTime * Horizontal_RotateDegree);
        if (HorizontalInput != 0)
        {
            Debug.Log(-Horizontal_RotateDegree * HorizontalInput);
            angle += -Horizontal_RotateDegree * HorizontalInput;
        }
        else
        {
            angle = 0;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            HorizontalInput = 0;
        }
        else
        {
            HorizontalInput = Input.GetAxis("Horizontal");
        }
        VerticalInput = Input.GetAxis("Vertical");
    }
}