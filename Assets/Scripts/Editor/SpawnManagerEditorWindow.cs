using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameDevHQ.Manager.SpawnManagerNS;

public class SpawnManagerEditorWindow : EditorWindow
{
    public string waveName = "new Wave";
    public int numberOfEnemies = 1;
    public List<EnemyType> enemies = new List<EnemyType>();
    public Vector2 scrollPos;

    public SpawnManager spawnManager;
    private Transform _startPoint;
    private Transform _endPoint;
    private float _spawnDelay = 1f;
    private List<Wave> _waves = new List<Wave>();
    private int _numberOfWaves = 1;
    private int _maxWaves = 20;


    [MenuItem("Window/Spawn Manager")]
    public static void ShowWindow()
    {
        SpawnManagerEditorWindow window = GetWindow<SpawnManagerEditorWindow>("Spawn Manager");
        window.maxSize = new Vector2(825, 600);
        window.minSize = window.maxSize;
    }

    public void OnGUI()
    {
        if (spawnManager == null)
        {
            SetSpawnManager();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(400));
        spawnManager = (SpawnManager)EditorGUILayout.ObjectField("Spawn Manager", spawnManager, typeof(SpawnManager), true);
        _startPoint = (Transform)EditorGUILayout.ObjectField("Start Point", _startPoint, typeof(Transform), true);
        _endPoint = (Transform)EditorGUILayout.ObjectField("End Point", _endPoint, typeof(Transform), true);
        _spawnDelay = EditorGUILayout.FloatField("Spawn Delay", _spawnDelay);
        _numberOfWaves = EditorGUILayout.IntSlider("Number Of Waves", _numberOfWaves, 1, _maxWaves);

        if (_spawnDelay < 0)
        {
            _spawnDelay = 0;
        }

        while (_waves.Count > _numberOfWaves)
        {
            _waves.RemoveAt(_waves.Count - 1);
        }

        for (int i = 0; i < _numberOfWaves; i++)
        {
            if (_waves.Count < _numberOfWaves)
            {
                _waves.Add(null);
            }
            _waves[i] = (Wave)EditorGUILayout.ObjectField(_waves[i], typeof(Wave), true);
        }

        if (GUILayout.Button("Update Spawn Manager"))
        {
            ChangeSpawnManager();
        }

        if (GUILayout.Button("Reload Spawn Manager"))
        {
            ReloadSpawnManager();
        }
        EditorGUILayout.EndVertical();

        DrawUILine(Color.gray);

        EditorGUILayout.BeginVertical(GUILayout.Width(400));
        waveName = EditorGUILayout.TextField("Name of Wave", waveName);
        numberOfEnemies = EditorGUILayout.IntSlider("Number Of Enemies", numberOfEnemies, 1, 100);

        while (enemies.Count > numberOfEnemies)
        {
            enemies.RemoveAt(enemies.Count - 1);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(400));

        for (int i = 0; i < numberOfEnemies; i++)
        {
            enemies.Add(null);
            enemies[i] = (EnemyType)EditorGUILayout.ObjectField(enemies[i], typeof(EnemyType), false);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Create Wave"))
        {
            CreateWave();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public void SetSpawnManager()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            GetInfo();
        }
    }

    public void GetInfo()
    {
        _startPoint = spawnManager.GetStartPoint();
        _endPoint = spawnManager.GetEndPoint();
        _spawnDelay = spawnManager.GetSpawnDelay();
        List<Wave> tempWaves = spawnManager.GetWaves();
        _waves.Clear();
        for (int i = 0; i < tempWaves.Count; i++)
        {
            _waves.Add(tempWaves[i]);
        }
        _numberOfWaves = _waves.Count;
    }
    
    public void ChangeSpawnManager()
    {
        spawnManager.UpdateSpawnManager(_startPoint, _endPoint, _spawnDelay, _waves);
    }

    public void ReloadSpawnManager()
    {
        SetSpawnManager();
    }

    public void CreateWave()
    {
        Wave asset = CreateInstance<Wave>();
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                asset.enemiesToSpawn.Add(enemy);
            }
        }

        AssetDatabase.CreateAsset(asset, "Assets/Scripts/Scriptable Objects/Waves/" + waveName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        ResetWindow();
    }

    public void ResetWindow()
    {
        waveName = "new Wave";
        numberOfEnemies = 0;
    }

    public void DrawUILine(Color color, int thickness = 2, int padding = 2)
    {
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
        rect.width = thickness;
        rect.y += padding / 2;
        rect.x += 2;
        rect.height += 550;
        EditorGUI.DrawRect(rect, color);
    }

}
