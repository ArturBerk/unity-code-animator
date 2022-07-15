using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.UI;

namespace CodeAnimator
{
    public class CodeAnimatorProgram : MonoBehaviour
    {
        [SerializeField]
        private KeyCode executeKey = KeyCode.Space;
        
        [SerializeField]
        private KeyCode openKey = KeyCode.O;

        [SerializeField]
        private Slider slider;

        private CancellationTokenSource cancellationTokenSource;
        private string directory;
        private IList<IAnimatorCommand> program;
        
        void Start()
        {
            directory = PlayerPrefs.GetString("RootDirectory", Application.dataPath);

            // var file = File.ReadAllText(@"C:\UnityProjects\YoutubeProject1\Assets\Scripts\Systems\Utils\Events\EventBufferSystem.cs");
            // var tree = SyntaxParser.Parse(file);
            // var code = SyntaxHighlighter.Highlight(tree, Style.VisualStudio);
            //Debug.Log(code);
        }

        void Update()
        {
            if (Input.GetKeyDown(executeKey))
            {
                Execute();
            }

            if (Input.GetKeyDown(openKey))
            {
                
                var filenames = StandaloneFileBrowser.OpenFilePanel("Open program", directory, "txt", false);
                if (filenames == null || filenames.Length == 0) return;
                directory = Path.GetDirectoryName(filenames[0]);
                PlayerPrefs.SetString("RootDirectory", directory);
                PlayerPrefs.Save();

                ParseProgram(File.ReadAllText(filenames[0]), Path.GetDirectoryName(filenames[0]));
                
                var animator = GetComponent<CodeAnimator>();
                if (animator != null)
                    animator.Clear();
                var animator2 = GetComponent<CodeAnimator2>();
                if (animator2 != null)
                    animator2.Clear();
            }
        }

        private void ParseProgram(string program, string workingPath = null)
        {
            var parser = new CodeAnimationProgramParser();
            if (workingPath != null)
                parser.WorkingPath = workingPath;
            this.program = parser.Parse(program);
        }

        private void Execute()
        {
            if (program == null) return;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            ExecuteProgram(program, cancellationTokenSource.Token).Forget();
        }

        private void SetProgress(float p)
        {
            if (slider != null)
                slider.value = p;
        }
        
        public async UniTaskVoid ExecuteProgram(IList<IAnimatorCommand> commands, CancellationToken cancellationToken)
        {
            var animator1 = GetComponent<CodeAnimator>();
            SetProgress(0);
            if (animator1 != null)
            {
                animator1.Clear();
                var count = commands.Count;
                for (var index = 0; index < commands.Count; index++)
                {
                    var animatorCommand = commands[index];
                    if (cancellationToken.IsCancellationRequested) return;
                    await animatorCommand.Apply(animator1, cancellationToken);
                    SetProgress((index + 1.0f) / count);
                }
            }
            var animator2 = GetComponent<CodeAnimator2>();
            if (animator2 != null)
            {
                animator2.Clear();
                var count = commands.Count;
                for (var index = 0; index < commands.Count; index++)
                {
                    var animatorCommand = commands[index];
                    if (cancellationToken.IsCancellationRequested) return;
                    await animatorCommand.Apply(animator2, cancellationToken);
                    SetProgress((index + 1.0f) / count);
                }
            }
        }
    }
}