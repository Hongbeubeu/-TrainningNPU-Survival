using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyScript))]
public class MyScriptEditor : Editor
{
    private Quaternion lastRotation = Quaternion.identity;

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        MyScript myScript = (MyScript)target;
        Vector3 start = myScript.startPoint;
        Vector3 end = myScript.endPoint;
        Vector3 direction = end - start;
        float length = direction.magnitude;

        // Vẽ đoạn thẳng
        Handles.color = Color.white;
        Handles.DrawLine(start, end);

        // Tính toán trục quay và góc quay
        var newRotation = Handles.RotationHandle(lastRotation, start);

        if (EditorGUI.EndChangeCheck())
        {
            // Xoay đoạn thẳng theo góc quay
            direction = newRotation * Quaternion.Inverse(lastRotation) * direction;
            myScript.endPoint = start + direction.normalized * length;
        }

        lastRotation = newRotation;
    }
}