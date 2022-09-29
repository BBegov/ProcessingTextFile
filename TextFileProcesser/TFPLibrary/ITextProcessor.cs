namespace TFPLibrary;

public interface ITextProcessor
{
    string[] SeparateTextToSingleWords(string[] text, string delimiter = " ");
    (string, int)[] CountWordsOccurrences(string[] words);
}
