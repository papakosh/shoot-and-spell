using UnityEngine;
using UnityEditor;
using System.IO;
/**
  Description: Window editor for managing player data using a gui.
 * 
 * Details: 
 * Init: Get the PlayerDataEditor window and show
 * OnGui: Assign save and load functions to respective buttons
 * LoadPlayerData: If a file exists, read data from the file into player data object, otherwise 
 * create new player data object
 * SavePlayerData: Write player data object to specified file as text
 */
public class PlayerDataEditor : EditorWindow
{
    public PlayerData playerData;
    private string playerDataFilename = "player.json";

    [MenuItem("Window/Player Data Editor")]
    static void Init()
    {
        PlayerDataEditor window = (PlayerDataEditor)EditorWindow.GetWindow(typeof(PlayerDataEditor));
        window.Show();
    }

    private void OnGUI()
    {
        if (playerData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("playerData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Data")) SavePlayerData();
        }
        if (GUILayout.Button("Load Data")) LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFilename);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
        }
        else
            playerData = new PlayerData();
    }

    private void SavePlayerData()
    {
        string dataAsJson = JsonUtility.ToJson(playerData);
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFilename);
        File.WriteAllText(filePath, dataAsJson);
    }
}