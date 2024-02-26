using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{

    public class Draggable : MonoBehaviour
    {
        public Vector2 dragSensitivity = new Vector2(0.1f, 0.1f);
        [Range(-180, 180)] public float dragTransform = 45;
        public Transform target;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Draggable))]
    public class DraggableInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var Target = target as Draggable;
            var rect = GUILayoutUtility.GetRect(200, 200, GUILayout.ExpandWidth(false));
            var quaternion = Quaternion.AngleAxis(Target.dragTransform, Vector3.down);

            var drag = Vector2.zero;
            if (!Target.target || !DrawDragArea(rect, ref drag, quaternion)) return;

            var drag3 = new Vector3(drag.x, 0, drag.y);
            drag3 = quaternion * drag3;
            var p = Target.target.position;
            p += new Vector3(drag3.z * Target.dragSensitivity.x, 0, drag3.x * Target.dragSensitivity.y);
            Target.target.position = p;

        }

        private bool DrawDragArea(Rect position, ref Vector2 offset, Quaternion axisTransform)
        {
            var controlID = GUIUtility.GetControlID(FocusType.Passive);

            var drag = false;
            offset = Vector2.zero;
            switch (Event.current.GetTypeForControl(controlID))
            {
                case UnityEngine.EventType.Repaint:
                    
                    GUI.Box(position, new GUIContent("Drag Here"));

                    break;

                case UnityEngine.EventType.MouseDown:

                    break;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        //GUIUtility.hotControl = 0;
                        //if (position.Contains(Event.current.mousePosition)) selected = true;
                    }

                    break;

                case UnityEngine.EventType.MouseDrag:
                    if (position.Contains(Event.current.mousePosition))
                    {
                        offset = Event.current.delta;
                        drag = true;
                    }

                    break;
            }

            return drag;
        }
    }
#endif

}