
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapMaker))]
public class MapGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生成地圖"))
        {
            var mm = target as MapMaker;
            mm.Export();
        }
    }
}
