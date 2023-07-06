using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshDeformer : MonoBehaviour
{
    private Mesh deformingMesh;
    private Vector3[] ogVertices, displacedVertices;
    private Vector3[] vertexVelocities;
    private Vector3[] boidsTrans;
    [SerializeField]
    private BoidManager boidManager;
    private int trianglesNum;
    [SerializeField][Range(0.0f,0.5f)]
    private float maxDiff=0.1f;
    private void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        ogVertices = deformingMesh.vertices;
        trianglesNum = deformingMesh.triangles.Length;
        vertexVelocities = new Vector3[ogVertices.Length];
        displacedVertices = new Vector3[ogVertices.Length];
        for (int i = 0; i < ogVertices.Length; i++)
        {
            displacedVertices[i] = ogVertices[i];
        }
    }

    private void Update()
    {
        boidsTrans = boidManager.boidsTrans;
        //Debug.Log(boidsTrans[0]);
        float dt = Time.deltaTime;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            bool isBoid = false;
            for (int j = 0; j < boidsTrans.Length; j++)
            {
                float diff = Vector3.Distance(displacedVertices[i], boidsTrans[j]);
                diff = Mathf.Abs(diff);
                if (diff <= maxDiff && !isBoid)
                {
                    //Debug.Log("test");
                    displacedVertices[i].y -= (boidsTrans[j].y-displacedVertices[i].y)*dt;
                    isBoid = true;
                }
            }

            if (!isBoid)
            {
                displacedVertices[i].y -= (displacedVertices[i].y - ogVertices[i].y)*dt;
            }
        }
        //Reconstructing Mesh
        Debug.Log("nb dis vertices: "+ displacedVertices.Length);
        deformingMesh.vertices = displacedVertices;
        List<int> triangles = new List<int>(trianglesNum);
        deformingMesh.triangles = new int[trianglesNum];
        for(int i=0; i<trianglesNum; i+=6)
        {
            triangles.Add(i);
            triangles.Add(i+2);
            triangles.Add(i+1);
            Debug.Log("k le plus haut: "+(i+2));
        }

        deformingMesh.triangles = triangles.ToArray();
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateTangents();
        GetComponent<MeshFilter>().mesh = deformingMesh;
    }
}
