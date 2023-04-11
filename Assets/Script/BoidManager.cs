using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;
    
    [SerializeField]
    public BoidSettings settings;
    [SerializeField]
    public ComputeShader compute;
    [SerializeField]
    public int SavageboidsNumber;

    private int boidsNumber;
    public Boid[] boids;
    [SerializeField] private GameObject boidPrefab;
    public List<Transform> targets = null;
    [SerializeField]
    public int boidsPerTarget=1;
    [SerializeField] private Transform spawnPoint;
    void Start () {
        //boids = FindObjectsOfType<Boid> ();
        boidsNumber = SavageboidsNumber + (targets.Count * boidsPerTarget);
        boids = new Boid[boidsNumber];
        for (int i = 0; i < boidsNumber; i++)
        {
            GameObject curBoid = Instantiate(boidPrefab, spawnPoint);
            Vector3 curPos = curBoid.transform.position;
            curBoid.transform.position = new Vector3(curPos.x + Random.Range(-1.0f, 1.0f),
                curPos.y + Random.Range(-1.0f, 1.0f), curPos.z + Random.Range(-1.0f, 1.0f));
        }
        boids = FindObjectsOfType<Boid> ();
        int latestBoid = 0;
        for (int i=0; i<targets.Count;i++){
            for (int j = 0; j < boidsPerTarget; j++, latestBoid++)
            {
                Boid b = boids[latestBoid];
                b.Initialize (settings, targets[i]);
            }

            if (latestBoid < boids.Length)
            {
                for (int j = latestBoid; j < boids.Length; j++)
                {
                    Boid b = boids[j];
                    b.Initialize (settings, null);
                }
            }
        }

    }

    void Update () {
        if (boids != null) {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer (numBoids, BoidData.Size);
            boidBuffer.SetData (boidData);

            compute.SetBuffer (0, "boids", boidBuffer);
            compute.SetInt ("numBoids", boids.Length);
            compute.SetFloat ("viewRadius", settings.perceptionRadius);
            compute.SetFloat ("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt (numBoids / (float) threadGroupSize);
            compute.Dispatch (0, threadGroups, 1, 1);

            boidBuffer.GetData (boidData);

            for (int i = 0; i < boids.Length; i++) {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid ();
            }

            boidBuffer.Release ();
        }
    }

    public struct BoidData {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}
