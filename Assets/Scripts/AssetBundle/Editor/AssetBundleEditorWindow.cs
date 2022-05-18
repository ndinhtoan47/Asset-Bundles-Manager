namespace GameFramework.AssetBundle.CustomEditor
{
#if UNITY_EDITOR

    using UnityEditor;
    using UnityEngine;

    public class AssetBundleEditorWindow : EditorWindow
    {
        private static Vector2 WINDOW_SIZE = new Vector2(800, 600);
	    private static Vector2 WINDOW_MIN_SIZE = new Vector2(400, 300);


        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Window/Asset Bundle Content Manager")]
        private static void ShowWindow()
        {
            AssetBundleEditorWindow window = GetWindow<AssetBundleEditorWindow>();
            window.titleContent = new GUIContent("Asset Bundle Content Manager");
            // window.minSize = window.maxSize = WINDOW_SIZE;
            window.minSize = WINDOW_MIN_SIZE;
            window.Init();
            window.Show();
        }

        private void Init() 
        {

        }
    }
#endif

}