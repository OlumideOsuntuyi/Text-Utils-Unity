using System;
using System.Collections.Generic;

namespace LostOasis.AutoComplete
{
    [System.Serializable]
    public class Autocomplete
    {
        private Dictionary<char, Node> nodes;
        private Dictionary<string, int> indexMap;
        private List<WordData> words;

        private Queue<char> queue;

        public Autocomplete()
        {
            nodes = new();
            words = new();
            indexMap = new Dictionary<string, int>();

            queue = new();
        }

        private void QueueWord(string word)
        {
            queue.Clear();
            foreach(var c in word)
            {
                queue.Enqueue(c);
            }
        }

        public void Load(string[] words, int[] frequencies)
        {
            for (int i = 0; i < words.Length; i++)
            {
                var word = new WordData()
                {
                    word = words[i],
                    frequency = frequencies[i],
                    index = this.words.Count
                };

                this.words.Add(word);
                Push(word);
            }

            foreach(var n in nodes.Values)
            {
                n.UpdateAll();
            }
        }

        public void Update()
        {
            foreach (var n in nodes.Values)
            {
                n.UpdateAll();
            }
        }

        public void TryLoadNewWordFromCSV(string line)
        {
            var cols = line.Split(',');
            if (cols.Length != 2) return;

            var word = new WordData()
            {
                word = cols[0].ToLower(),
                frequency = long.Parse(cols[1]),
                index = this.words.Count
            };

            indexMap.Add(word.word, word.index);
            this.words.Add(word);
            Push(word);
        }

        public void Push(WordData word)
        {
            QueueWord(word.word);

            char q = queue.Dequeue();
            if (!nodes.TryGetValue(q, out var node))
            {
                node = new Node(q, 0);
                nodes.Add(q, node);
            }

            node.Push(word, queue);
        }

        public void PushNewWord(string word)
        {
            WordData data = Get(word);
            if(data == null)
            {
                data = new WordData()
                {
                    word = word,
                    frequency = 1,
                    index = words.Count,
                    sortedLinkages = new()
                };

                indexMap.Add(data.word, data.index);
                Push(data);
            }
            else
            {
                data.frequency += 1;
            }

            Update();
        }

        public WordData Get(string word)
        {
            if(indexMap.TryGetValue(word, out int index))
            {
                return words[index];
            }

            return null;
        }

        public WordData AddNew(string str)
        {
            var word = new WordData()
            {
                word = str,
                frequency = 0,
                index = this.words.Count
            };

            indexMap.Add(word.word, word.index);
            this.words.Add(word);

            QueueWord(word.word);
            Push(word);
            return word;
        }

        public void PushLinkage(string prev, string next)
        {
            prev = prev.ToLower();
            next = next.ToLower();

            if (prev == next) return;

            var a = Get(prev);
            var b = Get(next);

            a ??= AddNew(prev);
            b ??= AddNew(next);

            a.AddLinkage(b.index);
        }

        public static char[] chars = new char[]
        {
            // Whitespace
            ' ', '\t', '\n', '\r', '\v', '\f',

            // ASCII Punctuation
            '.', ',', ';', ':', '!', '?', '-', '_', '"', '\'', '`',

            // Symbols & Math
            '=', '+', '*', '/', '\\', '|', '^', '&', '%', '$', '#', '@', '<', '>', '~',

            // Brackets
            '(', ')', '[', ']', '{', '}',

            // Other possible non-letter characters
            '–', '—', '…', '“', '”', '‘', '’', '·', '•',

            // Miscellaneous Unicode punctuation
            '©', '®', '°', '†', '‡', '§', '¶', '¡', '¿'
        };

        public void LoadLargeText(string text)
        {
            string[] words = text.Split(chars, StringSplitOptions.RemoveEmptyEntries);
            string lastword = words[0];
            string nextword;
            for (int i = 1; i < words.Length; i++)
            {
                nextword = words[i];
                if(nextword.Length > 0 && lastword.Length > 0)
                {
                    PushLinkage(lastword, nextword);
                }
                lastword = nextword;
            }

            foreach(var w in this.words)
            {
                w.SortLinkages();
            }
        }

        public static string GetLastWord(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var words = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length > 0 ? words[^1] : "";
        }

        public static (string lastWord, bool endsWithSpace) GetLastWordAndSpaceStatus(string input)
        {
            if (string.IsNullOrEmpty(input))
                return ("", false);

            bool endsWithSpace = char.IsWhiteSpace(input[^1]);

            var words = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string lastWord = words.Length > 0 ? words[^1] : "";

            return (lastWord, endsWithSpace);
        }


        public List<string> Complete(string word, int count = 3)
        {
            if (string.IsNullOrEmpty(word)) return new();

            QueueWord(word);
            char q = queue.Dequeue();

            return nodes[q].Complete(word, queue, count);
        }

        public List<string> Suggest(string lastword, int count=3)
        {
            const string EMPTY = "";
            lastword = lastword.ToLower();

            QueueWord(lastword);
            List<string> words = new();
            if (indexMap.TryGetValue(lastword, out int index))
            {

            }
            else
            {
                return words;
            }

            var linkages = this.words[index].sortedLinkages;
            for (int i = 0; i < count; i++)
            {
                if(i < linkages.Count)
                {
                    words.Add(this.words[linkages[i].nextWord].word);
                }
                else
                {
                    words.Add(EMPTY);
                }
            }
            return words;
        }
    }
}