using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class Create_Simulation : EditorWindow
{

    private GridSM grid;
    private BoidManager boidman;
    private Vector3 gridSize;
    private bool isHeight;
    private bool isPreview = false;
    private bool printVector;
    private GameObject target;

    
    
    [MenuItem("Fluid Simulation Tools/Create Simulation")]
    public static void ShowWindows()
    {
        GetWindow(typeof(Create_Simulation), false, "Create Simulation");
    }


    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChange;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChange;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    
    private void OnSelectionChange()
    {
        UpdateScriptReferences();
        Repaint();
    }

    private void UpdateScriptReferences()
    {
        grid = FindObjectOfType<GridSM>();
        boidman = FindObjectOfType<BoidManager>();
    }
    
    int getIndex(int x, int y, int z)
    {
        return x * (grid.cells_y * grid.cells_z) + y * grid.cells_z + z;
    }


    private void OnGUI()
    {
        
        if (grid == null)
        {
            EditorGUILayout.LabelField("No grid component found in the scene.");
            return;
        }
        
        GUILayout.Label("Configure your Fluid Simulation",EditorStyles.boldLabel);
        
        
         // Modification de la grille
        grid.cells_x = EditorGUILayout.IntField("Size X", grid.cells_x);
        grid.cells_y = EditorGUILayout.IntField("Size Y", grid.cells_y);
        grid.cells_z = EditorGUILayout.IntField("Size Z", grid.cells_z);
        grid.cell_size = EditorGUILayout.FloatField("Grid cell size", grid.cell_size);
        
        
        EditorGUILayout.Space();
        
        grid.nbBubulle = EditorGUILayout.IntField("Number of particle", grid.nbBubulle);
        grid.maxIterProjection =EditorGUILayout.IntField("Number of projection Iteration", grid.maxIterProjection);
        grid.maxIterPoisson = EditorGUILayout.IntField("Number of Poisson iteration", grid.maxIterPoisson);
        grid.bubullePrefab = EditorGUILayout.ObjectField("Prefab Particle", grid.bubullePrefab, typeof(GameObject), false) as GameObject;
        grid.boidsManager = EditorGUILayout.ObjectField("Boids Manager", grid.boidsManager, typeof(BoidManager), true) as BoidManager;
        grid.spawnPoint = EditorGUILayout.ObjectField("Particle SpawnPoint", grid.spawnPoint, typeof(GameObject), true) as GameObject;
        EditorGUILayout.Space();
        
        //Modification de velocité
        EditorGUILayout.LabelField("Velocity Configuration", EditorStyles.boldLabel);
        if (grid.velocity == null || grid.velocity.Length != grid.cells_x * grid.cells_y * grid.cells_z)
        {
            grid.velocity = new Vector3[grid.cells_x* grid.cells_y* grid.cells_z];
        }
        //Modification de pression
        EditorGUILayout.LabelField("Pressures Configuration", EditorStyles.boldLabel);
        if (grid.pressures == null || grid.pressures.Length != grid.cells_x * grid.cells_y * grid.cells_z)
        {
            grid.pressures = new float[grid.cells_x* grid.cells_y* grid.cells_z];
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Modification des Boids
        EditorGUILayout.LabelField("Boid Configuration", EditorStyles.boldLabel);
        boidman.boidsPerTarget = EditorGUILayout.IntField("Boid per Particule", boidman.boidsPerTarget);
        boidman.boidPrefab = EditorGUILayout.ObjectField("Prefab Boid", boidman.boidPrefab, typeof(GameObject), false) as GameObject;
        boidman.spawnPoint = EditorGUILayout.ObjectField("Boid SpawnPoint", boidman.spawnPoint, typeof(GameObject), true) as Transform;
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Preview configurator", EditorStyles.boldLabel);
        isPreview = EditorGUILayout.Toggle("ShowPreview", isPreview);
        isHeight = EditorGUILayout.Toggle("Print the Height ?", isHeight);
        printVector = EditorGUILayout.Toggle("Print the vector of Cells ?", printVector);
        

        //Choix des velocité de bases
        if (GUILayout.Button("Velocity = random"))
        {
            FillVelocityWithRandom();
        }
        
        if (GUILayout.Button("Velocity = Null"))
        {
            FillVelocityWithNullVector();
        }
        
        target = EditorGUILayout.ObjectField("Target look at", target, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Velocity = Look at Target"))
        {
            FillVelocityWithLookAt();
        }
       
        grid.syphonGO = EditorGUILayout.ObjectField("Syphon", grid.syphonGO, typeof(GameObject), true) as GameObject;
        
    }

    
    private void OnSceneGUI(SceneView sceneview)
    {
        
        
        if (grid == null || isPreview == false)
            return;

        float cellSize = grid.cell_size;

        if (isHeight)
        {
            for (int x = 0; x < grid.cells_x; x++)
            {
                for (int y = 0; y < grid.cells_y; y++)
                {
                    for (int z = 0; z < grid.cells_z; z++)
                    {
                
                        Vector3 position = new Vector3(x * cellSize,  y*cellSize,  z*cellSize);

                        Handles.color = Color.blue;
                        Handles.DrawWireCube(position, Vector3.one * cellSize);
                        
                        if (printVector)
                        {
                            Vector3 direction = grid.velocity[getIndex(x, y, z)].normalized;
                            ArrowDrawer.DrawArrow(position, direction, cellSize);
                        }
                
                    } 
                }
            } 
        }
        else
        {
            for (int x = 0; x < grid.cells_x; x++)
            {
                for (int z = 0; z < grid.cells_z; z++)
                    {
                        Vector3 position = new Vector3(x * cellSize,  cellSize,  z*cellSize);

                        Handles.color = Color.blue;
                        Handles.DrawWireCube(position, Vector3.one * cellSize);
                        if (printVector )
                        {
                            Vector3 direction = grid.velocity[getIndex(x, 0, z)].normalized;
                            ArrowDrawer.DrawArrow(position, direction, cellSize);
                        }

                    }
            }
        }
    }


    private void FillVelocityWithRandom()
    {
        for (int x = 0; x < grid.cells_x; x++)
        {
            for (int y = 0; y < grid.cells_y; y++)
            {
                for (int z = 0; z < grid.cells_z; z++)
                {
                    grid.velocity[getIndex(x, y, z)] = new Vector3(Random.Range(-1,1)*2,
                        Random.Range(-1,1)*2,
                        Random.Range(-1,1)*2);
                }
            }
        }
        Debug.Log(grid.velocity[0]);
    }
    
    private void FillVelocityWithNullVector()
    {
        for (int x = 0; x < grid.cells_x; x++)
        {
            for (int y = 0; y < grid.cells_y; y++)
            {
                for (int z = 0; z < grid.cells_z; z++)
                {
                    grid.velocity[getIndex(x, y, z)] = Vector3.zero;
                }
            }
        }
    }

    private void FillVelocityWithLookAt()
    {
        Vector3 targetPosition = target.transform.position;
        for (int x = 0; x < grid.cells_x; x++)
        {
            for (int y = 0; y < grid.cells_y; y++)
            {
                for (int z = 0; z < grid.cells_z; z++)
                {
                    grid.velocity[getIndex(x, y, z)] = ( targetPosition- new Vector3(x, y, z)).normalized;
                }
            }
        }
    }
}


public class ArrowDrawer
{
    private static readonly Color ArrowColor = Color.green;
    private static readonly float ArrowScale = 0.3f;

    public static void DrawArrow(Vector3 position, Vector3 direction, float cellSize)
    {
        Handles.color = ArrowColor;
        Handles.DrawLine(position, position + direction * cellSize * 0.5f);
        Handles.ArrowHandleCap(0, position + direction * cellSize * 0.5f, Quaternion.LookRotation(direction), cellSize * ArrowScale, EventType.Repaint);
    }
}