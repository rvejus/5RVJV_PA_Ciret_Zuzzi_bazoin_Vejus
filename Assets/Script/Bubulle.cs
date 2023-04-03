using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bubulle : MonoBehaviour
{    // position de la particule
    public Vector3 velocity;     // vitesse de la particule
    public float density;       // densité de la particule
    public float pressure;      // pression de la particule
    public float volume;
    [HideInInspector]public Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = 1.0f;
        volume = 4 / 3 * Mathf.PI * Mathf.Pow(GetComponent<SphereCollider>().radius, 3);
        density = rigidbody.mass / volume;
    }
}
