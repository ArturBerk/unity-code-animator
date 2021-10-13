using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace CodeAnimator
{
    public class CodeAnimator2 : MonoBehaviour
    {
        public string From;
        public string To;

        public float ScrollSpeed = 500;
        public Vector2 ScrollBorders = new Vector2(100, 100);

        [FormerlySerializedAs("delay"), SerializeField] private float characterDelay = 0.1f;
        [SerializeField]
        private float changeDelay = 0.1f;

        [SerializeField] private RectTransform linesContainer;

        [SerializeField] private CodeLine2 lineTemplate;

        private List<CodeLine2> lines = new List<CodeLine2>();

        private void Start()
        {
            currentText = new StringBuilder();
            currentTree = SyntaxParser.Parse(currentText);
        }

        private StringBuilder currentText = new StringBuilder();
        private CodeSyntaxTree currentTree;

        private Regex endLine = new Regex(".*(\\s*\n\\s*)$");

        private int GetCurrentLine(int index)
        {
            var lineNumber = 0;
            var t = currentText.ToString();
            for (int i = 0; i < index; i++)
            {
                if (t[i] == '\n') ++lineNumber;
            }

            return lineNumber;
        }

        private async UniTask ChangeAnimation2(string newCode)
        {
            var toTree = SyntaxParser.Parse(newCode);
            var changes = toTree.syntaxTree.GetChanges(currentTree.syntaxTree);
            currentTree = toTree;

            var changeStates = new List<List<State>>();
            
            var indexOffset = 0;
            var currentLine = 0;
            for (var index = 0; index < changes.Count; index++)
            {
                var states = new List<State>();
                changeStates.Add(states);
                var textChange = changes[index];
                var textChangeSpan = textChange.Span;
                var newText = textChange.NewText;
                var spanLength = textChangeSpan.Length;
                var spanStart = indexOffset + textChangeSpan.Start;

                currentLine = GetCurrentLine(spanStart);
                await Scroll(currentLine);

                var changeLine = currentLine;

                if (textChangeSpan.Length > 0)
                {
                    for (int i = spanLength - 1; i >= 0; i--)
                    {
                        RemoveChar2(states, i + spanStart);
                    }
                }

                if (newText.Length > 0)
                {
                    var endIndexExclusive = newText.Length;
                    var match = endLine.Match(newText);
                    if (match.Success)
                    {
                        var group = match.Groups[1];
                        endIndexExclusive -= group.Length;
                        AddCharAndContinue2(spanStart, group.Value);
                    }

                    for (int i = 0; i < endIndexExclusive; i++)
                    {
                        var c = newText[i];
                        if (Char.IsWhiteSpace(c))
                        {
                            AddCharAndContinue2(i + spanStart, newText[i]);
                        }
                        else
                        {
                            AddChar2(states, i + spanStart, newText[i]);
                        }

                        changeLine = GetCurrentLine(i + spanStart);
                        if (changeLine - currentLine > 5)
                        {
                            currentLine = changeLine;
                            states[states.Count - 1].ScrollTo = currentLine;
                            //await Scroll(currentLine);
                        }
                    }
                }

                indexOffset += textChange.NewText.Length - spanLength;
            }

            foreach (var changeState in changeStates)
                foreach (var state in changeState)
                    state.StartEvaluation();

            foreach (var states in changeStates)
            {
                if (changeDelay > 0) await UniTask.Delay(TimeSpan.FromSeconds(changeDelay));
                foreach (var state in states)
                {
                    var time = Time.time;
                    var lines = await state.Task;
                    if (state.ScrollTo >= 0)
                    {
                        await Scroll(state.ScrollTo);
                    }

                    ShowLines(lines);

                    var stepTime = Time.time - time;
                    var delay = this.characterDelay - stepTime;
                    if (delay > 0)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(delay));
                    }
                }
            }
        }

        private void ShowLines(string[] lines)
        {
            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var lineView = GetLineView(index);
                lineView.SetLine(index + 1, line);
            }

            CompleteLines(lines.Length);

            Layout();
        }

        private void RemoveChar2(List<State> changes, int index)
        {
            currentText.Remove(index, 1);
            changes.Add(new State(currentText.ToString()));
        }

        private void AddChar2(List<State> changes, int index, char c)
        {
            currentText.Insert(index, c);
            changes.Add(new State(currentText.ToString()));
        }

        private void AddCharAndContinue2(int index, char c)
        {
            currentText.Insert(index, c);
        }

        private void AddCharAndContinue2(int index, string c)
        {
            currentText.Insert(index, c);
        }

        private class State
        {
            private string to;
            private Task<string[]> task;
            private int scrollTo = -1;

            public int ScrollTo
            {
                get => scrollTo;
                set => scrollTo = value;
            }

            public Task<string[]> Task => task;

            public State(string to)
            {
                this.to = to;
            }

            public void StartEvaluation()
            {
                task = System.Threading.Tasks.Task.Run(() =>
                {
                    var tree = to.Parse();
                    var text = SyntaxHighlighter.Highlight(tree, Style.VisualStudio);
                    return text.Split('\n');
                });
            }
        }

        private void Layout()
        {
            var y = 0.0f;
            var width = linesContainer.rect.width;
            for (int i = 0; i < lines.Count; i++)
            {
                var codeLine = lines[i];
                if (!codeLine.gameObject.activeSelf) continue;
                var codeLineSize = codeLine.Layout(width);
                var child = codeLine.RectTransform;
                var height = codeLineSize.y;

                var newPosition = new Vector2(0, y);
                if ((newPosition - child.anchoredPosition).sqrMagnitude > 0.00001f)
                    child.anchoredPosition = new Vector2(0, y);
                y -= height;
            }

            var parentRect = (RectTransform) linesContainer.parent;
            linesContainer.sizeDelta = new Vector2(linesContainer.sizeDelta.x, Mathf.Max(-y, parentRect.rect.height));
        }

        private CodeLine2 GetLineView(int index)
        {
            if (index < lines.Count)
            {
                var codeLine2 = lines[index];
                codeLine2.gameObject.SetActive(true);
                return codeLine2;
            }

            for (int i = 0; i <= index; i++)
            {
                var newLine = Object.Instantiate(lineTemplate, linesContainer);
                lines.Add(newLine);
                newLine.gameObject.SetActive(true);
            }

            return lines[index];
        }

        private void CompleteLines(int index)
        {
            for (int i = index; i < lines.Count; i++)
            {
                lines[i].gameObject.SetActive(false);
            }
        }

        public async UniTask Scroll(int lineNumber)
        {
            if (lines.Count == 0) return;
            var parentRect = (RectTransform) linesContainer.parent;
            var line = lines[Mathf.Clamp(lineNumber - 1, 0, lines.Count - 1)];
            var h = line.RectTransform.rect.height;
            var y = line.RectTransform.anchoredPosition.y;

            var max = linesContainer.rect.height - parentRect.rect.height;

            //var deltaToParent = (-parentRect.rect.height) * 0.5f - (y + h * 0.5f);
            //if (deltaToParent < 0) deltaToParent = 0;
            var deltaToParent = Mathf.Clamp((-parentRect.rect.height) * 0.5f - (y + h * 0.5f), 0, max + 100);
            //rectTransform.anchoredPosition = new Vector2(0,deltaToParent);

            linesContainer.DOKill();
            await linesContainer.DOAnchorPosY(deltaToParent, ScrollSpeed).SetEase(Ease.InOutSine).ToUniTask();
        }

        public UniTask Apply(string code, bool animated = true)
        {
            if (animated)
                return ChangeAnimation2(code);
            currentText = new StringBuilder(code);
            currentTree = code.Parse();
            var lines = SyntaxHighlighter.Highlight(currentTree, Style.VisualStudio).Split('\n');

            ShowLines(lines);

            return UniTask.CompletedTask;
        }

        public void Clear()
        {
            currentText.Clear();
            currentTree = SyntaxParser.Parse("");
            CompleteLines(0);
            linesContainer.anchoredPosition = Vector2.zero;
        }
    }
}