using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A custom editor for <see cref="GridTile.DirTilePair"/>s.<br/>
/// Largely extrapolated from <see href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html"/>.
/// </summary>
[CustomPropertyDrawer(typeof(GridTile.DirTilePair))]
public class DirTilePairPropertyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        float halfWidth = position.width / 2;
        var tileRect = new Rect(position.x, position.y, halfWidth - 10, position.height);
        var atRect = new Rect(position.x + halfWidth - 5, position.y, 15, position.height);
        var dirRect = new Rect(position.x + halfWidth + 10, position.y, halfWidth - 10, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(tileRect, property.FindPropertyRelative("tile"), GUIContent.none);
        EditorGUI.LabelField(atRect, "@");
        EditorGUI.PropertyField(dirRect, property.FindPropertyRelative("dir"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}