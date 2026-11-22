using System;
using System.IO;
using System.Media;

namespace SchoolBorrowingSystem.UI
{
    /// <summary>
    /// Presentation-layer utility providing reliable speaker audio cues for Windows Forms.
    /// </summary>
    public static class AudioHelper
    {
        public static bool IsMuted { get; set; } = false;

        private static void PlayTone(int frequency, int durationMs, double volume = 0.85)
        {
            if (IsMuted) return;

            try
            {
                int sampleRate = 44100;
                int numSamples = (int)((sampleRate * durationMs) / 1000.0);
                int dataSize = numSamples * 2;

                using (var ms = new MemoryStream(44 + dataSize))
                using (var writer = new BinaryWriter(ms))
                {
                    // Standard 44-byte WAV header
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                    writer.Write(36 + dataSize);
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                    writer.Write(16);
                    writer.Write((short)1); // PCM
                    writer.Write((short)1); // Mono
                    writer.Write(sampleRate);
                    writer.Write(sampleRate * 2);
                    writer.Write((short)2);
                    writer.Write((short)16);
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                    writer.Write(dataSize);

                    double step1 = (2.0 * Math.PI * frequency) / sampleRate;
                    double step2 = (2.0 * Math.PI * (frequency * 2)) / sampleRate;

                    for (int i = 0; i < numSamples; i++)
                    {
                        double envelope = Math.Sin(Math.PI * Math.Min(1.0, (double)i / (numSamples * 0.08))) *
                                          (1.0 - ((double)i / numSamples));
                        double wave = (0.75 * Math.Sin(i * step1)) + (0.25 * Math.Sin(i * step2));
                        short sample = (short)(wave * short.MaxValue * volume * envelope);
                        writer.Write(sample);
                    }

                    ms.Position = 0;
                    using (var player = new SoundPlayer(ms))
                    {
                        player.PlaySync();
                    }
                }
            }
            catch
            {
                SystemSounds.Asterisk.Play();
            }
        }

        public static void PlayStartup()
        {
            PlayTone(523, 130, 0.85); // C5
            PlayTone(659, 130, 0.85); // E5
            PlayTone(784, 240, 0.90); // G5
        }

        public static void PlayMenuClick()
        {
            PlayTone(880, 65, 0.65);
        }

        public static void PlaySuccess()
        {
            PlayTone(587, 130, 0.85); // D5
            PlayTone(880, 240, 0.90); // A5
        }

        public static void PlayWarning()
        {
            PlayTone(440, 170, 0.85); // A4
            PlayTone(330, 250, 0.90); // E4
        }

        public static void PlayError()
        {
            PlayTone(200, 170, 0.90);
            PlayTone(160, 250, 0.90);
        }

        public static void PlayTestVictory()
        {
            PlayTone(523, 110, 0.85);
            PlayTone(659, 110, 0.85);
            PlayTone(784, 110, 0.85);
            PlayTone(1046, 280, 0.90);
        }
    }
}
