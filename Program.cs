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
        foreach (var mcmFile in mcmFiles)
        {
            File.Move(mcmFile, mcmFile.Replace(".mcm", ".mp4"), true);
        }

        var mp4Files = Directory.GetFiles(folder, "*.mp4");
        
        //var allFiles = mp4Files.Union(mcmFiles).ToArray();
        
        if (mp4Files.Length == 0)
        {
            Console.WriteLine("No files found!");
        }
        else
        {
            foreach (var file in mp4Files)
            {
                string filename = Path.GetFileName(file);
                string rtspUrl = $"rtsp://localhost:{port}/{filename}";

                Console.WriteLine("Video file: " + filename);
                Console.WriteLine(rtspUrl);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-re -stream_loop -1 -i \"{file}\" -c:v libx264 -preset ultrafast -pix_fmt yuv420p -c:a copy -f rtsp rtsp://rtsp-server:{port}/{filename}",
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
		System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
    }
}
