using UnityEngine;
using UnityEditor;

namespace SwarmLab.Editor
{
    [CustomEditor(typeof(SwarmManager))]
    public class SwarmManagerEditor : UnityEditor.Editor
    {
        // Cache the editor for the ScriptableObject
        private UnityEditor.Editor _configEditor;

        public override void OnInspectorGUI()
        {
            // 1. Draw the Default Script field (and any other direct fields on Manager)
            serializedObject.Update();
            
            SerializedProperty configProp = serializedObject.FindProperty("swarmConfig");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(configProp);
            if (EditorGUI.EndChangeCheck())
            {
                // If we changed the config asset, we need to destroy the old cached editor
                if (_configEditor != null) DestroyImmediate(_configEditor);
            }

            serializedObject.ApplyModifiedProperties();

            // 2. Draw the Embedded Inspector
            if (configProp.objectReferenceValue != null)
            {
                DrawEmbeddedConfigInspector(configProp.objectReferenceValue);
                
                EditorGUILayout.Space(10);
                DrawGenerationButtons();
            }
            else
            {
                EditorGUILayout.HelpBox("Assign a Swarm Config to begin.", MessageType.Info);
            }
        }

        private void DrawEmbeddedConfigInspector(Object configObject)
        {
            // Draw a visual separator
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space();

            // Create or reuse the editor for the ScriptableObject
            CreateCachedEditor(configObject, null, ref _configEditor);

            // Draw it! This invokes the OnInspectorGUI of SwarmConfigEditor
            _configEditor.OnInspectorGUI();
        }

        private void DrawGenerationButtons()
        {
            SwarmManager manager = (SwarmManager)target;

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear Swarm", GUILayout.Height(30)))
            {
                manager.ClearSwarm();
            }

            // Green "Generate" button
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            
            if (GUILayout.Button("Generate Swarm", GUILayout.Height(30)))
            {
                manager.GenerateSwarm();
            }
            
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndHorizontal();
        }
        
        private void OnSceneGUI()
        {
            SwarmManager manager = (SwarmManager)target;
            if (manager.Config == null) return;

            // Create a SerializedObject for the Config Asset so we can record Undos
            SerializedObject configSO = new SerializedObject(manager.Config);
            configSO.Update();

            SerializedProperty listProp = configSO.FindProperty("speciesConfigs");

            // Loop through each species in the list
            for (int i = 0; i < listProp.arraySize; i++)
            {
                SerializedProperty element = listProp.GetArrayElementAtIndex(i);
                
                SerializedProperty offsetProp = element.FindPropertyRelative("spawnOffset");
                SerializedProperty radiusProp = element.FindPropertyRelative("spawnRadius");
                SerializedProperty defProp = element.FindPropertyRelative("speciesDefinition");

                string speciesName = defProp.objectReferenceValue != null ? defProp.objectReferenceValue.name : $"Species {i}";
                Color speciesColor = Color.HSVToRGB((speciesName.GetHashCode() * 0.13f) % 1f, 1f, 1f);

                // --- 1. Coordinate Conversion ---
                // The data is Local (Offset), but Handles work in World Space.
                Vector3 worldCenter = manager.transform.TransformPoint(offsetProp.vector3Value);

                // --- 2. Draw Position Handle (Move the Sphere) ---
                EditorGUI.BeginChangeCheck();
                Handles.color = speciesColor;
                
                // Draw name label above the sphere
                Handles.Label(worldCenter + Vector3.up * (radiusProp.floatValue + 0.1f), speciesName, EditorStyles.boldLabel);

                Vector3 newWorldCenter = Handles.PositionHandle(worldCenter, manager.transform.rotation);

                if (EditorGUI.EndChangeCheck())
                {
                    // Convert back to Local Space before saving
                    offsetProp.vector3Value = manager.transform.InverseTransformPoint(newWorldCenter);
                }

                // --- 3. Draw Radius Handle (Resize the Sphere) ---
                EditorGUI.BeginChangeCheck();
                // RadiusHandle draws a wire sphere with 4 dots on the perimeter
                float newRadius = Handles.RadiusHandle(manager.transform.rotation, worldCenter, radiusProp.floatValue);
                
                if (EditorGUI.EndChangeCheck())
                {
                    radiusProp.floatValue = Mathf.Max(0.1f, newRadius); // Prevent negative radius
                }
            }

            // Write modified values back to the ScriptableObject
            if (configSO.hasModifiedProperties)
            {
                configSO.ApplyModifiedProperties();
            }
        }
    }
}