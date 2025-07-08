using System;
using System.Diagnostics;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        int port = 8554;

        if (args.Length == 0)
        {
            Console.WriteLine("Please specify the path to the directory with MP4 files");
            return;
        }

        string folder = args[0];

        if (!Directory.Exists(folder))
        {
            Console.WriteLine("Folder doesn't exist!");
            return;
        }
        // Проверяем есть ли текстовый файл с rtsp ссылками rtsp.txt
        if (File.Exists(folder + "/rtsp.txt"))
        {
            File.Delete(folder + "/rtsp.txt");
        }
        // Проверяем наличие файлов с расширением .mcm, если есть конвертируем в .mp4
        var mcmFiles = Directory.GetFiles(folder, "*.mcm");
        if (mcmFiles.Length > 0)
        {
            foreach (var file in mcmFiles)
            {
                var originalFileName = Path.GetFileNameWithoutExtension(file);
                var outputFile = folder + "/" + originalFileName;
                ConvertToMp4(file, outputFile);
            }
        }
        // Ищем файлы .mp4 
        var mp4Files = Directory.GetFiles(folder, "*.mp4");
        if (mp4Files.Length == 0)
        {
            Console.WriteLine("No MP4 or MCM files found!");
        }
        else
        {
            // Проверяем наличие имен файлов на кириллице, если такие есть переименовываем на транслите
            foreach (var file in mp4Files)
            {
                var originalFileName = Path.GetFileName(file);
                var translitedFileName = TranslitFileName(originalFileName);
                // Проверяем существует ли файл в директории с таким же именем как после транслитерации
                // Если существует, добавляем DateTime в начало имени
                if (File.Exists(folder + "/" + translitedFileName) && originalFileName != translitedFileName)
                {
                    System.IO.File.Move(folder + "/" + originalFileName, folder + "/" + DateTime.Now.ToString("yy-dd-M-HH-mm-ss") + translitedFileName);
                }
                else
                {
                    System.IO.File.Move(folder + "/" + originalFileName, folder + "/" + translitedFileName);
                }
            }
            
            var mp4FilesAfterTranslit = Directory.GetFiles(folder, "*.mp4");
            
            foreach (var file in mp4FilesAfterTranslit)
            {
                StartRtspStream(file, port);
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

    }

    static void ConvertToMp4(string inputFile, string outputFile)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{inputFile}\" -vcodec copy -acodec copy \"{outputFile}.mp4\"",
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();
        Console.WriteLine($"Convert completed: {outputFile}");
    }

    static void StartRtspStream(string inputFile, int port)
    {
        var filename = Path.GetFileName(inputFile);
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments =
                $"-re -stream_loop -1 -i \"{inputFile}\" -c:v libx264 -preset ultrafast -pix_fmt yuv420p -c:a copy -movflags +faststart -f rtsp rtsp://rtsp-server:{port}/{filename}",
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var process = new Process { StartInfo = processStartInfo };
        process.Start();
        
        try
        {
            FileInfo fileDirectory = new FileInfo(inputFile);
            StreamWriter sw = new StreamWriter(fileDirectory.DirectoryName + "/rtsp.txt", true, Encoding.ASCII);
            sw.WriteLine($"Stream started: rtsp://localhost:{port}/{filename}");
            sw.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

        //Console.WriteLine($"Stream started: rtsp://localhost:{port}/{filename}");

    }

    static string TranslitFileName(string fileName)
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
