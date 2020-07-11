using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaveMakerEditorWindow : EditorWindow
{
    public string waveName = "new Wave";
    public float spawnDelay = 0f;
    public int numberOfEnemies = 0;
    public List<Object> enemies = new List<Object>();
    public Vector2 scrollPos;

    [MenuItem("Window/Wave Creator")]
    public static void ShowWindow()
    {
        GetWindow<WaveMakerEditorWindow>("Wave Creator");
    }

    public void OnGUI()
    {
        GUILayout.Label("Configure the Wave.");
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        waveName = EditorGUILayout.TextField("Name of Wave", waveName);
        spawnDelay = EditorGUILayout.FloatField("Spawn Delay", spawnDelay);
        numberOfEnemies = EditorGUILayout.IntField("Number Of Enemies", numberOfEnemies);

        if (numberOfEnemies < 1)
        {
            numberOfEnemies = 0;
            enemies.Clear();
        }

        while (enemies.Count > numberOfEnemies)
        {
            enemies.RemoveAt(enemies.Count - 1);
        }
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(400));

        for (int i = 0; i < numberOfEnemies; i++)
        {
            enemies.Add(new Object());
            enemies[i] = EditorGUILayout.ObjectField(enemies[i], typeof(EnemyType), false);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Create Wave"))
        {
            CreateWave();
        }

        EditorGUILayout.EndVertical();
    }

    public void CreateWave()
    {
        Wave asset = CreateInstance<Wave>();
        asset.spawnDelay = spawnDelay;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                asset.enemiesToSpawn.Add((EnemyType)enemy);
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
        spawnDelay = 0f;
        numberOfEnemies = 0;
    }
}
