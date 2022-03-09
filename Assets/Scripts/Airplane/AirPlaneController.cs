using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlaneController : MonoBehaviour
{
    [Header("�̵� ���� ���� ����")]
    [SerializeField] Vector2 XpositionRange;
    [SerializeField] Vector2 YpositionRange;

    [Header("ĳ���� ȸ�� ����")]
    [SerializeField] [Range(0, 10)] private float Horizontal_RotateDegree;
    [SerializeField] [Range(0, 10)] private float Vertical_RotateDegree;

    [Header("ĳ���� �̵� �ӵ�")]
    [SerializeField] [Range(0, 10)] private float Speed;

    private float HorizontalInput;
    private float VerticalInput;

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
        RotatePerFrame();
        SetRotation();
        //StoppingEvent();
    }

    // �����Ӹ��� ȸ���� ������ �ִٸ� ȸ���ϱ�
    private void RotatePerFrame()
    {
        transform.eulerAngles += new Vector3(-VerticalInput * Vertical_RotateDegree / 2, 0, -HorizontalInput * Horizontal_RotateDegree / 2);
    }

    private void SetRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.eulerAngles += new Vector3(0, 0, 1 * Horizontal_RotateDegree);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.eulerAngles += new Vector3(0, 0, -1 * Horizontal_RotateDegree);
        }
    }

    // �÷��̾ ���������� �� ����Ǵ� �̺�Ʈ
    private void StoppingEvent()
    {
        if (HorizontalInput == 0 && VerticalInput == 0)
        {
            StopConfirm();
        }
    }

    // �÷��̾ �������ִ��� Ȯ���ϴ� �˻�
    private void StopConfirm()
    {
        if (transform.eulerAngles != Vector3.zero)
        {
            ResetRotation();
            ReturnRotation();
        }
    }

    // ������ 1 �̸��̸� 0���� �ʱ�ȭ���ֱ�
    private void ResetRotation()
    {
        if (Mathf.Abs(transform.eulerAngles.x) < 1)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        if (Mathf.Abs(transform.eulerAngles.y) < 1)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        }
        if (Mathf.Abs(transform.eulerAngles.z) < 1)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
    }

    // ������ 1 �̻��̸� ������ �ε巴�� 0���� �����
    private void ReturnRotation()
    {
        if (transform.eulerAngles.x != 0)
        {
            transform.eulerAngles += new Vector3(transform.rotation.x > 0 ? -0.1f : 0.1f, 0, 0);
        }
        if (transform.eulerAngles.y != 0)
        {
            transform.eulerAngles += new Vector3(transform.rotation.y > 0 ? -0.1f : 0.1f, 0, 0);
        }
        if (transform.eulerAngles.z != 0)
        {
            transform.eulerAngles += new Vector3(transform.rotation.z > 0 ? -0.1f : 0.1f, 0, 0);
        }
    }

    void FixedUpdate()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        Vector3 TargetPlusPosition = transform.position + new Vector3(HorizontalInput, VerticalInput, 0);
        if (TargetPlusPosition.x >= XpositionRange.x && TargetPlusPosition.x <= XpositionRange.y &&
            TargetPlusPosition.y >= YpositionRange.x && TargetPlusPosition.y <= YpositionRange.y)
        {
            transform.position += (new Vector3(HorizontalInput, 0, VerticalInput) * Speed);
        }
    }
}
