using System.Diagnostics;
using System.IO;

namespace SinalEscolar.Classes
{
    public static class MediaPlayer
    { 
        public static void Play(string songPath)
        {
            if (string.IsNullOrEmpty(songPath) || !File.Exists(songPath))
                return;

            Process.Start(songPath);
        }

        public static void StopPlaying()
        {
            var procs = Process.GetProcessesByName("wmplayer");
            foreach (var p in procs)
                p.Kill();
        }
    }
}
