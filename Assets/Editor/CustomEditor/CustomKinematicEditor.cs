using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Playground
{
    [CustomEditor(typeof(CustomKinematic))]
    public class CustomKinematicEditor : Editor
    {
        static readonly Vector3 Offset = (Vector3.left + Vector3.down) * 0.5f;

        Rigidbody2D rigid;
        CustomKinematic kinematic;
        Collider2D collider;

        void OnEnable()
        {
            Initialize();
        }

        void Initialize()
        {
            kinematic = target as CustomKinematic;
            collider = kinematic.gameObject.GetComponent<Collider2D>();
            rigid = kinematic.gameObject.GetComponent<Rigidbody2D>();
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        static void DrawGizmosSelected(CustomKinematic kinematic, GizmoType gizmoType)
        {
            Handles.color = Color.white;
            Handles.Label(kinematic.transform.position + Offset, "Grounded : " + kinematic.Grounded);
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();
        }

        void DrawInspector()
        {
            DrawDefaultInspector();

            bool shouldResetRigidbody2D = (rigid.bodyType != RigidbodyType2D.Kinematic) ||
                    (rigid.simulated == false) || 
                    (rigid.useFullKinematicContacts == false) || 
                    (rigid.freezeRotation == false);

            if (shouldResetRigidbody2D)
            {
                EditorGUILayout.HelpBox("Component 'Rigidbody2D' must be 'Kinematic'", MessageType.Error);
            }

            if (collider)
            {
                if (!collider.enabled)
                {
                    EditorGUILayout.HelpBox("Component 'Collider2D' need to set enabled 'True'", MessageType.Warning);
                }

                if (collider.isTrigger)
                {
                    EditorGUILayout.HelpBox("Component 'Collider2D' couldn't be 'Trigger'", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Component 'Collider2D' is require to collide with another collider.", MessageType.Error);

                EditorGUILayout.PrefixLabel("Select Collider2D:");
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, typeof(BoxCollider2D)).image))
                {
                    kinematic.gameObject.AddComponent<BoxCollider2D>();
                }
                else if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, typeof(CircleCollider2D)).image))
                {
                    kinematic.gameObject.AddComponent<CircleCollider2D>();
                }
                else if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, typeof(CapsuleCollider2D)).image))
                {
                    kinematic.gameObject.AddComponent<CapsuleCollider2D>();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (shouldResetRigidbody2D && GUILayout.Button("Reset Rigidbody2D"))
            {
                ResetRigidbody2D();
            }
        }

        [ContextMenu("Reset Rigidbody2D")]
        public void ResetRigidbody2D()
        {
            if (rigid == null)
            {
                string message = "CustomKinematic doesn't have a Rigidbody2D...\nWant to add one?";
                bool isConfirmAddComponent = EditorUtility.DisplayDialog("Error", message, "Add a new Rigidbody2D", "No");

                if (!isConfirmAddComponent)
                    return;

                kinematic.gameObject.AddComponent<Rigidbody2D>();
                rigid = kinematic.gameObject.GetComponent<Rigidbody2D>();
            }

            Undo.RecordObject(rigid, "Reset Rigidbody2D");

            rigid.bodyType = RigidbodyType2D.Kinematic;
            rigid.simulated = true;
            rigid.useFullKinematicContacts = true;
            rigid.freezeRotation = true;
        }
    }
}

