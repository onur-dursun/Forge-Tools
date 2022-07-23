using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WhaleForge.Tools
{
    public class Translator : EditorWindow
    {
        public Vector3 translateVector;

        [MenuItem("ForgeTools/Translator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Translator));
        }
        void OnGUI()
        {
            translateVector = EditorGUILayout.Vector3Field("TranslateVector", translateVector);

            if (GUILayout.Button("Set"))
            {
                if (Selection.activeGameObject != null)
                    translateVector = Selection.activeGameObject.transform.localPosition;

            }

            if (GUILayout.Button("Translate"))
            {

                GameObject[] objs = Selection.gameObjects;
                // Loop through every GameObject in the array above
                foreach (GameObject g in objs)
                {
                    g.transform.localPosition -= translateVector;
                }
            }


        }

        bool IsSelectionValid()
        {
            return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
        }
    }
}