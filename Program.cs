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
        }

        var files = Directory.GetFiles(folder, "*.mp4");
        
        if (files.Length == 0)
        {
            Console.WriteLine("No files found!");
        }
        else
        {
            foreach (var file in files)
            {
                string filename = Path.GetFileName(file);
                string rtspUrl = $"rtsp://localhost:{port}/{filename}";

                Console.WriteLine("Video file: " + filename);
                Console.WriteLine(rtspUrl);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-re -i \"{file}\" -c:v copy -c:a copy -f rtsp rtsp://rtsp-server:{port}/{filename}",
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
