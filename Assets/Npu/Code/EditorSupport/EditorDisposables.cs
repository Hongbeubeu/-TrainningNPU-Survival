﻿using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


namespace Npu.EditorSupport
{

    public class UndoAndDirty : System.IDisposable
    {
        private Object _target;
        
        public UndoAndDirty(Object target, string undoText=null)
        {
            _target = target;
            Undo.RegisterCompleteObjectUndo(_target, undoText ?? _target.name);
        }
        
        public void Dispose()
        {
            EditorUtility.SetDirty(_target);
        }
    }
    
    public class DisabledGui : System.IDisposable
    {
        private bool _enabled;
        public DisabledGui(bool disable)
        {
            _enabled = GUI.enabled;
            GUI.enabled = !disable;
        }
        public void Dispose()
        {
            GUI.enabled = _enabled;
        }
    }

    public class GuiColor : System.IDisposable
    {
        Color c;
        public GuiColor(Color c)
        {
            this.c = GUI.color;
            GUI.color = c;
        }
        public void Dispose()
        {
            GUI.color = c;
        }
    }
    
    public class BackgroundColor : System.IDisposable
    {
        Color c;
        public BackgroundColor(Color c)
        {
            this.c = GUI.backgroundColor;
            GUI.backgroundColor = c;
        }
        public void Dispose()
        {
            GUI.backgroundColor = c;
        }
    }

    public class GizmosColor : System.IDisposable
    {
        Color c;
        public GizmosColor(Color c)
        {
            this.c = Gizmos.color;
            Gizmos.color = c;
        }
        public void Dispose()
        {
            Gizmos.color = c;
        }
    }
    
    public class HandlesColor : System.IDisposable
    {
        Color c;
        public HandlesColor(Color c)
        {
            this.c = Handles.color;
            Handles.color = c;
        }
        public void Dispose()
        {
            Handles.color = c;
        }
    }

    public class VerticalHelpBox : System.IDisposable
    {
        
        public VerticalHelpBox(params  GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, options);
        }
        
        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }

    public class HorizontalHelpBox : System.IDisposable
    {
        
        public HorizontalHelpBox(params  GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }

    public class ScrollView : System.IDisposable
    {
        
        public ScrollView(ref Vector2 pos)
        {
            pos = EditorGUILayout.BeginScrollView(pos);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }

    public class HorizontalLayout : System.IDisposable
    {
        private bool centered;
        public HorizontalLayout(bool centered=false, params  GUILayoutOption[] options)
        {
            this.centered = centered;
            EditorGUILayout.BeginHorizontal(options);
            if (centered) GUILayout.FlexibleSpace();
        }
        public void Dispose()
        {
            if (centered) GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    public class VerticalLayout : System.IDisposable
    {
        public VerticalLayout(params  GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }
        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }

    public class LabelWidth : System.IDisposable
    {
        float originalWidth;
        public LabelWidth(float width)
        {
            originalWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
        }
        public void Dispose()
        {
            EditorGUIUtility.labelWidth = originalWidth;
        }
    }

    public class Indent : System.IDisposable
    {
        int indent;
        public Indent(int indent=1)
        {
            this.indent = indent;
            EditorGUI.indentLevel += indent;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel -= indent;
        }
    }
    
    
}

#endif