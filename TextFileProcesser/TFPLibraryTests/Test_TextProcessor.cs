using TFPLibrary;

namespace TFPLibraryTests;

[TestClass]
public class TestTextProcessor
{
    [TestMethod]
    [DataRow("   My  Words ")]
    [DataRow("My.Words Run")]
    public void SeparateTextToSingleWords_TwoWords_ReturnTwoWords(string text)
    {
        // Arrange
        const int expected = 2;

        // Act
        var actual = TextProcessor.SeparateTextToSingleWords(text);

        // Assert
        Assert.AreEqual(expected, actual.Length);
    }

    [TestMethod]
    [DataRow("")]
    public void SeparateTextToSingleWords_NullOrEmpty_ReturnEmptyString(string text)
    {
        // Arrange
        const int expected = 1;

        // Act
        var actual = TextProcessor.SeparateTextToSingleWords(text);

        // Assert
        Assert.AreEqual(expected, actual.Length);
    }

    [TestMethod]
    public void CountWordsOccurrences_StringArray_ReturnWordsAndCounts()
    {
        // Arrange
        const string word1 = "My";
        const string word2 = "Words";
        var words = new[] { word1, word2, word1, word1 };
        var expected = new Dictionary<string, int>
        {
            { word1, 3 }, 
            { word2, 1 }
        };

        // Act
        var actual = TextProcessor.CountWordsOccurrences(words);

        // Assert
        Assert.AreEqual(expected[word1], actual[word1]);
        Assert.AreEqual(expected[word2], actual[word2]);
        Assert.AreEqual(expected.Count, actual.Count);
    }

    [TestMethod]
    public void ConvertToDescendingArray_GivenDictionary_ReturnsDescendingResults()
    {
        // Arrange
        const string word1 = "My";
        const string word2 = "Words";
        const string word3 = "Run";
        var wordsAndCounts = new Dictionary<string, int>
        {
            { word1, 1 },
            { word2, 3 },
            { word3, 2 }
        };
        var expected = new[]
        {
            (word2, 3),
            (word3, 2),
            (word1, 1)
        };

        // Act
        var actual = TextProcessor.ConvertToDescendingArray(wordsAndCounts);

        // Assert
        Assert.AreEqual(expected.First().Item1, actual.First().Item1);
        Assert.AreEqual(expected.Last().Item1, actual.Last().Item1);
    }
}
