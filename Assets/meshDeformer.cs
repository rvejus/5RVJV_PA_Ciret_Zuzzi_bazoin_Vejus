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
    [SerializeField]
    private GridSM _gridSm;
    private int trianglesNum;
    [SerializeField][Range(0.0f,5f)]
    private float maxDiff=0.1f;
    [SerializeField][Range(0.1f,3f)]
    private float amplify=1f;
    private int Xsize;
    private int Zsize;
    [SerializeField]
    private GameObject prefabVisu;
    private void Start()
    {
        deformingMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = deformingMesh;
        Xsize = _gridSm.cells_x;
        Zsize = _gridSm.cells_z;
        float ypos = transform.position.y;
        ogVertices = new Vector3[(Xsize+1) * (Zsize+1)];
        Debug.Log(ogVertices.Length);
        Vector2[] uv = new Vector2[ogVertices.Length];
        Debug.Log(uv.Length);
        Vector4[] tangents = new Vector4[ogVertices.Length];
        Debug.Log(tangents.Length);
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z=0; z <= Zsize; z++)
        {
            for (int x = 0; x <= Xsize; i++, x++)
            {
                ogVertices[i] = new Vector3(x, 0, z);
                uv[i] = new Vector2((float)x / Xsize, (float)z / Zsize);
                tangents[i] = tangent;
                //Instantiate(prefabVisu, ogVertices[i], Quaternion.Euler(Vector3.zero));
            }
        }

        deformingMesh.vertices = ogVertices;
        deformingMesh.uv = uv;
        deformingMesh.tangents = tangents;
        displacedVertices = new Vector3[ogVertices.Length];
        for (int i = 0; i < ogVertices.Length; i++)
        {
            displacedVertices[i] = ogVertices[i];
        }
        reconstructMesh();
    }

    private void reconstructMesh()
    {
        //Reconstructing Mesh
        //Debug.Log("nb dis vertices: "+ displacedVertices.Length);
        //Debug.Log("reconstruct: "+displacedVertices.Length);
        deformingMesh.vertices = displacedVertices;
        int[] triangles = new int[Xsize * Zsize * 6];
        for (int ti = 0, vi = 0, z = 0; z < Zsize; z++, vi++) {
            for (int x = 0; x < Xsize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + Xsize + 1;
                triangles[ti + 5] = vi + Xsize + 2;
            }
        }
        deformingMesh.triangles = triangles;
        
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateTangents();
        GetComponent<MeshFilter>().mesh = deformingMesh;
        
    }
    private void Update()
    {
        boidsTrans = boidManager.boidsTrans;
        //Debug.Log(boidsTrans[0]);
        float dt = Time.deltaTime;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            bool isBoid = false;
            float actDiff = 1000.0f;
            for (int j = 0; j < boidsTrans.Length; j++)
            {
                float diff = Vector3.Distance(displacedVertices[i], boidsTrans[j]);
                diff = Mathf.Abs(diff);
                if (diff <= maxDiff)
                {
                    //Debug.Log("test");
                    if (diff < actDiff)
                    {
                        actDiff = diff;
                    }
                    //displacedVertices[i].y -= (boidsTrans[j].y-displacedVertices[i].y)*dt*amplify;
                    isBoid = true;
                }
            }

            if (!isBoid)
            {
                displacedVertices[i].y -= (displacedVertices[i].y - ogVertices[i].y)*dt;
            }
            else
            {
                displacedVertices[i].y -= actDiff * dt * amplify;
            }
        }
        reconstructMesh();
    }
}
