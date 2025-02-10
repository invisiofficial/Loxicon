using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
public abstract class AChangable : MonoBehaviour
{
    private void Start() => Recreate();

    public abstract void Recreate();
}

#if UNITY_EDITOR
[CustomEditor(typeof(AChangable), true)]
[CanEditMultipleObjects]
public class AChangableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck()) (target as AChangable).Recreate();
    }
}
#endif