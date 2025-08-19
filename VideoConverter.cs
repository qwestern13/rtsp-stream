namespace VideoFinder;

public class ConvertToMp4
{
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
}