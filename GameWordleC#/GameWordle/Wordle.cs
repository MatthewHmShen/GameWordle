using System;
using System.IO;
using System.Linq;

namespace WordleGame
{
    public class WordleGameLogic
    {
        private const int MaxWords = 10000;
        private const int MaxWordLength = 6;
        private string[] wordList = new string[MaxWords];
        private int wordCount = 0;
        private string currentSecret = "";

        public string[] WordList => wordList.Take(wordCount).ToArray();
        public int WordCount => wordCount;
        public string CurrentSecret => currentSecret;

        public void LoadWords(string filename)
        {
            try
            {
                var lines = File.ReadAllLines(filename);

                foreach (var line in lines)
                {
                    if (wordCount >= MaxWords) break;

                    var word = line.Trim().ToUpper();
                    if (word.Length == 5) // 默认5字母单词
                    {
                        wordList[wordCount++] = word;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading word list: {ex.Message}");
            }
        }

        public void StartNewGame()
        {
            if (wordCount == 0)
            {
                throw new Exception("No valid words found in the word list");
            }

            currentSecret = wordList[new Random().Next(wordCount)];
        }

        public string EvaluateGuess(string guess)
        {
            if (guess.Length != 5)
            {
                throw new ArgumentException("Guess must be 5 letters long");
            }

            if (!wordList.Take(wordCount).Contains(guess))
            {
                throw new ArgumentException("Word not in word list");
            }

            return EvaluateGuessInternal(currentSecret, guess);
        }

        private string EvaluateGuessInternal(string secret, string guess)
        {
            char[] result = new char[5];
            bool[] secretMatched = new bool[5];
            bool[] guessMatched = new bool[5];

            // First pass: check for exact matches (G)
            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == secret[i])
                {
                    result[i] = 'G';
                    secretMatched[i] = true;
                    guessMatched[i] = true;
                }
            }

            // Second pass: check for present but wrong position (Y)
            for (int i = 0; i < 5; i++)
            {
                if (guessMatched[i]) continue;

                for (int j = 0; j < 5; j++)
                {
                    if (secretMatched[j]) continue;

                    if (guess[i] == secret[j])
                    {
                        result[i] = 'Y';
                        secretMatched[j] = true;
                        guessMatched[i] = true;
                        break;
                    }
                }

                if (!guessMatched[i])
                {
                    result[i] = 'B';
                }
            }

            return new string(result);
        }
    }
}
