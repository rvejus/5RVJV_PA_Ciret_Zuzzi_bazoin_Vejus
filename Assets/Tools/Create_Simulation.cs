using System;
using UnityEditor;
using UnityEngine;


public class Create_Simulation : EditorWindow
{

    private GridSM grid;
    private BoidManager boidman;
    private Vector3 gridSize;
    
    
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
    
    private void CalculateGridSize()
    {
        gridSize = new Vector3(grid.cells_x * grid.cell_size, grid.cells_y * grid.cell_size, grid.cells_z * grid.cell_size);
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
        
        CalculateGridSize();
        EditorGUILayout.Space();
        
        grid.nbBubulle = EditorGUILayout.IntField("Number of particle", grid.nbBubulle);
        grid.maxIterProjection =EditorGUILayout.IntField("Number of projection Iteration", grid.maxIterProjection);
        grid.maxIterPoisson = EditorGUILayout.IntField("Number of Poisson iteration", grid.maxIterPoisson);
        grid.bubullePrefab = EditorGUILayout.ObjectField("Prefab Particle", grid.bubullePrefab, typeof(GameObject), false) as GameObject;
        grid.boidsManager = EditorGUILayout.ObjectField("Boids Manager", grid.boidsManager, typeof(BoidManager), true) as BoidManager;
        
        EditorGUILayout.Space();
        
        //Modification de velocité
        EditorGUILayout.LabelField("Velocity Configuration", EditorStyles.boldLabel);
        if (grid.velocity == null || grid.velocity.Length != grid.cells_x * grid.cells_y * grid.cells_z)
        {
            grid.velocity = new Vector3[grid.cells_x, grid.cells_y, grid.cells_z];
        }
        //Modification de pression
        EditorGUILayout.LabelField("Pressures Configuration", EditorStyles.boldLabel);
        if (grid.pressures == null || grid.pressures.Length != grid.cells_x * grid.cells_y * grid.cells_z)
        {
            grid.pressures = new float[grid.cells_x, grid.cells_y, grid.cells_z];
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Modification des Boids
        EditorGUILayout.LabelField("Boid Configuration", EditorStyles.boldLabel);
        boidman.boidsPerTarget = EditorGUILayout.IntField("Boid per Particule", boidman.boidsPerTarget);
        boidman.boidPrefab = EditorGUILayout.ObjectField("Prefab Boid", boidman.boidPrefab, typeof(GameObject), false) as GameObject;
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        

    }

    
    private void OnSceneGUI(SceneView sceneview)
    {
        if (grid == null)
            return;

        float cellSize = grid.cell_size;

        for (int x = 0; x < grid.cells_x; x++)
        {
            for (int z = 0; z < grid.cells_z; z++)
            {
                
                    Vector3 position = new Vector3(x * cellSize,  cellSize,  z*cellSize);

                    Handles.color = Color.blue;
                    Handles.DrawWireCube(position, Vector3.one * cellSize);
                
            }
        }
    }
}
