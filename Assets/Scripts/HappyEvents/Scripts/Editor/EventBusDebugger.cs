using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HappyEvents;

namespace HappyEvents.Editor
{
    public class EventBusDebugger : EditorWindow
    {
        private Vector2 _sidebarScroll, _mainScroll, _logScroll;
        private EventInstance _selectedBus;
        private EventLog? _selectedLog;
        private string _searchQuery = "";
        private bool _autoScroll = true;

        // Stil Tanımlamaları
        private GUIStyle _headerStyle, _cardStyle, _logStyle, _sidebarItemStyle;
        private Color _accentBlue = new Color(0.2f, 0.6f, 1f);
        private Color _bgSelected = new Color(0.25f, 0.35f, 0.5f); // Seçili arka plan rengi

        [MenuItem("Tools/Happy Events/Event Bus Monitor Pro")]
        public static void ShowWindow() => GetWindow<EventBusDebugger>("Event Monitor Pro");

        private void OnGUI()
        {
            InitStyles();
            DrawTopToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawSidebar();
            // Sidebar ile ana içerik arasına ince bir çizgi
            DrawVerticalLine(new Color(0.1f, 0.1f, 0.1f), 1);
            DrawMainContent();
            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying) Repaint();
        }

        private void InitStyles()
        {
            // Stiller null ise veya yeniden oluşturulması gerekiyorsa oluştur
            if (_headerStyle == null) _headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, alignment = TextAnchor.MiddleLeft };
            if (_cardStyle == null) _cardStyle = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) };
            if (_logStyle == null) _logStyle = new GUIStyle(EditorStyles.label) { fontSize = 11, richText = true };
        }

        private void DrawTopToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(30));
            GUILayout.Space(5);
            GUILayout.Label("✨ Event Bus Monitor Pro V2", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            _searchQuery = EditorGUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField, GUILayout.Width(250));
            if (GUILayout.Button("✕", EditorStyles.toolbarButton, GUILayout.Width(25))) { _searchQuery = ""; GUI.FocusControl(null); }

            GUILayout.Space(10);
            if (GUILayout.Button("🗑 Clear History", EditorStyles.toolbarButton)) _selectedBus?.ClearHistory();
            _autoScroll = GUILayout.Toggle(_autoScroll, "⚓ Auto Scroll", EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal();
        }

        // --- FIX 1: SIDEBAR ÇİZİMİ GÜNCELLENDİ ---
        private void DrawSidebar()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(220), GUILayout.ExpandHeight(true));
            // Arka planı hafif koyulaştır
            EditorGUI.DrawRect(new Rect(0, 30, 220, position.height), new Color(0.12f, 0.12f, 0.12f));

            _sidebarScroll = EditorGUILayout.BeginScrollView(_sidebarScroll);
            GUILayout.Space(5);

            var buses = EventBusRegistry.GetAllBuses();
            if (buses.Count == 0)
            {
                EditorGUILayout.HelpBox("No active bus found in runtime.", MessageType.Info);
            }

            foreach (var bus in buses)
            {
                bool isSelected = _selectedBus == bus;

                // 1. Alanı Rezerve Et (Sabit yükseklik 50px)
                Rect itemRect = EditorGUILayout.GetControlRect(false, 50);

                // 2. Seçiliyse Arka Planı Boya (Daha temiz yöntem)
                if (isSelected)
                {
                    EditorGUI.DrawRect(itemRect, _bgSelected);
                }

                // 3. Tıklama Algılama (Görünmez buton)
                if (GUI.Button(itemRect, GUIContent.none, GUIStyle.none))
                {
                    _selectedBus = bus;
                    _selectedLog = null; // Bus değişirse log seçimini sıfırla
                    GUI.FocusControl(null);
                }

                // 4. İçerik Etiketlerini Çiz (Rect'in içine manuel yerleştirme)
                Rect labelRect = new Rect(itemRect.x + 10, itemRect.y + 5, itemRect.width - 20, 20);
                GUI.Label(labelRect, bus.Name.ToUpper(), isSelected ? EditorStyles.whiteBoldLabel : EditorStyles.boldLabel);

                Rect subRect = new Rect(itemRect.x + 10, itemRect.y + 25, itemRect.width - 20, 20);
                GUI.Label(subRect, $"{bus.GetSubscribers().Count} Listeners • {bus.History.Count} Events", EditorStyles.miniLabel);

                GUILayout.Space(2); // Öğeler arası minik boşluk
                DrawLine(new Color(0.2f, 0.2f, 0.2f), 1); // Ayırıcı çizgi
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawMainContent()
        {
            if (_selectedBus == null)
            {
                DrawEmptyState();
                return;
            }

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            DrawBusDashboard();
            DrawLogList();
            if (_selectedLog.HasValue) DrawLogDetails();
            EditorGUILayout.EndVertical();
        }

        private void DrawEmptyState()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            GUILayout.Label(EditorGUIUtility.IconContent("d_console.infoicon"), GUILayout.Width(40), GUILayout.Height(40));
            GUILayout.Label("Select an Event Bus from the sidebar\nto inspect live traffic.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        private void DrawBusDashboard()
        {
            EditorGUILayout.BeginVertical(_cardStyle);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            var totalEvents = _selectedBus.History.Count;
            var avgTime = totalEvents > 0 ? _selectedBus.History.Average(x => x.ExecutionTime) : 0;

            DrawStatCard("Total Events", totalEvents.ToString(), _accentBlue);
            DrawStatCard("Avg Latency", $"{avgTime:F3}ms", GetPerfColor(avgTime));
            DrawStatCard("Subscribers", _selectedBus.GetSubscribers().Count.ToString(), Color.white);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawLogList()
        {
            GUILayout.Space(10);
            GUILayout.Label(" LIVE TRAFFIC", _headerStyle);
            
            // Log alanı arka planı
            var bgRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(bgRect, new Color(0.14f, 0.14f, 0.14f));

            _logScroll = EditorGUILayout.BeginScrollView(_logScroll);

            var logs = _selectedBus.History;
            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                if (!string.IsNullOrEmpty(_searchQuery) && !log.EventName.ToLower().Contains(_searchQuery.ToLower())) continue;

                bool isSelected = _selectedLog.HasValue && _selectedLog.Value.Equals(log);

                Rect logRect = EditorGUILayout.BeginHorizontal(isSelected ? "selectionRect" : GUIStyle.none, GUILayout.Height(28));
                
                // Tıklama
                if (GUI.Button(logRect, GUIContent.none, GUIStyle.none))
                {
                    _selectedLog = log;
                    GUI.FocusControl(null);
                }

                // Zaman ve İsim
                GUILayout.Space(5);
                GUILayout.Label($"<color=#888888>[{log.Timestamp}]</color>", _logStyle, GUILayout.Width(65));
                GUILayout.Label($"<b>{log.EventName}</b>", _logStyle, GUILayout.Width(200));

                // Performans Barı
                GUILayout.FlexibleSpace();
                Rect barRect = GUILayoutUtility.GetRect(80, 8);
                barRect.y += 10; // Ortala
                DrawPerformanceBar(barRect, log.ExecutionTime);

                GUILayout.Space(10);
                GUILayout.Label($"{log.ExecutionTime:F3}ms", EditorStyles.miniLabel, GUILayout.Width(60));
                GUILayout.Space(5);

                EditorGUILayout.EndHorizontal();
                DrawLine(new Color(0.2f, 0.2f, 0.2f, 0.5f), 1);
            }

            if (_autoScroll && Event.current.type == EventType.Layout) _logScroll.y = float.MaxValue;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        // --- FIX 2: KAPATMA HATASI GİDERİLDİ ---
        private void DrawLogDetails()
        {
            // Ekstra güvenlik kontrolü
            if (!_selectedLog.HasValue) return;

            GUILayout.Space(10);
            // Paneli sarmalayan kutu
            EditorGUILayout.BeginVertical(_cardStyle, GUILayout.Height(200));
            
            // Başlık ve Kapat Butonu
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("EVENT DETAILS", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            // KRİTİK DÜZELTME: Kapat butonuna basılınca çizimi anında durdur.
            if (GUILayout.Button(new GUIContent("✕", "Close Details"), EditorStyles.label, GUILayout.Width(20)))
            {
                _selectedLog = null;
                // GUI çizim döngüsünden acil çıkış yap.
                // Bu olmazsa, Unity olmayan bir değişkeni çizmeye çalışıp hata verir.
                GUIUtility.ExitGUI(); 
                return; 
            }
            EditorGUILayout.EndHorizontal();
            DrawLine(_accentBlue, 2);
            GUILayout.Space(5);

            // İçerik (Payload ve Stack Trace)
            EditorGUILayout.BeginHorizontal();
            
            // Sol taraf: Payload
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.35f));
            GUILayout.Label("Payload (JSON)", EditorStyles.miniBoldLabel);
            string json = JsonUtility.ToJson(_selectedLog.Value.Payload, true);
            EditorGUILayout.SelectableLabel(json, EditorStyles.textArea, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Sağ taraf: Stack Trace
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Stack Trace (Source)", EditorStyles.miniBoldLabel);
            
            // Stack Trace'i daha okunaklı hale getiren özel stil
            GUIStyle stackStyle = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true, richText = true };
            // Sadece ilk 3 satırı alıp kalabalığı azaltalım, tamamı zaten selectable.
            string simplifiedStack = _selectedLog.Value.StackTrace.Split('\n').Take(3).Aggregate((a, b) => a + "\n" + b);
            simplifiedStack += "\n...";
            
            EditorGUILayout.SelectableLabel(simplifiedStack, stackStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        
        

        // --- Helper Çizim Metodları ---

        private void DrawStatCard(string label, string value, Color valColor)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            GUILayout.Label(label.ToUpper(), EditorStyles.miniLabel);
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, normal = { textColor = valColor } };
            GUILayout.Label(value, style);
            EditorGUILayout.EndVertical();
        }

        private void DrawPerformanceBar(Rect rect, double time)
        {
            // Barın arka planı
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            // Barın doluluk oranı (max 2ms kabul edelim)
            float fillRatio = Mathf.Clamp01((float)time / 2.0f);
            Rect fillRect = new Rect(rect.x, rect.y, rect.width * fillRatio, rect.height);
            EditorGUI.DrawRect(fillRect, GetPerfColor(time));
        }

        private Color GetPerfColor(double time)
        {
            if (time < 0.5) return new Color(0.3f, 0.8f, 0.3f); // Yeşil
            if (time < 1.5) return new Color(1f, 0.8f, 0.2f);   // Sarı
            return new Color(1f, 0.3f, 0.3f);                   // Kırmızı
        }

        private void DrawLine(Color color, int thickness)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            rect.height = thickness;
            EditorGUI.DrawRect(rect, color);
        }
        
        private void DrawVerticalLine(Color color, int thickness)
        {
            Rect rect = EditorGUILayout.BeginVertical(GUILayout.Width(thickness), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(rect, color);
            EditorGUILayout.EndVertical();
        }
    }
}