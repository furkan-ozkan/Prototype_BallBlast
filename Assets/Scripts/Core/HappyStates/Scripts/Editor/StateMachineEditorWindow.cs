using HappyStates.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace HappyStates.Scripts.Editor
{
    using UnityEditor;
using UnityEngine;
using HappyStates.Scripts.Core;

public class StateMachineEditorWindow : EditorWindow
{
    private StateMachine _selectedMachine;
    private StateHistoryLog? _selectedLog; // Seçilen log detayını tutar
    private Vector2 _scrollLeft, _scrollMid;

    [MenuItem("Window/HappyStates/SM Dashboard")]
    public static void ShowWindow() => GetWindow<StateMachineEditorWindow>("SM Dashboard");

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // --- SOL: MAKİNE LİSTESİ ---
        DrawSidebar();

        // --- SAĞ: ANA DASHBOARD ---
        EditorGUILayout.BeginVertical();
        if (_selectedMachine != null)
        {
            DrawHeader();
            DrawLiveMonitor();
            DrawHistoryList();
            DrawDetailPanel(); // Yeni detay paneli
        }
        else DrawEmptyState();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        if (Application.isPlaying) Repaint();
    }

    private void DrawSidebar()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(180), GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("MACHINES", EditorStyles.miniBoldLabel);
        _scrollLeft = EditorGUILayout.BeginScrollView(_scrollLeft);
        foreach (var sm in StateMachine.AllMachines)
        {
            if (GUILayout.Toggle(_selectedMachine == sm, sm.gameObject.name, "Button", GUILayout.Height(24)))
                _selectedMachine = sm;
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label(_selectedMachine.name.ToUpper(), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("FOCUS IN SCENE", EditorStyles.toolbarButton))
            {
                Selection.activeGameObject = _selectedMachine.gameObject;
                SceneView.FrameLastActiveSceneView();
            }
        }
    }

    private void DrawLiveMonitor()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        var state = _selectedMachine.CurrentState;
        string stateName = state != null ? state.GetType().Name : "NULL";
        
        GUILayout.Label("LIVE MONITOR", EditorStyles.miniLabel);
        EditorGUILayout.LabelField(stateName, new GUIStyle(EditorStyles.boldLabel) { fontSize = 18 });
        
        if (state is BaseState b)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(4));
            EditorGUI.ProgressBar(r, (b.TimeInState % 5) / 5f, "");
            GUILayout.Label($"Active for {b.TimeInState:F2}s", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawHistoryList()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("TRANSITION HISTORY (Select to view details)", EditorStyles.miniLabel);
        
        _scrollMid = EditorGUILayout.BeginScrollView(_scrollMid, "box", GUILayout.Height(200));
        for (int i = _selectedMachine.History.Count - 1; i >= 0; i--)
        {
            var log = _selectedMachine.History[i];
            bool isThisSelected = _selectedLog.HasValue && _selectedLog.Value.Equals(log);

            // Satırı buton gibi yapalım
            GUI.backgroundColor = isThisSelected ? new Color(0.3f, 0.5f, 1f) : Color.white;
            if (GUILayout.Button($"[{log.Duration}s] {log.FromState} -> {log.ToState}", "MenuItem"))
            {
                _selectedLog = log;
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndScrollView();
    }

    private void DrawDetailPanel()
    {
        if (!_selectedLog.HasValue) return;
        var log = _selectedLog.Value;

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical("helpBox");
        GUILayout.Label("TRANSITION DETAILS", EditorStyles.miniBoldLabel);
        EditorGUILayout.Space(5);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Logic Rule:", EditorStyles.miniLabel);
            EditorGUILayout.TextArea(log.Logic, EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("OPEN IN IDE", GUILayout.Width(100), GUILayout.Height(30)))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(log.FilePath, log.LineNumber);
            }
        }

        EditorGUILayout.LabelField($"File: {System.IO.Path.GetFileName(log.FilePath)} : Line {log.LineNumber}", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
    }

    private void DrawEmptyState()
    {
        GUILayout.FlexibleSpace();
        GUILayout.Label("Select a machine from the left sidebar", EditorStyles.centeredGreyMiniLabel);
        GUILayout.FlexibleSpace();
    }
}
}