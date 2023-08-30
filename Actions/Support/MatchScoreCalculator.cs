using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeRushStreamDeck
{
    public static class MatchScoreCalculator
    {
        public static double GetScore(List<string> list1, List<string> list2, double maxScore = 1)
        {
            List<string> longestList, shortestList;
            GetLongestShortestLists(list1, list2, out longestList, out shortestList);

            double maxScorePerCharacter = maxScore / GetTotalCharacterCount(longestList);

            if (longestList.Count == 0 || shortestList.Count / (double)longestList.Count < 0.5)
                return 0;

            double score = 0;

            int searchForwardIndex = 0;
            for (; searchForwardIndex < shortestList.Count; searchForwardIndex++)
            {
                double thisScore = GetMatchScore(shortestList[searchForwardIndex], longestList[searchForwardIndex], maxScorePerCharacter);
                if (thisScore > 0)
                    score += thisScore;
                else
                    break;
            }

            int numRemainingToMatch = shortestList.Count - searchForwardIndex;

            if (numRemainingToMatch > 0)
                for (int i = 0; i < numRemainingToMatch; i++)
                {
                    int shortIndex = shortestList.Count - 1 - i;
                    int longIndex = longestList.Count - 1 - i;
                    double thisScore = GetMatchScore(shortestList[shortIndex], longestList[longIndex], maxScorePerCharacter);
                    if (thisScore > 0)
                        score += thisScore;
                    else
                        break;
                }

            return score;
        }

        static double GetMatchScore(char chr1, char chr2, double maxScorePerCharacter)
        {
            if (chr1 == chr2)
                return maxScorePerCharacter;

            return 0;
        }

        static double GetMatchScore(string str1, string str2, double maxScorePerCharacter)
        {
            if (str1.Length == str2.Length)
            {
                if (str1 == str2)
                    return maxScorePerCharacter * str1.Length;
            }

            return 0;
            //if (!options.HasFlag(MatchScoreOptions.AllowPartialMatches))
            //    return 0;

            //GetLongestShortestStrings(str1, str2, out string longestStr, out string shortestStr);

            //double score = 0;

            //int searchForwardIndex = 0;
            //for (; searchForwardIndex < shortestStr.Length; searchForwardIndex++)
            //{
            //    double thisScore = GetMatchScore(shortestStr[searchForwardIndex], longestStr[searchForwardIndex], options, maxScorePerCharacter);
            //    if (thisScore > 0)
            //        score += thisScore;
            //    else
            //        break;
            //}

            //int numRemainingToMatch = shortestStr.Length - searchForwardIndex;

            //if (numRemainingToMatch > 0)
            //    for (int i = 0; i < numRemainingToMatch; i++)
            //    {
            //        int shortIndex = shortestStr.Length - 1 - i;
            //        int longIndex = longestStr.Length - 1 - i;
            //        double thisScore = GetMatchScore(shortestStr[shortIndex], longestStr[longIndex], options, maxScorePerCharacter);
            //        if (thisScore > 0)
            //            score += thisScore;
            //        else
            //            break;
            //    }

            //return score;
        }

        //static void GetLongestShortestStrings(string str1, string str2, out string longestStr, out string shortestStr)
        //{
        //    if (str1.Length > str2.Length)
        //    {
        //        longestStr = str1;
        //        shortestStr = str2;
        //    }
        //    else
        //    {
        //        longestStr = str2;
        //        shortestStr = str1;
        //    }
        //}

        private static void GetLongestShortestLists(List<string> list1, List<string> list2, out List<string> longestList, out List<string> shortestList)
        {
            if (list1.Count > list2.Count)
            {
                longestList = list1;
                shortestList = list2;
            }
            else
            {
                longestList = list2;
                shortestList = list1;
            }
        }

        static int GetTotalCharacterCount(List<string> list)
        {
            int count = 0;
            foreach (string item in list)
                count += item.Length;
            return count;
        }
    }
}
