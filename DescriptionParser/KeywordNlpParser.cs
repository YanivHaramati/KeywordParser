using edu.stanford.nlp.ling;
using edu.stanford.nlp.tagger.maxent;
using java.io;
using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace AppProjection
{
    public class KeywordNlpParser
    {
        private const string Model =
            @"..\..\..\AppProjection\stanford-postagger-2013-06-20\models\wsj-0-18-bidirectional-nodistsim.tagger";
			
		private static readonly IList<string> InterestPatterns = new List<string>
            {
                "CD:N",
                "JJ:N"
            };

        private static MaxentTagger _tagger = null;

        public static IEnumerable<string> GetKeywordsFromDescription(string description)
        {
            Reader r = new StringReader(description);
            return TagReader(r);
        }                

        private static IEnumerable<string> TagReader(Reader reader)
        {
            if(_tagger == null)
            {
                //var b = System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + Model);
                _tagger = new MaxentTagger(AppDomain.CurrentDomain.BaseDirectory + Model);
            }
            var results = new List<string>();
            var keywordLists = MaxentTagger.tokenizeText(reader)
                .toArray()
                .Cast<List>()
                .Select(sentence => Tagger.tagSentence(sentence)).Select(GetInterestingKeywords);

            foreach (var listOfKeywords in keywordLists.Where(listOfKeywords => listOfKeywords.Any()))
            {
                listOfKeywords.ToList().ForEach(kw => results.Add(string.Join(" ", kw)));
            } 
            return results;
        }

        private static IEnumerable<IEnumerable<string>> GetInterestingKeywords(ArrayList tSentence)
        {
            var keywords = new List<List<string>>();
            var phrase = new List<string>();
            foreach (var pattern in InterestPatterns)
            {
                phrase.Clear();
                var p = pattern.Split(':');
                for (var i = 0; i < tSentence.size(); i++)
                {
                    var tw = (TaggedWord) tSentence.get(i);
                    // start condition is an empty phrase and tag matches start pattern.
                    if (!phrase.Any())
                    {
                        // start condition
                        if (tw.tag().StartsWith(p[0]))
                        {
                            phrase.Add(tw.value());
                        }
                    }                    
                    // phrase started, so fill in the blanks.
                    else
                    {
                        phrase.Add(tw.value());
                        // end condition
                        if (!tw.tag().StartsWith(p[1])) continue;
                        // match greedy patterns
                        var nextTaggedWord = i+1 < tSentence.size()
                            ? (TaggedWord) tSentence.get(i+1)
                            : null;
                        if (nextTaggedWord != null && nextTaggedWord.tag().StartsWith(p[1])) continue;
                        keywords.Add(phrase.ToList());
                        phrase.Clear();
                    }
                }
            }
            // remove subsets such as "wide floors" compared to "7 ' wide floors"
            if (keywords.Any())
            {
                keywords = RemoveSubsets(keywords);
            }
            return keywords;
        }
		
		private static List<List<string>> RemoveSubsets(IList<List<string>> lst)
        {
            var result = lst.ToList();
            foreach (var l in lst)
            {
                foreach (var k in lst)
                {
                    if (l != k)
                    {
                        var s1 = string.Join("", l);
                        var s2 = string.Join("", k);
                        if (s1.IndexOf(s2, System.StringComparison.Ordinal) != -1 && result.Contains(k))
                        {
                            result.Remove(k);
                        }
                    }
                }
            }
            return result;
        }
    }
}
