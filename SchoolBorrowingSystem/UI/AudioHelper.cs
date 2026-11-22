using System;
using System.IO;
using System.Media;

namespace SchoolBorrowingSystem.UI
{
    /// <summary>
    /// Plays short beeps for success / warning / error.
    ///
    /// The old version played each note with PlaySync(), which FROZE the window
    /// until the sound finished (and it beeped on every tab click).
    /// Now all notes of a sound are put into one WAV file in memory and played
    /// in the background with Play(), so the app never waits for the sound.
    /// </summary>
    public static class AudioHelper
    {
        private const int SampleRate = 44100;

        public static bool IsMuted { get; set; }

        // Keep a reference so the player isn't cleaned up while it's still playing
        private static SoundPlayer _player;

        public static void PlayStartup()
        {
            PlayNotes(new int[] { 523, 659, 784 }, new int[] { 110, 110, 200 });
        }

        public static void PlaySuccess()
        {
            PlayNotes(new int[] { 587, 880 }, new int[] { 100, 180 });
        }

        public static void PlayWarning()
        {
            PlayNotes(new int[] { 440, 330 }, new int[] { 140, 200 });
        }

        public static void PlayError()
        {
            PlayNotes(new int[] { 220, 165 }, new int[] { 140, 200 });
        }

        public static void PlayTestVictory()
        {
            PlayNotes(new int[] { 523, 659, 784, 1046 }, new int[] { 90, 90, 90, 240 });
        }

        /// <summary>
        /// frequencies[i] is played for durations[i] milliseconds, one after another.
        /// </summary>
        private static void PlayNotes(int[] frequencies, int[] durations)
        {
            if (IsMuted)
            {
                return;
            }

            try
            {
                // Step 1: count how many samples all the notes need
                int totalSamples = 0;
                for (int i = 0; i < durations.Length; i++)
                {
                    totalSamples += SampleRate * durations[i] / 1000;
                }

                // Step 2: write the WAV header + the sound data into memory
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                WriteWavHeader(writer, totalSamples);

                for (int i = 0; i < frequencies.Length; i++)
                {
                    WriteTone(writer, frequencies[i], durations[i]);
                }

                writer.Flush();
                stream.Position = 0;

                // Step 3: play it without blocking the window
                if (_player != null)
                {
                    _player.Stop();
                    _player.Dispose();
                }
                _player = new SoundPlayer(stream);
                _player.Play();
            }
            catch (Exception)
            {
                // Sound is just a nice extra. If the PC has no audio device, ignore it.
            }
        }

        // Standard 44-byte header for 16-bit mono PCM audio
        private static void WriteWavHeader(BinaryWriter writer, int sampleCount)
        {
            int dataSize = sampleCount * 2;

            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataSize);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);       // PCM format
            writer.Write((short)1);       // mono
            writer.Write(SampleRate);
            writer.Write(SampleRate * 2); // bytes per second
            writer.Write((short)2);       // bytes per sample
            writer.Write((short)16);      // bits per sample
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);
        }

        private static void WriteTone(BinaryWriter writer, int frequency, int durationMs)
        {
            int samples = SampleRate * durationMs / 1000;
            double volume = 0.5;

            for (int i = 0; i < samples; i++)
            {
                // Fade in quickly and fade out slowly so it doesn't "click"
                double progress = (double)i / samples;
                double fade = Math.Min(1.0, progress * 12) * (1.0 - progress);

                double wave = 0.75 * Math.Sin(2 * Math.PI * frequency * i / SampleRate)
                            + 0.25 * Math.Sin(2 * Math.PI * frequency * 2 * i / SampleRate);

                writer.Write((short)(wave * fade * volume * short.MaxValue));
            }
        }
    }
}
