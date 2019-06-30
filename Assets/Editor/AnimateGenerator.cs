using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharAnimateMaker))]
public class AnimateGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("修正角色瞄點"))
        {
            var mm = target as CharAnimateMaker;
            mm.CalcPivot();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("生成玩家動畫"))
        {
            var mm = target as CharAnimateMaker;
            mm.Export();
        }

        
    }
}