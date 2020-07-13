using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameDevHQ.Manager.SpawnManagerNS;

public class SpawnManagerEditorWindow : EditorWindow
{
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
        GetWindow<SpawnManagerEditorWindow>("Spawn Manager");
    }

    public void OnGUI()
    {
        if (spawnManager == null)
        {
            SetSpawnManager();
        }
        
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

    }

    public void SetSpawnManager()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager != null)
        {
            GetInfo();
            Debug.Log("Found it!");
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
        Debug.Log("Reload");
        SetSpawnManager();
    }
}
