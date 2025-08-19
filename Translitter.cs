using System.Text;
namespace VideoFinder;

public static class Translitter
{
    public static string TranslitFileName(string fileName)
    {
        StringBuilder translitedFileName = new StringBuilder();

        foreach (char c in fileName)
        {
            if (TranslitMap.ContainsKey(c))
            {
                translitedFileName.Append(TranslitMap[c]);
            }
            else
            {
                translitedFileName.Append(c);
            }
        }

        return translitedFileName.ToString();

    }
    
    private static readonly Dictionary<char, string> TranslitMap = new Dictionary<char, string>
    {
        {'а', "a"},   {'б', "b"},   {'в', "v"},   {'г', "g"},   {'д', "d"},
        {'е', "e"},   {'ё', "e"},   {'ж', "zh"},  {'з', "z"},   {'и', "i"},
        {'й', "y"},   {'к', "k"},   {'л', "l"},   {'м', "m"},   {'н', "n"},
        {'о', "o"},   {'п', "p"},   {'р', "r"},   {'с', "s"},   {'т', "t"},
        {'у', "u"},   {'ф', "f"},   {'х', "kh"},  {'ц', "ts"},  {'ч', "ch"},
        {'ш', "sh"},  {'щ', "sch"}, {'ъ', ""},    {'ы', "y"},   {'ь', ""},
        {'э', "e"},   {'ю', "yu"},  {'я', "ya"},
        {'А', "A"},   {'Б', "B"},   {'В', "V"},   {'Г', "G"},   {'Д', "D"},
        {'Е', "E"},   {'Ё', "E"},   {'Ж', "Zh"},  {'З', "Z"},   {'И', "I"},
        {'Й', "Y"},   {'К', "K"},   {'Л', "L"},   {'М', "M"},   {'Н', "N"},
        {'О', "O"},   {'П', "P"},   {'Р', "R"},   {'С', "S"},   {'Т', "T"},
        {'У', "U"},   {'Ф', "F"},   {'Х', "Kh"},  {'Ц', "Ts"},  {'Ч', "Ch"},
        {'Ш', "Sh"},  {'Щ', "Sch"}, {'Ъ', ""},    {'Ы', "Y"},   {'Ь', ""},
        {'Э', "E"},   {'Ю', "Yu"},  {'Я', "Ya"},  {' ', "_"},  {'(', "_"},
        {')', "_"}
    };
}