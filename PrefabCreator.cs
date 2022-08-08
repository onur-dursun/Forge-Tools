// Create a folder (right click in the Assets directory, click Create>New Folder)
// and name it “Editor” if one doesn’t exist already. Place this script in that folder.

// This script creates a new menu item Examples>Create Prefab in the main menu.
// Use it to create Prefab(s) from the selected GameObject(s).
// It will be placed in the root Assets folder.

using UnityEngine;
using UnityEditor;
using System.IO;

namespace Whaleforge.Tools
{
    public class PrefabCreator : EditorWindow
    {
        [SerializeField] public float multiplier;
        [SerializeField] public string folderPath;
        [SerializeField] public string packageName;
        [SerializeField] public Material mat;
        [SerializeField] public bool colliderBool;

        string notification;

        const float space = 10f;
        // Creates a new menu item 'Examples > Create Prefab' in the main menu.
        [MenuItem("ForgeTools/Prefab Creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(PrefabCreator));
        }
        private void OnEnable()
        {
            var scriptableContainer = AssetDatabase.LoadAssetAtPath<ScriptableContainer>("Assets/Editor/DefaultValues.asset");

            if (scriptableContainer == null)
                return;

            mat = scriptableContainer.mat;
            folderPath = scriptableContainer.folderName;
            packageName = scriptableContainer.packageName;
            multiplier = scriptableContainer.multiplier;
        }

        void OnGUI()
        {

            GUILayout.Label("Prefab oluşturmak istediğimiz klasörün adını giriyoruz. Örn: Props", EditorStyles.helpBox);
            GUILayout.Label(folderPath);
            if (GUILayout.Button("Select Folder"))
            {
                folderPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
            }

            GUILayout.Space(space);
            GUILayout.Label("Prefab oluştururken material de atamak istiyorsak material seçiyoruz.", EditorStyles.helpBox);
            mat = (Material)EditorGUILayout.ObjectField("Material", mat, typeof(Material), false);

            GUILayout.Space(space);
            GUILayout.Label("Prefab oluştururken collider de eklemek istiyorsak onu seçiyoruz.", EditorStyles.helpBox);
            colliderBool = (bool)EditorGUILayout.Toggle("Collider", colliderBool);

            GUILayout.Space(space);
            GUILayout.Label("Prefab oluşturma butonu. Seçili objeleri prefaba dönüştürür.", EditorStyles.helpBox);

            if (GUILayout.Button("Create Prefab", EditButton()))
            {
                Debug.Log("button");
                if (IsSelectionValid() && CheckStrings())
                    return;

                CreatePrefab(CreatePath());
            }

            GUI.backgroundColor = Color.white;

            GUILayout.Space(space);
            GUILayout.Label(notification, EditorStyles.helpBox);


            /* if (GUILayout.Button("Assign Material"))
             {
                 if (mat == null)
                     return;
                 AssignMaterial();
             }*/

            GUILayout.Space(space * 4);
            GUILayout.Label("Alignment Settings", EditorStyles.boldLabel);
            GUILayout.Label("Sahnedeki prefabları grid şeklinde düzenlemeye yarar.", EditorStyles.helpBox);
            multiplier = EditorGUILayout.FloatField("Multiplier", multiplier);

            if (GUILayout.Button("Align Objects"))
            {
                AlignObjects(multiplier);
            }



            #region Local Funcs.

            string CreatePath()
            {
                //var assetPath = "Assets/RedWorks - LP " + packageName + "/Assets/Prefabs/";
                string newFolderPath = folderPath.Substring(folderPath.IndexOf("Assets")) + "/";
                Debug.Log("Folder Path: " + newFolderPath);

                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    Debug.LogError("Folder path is not valid!");
                    return null;
                }

                return newFolderPath;
            }

            void CreatePrefab(string newFolderPath)
            {
                Debug.Log("Folder Path: " + newFolderPath);

                if (newFolderPath == null)
                    return;

                GameObject[] objs = Selection.gameObjects;
                // Loop through every GameObject in the array above
                foreach (GameObject g in objs)
                {
                    // Set the path as within the Assets folder,
                    // and name it as the GameObject's name with the .Prefab format


                    string localPath = newFolderPath + g.name + ".prefab";
                    Debug.Log(localPath);

                    // Make sure the file name is unique, in case an existing Prefab has the same name.
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                    ResetTransform(g);

                    if (mat != null)
                    {
                        Renderer[] renderers = g.GetComponentsInChildren<Renderer>(true);
                        if (renderers.Length > 0)
                            foreach (var renderer in renderers)
                            {
                                renderer.sharedMaterial = mat;
                                if (colliderBool && renderer.gameObject.GetComponent<Collider>() == null)
                                    renderer.gameObject.AddComponent<MeshCollider>();
                            }
                    }

                    // Create the new Prefab.
                    PrefabUtility.SaveAsPrefabAssetAndConnect(g, localPath, InteractionMode.UserAction);
                    Debug.Log("Prefab created: " + localPath);
                }

                GUI.skin.box.fontSize = 1;
                //ShowNotification(new GUIContent());
                notification = newFolderPath + " klasörünün içerisinde " + objs.Length + " tane prefab oluşturuldu.";
            }

            bool IsSelectionValid()
            {
                return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
            }

            void ResetTransform(GameObject g)
            {
                g.transform.position = Vector3.zero;
                g.transform.rotation = Quaternion.identity;
                g.transform.localScale = Vector3.one;
            }

            void AlignObjects(float multiplier)
            {
                // Keep track of the currently selected GameObject(s)
                if (!IsSelectionValid())
                    return;
                int i = 0;
                // Loop through every GameObject in the array above
                Transform[] ts = Selection.transforms;
                System.Array.Sort(ts, new UnityTransformSort());
                int extents = 0;
                foreach (Transform t in ts)
                {
                    extents += (int)t.GetComponent<Renderer>().bounds.size.x / 2;
                    t.localPosition = Vector3.right * extents + Vector3.right * multiplier * i;

                    extents += (int)t.GetComponent<Renderer>().bounds.size.x / 2;
                    i++;
                    Debug.Log("Object aligned: " + (int)t.GetComponent<Renderer>().bounds.extents.x);
                }
            }

            bool CheckStrings()
            {
                return folderPath == null || folderPath == "";
            }

            GUIStyle EditButton()
            {
                var style = new GUIStyle(GUI.skin.button);
                style.fontStyle = FontStyle.Bold;
                GUI.backgroundColor = new Color32(239, 149, 156, 255);
                return style;
            }

            #endregion
        }
    }


    public class UnityTransformSort : System.Collections.Generic.IComparer<Transform>
    {
        public int Compare(Transform lhs, Transform rhs)
        {
            if (lhs == rhs) return 0;
            if (lhs == null) return -1;
            if (rhs == null) return 1;
            return (lhs.GetSiblingIndex() > rhs.GetSiblingIndex()) ? 1 : -1;
        }
    }
}