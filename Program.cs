using System;
using System.Diagnostics;
using System.IO;

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
       
        var mcmFiles = Directory.GetFiles(folder, "*.mcm");
        if (mcmFiles.Length > 0)
        {
            foreach (var file in mcmFiles)
            {
                var originalFileName = Path.GetFileNameWithoutExtension(file);
                var outputFile = folder + "/" +  originalFileName;
                ConvertToMp4(file, outputFile);
            } 
        }
        
        var mp4Files = Directory.GetFiles(folder, "*.mp4");
        if (mp4Files.Length == 0)
        {
            Console.WriteLine("No MP4 files found!");
        }
        else
        {
            foreach (var file in mp4Files)
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
            Arguments = $"-re -stream_loop -1 -i \"{inputFile}\" -c:v libx264 -preset ultrafast -pix_fmt yuv420p -c:a copy -movflags +faststart -f rtsp rtsp://rtsp-server:{port}/{filename}",
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var process = new Process { StartInfo = processStartInfo };
        process.Start();

        Console.WriteLine($"Stream started: rtsp://localhost:{port}/{filename}");
        
    }
}
