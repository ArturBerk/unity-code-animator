using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.UI;

namespace CodeAnimator
{
    public class CodeAnimator : MonoBehaviour
    {
        public float ScrollSpeed = 500;
        
        [SerializeField] private CodeLine lineTemplate;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private bool highlightCS;

        public bool HighlightCs => highlightCS;

        public string recordDirectory = "..";

        private RecorderWindow recorderWindow;
        private MovieRecorderSettings movieRecorderSettings;

        private void OnEnable()
        {
            if (EditorWindow.HasOpenInstances<RecorderWindow>())
            {
                recorderWindow = EditorWindow.GetWindow<RecorderWindow>();
                var recorderListField = typeof(RecorderWindow).GetField("m_RecordingListItem", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var recordingListItem = recorderListField.GetValue(recorderWindow);
                var listProperty = recordingListItem.GetType().GetProperty("items", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var itemList = (System.Collections.IList)listProperty.GetValue(recordingListItem);
                var firstRecorder = itemList[0];
                var settingsProperty = firstRecorder.GetType().GetProperty("settings", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                movieRecorderSettings = (MovieRecorderSettings) settingsProperty.GetValue(firstRecorder);
            }
        }

        public async UniTask Scroll(int lineNumber)
        {
            if (lines.Count == 0) return;
            var rectTransform = (RectTransform) transform;
            var parentRect = (RectTransform) rectTransform.parent;
            var line = lines[Mathf.Clamp(lineNumber - 1, 0, lines.Count - 1)];
            var h = line.RectTransform.rect.height;
            var y = line.RectTransform.anchoredPosition.y;

            var max = rectTransform.rect.height - parentRect.rect.height;
            
            //var deltaToParent = (-parentRect.rect.height) * 0.5f - (y + h * 0.5f);
            //if (deltaToParent < 0) deltaToParent = 0;
            //var deltaToParent = Mathf.Clamp((-parentRect.rect.height) * 0.5f - (y + h * 0.5f), 0, max);
            //rectTransform.anchoredPosition = new Vector2(0,deltaToParent);
            var rawDelta = (-parentRect.rect.height) * 0.5f - (y + h * 0.5f);
            var deltaToParent = Mathf.Clamp(rawDelta, 0, max + 500);
            
            rectTransform.DOKill();
            await rectTransform.DOAnchorPosY(deltaToParent, ScrollSpeed).SetEase(Ease.InOutSine).ToUniTask();
        }
        
        public async UniTask SetLinesAsync(string[] newLines, string filename)
        {
            var currentLines = this.lines.Select(l => l.OriginalLine).ToArray();

            for (var index = 0; index < newLines.Length; index++)
            {
                var newLine = newLines[index];
                if (string.IsNullOrWhiteSpace(newLine)) newLines[index] = " ";
            }

            // var diff2 = new Differ();
            // var diffResult = diff2.CreateLineDiffs(string.Join("\n", currentLines), string.Join("\n", newLines), false);
            // diffResult.
            var diff = new MyersDiff<string>(currentLines, newLines, true);
            var changes = diff.Execute();

            if (recorderWindow != null)
            {
                var fullPath = Path.GetFullPath(filename);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
                //movieRecorderSettings.FileNameGenerator.FileName = fileNameWithoutExtension;
                movieRecorderSettings.OutputFile = Path.Combine(Path.GetDirectoryName(fullPath), 
                    recordDirectory, nameText.text +  "_" + fileNameWithoutExtension);
                recorderWindow.StartRecording();
            }

            for (var index = 0; index < changes.Count; index++)
            {
                var textChange = changes[index];
                var updateIndices = false;
                var updateIndicesOffset = 0;
                switch (textChange.ChangeType)
                {
                    case ChangeType.Change:
                        await Scroll(textChange.StartCurrent + (int)(textChange.Length * 0.5f));
                        for (int i = 0; i < textChange.Length; i++)
                        {
                            lines[textChange.StartCurrent + i].Remove();
                            var newLine = CreateCodeLine(newLines[textChange.StartNew + i]);
                            lines[textChange.StartCurrent + i] = newLine;
                        }

                        // updateIndices = true;
                        // updateIndicesOffset = textChange.Length;

                        break;
                    case ChangeType.Delete:
                        await Scroll(textChange.StartCurrent);
                        for (int i = 0; i < textChange.Length; i++)
                        {
                            lines[textChange.StartCurrent + i].Remove();
                        }

                        lines.RemoveRange(textChange.StartCurrent, textChange.Length);
                        
                        updateIndices = true;
                        updateIndicesOffset = -textChange.Length;
                        break;
                    case ChangeType.Insert:
                        await Scroll(textChange.StartCurrent + (int)(textChange.Length * 0.5f));
                        for (int i = 0; i < textChange.Length; i++)
                        {
                            var newLine = CreateCodeLine(newLines[textChange.StartNew + i]);
                            lines.Insert(textChange.StartCurrent + i, newLine);
                        }
                        updateIndices = true;
                        updateIndicesOffset = textChange.Length;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (updateIndices)
                {
                    for (int i = index + 1; i < changes.Count; i++)
                    {
                        var change = changes[i];
                        change.StartCurrent += updateIndicesOffset;
                        changes[i] = change;
                    }
                }
                
                Layout();
                await UniTask.Delay(500);
            }
            
            if (recorderWindow != null)
            {
                await UniTask.Delay(1000);
                recorderWindow.StopRecording();
            }
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetLines(string[] newLines, string filename)
        {
            var currentLines = this.lines.Select(l => l.OriginalLine).ToArray();
            var diff = new MyersDiff<string>(currentLines, newLines, false);
            var changes = diff.Execute();

            for (var index = 0; index < changes.Count; index++)
            {
                var textChange = changes[index];
                var updateIndices = false;
                var updateIndicesOffset = 0;
                switch (textChange.ChangeType)
                {
                    case ChangeType.Change:
                        for (int i = 0; i < textChange.Length; i++)
                        {
                            lines[textChange.StartCurrent + i].Remove();
                            var newLine = CreateCodeLine(newLines[textChange.StartNew + i]);
                            lines[textChange.StartCurrent + i] = newLine;
                        }

                        //updateIndices = true;
                        //updateIndicesOffset = textChange.Length;
                        break;
                    case ChangeType.Delete:
                        for (int i = 0; i < textChange.Length; i++)
                        {
                            lines[textChange.StartCurrent + i].Remove();
                        }

                        lines.RemoveRange(textChange.StartCurrent, textChange.Length);
                        
                        //updateIndicesStart = true;
                        //updateIndicesOffset = textChange.Length;
                        break;
                    case ChangeType.Insert:
                        for (int i = 0; i < textChange.Length; i++)
                        {
                            var newLine = CreateCodeLine(newLines[textChange.StartNew + i]);
                            lines.Insert(textChange.StartCurrent + i, newLine);
                        }
                        updateIndices = true;
                        updateIndicesOffset = textChange.Length;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (updateIndices)
                {
                    for (int i = index + 1; i < changes.Count; i++)
                    {
                        var change = changes[i];
                        change.StartCurrent += updateIndicesOffset;
                        changes[i] = change;
                    }
                }
            }

            Layout();
        }

        private CodeLine CreateCodeLine(string line)
        {
            var codeLine = Instantiate(lineTemplate, transform);
            codeLine.OriginalLine = line;
            // codeLine.Text = Colorize(line);
            codeLine.Text =line;
            codeLine.gameObject.SetActive(true);
            return codeLine;
        }

        private List<CodeLine> lines = new List<CodeLine>();

        private StringBuilder tmp = new StringBuilder();

        // private string Colorize(string text)
        // {
        //     tmp.Clear();
        //     var startIndex = 0;
        //     var colorizer = new Colorizer();
        //     while (CSharpLexicalRules.Set.TryGetToken(text, startIndex, out var token))
        //     {
        //         tmp.Append(colorizer.Colorize(text, token));
        //         startIndex += token.Length;
        //     }
        //
        //     return tmp.ToString();
        // }

        private void Layout()
        {
            var rectTransform = (RectTransform) transform;
            var y = 0.0f;
            for (int i = 0; i < lines.Count; i++)
            {
                var codeLine = lines[i];
                var child = codeLine.RectTransform;
                var layout = codeLine.LayoutElement;
                codeLine.Number = i + 1;

                child.sizeDelta = new Vector2(rectTransform.rect.width, child.sizeDelta.x);
                layout.CalculateLayoutInputHorizontal();
                layout.CalculateLayoutInputVertical();

                var height = LayoutUtility.GetPreferredHeight(child);
                child.sizeDelta = new Vector2(rectTransform.rect.width, height);

                child.anchorMin = Vector2.up;
                child.anchorMax = Vector2.up;

                codeLine.Position = new Vector2(0, y);

                y -= height;
            }

            var parentRect = (RectTransform) rectTransform.parent;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Max(-y, parentRect.rect.height));
        }

        public void Clear()
        {
            var rectTransform = (RectTransform) transform;
            rectTransform.DOKill();
            rectTransform.anchoredPosition = Vector2.zero;
            foreach (var line in lines)
            {
                line.Remove();
            }
            lines.Clear();
        }
    }
}