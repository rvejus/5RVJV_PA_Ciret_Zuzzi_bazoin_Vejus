using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWorldPosZero : MonoBehaviour
{
    void LateUpdate()
    {
        transform.position = Vector3.zero;
    }
}
