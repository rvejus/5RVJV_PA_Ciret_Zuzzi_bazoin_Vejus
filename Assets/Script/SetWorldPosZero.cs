using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWorldPosZero : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
