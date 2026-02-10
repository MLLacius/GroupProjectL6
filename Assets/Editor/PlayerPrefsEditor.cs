using UnityEngine;
using UnityEditor;

//This script has been taken and tweaked from a past project of Luke's and Leyton's :)

public class PlayerPrefsEditor : EditorWindow
{
    // Variable to hold the value while we edit it in the window
    private int editedHigh;
    private int editedCollectible;
    private int editedLastScore;
    private float editedVolume;
    private string editedResolution;
    private int editedResolutionX;
    private int editedResolutionY;

    private Vector2 scrollPos;

    private const string highScoreKeyName = "HighScore";
    private const string collectibleKeyName = "Collectibles";
    private const string lastScoreKeyName = "Last Score";
    private const string volumeKeyName = "Volume";
    private const string resolutionKeyName = "Resolution";
    private const string resolutionXKeyName = "ResolutionX";
    private const string resolutionYKeyName = "ResolutionY";

    [MenuItem("Tools/Player Preferences")]
    public static void OpenWindow()
    {
        PlayerPrefsEditor window = (PlayerPrefsEditor)EditorWindow.GetWindow(typeof(PlayerPrefsEditor));
        window.titleContent = new GUIContent("Player Preferences");
        window.Show();
    }

    //Load the current value when the window opens
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(highScoreKeyName))
        {
            editedHigh = PlayerPrefs.GetInt(highScoreKeyName);
        }
        if (PlayerPrefs.HasKey(collectibleKeyName))
        {
            editedCollectible = PlayerPrefs.GetInt(collectibleKeyName);
        }
        if (PlayerPrefs.HasKey(lastScoreKeyName))
        {
            editedLastScore = PlayerPrefs.GetInt(lastScoreKeyName);
        }
        if (PlayerPrefs.HasKey(volumeKeyName))
        {
            editedVolume = PlayerPrefs.GetFloat(volumeKeyName);
        }
        if(PlayerPrefs.HasKey(resolutionKeyName))
        {
            editedResolution = PlayerPrefs.GetString(resolutionKeyName);
        }
        if(PlayerPrefs.HasKey(resolutionXKeyName))
        {
            editedResolutionX = PlayerPrefs.GetInt(resolutionXKeyName);
        }
        if(PlayerPrefs.HasKey(resolutionYKeyName))
        {
            editedResolutionY = PlayerPrefs.GetInt(resolutionYKeyName);
        }
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        //Last score editor
        GUILayout.Label("Last Score Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current Last Score:", PlayerPrefs.GetInt(lastScoreKeyName, 0).ToString());
        EditorGUILayout.Space();
        editedLastScore = EditorGUILayout.IntField("New Previous Score:", editedLastScore);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save Last Score"))
        {
            PlayerPrefs.SetInt(lastScoreKeyName, editedLastScore);
            PlayerPrefs.Save();
            Debug.Log($"Last Score updated to: {lastScoreKeyName}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Last Score"))
        {
            PlayerPrefs.DeleteKey(lastScoreKeyName);
            editedHigh = 0;
            Debug.Log("Last Score key deleted.");
        }

        //High Score Editor
        GUILayout.Label("High Score Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current High Score:", PlayerPrefs.GetInt(highScoreKeyName, 0).ToString());
        EditorGUILayout.Space();
        editedHigh = EditorGUILayout.IntField("New High Score:", editedHigh);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save High Score"))
        {
            PlayerPrefs.SetInt(highScoreKeyName, editedHigh);
            PlayerPrefs.Save(); 
            Debug.Log($"High Score updated to: {editedHigh}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Highscore"))
        {
            PlayerPrefs.DeleteKey(highScoreKeyName);
            editedHigh = 0;
            Debug.Log("High Score key deleted.");
        }

        //Collectibles Editor
        GUILayout.Label("Collectibles Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current Collectibles Value:", PlayerPrefs.GetInt(collectibleKeyName, 0).ToString());
        EditorGUILayout.Space();  
        editedCollectible = EditorGUILayout.IntField("New Collectibles Value:", editedCollectible);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save Collectibles"))
        {
            PlayerPrefs.SetInt(collectibleKeyName, editedCollectible);
            PlayerPrefs.Save(); 
            Debug.Log($"Collectible count updated to: {editedCollectible}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Collectibles"))
        {
            PlayerPrefs.DeleteKey(collectibleKeyName);
            editedCollectible = 0;
            Debug.Log("Collectibles key deleted.");
        }

        //Global Volume Editor
        GUILayout.Label("Global Volume Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current Global Volume Value:", PlayerPrefs.GetFloat(volumeKeyName, 0).ToString());
        EditorGUILayout.Space();
        editedVolume = EditorGUILayout.FloatField("New Global Volume Value:", editedVolume);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save Global Volume"))
        {
            PlayerPrefs.SetFloat(volumeKeyName, editedVolume);
            PlayerPrefs.Save();
            Debug.Log($"Global volume count updated to: {editedVolume}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Global Volume"))
        {
            PlayerPrefs.DeleteKey(volumeKeyName);
            editedVolume = 50;
            Debug.Log("Global volume key deleted.");
        }

        //Resolution Text Editor
        GUILayout.Label("Resolution Text Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current Resolution Text Value:", PlayerPrefs.GetString(resolutionKeyName));
        EditorGUILayout.Space();
        editedResolution = EditorGUILayout.TextField("New Resolution Text Value:", editedResolution);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save Resolution Text"))
        {
            PlayerPrefs.SetString(resolutionKeyName, editedResolution);
            PlayerPrefs.Save();
            Debug.Log($"Resolution text updated to: {editedResolution}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Resolution Text"))
        {
            PlayerPrefs.DeleteKey(resolutionKeyName);
            editedResolution = "";
            Debug.Log("Resolution text deleted.");
        }

        //Resolution X Editor
        GUILayout.Label("Resolution X Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current Resolution X Value:", PlayerPrefs.GetInt(resolutionXKeyName, 0).ToString());
        EditorGUILayout.Space();
        editedResolutionX = EditorGUILayout.IntField("New Resolution X Value:", editedResolutionX);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save Resolution X"))
        {
            PlayerPrefs.SetInt(resolutionXKeyName, editedResolutionX);
            PlayerPrefs.Save();
            Debug.Log($"Resolution X updated to: {editedResolutionX}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Resolution X"))
        {
            PlayerPrefs.DeleteKey(resolutionXKeyName);
            editedResolutionX = 0;
            Debug.Log("Resolution X key deleted.");
        }

        //Resolution Y Editor
        GUILayout.Label("Resolution Y Settings", EditorStyles.boldLabel);
        // Display the current saved value for reference
        EditorGUILayout.LabelField("Current Resolution Y Value:", PlayerPrefs.GetInt(resolutionYKeyName, 0).ToString());
        EditorGUILayout.Space();
        editedResolutionY = EditorGUILayout.IntField("New Resolution Y Value:", editedResolutionY);
        EditorGUILayout.Space();
        //Save Button
        if (GUILayout.Button("Save Resolution Y"))
        {
            PlayerPrefs.SetInt(resolutionYKeyName, editedResolutionY);
            PlayerPrefs.Save();
            Debug.Log($"Resolution Y updated to: {editedResolutionY}");
        }
        //Reset Button
        if (GUILayout.Button("Reset Resolution Y"))
        {
            PlayerPrefs.DeleteKey(resolutionYKeyName);
            editedResolutionY = 0;
            Debug.Log("Resolution Y key deleted.");
        }

        EditorGUILayout.EndScrollView();
    }
}