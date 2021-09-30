﻿/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(MediaPlayerImage), true)]
public class MediaPlayerImageEditor : ImageEditor
{
  SerializedProperty m_ButtonType;

  protected override void OnEnable()
  {
    base.OnEnable();

    m_ButtonType = serializedObject.FindProperty("m_ButtonType");
  }

  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    EditorGUILayout.PropertyField(m_ButtonType);
    serializedObject.ApplyModifiedProperties();
  }
}
