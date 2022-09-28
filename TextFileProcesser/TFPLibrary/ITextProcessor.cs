namespace TFPLibrary;

public interface ITextProcessor
{
    string[] SeparateToSingleWords(string[] text, string delimiter = " ");
    (string, int)[] CountWordsOccurrences(string[] words);
}
