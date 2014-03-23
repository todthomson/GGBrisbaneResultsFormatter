using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GGBrisbaneResultsFormatter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string inputFile = "2014-03-18.event";
            string input;

            using (var reader = new FileInfo(inputFile).OpenText())
            {
                input = reader.ReadToEnd();
            }

            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var output = new StringBuilder();

            output.AppendLine(string.Format("{0,-6}{1,-23}{2,-8}{3,-9}{4,-9}{5,-9}{6}", "Rank", "Name", "Points", "OMW%", "GW%", "OGW%", "Deck"));

            foreach (var line in lines)
            {
                var rank = string.Format("{0,-6}", new string(line.TakeWhile(MatchesRank).ToArray()).Trim());

                var name = string.Format("{0,-23}", new string(line.SkipWhile(MatchesRank).TakeWhile(MatchesName).ToArray()).Trim());

                var rest = new string(line.SkipWhile(MatchesRank).SkipWhile(MatchesName).ToArray());
                var tokens = rest.Split(); //split on char.IsWhiteSpace();

                var points = string.Format("{0,-8}", tokens[0]);
                var opponentMatchWinPercentage = string.Format("{0,-9}", tokens[1]);
                var gameWinPercentage = string.Format("{0,-9}", tokens[2]);
                var opponentGameWinPercentage = string.Format("{0,-9}", tokens[3]);

                output.AppendLine(string.Format("{0}{1}{2}{3}{4}{5}", rank, name, points, opponentMatchWinPercentage, gameWinPercentage,
                    opponentGameWinPercentage));
            }

            var parts = inputFile.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            using (var writer = new FileInfo(parts[0] + "_formatted." + parts[1]).CreateText())
            {
                writer.Write(output.ToString());
            }
        }

        private static Func<char, bool> MatchesName
        {
            get { return c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == ',' || c == '-' || c == '\''; }
        }

        private static Func<char, bool> MatchesRank
        {
            get { return c => char.IsDigit(c) || char.IsWhiteSpace(c); }
        }
    }
}
