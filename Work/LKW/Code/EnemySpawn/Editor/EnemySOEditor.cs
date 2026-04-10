using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chipmunk.Modules.StatSystem;
using Code.SHS.Utility.DynamicFieldBinding;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.EnemySpawn.Editor
{
    [CustomEditor(typeof(EnemySO))]
    [CanEditMultipleObjects]
    public class EnemySOEditor : UnityEditor.Editor
    {
        private static readonly MethodInfo LoadStatsFromPrefabMethod =
            typeof(EnemySO).GetMethod("LoadStatsFromPrefab", BindingFlags.Instance | BindingFlags.NonPublic);

        private SerializedProperty enemyPrefabProperty;
        private SerializedProperty spawnRarityWeightProperty;
        private SerializedProperty equipmentsProperty;
        private SerializedProperty bulletDataProperty;
        private SerializedProperty statOverridesProperty;
        private SerializedProperty stateDatasProperty;
        private SerializedProperty behaviourPrefabsProperty;
        private SerializedProperty passiveSkillProperty;
        private SerializedProperty activeSkillProperty;

        private ReorderableList equipmentsList;
        private ReorderableList statOverridesList;
        private ReorderableList stateDatasList;

        private bool showCore = true;
        private bool showEquipment = true;
        private bool showStats = true;
        private bool showStates = false;
        private bool showBehaviours = true;
        private bool showSkills = true;

        private string actionMessage;
        private MessageType actionMessageType = MessageType.None;

        private void OnEnable()
        {
            enemyPrefabProperty = serializedObject.FindProperty("enemyPrefab");
            spawnRarityWeightProperty = serializedObject.FindProperty("spawnRarityWeight");
            equipmentsProperty = serializedObject.FindProperty("equipments");
            bulletDataProperty = serializedObject.FindProperty("bulletData");
            statOverridesProperty = serializedObject.FindProperty("statOverrides");
            stateDatasProperty = serializedObject.FindProperty("stateDatas");
            behaviourPrefabsProperty = serializedObject.FindProperty("behaviourPrefabs");
            passiveSkillProperty = serializedObject.FindProperty("passiveSkill");
            activeSkillProperty = serializedObject.FindProperty("activeSkill");

            CreateEquipmentsList();
            CreateStatOverridesList();
            CreateStateDatasList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EnemySO enemy = target as EnemySO;

            DrawScriptField(enemy);
            EditorGUILayout.Space(4f);
            DrawHeroCard(enemy);
            EditorGUILayout.Space(6f);
            DrawQuickActions();
            DrawActionMessage();
            DrawValidationSummary(enemy);
            EditorGUILayout.Space(4f);

            DrawSection("Core", GetCoreSummary(enemy), new Color(0.24f, 0.56f, 0.92f), ref showCore, DrawCoreSection);
            DrawSection("Equipment", GetEquipmentSummary(), new Color(0.87f, 0.58f, 0.22f), ref showEquipment, DrawEquipmentSection);
            DrawSection("Stats", GetStatsSummary(), new Color(0.24f, 0.72f, 0.46f), ref showStats, DrawStatsSection);
            DrawSection("States", GetStatesSummary(), new Color(0.18f, 0.68f, 0.78f), ref showStates, DrawStatesSection);
            DrawSection("Behaviours", GetBehavioursSummary(), new Color(0.90f, 0.36f, 0.32f), ref showBehaviours, DrawBehavioursSection);
            DrawSection("Skills", GetSkillsSummary(enemy), new Color(0.92f, 0.74f, 0.24f), ref showSkills, () => DrawSkillsSection(enemy));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptField(EnemySO enemy)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                MonoScript script = enemy != null ? MonoScript.FromScriptableObject(enemy) : null;
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }

        private void DrawHeroCard(EnemySO enemy)
        {
            using (new EditorGUILayout.VerticalScope(Styles.HeroCard))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    Rect previewRect = GUILayoutUtility.GetRect(50f, 50f, GUILayout.Width(50f), GUILayout.Height(50f));
                    DrawPreview(previewRect, enemy);

                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Space(2f);
                        GUILayout.Label(enemy != null ? enemy.name : "EnemySO", Styles.HeroTitle);
                        GUILayout.Label(GetHeroSubtitle(enemy), Styles.HeroSubtitle);
                        GUILayout.Label(GetHeroStatusLine(enemy), Styles.HeroStatus);
                    }

                    GUILayout.FlexibleSpace();

                    using (new EditorGUILayout.VerticalScope(GUILayout.Width(84f)))
                    {
                        GUILayout.Label("Rarity", Styles.SideLabel);
                        EditorGUILayout.PropertyField(spawnRarityWeightProperty, GUIContent.none);
                    }
                }

                EditorGUILayout.Space(6f);

                DrawMetricRow("Equip", equipmentsProperty.arraySize.ToString(), "Stats", statOverridesProperty.arraySize.ToString(), "States", stateDatasProperty.arraySize.ToString());
                DrawMetricRow("Behaviours", behaviourPrefabsProperty.arraySize.ToString(), "Passive", GetConfiguredPassiveCount(enemy).ToString(), "Active", GetConfiguredActiveCount(enemy).ToString());
            }
        }

        private void DrawQuickActions()
        {
            using (new EditorGUILayout.VerticalScope(Styles.ToolbarCard))
            {
                EditorGUILayout.LabelField("Quick Actions", Styles.ToolbarTitle);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Load Stats From Prefab", GUILayout.Height(24f)))
                    {
                        HandleLoadStatsFromPrefab();
                    }

                    if (GUILayout.Button("Sync All Patches", GUILayout.Height(24f)))
                    {
                        HandleSyncAllPatches();
                    }

                    if (GUILayout.Button("Generate Setters", GUILayout.Height(24f)))
                    {
                        HandleGenerateSetters();
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Expand All Sections", GUILayout.Height(22f)))
                    {
                        SetAllSectionsExpanded(true);
                    }

                    if (GUILayout.Button("Collapse All Sections", GUILayout.Height(22f)))
                    {
                        SetAllSectionsExpanded(false);
                    }
                }
            }
        }

        private void DrawActionMessage()
        {
            if (string.IsNullOrWhiteSpace(actionMessage))
            {
                return;
            }

            EditorGUILayout.HelpBox(actionMessage, actionMessageType);
        }

        private void DrawValidationSummary(EnemySO enemy)
        {
            List<string> warnings = CollectWarnings(enemy);
            if (warnings.Count == 0)
            {
                return;
            }

            string message = string.Join("\n", warnings.Select(warning => $"- {warning}"));
            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }

        private void DrawCoreSection()
        {
            EditorGUILayout.PropertyField(enemyPrefabProperty);
            EditorGUILayout.PropertyField(bulletDataProperty);
        }

        private void DrawEquipmentSection()
        {
            equipmentsList.DoLayoutList();
        }

        private void DrawStatsSection()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(
                    $"Enabled Overrides: {CountEnabledOverrides(statOverridesProperty)} / {statOverridesProperty.arraySize}",
                    Styles.ContextLabel);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Load From Prefab", GUILayout.Width(140f)))
                {
                    HandleLoadStatsFromPrefab();
                }
            }

            EditorGUILayout.Space(2f);
            statOverridesList.DoLayoutList();
        }

        private void DrawStatesSection()
        {
            stateDatasList.DoLayoutList();
        }

        private void DrawBehavioursSection()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(
                    $"Configured Targets: {CountConfiguredBehaviourTargets()} / {behaviourPrefabsProperty.arraySize}",
                    Styles.ContextLabel);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Sync", GUILayout.Width(72f)))
                {
                    HandleSyncAllPatches();
                }

                if (GUILayout.Button("Generate", GUILayout.Width(84f)))
                {
                    HandleGenerateSetters();
                }
            }

            EditorGUILayout.Space(2f);
            behaviourPrefabsProperty.isExpanded = true;
            EditorGUILayout.PropertyField(behaviourPrefabsProperty, includeChildren: true);
        }

        private void DrawSkillsSection(EnemySO enemy)
        {
            DrawSkillPanel("Passive Slots", $"{GetConfiguredPassiveCount(enemy)} configured", passiveSkillProperty);
            EditorGUILayout.Space(6f);
            DrawSkillPanel("Active Slots", $"{GetConfiguredActiveCount(enemy)} configured", activeSkillProperty);
        }

        private void DrawSkillPanel(string title, string summary, SerializedProperty property)
        {
            using (new EditorGUILayout.VerticalScope(Styles.SubSectionCard))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(title, Styles.SubSectionTitle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(summary, Styles.SubSectionSummary);
                }

                EditorGUILayout.Space(2f);
                property.isExpanded = true;
                EditorGUILayout.PropertyField(property, includeChildren: true);
            }
        }

        private void DrawSection(string title, string summary, Color accentColor, ref bool expanded, Action body)
        {
            using (new EditorGUILayout.VerticalScope(Styles.SectionCard))
            {
                Rect headerRect = EditorGUILayout.GetControlRect(false, 26f);
                DrawSectionHeader(headerRect, title, summary, accentColor, ref expanded);

                if (expanded)
                {
                    EditorGUILayout.Space(4f);
                    body?.Invoke();
                }
            }

            EditorGUILayout.Space(8f);
        }

        private static void DrawSectionHeader(Rect rect, string title, string summary, Color accentColor, ref bool expanded)
        {
            EditorGUI.DrawRect(rect, Styles.SectionHeaderBackground);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), accentColor);

            Rect foldoutRect = new Rect(rect.x + 10f, rect.y + 3f, rect.width - 120f, rect.height - 6f);
            Rect summaryRect = new Rect(rect.xMax - 112f, rect.y + 4f, 104f, rect.height - 8f);

            expanded = EditorGUI.Foldout(foldoutRect, expanded, title, true, Styles.SectionFoldout);
            EditorGUI.LabelField(summaryRect, summary, Styles.SectionSummary);
        }

        private void DrawMetricRow(
            string firstLabel,
            string firstValue,
            string secondLabel,
            string secondValue,
            string thirdLabel,
            string thirdValue)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawMetricChip(firstLabel, firstValue);
                DrawMetricChip(secondLabel, secondValue);
                DrawMetricChip(thirdLabel, thirdValue);
            }
        }

        private static void DrawMetricChip(string label, string value)
        {
            GUILayout.Label($"{label}  {value}", Styles.MetricChip, GUILayout.Height(20f));
        }

        private static void DrawPreview(Rect rect, EnemySO enemy)
        {
            Texture texture = null;
            if (enemy != null && enemy.enemyPrefab != null)
            {
                texture = AssetPreview.GetMiniThumbnail(enemy.enemyPrefab);
            }

            texture ??= AssetPreview.GetMiniThumbnail(enemy);

            EditorGUI.DrawRect(rect, Styles.PreviewBackground);
            if (texture != null)
            {
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUI.LabelField(rect, "?", Styles.CenteredPreviewFallback);
            }
        }

        private void CreateEquipmentsList()
        {
            equipmentsList = new ReorderableList(serializedObject, equipmentsProperty, true, true, true, true);
            equipmentsList.drawHeaderCallback = rect => DrawCollectionHeader(rect, "Equipments", $"{equipmentsProperty.arraySize} slots");
            equipmentsList.elementHeight = EditorGUIUtility.singleLineHeight + 8f;
            equipmentsList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = equipmentsProperty.GetArrayElementAtIndex(index);
                SerializedProperty typeProperty = element.FindPropertyRelative("type");
                SerializedProperty itemDataProperty = element.FindPropertyRelative("itemData");

                Rect row = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
                float leftWidth = Mathf.Min(120f, row.width * 0.34f);
                Rect leftRect = new Rect(row.x, row.y, leftWidth, row.height);
                Rect rightRect = new Rect(row.x + leftWidth + 6f, row.y, row.width - leftWidth - 6f, row.height);

                EditorGUI.PropertyField(leftRect, typeProperty, GUIContent.none);
                EditorGUI.PropertyField(rightRect, itemDataProperty, GUIContent.none);
            };
        }

        private void CreateStatOverridesList()
        {
            statOverridesList = new ReorderableList(serializedObject, statOverridesProperty, true, true, true, true);
            statOverridesList.drawHeaderCallback = rect =>
            {
                DrawCollectionHeader(rect, "Stat Overrides", $"{CountEnabledOverrides(statOverridesProperty)} active");
            };
            statOverridesList.elementHeightCallback = index =>
            {
                SerializedProperty element = statOverridesProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, GUIContent.none, true) + 6f;
            };
            statOverridesList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = statOverridesProperty.GetArrayElementAtIndex(index);
                rect.y += 2f;
                rect.height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };
        }

        private void CreateStateDatasList()
        {
            stateDatasList = new ReorderableList(serializedObject, stateDatasProperty, true, true, true, true);
            stateDatasList.drawHeaderCallback = rect => DrawCollectionHeader(rect, "State Datas", $"{stateDatasProperty.arraySize} entries");
            stateDatasList.elementHeight = EditorGUIUtility.singleLineHeight + 8f;
            stateDatasList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = stateDatasProperty.GetArrayElementAtIndex(index);
                Rect row = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(row, element, GUIContent.none);
            };
        }

        private static void DrawCollectionHeader(Rect rect, string title, string summary)
        {
            EditorGUI.LabelField(rect, title, Styles.CollectionTitle);

            Rect summaryRect = new Rect(rect.xMax - 120f, rect.y, 120f, rect.height);
            EditorGUI.LabelField(summaryRect, summary, Styles.CollectionSummary);
        }

        private void HandleLoadStatsFromPrefab()
        {
            ApplyPendingChanges();

            if (LoadStatsFromPrefabMethod == null)
            {
                SetActionMessage("Could not find LoadStatsFromPrefab().", MessageType.Error);
                return;
            }

            foreach (EnemySO enemy in targets.OfType<EnemySO>())
            {
                Undo.RecordObject(enemy, "Load Enemy Stats From Prefab");
                LoadStatsFromPrefabMethod.Invoke(enemy, null);
                EditorUtility.SetDirty(enemy);
            }

            serializedObject.UpdateIfRequiredOrScript();
            SetActionMessage("Reloaded stat overrides from the prefab source.", MessageType.Info);
        }

        private void HandleSyncAllPatches()
        {
            ApplyPendingChanges();

            int patchCount = 0;
            foreach (EnemySO enemy in targets.OfType<EnemySO>())
            {
                patchCount += ForEachPatch(enemy, patch =>
                {
                    patch.SyncInputs();
                    return true;
                });
                EditorUtility.SetDirty(enemy);
            }

            serializedObject.UpdateIfRequiredOrScript();
            SetActionMessage($"Synced {patchCount} patches.", MessageType.Info);
        }

        private void HandleGenerateSetters()
        {
            ApplyPendingChanges();

            int patchCount = 0;
            foreach (EnemySO enemy in targets.OfType<EnemySO>())
            {
                patchCount += ForEachPatch(enemy, patch =>
                {
                    patch.GenerateSetter();
                    return true;
                });
                EditorUtility.SetDirty(enemy);
            }

            serializedObject.UpdateIfRequiredOrScript();
            SetActionMessage($"Regenerated {patchCount} patch setters.", MessageType.Info);
        }

        private static int ForEachPatch(EnemySO enemy, Func<IFieldPatchRuntime, bool> callback)
        {
            if (enemy == null || callback == null)
            {
                return 0;
            }

            int count = 0;

            if (enemy.behaviourPrefabs != null)
            {
                for (int i = 0; i < enemy.behaviourPrefabs.Length; i++)
                {
                    IFieldPatchRuntime patch = enemy.behaviourPrefabs[i];
                    if (patch != null && callback(patch))
                    {
                        count++;
                    }
                }
            }

            if (enemy.passiveSkill != null)
            {
                foreach (IFieldPatchRuntime patch in enemy.passiveSkill.Values)
                {
                    if (patch != null && callback(patch))
                    {
                        count++;
                    }
                }
            }

            if (enemy.activeSkill != null)
            {
                foreach (IFieldPatchRuntime patch in enemy.activeSkill.Values)
                {
                    if (patch != null && callback(patch))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void ApplyPendingChanges()
        {
            if (!serializedObject.hasModifiedProperties)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetActionMessage(string message, MessageType messageType)
        {
            actionMessage = message;
            actionMessageType = messageType;
        }

        private void SetAllSectionsExpanded(bool expanded)
        {
            showCore = expanded;
            showEquipment = expanded;
            showStats = expanded;
            showStates = expanded;
            showBehaviours = expanded;
            showSkills = expanded;
            Repaint();
        }

        private string GetCoreSummary(EnemySO enemy)
        {
            string prefabName = enemy != null && enemy.enemyPrefab != null ? enemy.enemyPrefab.name : "No Prefab";
            string bulletName = bulletDataProperty.objectReferenceValue != null ? bulletDataProperty.objectReferenceValue.name : "No Bullet";
            return $"{prefabName} / {bulletName}";
        }

        private string GetEquipmentSummary()
        {
            int missingItems = CountMissingEquipmentItems();
            return missingItems > 0
                ? $"{equipmentsProperty.arraySize} slots, {missingItems} empty"
                : $"{equipmentsProperty.arraySize} slots";
        }

        private string GetStatsSummary()
        {
            List<string> duplicates = GetDuplicatedStatNames();
            return duplicates.Count > 0
                ? $"{statOverridesProperty.arraySize} entries, duplicates"
                : $"{CountEnabledOverrides(statOverridesProperty)} active";
        }

        private string GetStatesSummary()
        {
            int missingStates = CountMissingReferences(stateDatasProperty);
            return missingStates > 0
                ? $"{stateDatasProperty.arraySize} entries, {missingStates} missing"
                : $"{stateDatasProperty.arraySize} entries";
        }

        private string GetBehavioursSummary()
        {
            int configuredTargets = CountConfiguredBehaviourTargets();
            return $"{configuredTargets} / {behaviourPrefabsProperty.arraySize} configured";
        }

        private string GetSkillsSummary(EnemySO enemy)
        {
            return $"Passive {GetConfiguredPassiveCount(enemy)} / Active {GetConfiguredActiveCount(enemy)}";
        }

        private string GetHeroSubtitle(EnemySO enemy)
        {
            if (enemy == null)
            {
                return "Enemy Scriptable Object";
            }

            if (enemy.enemyPrefab == null)
            {
                return "Enemy prefab is not assigned yet";
            }

            return enemy.enemyPrefab.name;
        }

        private string GetHeroStatusLine(EnemySO enemy)
        {
            if (enemy == null)
            {
                return "Configure enemy data, behaviours, and skills from one place.";
            }

            int enabledOverrides = CountEnabledOverrides(statOverridesProperty);
            return $"Stats {enabledOverrides} active, behaviours {CountConfiguredBehaviourTargets()}, skills {GetConfiguredPassiveCount(enemy) + GetConfiguredActiveCount(enemy)} configured";
        }

        private int CountConfiguredBehaviourTargets()
        {
            int count = 0;
            for (int i = 0; i < behaviourPrefabsProperty.arraySize; i++)
            {
                SerializedProperty element = behaviourPrefabsProperty.GetArrayElementAtIndex(i);
                SerializedProperty targetProperty = element.FindPropertyRelative("_target");
                if (targetProperty?.objectReferenceValue != null)
                {
                    count++;
                }
            }

            return count;
        }

        private int GetConfiguredPassiveCount(EnemySO enemy)
        {
            return CountConfiguredSkillPatches(enemy?.passiveSkill?.Values);
        }

        private int GetConfiguredActiveCount(EnemySO enemy)
        {
            return CountConfiguredSkillPatches(enemy?.activeSkill?.Values);
        }

        private static int CountConfiguredSkillPatches(IEnumerable<IFieldPatchRuntime> patches)
        {
            if (patches == null)
            {
                return 0;
            }

            int count = 0;
            foreach (IFieldPatchRuntime patch in patches)
            {
                if (patch?.TargetObject != null)
                {
                    count++;
                }
            }

            return count;
        }

        private List<string> CollectWarnings(EnemySO enemy)
        {
            List<string> warnings = new List<string>();

            if (enemy == null)
            {
                warnings.Add("EnemySO target could not be resolved.");
                return warnings;
            }

            if (enemy.enemyPrefab == null)
            {
                warnings.Add("Enemy Prefab is missing.");
            }
            else if (enemy.enemyPrefab.GetComponentInChildren<StatOverrideBehavior>() == null)
            {
                warnings.Add("Enemy Prefab has no StatOverrideBehavior. Load From Prefab may return nothing.");
            }

            int missingEquipments = CountMissingEquipmentItems();
            if (missingEquipments > 0)
            {
                warnings.Add($"{missingEquipments} equipment slots are missing Item Data.");
            }

            int missingStatRefs = CountMissingStatReferences();
            if (missingStatRefs > 0)
            {
                warnings.Add($"{missingStatRefs} stat overrides are missing Stat references.");
            }

            List<string> duplicates = GetDuplicatedStatNames();
            if (duplicates.Count > 0)
            {
                warnings.Add($"Duplicated stat overrides: {string.Join(", ", duplicates)}");
            }

            int missingStates = CountMissingReferences(stateDatasProperty);
            if (missingStates > 0)
            {
                warnings.Add($"{missingStates} state data entries are missing references.");
            }

            int unconfiguredBehaviours = behaviourPrefabsProperty.arraySize - CountConfiguredBehaviourTargets();
            if (unconfiguredBehaviours > 0)
            {
                warnings.Add($"{unconfiguredBehaviours} behaviour patches are missing targets.");
            }

            return warnings;
        }

        private int CountMissingEquipmentItems()
        {
            int count = 0;
            for (int i = 0; i < equipmentsProperty.arraySize; i++)
            {
                SerializedProperty element = equipmentsProperty.GetArrayElementAtIndex(i);
                if (element.FindPropertyRelative("itemData")?.objectReferenceValue == null)
                {
                    count++;
                }
            }

            return count;
        }

        private int CountMissingStatReferences()
        {
            int count = 0;
            for (int i = 0; i < statOverridesProperty.arraySize; i++)
            {
                SerializedProperty element = statOverridesProperty.GetArrayElementAtIndex(i);
                if (element.FindPropertyRelative("stat")?.objectReferenceValue == null)
                {
                    count++;
                }
            }

            return count;
        }

        private List<string> GetDuplicatedStatNames()
        {
            Dictionary<string, int> occurrences = new Dictionary<string, int>();

            for (int i = 0; i < statOverridesProperty.arraySize; i++)
            {
                SerializedProperty element = statOverridesProperty.GetArrayElementAtIndex(i);
                Object statObject = element.FindPropertyRelative("stat")?.objectReferenceValue;
                if (statObject == null)
                {
                    continue;
                }

                string statName = GetStatDisplayName(statObject);
                if (occurrences.ContainsKey(statName))
                {
                    occurrences[statName]++;
                }
                else
                {
                    occurrences.Add(statName, 1);
                }
            }

            return occurrences
                .Where(pair => pair.Value > 1)
                .Select(pair => pair.Key)
                .OrderBy(name => name)
                .ToList();
        }

        private static int CountEnabledOverrides(SerializedProperty overridesProperty)
        {
            if (overridesProperty == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < overridesProperty.arraySize; i++)
            {
                SerializedProperty element = overridesProperty.GetArrayElementAtIndex(i);
                if (element.FindPropertyRelative("isUseOverride")?.boolValue == true)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountMissingReferences(SerializedProperty property)
        {
            if (property == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    count++;
                }
            }

            return count;
        }

        private static string GetStatDisplayName(Object statObject)
        {
            if (statObject == null)
            {
                return "(None)";
            }

            SerializedObject statSerializedObject = new SerializedObject(statObject);
            SerializedProperty statNameProperty = statSerializedObject.FindProperty("statName");
            string statName = statNameProperty?.stringValue;
            return string.IsNullOrWhiteSpace(statName) ? statObject.name : statName;
        }

        private static class Styles
        {
            private static GUIStyle heroCard;
            private static GUIStyle toolbarCard;
            private static GUIStyle sectionCard;
            private static GUIStyle subSectionCard;
            private static GUIStyle heroTitle;
            private static GUIStyle heroSubtitle;
            private static GUIStyle heroStatus;
            private static GUIStyle sideLabel;
            private static GUIStyle metricChip;
            private static GUIStyle toolbarTitle;
            private static GUIStyle sectionFoldout;
            private static GUIStyle sectionSummary;
            private static GUIStyle subSectionTitle;
            private static GUIStyle subSectionSummary;
            private static GUIStyle collectionTitle;
            private static GUIStyle collectionSummary;
            private static GUIStyle contextLabel;
            private static GUIStyle centeredPreviewFallback;

            public static Color SectionHeaderBackground =>
                EditorGUIUtility.isProSkin ? new Color(0.16f, 0.18f, 0.21f) : new Color(0.85f, 0.88f, 0.91f);

            public static Color PreviewBackground =>
                EditorGUIUtility.isProSkin ? new Color(0.12f, 0.13f, 0.15f) : new Color(0.92f, 0.93f, 0.95f);

            public static GUIStyle HeroCard => heroCard ??= new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(12, 12, 12, 12)
            };

            public static GUIStyle ToolbarCard => toolbarCard ??= new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            public static GUIStyle SectionCard => sectionCard ??= new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            public static GUIStyle SubSectionCard => subSectionCard ??= new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 8, 8)
            };

            public static GUIStyle HeroTitle => heroTitle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15
            };

            public static GUIStyle HeroSubtitle => heroSubtitle ??= new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                wordWrap = true
            };

            public static GUIStyle HeroStatus => heroStatus ??= new GUIStyle(EditorStyles.miniLabel)
            {
                wordWrap = true
            };

            public static GUIStyle SideLabel => sideLabel ??= new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.UpperLeft
            };

            public static GUIStyle MetricChip => metricChip ??= new GUIStyle(EditorStyles.miniButton)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 6, 0, 0),
                padding = new RectOffset(8, 8, 3, 3)
            };

            public static GUIStyle ToolbarTitle => toolbarTitle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                margin = new RectOffset(0, 0, 0, 6)
            };

            public static GUIStyle SectionFoldout => sectionFoldout ??= new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            public static GUIStyle SectionSummary => sectionSummary ??= new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight
            };

            public static GUIStyle SubSectionTitle => subSectionTitle ??= new GUIStyle(EditorStyles.boldLabel);

            public static GUIStyle SubSectionSummary => subSectionSummary ??= new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight
            };

            public static GUIStyle CollectionTitle => collectionTitle ??= new GUIStyle(EditorStyles.boldLabel);

            public static GUIStyle CollectionSummary => collectionSummary ??= new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight
            };

            public static GUIStyle ContextLabel => contextLabel ??= new GUIStyle(EditorStyles.miniLabel)
            {
                wordWrap = true
            };

            public static GUIStyle CenteredPreviewFallback => centeredPreviewFallback ??= new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
