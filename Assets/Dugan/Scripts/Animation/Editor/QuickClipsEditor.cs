using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This class just makes the private fields in the quick clips class accessible in the editor, but still leaves them private so the API doesn't have them exposed to the user.
namespace Dugan.Animation.Editor {
	[CustomEditor(typeof(Dugan.Animation.QuickClips), true)]
	[CanEditMultipleObjects]
	public class QuickClipsEditor : UnityEditor.Editor {

		SerializedProperty _clip;
		SerializedProperty animations;
		SerializedProperty playAutomatically;
		SerializedProperty _animatePhysics;
		SerializedProperty cullingMode;
		

		private void OnEnable() {
			_clip = serializedObject.FindProperty("_clip");
			animations = serializedObject.FindProperty("animations");
			playAutomatically = serializedObject.FindProperty("playAutomatically");
			_animatePhysics = serializedObject.FindProperty("_animatePhysics");
			cullingMode = serializedObject.FindProperty("cullingMode");
		}

		public override void OnInspectorGUI() {
			//Completely overriding the base gui here.
			//base.OnInspectorGUI();
			EditorGUILayout.PropertyField(_clip, new GUIContent("Animation"));
			EditorGUILayout.PropertyField(animations, new GUIContent("Animations"));
			EditorGUILayout.PropertyField(playAutomatically, new GUIContent("Play Automatically"));
			EditorGUILayout.PropertyField(_animatePhysics, new GUIContent("Animate Physics"));
			EditorGUILayout.PropertyField(cullingMode, new GUIContent("Culling Type"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}
