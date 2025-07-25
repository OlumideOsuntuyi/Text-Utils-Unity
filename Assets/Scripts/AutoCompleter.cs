using System.Collections.Generic;
using System.IO;

using TMPro;

using UnityEngine;

using Voxel;

namespace LostOasis.AutoComplete
{
    public class AutoCompleter : MonoBehaviour
    {
        private Autocomplete completer;
        public TextAsset text;
        public TextAsset megaAsset;

        public int count;
        public string input;
        public List<string> result;

        public static string staticDebug;
        public static int node;
        public static int words;
        public static int linkage;

        public string debug;
        public bool complete;
        public string lastInput;

        public bool loadOnAwake;

        public TMP_InputField inputField;
        public List<TMP_Text> labels;

        private string PATH;
        [SerializeField] private bool loadMegaAssets;
        [SerializeField] private string loadMegaAssetPath;
        [SerializeField] private int startMegaAssetIndex;
        [SerializeField] private int lastMegaAssetIndex;


        string pth;
        string txt;
        private void Awake()
        {
            completer = new();
            PATH = Path.Combine(Application.persistentDataPath, "autocomplete.bin");
        }

        private void Start()
        {
            var text = this.text.text;
            var lines = text.Split('\n');
            var datapth = Application.dataPath;

            ThreadsManager.QueueAction(() =>
            {
                if (!loadOnAwake)
                {
                    for (int i = 1; i < lines.Length; i++)
                    {
                        completer.TryLoadNewWordFromCSV(lines[i]);
                        staticDebug = $"{i} / {lines.Length} and {node} nodes total and {words} words";
                    }

                    completer.Update();
                    if(loadMegaAssets)
                    {
                        for (int i = startMegaAssetIndex; i <= lastMegaAssetIndex; i++)
                        {
                            pth = Path.Combine(datapth, loadMegaAssetPath + $"{i}.txt");
                            txt = File.ReadAllText(pth);

                            completer.LoadLargeText(txt);
                            staticDebug = $"{i} / {lastMegaAssetIndex - startMegaAssetIndex} and {node} nodes total, {words} words and {linkage} linkages";
                        }
                    }

                    FileHandler.SaveObject(complete, PATH);
                }
                else
                {
                    completer = FileHandler.LoadObject<Autocomplete>(PATH);
                }

                complete = true;

            });   
        }

        private void Update()
        {
            debug = staticDebug;
            if (!complete || lastInput == input) return;

            result = completer.Complete(input, Mathf.Max(1, count));
            lastInput = input;
        }

        public void OnGenerate()
        {
            string str = inputField.text;
            var lastword = Autocomplete.GetLastWordAndSpaceStatus(str);
            bool suggest = lastword.endsWithSpace && !string.IsNullOrEmpty(lastword.lastWord);

            if(suggest)
            {
                result = completer.Suggest(lastword.lastWord, Mathf.Max(1, count));
            }
            else if(lastword.lastWord.Length > 0)
            {
                result = completer.Complete(lastword.lastWord, Mathf.Max(1, count));
            }


            for (int i = 0; i < labels.Count; i++)
            {
                if (i >= result.Count) continue;
                labels[i].text = result[i];
            }
        }
    }
}