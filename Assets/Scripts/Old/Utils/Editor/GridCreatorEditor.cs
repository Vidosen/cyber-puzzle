using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomEditor(typeof(GridCreator))]
    public class GridCreatorEditor : UnityEditor.Editor
    {
        private bool currentToggle = false;

        public override void OnInspectorGUI()
        {
            var gridCreator = target as GridCreator;
            
            
            base.OnInspectorGUI();
            
            var clearButtonPressed = GUILayout.Button("Clear");
            if (clearButtonPressed)
                gridCreator.ClearObjectChildren();
            var applyButtonPressed = GUILayout.Button("Apply");
            if (applyButtonPressed || currentToggle)
                gridCreator.ApplyGridCreatorModifications();
            
            currentToggle = GUILayout.Toggle(currentToggle, "Apply in Update?");
        }


    }
}
