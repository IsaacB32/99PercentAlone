using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CustomEditor(typeof(Menu))]
public class MenuEditor : NaughtyInspector
{
    private const string CURSOR_KEY = "VirtualCursor";
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Menu menu = (Menu)target;
        if (!menu.Cursor)
        {
            if (GUILayout.Button("Create New Cursor"))
            {
                MenuVirtualCursor[] cursors = menu.GetComponentsInChildren<MenuVirtualCursor>();
                if (cursors.Length > 0)
                {
                    cursors[0].transform.SetAsLastSibling();
                    menu.AssignCursor(cursors[0]);
                }
                else
                {
                    AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(CURSOR_KEY);
                    handle.Completed += result =>
                    {
                        GameObject cursorObj = result.Result;
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            cursorObj.name = "VirtualCursor";
                            cursorObj.transform.parent = menu.transform;
                            cursorObj.transform.SetAsLastSibling();
                            menu.AssignCursor(cursorObj.GetComponent<MenuVirtualCursor>());
                        }
                    };
                }
            }
        }
    }
}
