using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class GameDataEditor : EditorWindow
{
    public GameData gameData;
    private string gameDataFilename = "data.json";
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Game Data Editor")]
    static void Init()
    {
        GameDataEditor window = (GameDataEditor)EditorWindow.GetWindow(typeof(GameDataEditor));
        window.Show();
    }

    // Runs continuously (similar to OnUpdate)
    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(600), GUILayout.Height(600));

        if (gameData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("gameData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Data"))
            {
                SaveGameData();
            }
            
        }

        if (GUILayout.Button("Load Data"))
        {
            LoadGameData();
        }
        GUILayout.EndScrollView();

    }

    private void LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            gameData = new GameData();
        }
    }

    private void SaveGameData()
    {
        string dataAsJson = JsonUtility.ToJson(gameData);
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFilename);
        File.WriteAllText(filePath, dataAsJson);
    }
}
