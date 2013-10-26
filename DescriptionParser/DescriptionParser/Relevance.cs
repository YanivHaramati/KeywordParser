using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.tagger.maxent;
using ikvm.extensions;
using java.io;
using java.util;

namespace DescriptionParser
{
    public static class Relevance
    {
        public const string Model =  @"..\..\Data\stanford-postagger-2013-06-20\models\wsj-0-18-bidirectional-nodistsim.tagger";       
        private static readonly MaxentTagger Tagger = new MaxentTagger(Model);

        static readonly IList<string> InterestPatterns = new List<string>
            {
                "CD:N",
                "JJ:N",
                "NNP:NN",
                "RB:NN"
            };

        static readonly IDictionary<string, int> RemoveList = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"-RRB-", 0},
            {"-LRB-", 0}
        }; 

        static readonly IDictionary<string, int> SkipThrough = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"TO", 0}
        }; 

        private static IEnumerable<string> TagReader(Reader reader)
        {
            var results = new List<string>();
            MaxentTagger.tokenizeText(reader)
                .toArray()
                .Cast<List>()
                .Select(sentence => Tagger.tagSentence(sentence))
                .ToList()
                .ForEach(kwl => results.AddRange(GetInterestingKeywords(kwl)));
            return RemoveSubsets(results);
        }

        private static IEnumerable<string> GetInterestingKeywords(List tSentence)
        {
            System.Console.WriteLine("Sentence: {0}", tSentence);
            var keywords = new List<string>();
            var phrase = new StringBuilder();
            foreach (var pattern in InterestPatterns)
            {
                phrase.Clear();
                var p = pattern.Split(':');
                for (var i = 0; i < tSentence.size(); i++)
                {
                    var tw = (TaggedWord) tSentence.get(i);
                    // start condition is an empty phrase and tag matches start pattern.
                    if (phrase.Length == 0)
                    {
                        // start condition
                        if (tw.tag().StartsWith(p[0]))
                        {
                            phrase.Append(CleanValue(tw.value()));
                        }
                    }                    
                    // phrase started, so fill in the blanks.
                    else
                    {
                        phrase.Append(CleanValue(tw.value()));
                        // end condition
                        if (!tw.tag().StartsWith(p[1])) continue;

                        // match greedy patterns
                        var nextTaggedWord = i+1 < tSentence.size()
                            ? (TaggedWord) tSentence.get(i+1)
                            : null;

                        if (nextTaggedWord == null ||
                            !SkipThrough.ContainsKey(nextTaggedWord.tag()) && !nextTaggedWord.tag().StartsWith(p[1]))
                        {
                            keywords.Add(phrase.ToString().toLowerCase().Trim());
                            phrase.Clear();
                        }                        
                    }
                }
            }
            return keywords;
        }

        private static string CleanValue(string value)
        {
            if (RemoveList.ContainsKey(value))
            {
                return string.Empty;
            }
            return " " + value.replaceAll("\\\\", "");
        }

        private static IEnumerable<string> RemoveSubsets(IList<string> lst)
        {
            // get rid of dups.
            var result = new HashSet<string>(lst);

            // get rid of subsets.
            foreach (
                var k in
                    lst.Select(l => l.Trim())
                        .SelectMany(
                            l =>
                                lst.Select(k => k.Trim())
                                    .Where(k => l != k)
                                    .Where(k => l.IndexOf(k, StringComparison.Ordinal) != -1 && result.Contains(k))))
            {
                result.Remove(k);
            }
            return result.ToList();
        }

        public static void TagFile(string fileName)
        {
            TagReader(new BufferedReader(new FileReader(fileName)));
        }

        public static void TagText(string text)
        {
            var res = TagReader(new StringReader(text));
//            System.Console.WriteLine("Text: {0}", text);
            
            System.Console.WriteLine();
            foreach (var kw in res)
            {
                System.Console.WriteLine("keywords: {0}", kw);
            }                  
        }
    }    
}
