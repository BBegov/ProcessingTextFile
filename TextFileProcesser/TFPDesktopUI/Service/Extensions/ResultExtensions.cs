using System.Collections.Generic;
using System.Linq;
using TFPDesktopUI.Models;

namespace TFPDesktopUI.Service.Extensions;

public static class Extensions
{
    public static TextFileResult ToTextFileResults(this (string, int) wordsWithOccurrence)
    {
        return new TextFileResult
        {
            Word = wordsWithOccurrence.Item1,
            Occurrence = wordsWithOccurrence.Item2
        };
    }

    public static List<TextFileResult> ToTextFileResults(this (string, int)[] wordsWithOccurrences)
    {
        return wordsWithOccurrences.Select(ToTextFileResults).ToList();
    }
}