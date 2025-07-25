using System.Collections.Generic;

namespace LostOasis.AutoComplete
{
    [System.Serializable]
    public class WordData
    {
        public string word;
        public long frequency;
        public int index;
        public List<WordLinkage> sortedLinkages;
        public Dictionary<int, int> linkageIDs;

        public WordData()
        {
            sortedLinkages = new();
            linkageIDs = new();
            AutoCompleter.words++;
        }

        public void AddLinkage(int index)
        {
            if(!linkageIDs.TryGetValue(index, out int linkageID))
            {
                var linkage = new WordLinkage()
                {
                    nextWord = index,
                    frequency = 0
                };

                linkageID = sortedLinkages.Count;
                linkageIDs.Add(index, linkageID);
                sortedLinkages.Add(linkage);
            }

            sortedLinkages[linkageID].frequency += 1;
        }

        public void SortLinkages()
        {
            sortedLinkages.Sort((a, b) =>
            {
                return b.frequency.CompareTo(a.frequency);
            });
        }
    }


    [System.Serializable]
    public class WordLinkage
    {
        public int nextWord;
        public long frequency;

        public WordLinkage()
        {
            AutoCompleter.linkage++;
        }
    }
}