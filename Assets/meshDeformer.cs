using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshDeformer : MonoBehaviour
{
    private Mesh deformingMesh;
    private Vector3[] ogVertices, displacedVertices;
    private Vector3[] vertexVelocities;

    private void Start()
    {
        
        deformingMesh = GetComponent<MeshFilter>().mesh;
        ogVertices = deformingMesh.vertices;
        vertexVelocities = new Vector3[ogVertices.Length];
        displacedVertices = new Vector3[ogVertices.Length];
        for (int i = 0; i < ogVertices.Length; i++)
        {
            displacedVertices[i] = ogVertices[i];
        }
    }

    private void Update()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            
        }
    }
}
