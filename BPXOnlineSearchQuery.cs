using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            string[] components = query.Split(" ");
            foreach (string comp in components)
            {
                if (comp.Contains(":"))
                {
                    string[] keywordComponents = comp.Split(":");
                    if (keywordComponents[0].ToLower() == "from")
                    {
                        _creator = keywordComponents[1].ToLower();
                        continue;
                    }
                    else if (keywordComponents[0].ToLower() == "tags")
                    {
                        string[] tagComponents = keywordComponents[1].Split(",");
                        foreach (string t in tagComponents)
                        {
                            string tag = t.ToLower().Trim();
                            if (!string.IsNullOrEmpty(tag))
                            {
                                _tags.Add(tag);
                            }
                        }
                        continue;
                    }
                }

                //No keywords so search term
                string c = comp.ToLower().Trim();
                if (!string.IsNullOrEmpty(c))
                {
                    _searchTerms.Add(c);
                }
            }

            searchTerms = _searchTerms.ToArray();
            creator = _creator;
            tags = _tags.ToArray();
        }
    }
}
