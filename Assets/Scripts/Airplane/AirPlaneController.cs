using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlaneController : MonoBehaviour
{
    [Header("ĳ���� ȸ�� ����")]
    [SerializeField] [Range(0, 10)] private float Horizontal_RotateDegree;
    [SerializeField] [Range(0, 10)] private float Vertical_RotateDegree;

    private float HorizontalInput;
    private float VerticalInput;

    void Start()
    {
        InitRocalRotation();
    }

    // ����� ���� 0���� �ʱ�ȭ
    private void InitRocalRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        RotatePerFrame();
        StoppingEvent();
    }

    // �����Ӹ��� ȸ���� ������ �ִٸ� ȸ���ϱ�
    private void RotatePerFrame()
    {
        transform.Rotate(new Vector3(-VerticalInput * Vertical_RotateDegree, 0, -HorizontalInput * Horizontal_RotateDegree));
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
        if (transform.rotation != Quaternion.identity)
        {
            ResetRotation();
            ReturnRotation();
        }
    }

    // ������ 1 �̸��̸� 0���� �ʱ�ȭ���ֱ�
    private void ResetRotation()
    {
        if (Mathf.Abs(transform.rotation.x) < 1)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
        }
        if (Mathf.Abs(transform.localEulerAngles.y) < 1)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        }
        if (Mathf.Abs(transform.localEulerAngles.z) < 1)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0));
        }
    }

    // ������ 1 �̻��̸� ������ �ε巴�� 0���� �����
    private void ReturnRotation()
    {
        if (transform.rotation.x != 0)
        {
            transform.Rotate(new Vector3(transform.rotation.x > 0 ? -0.1f : 0.1f, 0, 0));
        }
        if (transform.rotation.y != 0)
        {
            transform.Rotate(new Vector3(transform.rotation.y > 0 ? -0.1f : 0.1f, 0, 0));
        }
        if (transform.rotation.z != 0)
        {
            transform.Rotate(new Vector3(transform.rotation.z > 0 ? -0.1f : 0.1f, 0, 0));
        }
    }

    void FixedUpdate()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        //transform.position += (new Vector3(h, v, 0));
    }
}
