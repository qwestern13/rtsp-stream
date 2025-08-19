using System.Diagnostics;
using System.Text;
namespace VideoFinder;

public static class RtspStreamer
{
    public static void StartRtspStream(string inputFile, int port)
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
            StreamWriter sw = new StreamWriter(fileDirectory.DirectoryName + "/rtsp-links.txt", true, Encoding.ASCII);
            sw.WriteLine($"Stream started: rtsp://localhost:{port}/{filename}");
            sw.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

    }
}