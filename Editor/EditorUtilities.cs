using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SAS.StateMachineGraph.Editor
{
    public static class EditorUtilities
    {
        public static void VerticalLine(Rect rect, float width, Color color)
        {
            var style = new GUIStyle();
            style.normal.background = EditorGUIUtility.whiteTexture;
            style.fixedWidth = width;

            var c = GUI.color;
            GUI.color = color;
            GUI.Box(rect, GUIContent.none, style);
            GUI.color = c;
        }

        public static void HorizontalLine(Rect rect, float width, Color color)
        {
            var style = new GUIStyle();
            style.normal.background = EditorGUIUtility.whiteTexture;
            style.fixedHeight = width;

            var c = GUI.color;
            GUI.color = color;
            GUI.Box(rect, GUIContent.none, style);
            GUI.color = c;
        }

        public static void DrawArrowLine(Vector3 start, Vector3 end)
        {
            var arrowHead = new Vector3[3];
            var arrowLine = new Vector3[2];

            var right = (end - start).normalized;
            var up = Vector3.Cross(Vector3.forward, right).normalized;
            var size = HandleUtility.GetHandleSize(end);
            var width = size * 0.3f;
            var height = size * 0.6f;
            var mid = start + (end - start).normalized * Vector2.Distance(start, end) * 0.5f;
            arrowHead[0] = mid;
            arrowHead[1] = mid - right * height + up * width;
            arrowHead[2] = mid - right * height - up * width;

            arrowLine[0] = start;
            arrowLine[1] = end - right * height;

            Handles.color = Color.grey;
            Handles.DrawAAPolyLine(5, arrowLine);
            Handles.color = Color.white;
            Handles.DrawAAConvexPolygon(arrowHead);
        }
    }
}
