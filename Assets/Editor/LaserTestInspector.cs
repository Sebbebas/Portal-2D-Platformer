using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Laser))]
public class LaserEditor : Editor
{
    SerializedProperty lineRendererProperty;
    SerializedProperty laserStartPointProperty;
    SerializedProperty maxLineLengthProperty;
    SerializedProperty laserIncreaseProperty;
    SerializedProperty laserModeProperty;
    SerializedProperty startVFXProperty;
    SerializedProperty endVFXProperty;

    SerializedProperty switchTimeProperty;
    SerializedProperty angleAProperty;
    SerializedProperty angleBProperty;

    SerializedProperty pointAProperty;
    SerializedProperty pointBProperty;
    SerializedProperty timeFromAToBProperty;
    SerializedProperty timeToPauseOnPointProperty;

    void OnEnable()
    {
        // Universal Properties
        lineRendererProperty = serializedObject.FindProperty("lineRenderer");
        laserStartPointProperty = serializedObject.FindProperty("laserStartPoint");
        maxLineLengthProperty = serializedObject.FindProperty("maxLineLength");
        laserIncreaseProperty = serializedObject.FindProperty("laserIncrease");
        startVFXProperty = serializedObject.FindProperty("startVFX");
        endVFXProperty = serializedObject.FindProperty("endVFX");

        // LaserMode Specific Properties
        laserModeProperty = serializedObject.FindProperty("laserMode");

        switchTimeProperty = serializedObject.FindProperty("switchTime");
        angleAProperty = serializedObject.FindProperty("angleA");
        angleBProperty = serializedObject.FindProperty("angleB");

        pointAProperty = serializedObject.FindProperty("pointA");
        pointBProperty = serializedObject.FindProperty("pointB");
        timeFromAToBProperty = serializedObject.FindProperty("timeFromAToB");
        timeToPauseOnPointProperty = serializedObject.FindProperty("timeToPauseOnPoint");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Universal properties
        EditorGUILayout.PropertyField(lineRendererProperty);
        EditorGUILayout.PropertyField(laserStartPointProperty);
        EditorGUILayout.PropertyField(maxLineLengthProperty);
        EditorGUILayout.PropertyField(laserIncreaseProperty);
        EditorGUILayout.PropertyField(startVFXProperty);
        EditorGUILayout.PropertyField(endVFXProperty);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(laserModeProperty);

        Laser.LaserMode selectedMode = (Laser.LaserMode)laserModeProperty.enumValueIndex;

        switch (selectedMode)
        {
            case Laser.LaserMode.Rotate:
                EditorGUILayout.PropertyField(switchTimeProperty);
                EditorGUILayout.PropertyField(angleAProperty);
                EditorGUILayout.PropertyField(angleBProperty);
                break;
            case Laser.LaserMode.Moving:
                EditorGUILayout.PropertyField(pointAProperty);
                EditorGUILayout.PropertyField(pointBProperty);
                EditorGUILayout.PropertyField(timeFromAToBProperty);
                EditorGUILayout.PropertyField(timeToPauseOnPointProperty);
                break;
            // Add cases for other modes if needed
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
