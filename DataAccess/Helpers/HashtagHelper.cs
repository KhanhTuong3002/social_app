using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    public static class HashtagHelper
    {
        public static List<string> GetHashtags(string postText)
        {
            var hashtagsPattern = new Regex(@"#\w+");
            var matches = hashtagsPattern.Matches(postText)
             .Select(matches => matches.Value.TrimEnd('.',',','!','?').ToLower())
             .Distinct()
             .ToList();

            return matches;
        }
    }
}
