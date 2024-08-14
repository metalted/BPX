using System;
using System.Collections.Generic;

namespace BPX
{
    public class BPXOnlineSearchQuery
    {
        public string[] searchTerms;
        public string creator;
        public string[] tags;

        public BPXOnlineSearchQuery(string query)
        {
            List<string> _searchTerms = new List<string>();
            List<string> _tags = new List<string>();
            string _creator = "";

            bool insideQuotes = false;
            string currentTerm = "";
            string keyword = "";

            for (int i = 0; i < query.Length; i++)
            {
                char c = query[i];

                if (c == '\'')
                {
                    insideQuotes = !insideQuotes;
                    continue;
                }

                if (c == ' ' && !insideQuotes)
                {
                    ProcessTerm(currentTerm, ref _creator, _tags, _searchTerms, ref keyword);
                    currentTerm = "";
                }
                else if (c == ':' && !insideQuotes)
                {
                    keyword = currentTerm.ToLower().Trim();
                    currentTerm = "";
                }
                else
                {
                    currentTerm += c;
                }
            }

            // Add the last term
            ProcessTerm(currentTerm, ref _creator, _tags, _searchTerms, ref keyword);

            searchTerms = _searchTerms.ToArray();
            creator = _creator;
            tags = _tags.ToArray();

            foreach (string s in searchTerms)
            {
                Plugin.Instance.LogMessage("SearchTerm:" + s);
            }

            Plugin.Instance.LogMessage("Creator:" + creator);

            foreach (string s in tags)
            {
                Plugin.Instance.LogMessage("Tags:" + s);
            }
        }

        private void ProcessTerm(string term, ref string creator, List<string> tags, List<string> searchTerms, ref string keyword)
        {
            term = term.Trim().ToLower();

            if (string.IsNullOrEmpty(term))
                return;

            if (!string.IsNullOrEmpty(keyword))
            {
                if (keyword == "from")
                {
                    creator = term;
                }
                else if (keyword == "tags")
                {
                    string[] tagComponents = term.Split(",");
                    foreach (string t in tagComponents)
                    {
                        string tag = t.Trim();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            tags.Add(tag);
                        }
                    }
                }

                keyword = ""; // Reset keyword after use
            }
            else
            {
                searchTerms.Add(term);
            }
        }
    }
}