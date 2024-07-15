#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class DatabaseStaticClassWriter : EditorWindow
{
    string newText;
    string filePath;

    public static void ShowWindow(string filePath, string newText)
    {
        DatabaseStaticClassWriter window = GetWindow<DatabaseStaticClassWriter>("Content has been changed");
        window.newText = newText;
        window.filePath = filePath;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Do you want to generate static class?");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            Apply();
            Close();
        }

        if (GUILayout.Button("Abort"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
        }

    private void Apply()
    {
        Debug.Log($"Writing to {filePath}");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, newText);
        Debug.Log("Writing complete");
    }
}
#endif