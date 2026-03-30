/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EnjoyGameClub.TextLifeFramework.Core.Match
{
    public static class TextMatcher
    {
        private const string TAG_REGEX_PATTERN = @"<(?<tag>\w+)>|<@(?<tag>\w+)>";

        public static (string parseText, List<MatchData> matchDatas) ParseRichText(string text, MatchConfig matchConfig)
        {
            var matches = new Regex(TAG_REGEX_PATTERN, RegexOptions.Compiled).Matches(text);
            var cleanText = new StringBuilder();
            var matchDataList = new List<MatchData>();
            var tagStacks = new Dictionary<string, Stack<int>>();
            var offset = 0; // 记录标签删除导致的偏移量
            int i = 0, matchIndex = 0;

            while (i < text.Length)
                // 检查是否遇到匹配的标签
                if (matchIndex < matches.Count && i == matches[matchIndex].Index)
                {
                    var match = matches[matchIndex];
                    matchIndex++;

                    var tagName = match.Groups["tag"].Value;
                    if (matchConfig.MatchList.FirstOrDefault(x => x.matchTag == tagName) != null)
                    {
                        if (match.Value == $"<{tagName}>")
                        {
                            if (!tagStacks.ContainsKey(tagName))
                                tagStacks[tagName] = new Stack<int>();

                            tagStacks[tagName].Push(match.Index - offset);
                        }
                        else if (match.Value == $"<@{tagName}>")
                        {
                            if (tagStacks.ContainsKey(tagName) && tagStacks[tagName].Count > 0)
                            {
                                var startIndex = tagStacks[tagName].Pop();
                                var endIndex = match.Index - offset;
                                var extractedContent =
                                    cleanText.ToString().Substring(startIndex, endIndex - startIndex);

                                matchDataList.Add(new MatchData
                                {
                                    MatchTag = tagName,
                                    Content = extractedContent,
                                    StartIndex = startIndex,
                                    EndIndex = endIndex
                                });
                            }
                        }

                        offset += match.Length;
                        i += match.Length;
                    }
                }
                else
                {
                    cleanText.Append(text[i]);
                    i++;
                }

            // 处理未闭合的标签（范围延续到末尾）
            foreach (var pair in tagStacks)
            {
                var tagName = pair.Key;
                var stack = pair.Value;

                while (stack.Count > 0)
                {
                    var startIndex = stack.Pop();
                    var endIndex = cleanText.Length;
                    var extractedContent = cleanText.ToString().Substring(startIndex, endIndex - startIndex);

                    matchDataList.Add(new MatchData
                    {
                        MatchTag = tagName,
                        Content = extractedContent,
                        StartIndex = startIndex,
                        EndIndex = endIndex
                    });
                }
            }

            return (cleanText.ToString(), matchDataList);
        }

        public static string RemoveTags(string text, List<MatchData> matches)
        {
            foreach (var match in matches)
            {
                var tagName = match.MatchTag;
                var pattern = $@"<({tagName})>|<@({tagName})>"; // 匹配 <tag> 和 <@tag>
                text = Regex.Replace(text, pattern, ""); // 直接替换为空
            }

            return text;
        }
    }
}