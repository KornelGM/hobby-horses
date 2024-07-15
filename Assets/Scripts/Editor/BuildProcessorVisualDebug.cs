using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProcessorVisualDebug : IPreprocessBuildWithReport
{
    
    public int callbackOrder { get; }
    
    public void OnPreprocessBuild(BuildReport report)
    {
        GameObject visualDebugManagerGameObject = GameObject.Find("VisualDebugManager");
        if (visualDebugManagerGameObject == null) return;
        
        VisualDebugManager visualDebugManager = visualDebugManagerGameObject.GetComponent<VisualDebugManager>();
        
        if (visualDebugManager == null) return;
        if (!visualDebugManager.Enabled) return;
        
        bool userConfirmation = EditorUtility.DisplayDialog(
            title: "Potwierdzenie",
            message: "Czy na pewno chcesz zbudować grę z włączonym Visual Debug?",
            ok: "Tak",
            cancel: "Nie");
            
        if (userConfirmation) return;
            
        visualDebugManager.Enabled = false;
        EditorUtility.SetDirty(visualDebugManager);
    }
}
