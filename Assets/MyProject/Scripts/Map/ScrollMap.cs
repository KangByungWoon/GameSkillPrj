using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMap : MonoBehaviour
{
    [Header("ScrollMap Object")]
    [SerializeField] private GameObject[] ScrollMapObj = new GameObject[2];

    [Header("Move Information")]
    [SerializeField] private float Map_MoveSpeed;
    [SerializeField] private float ResetPosition;
    [SerializeField] private float EndPosition;

    private void Start()
    {
    }

    void Update()
    {
        foreach (GameObject obj in ScrollMapObj)
        {
            obj.transform.position += Vector3.back * Map_MoveSpeed * Time.timeScale;
            if (obj.transform.position.z <= EndPosition)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, ResetPosition);
            }
        }
    }
}
