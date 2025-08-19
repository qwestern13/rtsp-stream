namespace VideoFinder;
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
        if (File.Exists(folder + "/rtsp-links.txt"))
        {
            File.Delete(folder + "/rtsp-links.txt");
        }
        // Проверяем наличие файлов с расширением .mcm, если есть конвертируем в .mp4
        var mcmFiles = Directory.GetFiles(folder, "*.mcm");
        if (mcmFiles.Length > 0)
        {
            foreach (var file in mcmFiles)
            {
                var originalFileName = Path.GetFileNameWithoutExtension(file);
                var outputFile = folder + "/" + originalFileName;
                VideoConverter.ConvertToMp4(file, outputFile);
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
                var translitedFileName = Translitter.TranslitFileName(originalFileName);
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
                RtspStreamer.StartRtspStream(file, port);
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

    }
}
