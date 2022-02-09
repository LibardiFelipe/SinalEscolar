using System.Diagnostics;
using System.IO;

namespace SinalEscolar.Classes
{
    public static class MediaPlayer
    { 
        public static void Play(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            Process.Start(filePath);
        }

        public static void StopPlaying()
        {
            var procs = Process.GetProcessesByName("wmplayer");
            foreach (var p in procs)
                p.Kill();
        }
    }
}
