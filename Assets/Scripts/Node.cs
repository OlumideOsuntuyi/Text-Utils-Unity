using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LostOasis.AutoComplete
{
    [System.Serializable]
    public class Node
    {
        char alphabet;
        byte depth;
        long frequency = 0;
        long endingFrequency = 0;
        long userbias = 0;
        Dictionary<char, Node> branches;
        List<Node> modesChars;

        public long Bias => frequency + userbias;

        public Node(char alphabet, byte depth)
        {
            this.alphabet = alphabet;
            this.depth = depth;
            branches = new();
            modesChars = new();
            AutoCompleter.node++;
        }

        public void Push(WordData data, Queue<char> queue)
        {
            if (queue.Count == 0) return;

            char q = queue.Dequeue();

            if(!branches.TryGetValue(q, out var node))
            {
                node = new Node(q, (byte)(depth + 1));
                branches.Add(q, node);
                modesChars.Add(node);
            }

            if (queue.Count > 0) // if 
            {
                node.Push(data, queue);
            }
            else
            {
                endingFrequency += data.frequency;
            }
        }

        public void UpdateAll()
        {
            foreach (var n in branches.Values)
            {
                n.UpdateTopmostChars();
            }

            UpdateTopmostChars();
        }

        private void UpdateTopmostChars()
        {
            modesChars.Sort((a, b) =>
            {
                return b.endingFrequency.CompareTo(a.endingFrequency);
            });
        }

        public string Word(string prev, int depth, List<string> exclusionList = null)
        {
            string w = prev + this.alphabet;
            foreach (var br in modesChars)
            {
                string r = br.Word(w, depth + 1, exclusionList);
                if (exclusionList == null || !exclusionList.Contains(r))
                {
                    return r;
                }
            }

            return w;
        }

        public List<string> Words(string prev = "", int count = 3)
        {
            List<string> words = new();
            int i = 0;
            const int MAX_TRIES = 64;

            while (words.Count < count && i < MAX_TRIES)
            {
                string w = prev;

                if (modesChars.Count == 0)
                {
                    return words;
                }

                foreach (var nd in modesChars)
                {
                    string _w = nd.Word(w, 0, words);

                    if (!words.Contains(_w))
                        words.Add(_w);

                    if(words.Count >= count)
                    {
                        return words;
                    }
                }

                i++;
            }

            return words;
        }

        public List<string> Complete(string original, Queue<char> queue, int count = 3)
        {
            if (this.branches.Count == 0 || queue.Count == 0)
            {
                if(branches.Count == 0 && queue.Count > 0)
                {
                    // UnityEngine.Debug.Log($"Branch ended yet queue hasn't ended with {queue.Count} chars left");
                }

                return this.Words(original, count);
            }

            char q = queue.Dequeue();

            if (!this.branches.ContainsKey(q))
                return new List<string>();

            return this.branches[q].Complete(original, queue, count);
        }
    }
}