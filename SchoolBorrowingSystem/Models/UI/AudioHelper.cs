using System;
using System.IO;
using System.Media;

namespace SchoolBorrowingSystem.UI
{
    /// <summary>
    /// Presentation-layer utility providing reliable speaker audio cues and GTA Vice synth audio for Windows Forms.
    /// </summary>
    public static class AudioHelper
    {
        public static bool IsMuted { get; set; } = false;

        private static SoundPlayer _bgmPlayer;
        private static MemoryStream _bgmStream;

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
                    WriteWavHeader(writer, sampleRate, dataSize);

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

        /// <summary>
        /// Plays the full non-blocking GTA 6 loading audio sequence:
        /// 6-star neon whoosh + emblem bass drop + Vice City sunset synthwave pad & pulsing bass.
        /// If a file named "gta_loading.wav" exists next to the .exe, it plays that file instead.
        /// </summary>
        public static void PlayGtaLoadingSequence()
        {
            if (IsMuted) return;
            StopGtaLoadingAudio();

            try
            {
                string customWav = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gta_loading.wav");
                if (File.Exists(customWav))
                {
                    _bgmPlayer = new SoundPlayer(customWav);
                    _bgmPlayer.PlayLooping();
                    return;
                }

                // Synthesize a 12-second cinematic GTA 6 / Vice City soundtrack in memory
                int sampleRate = 44100;
                double totalSeconds = 12.0;
                int numSamples = (int)(sampleRate * totalSeconds);
                int dataSize = numSamples * 2;

                _bgmStream = new MemoryStream(44 + dataSize);
                using (var writer = new BinaryWriter(_bgmStream, System.Text.Encoding.ASCII, leaveOpen: true))
                {
                    WriteWavHeader(writer, sampleRate, dataSize);

                    var rng = new Random(6);
                    // Vice City minor chord progression (Am7 -> Fmaj7 -> Dm9 -> Em7)
                    double[][] chords =
                    {
                        new[] { 220.00, 261.63, 329.63, 392.00 }, // Am7
                        new[] { 174.61, 220.00, 261.63, 329.63 }, // Fmaj7
                        new[] { 146.83, 220.00, 261.63, 329.63 }, // Dm9
                        new[] { 164.81, 196.00, 246.94, 293.66 }  // Em7
                    };
                    double[] bassRoots = { 55.00, 43.65, 73.42, 82.41 }; // Deep sub-bass roots (A1, F1, D2, E2)

                    double lpNoise = 0.0;

                    for (int i = 0; i < numSamples; i++)
                    {
                        double t = (double)i / sampleRate;
                        double mix = 0.0;

                        // ACT 1 (0.0s - 1.8s): 6 Neon Stars Shimmer + Rising Whoosh
                        if (t < 1.9)
                        {
                            double starEnv = Math.Sin(Math.PI * Math.Min(1.0, t / 1.8));
                            double noise = (rng.NextDouble() * 2.0 - 1.0);
                            lpNoise = lpNoise * 0.92 + noise * 0.08;
                            mix += lpNoise * 0.35 * starEnv;

                            // 6 quick neon star chimes
                            for (int s = 0; s < 6; s++)
                            {
                                double st = s * 0.14;
                                if (t >= st && t < st + 0.35)
                                {
                                    double dt = t - st;
                                    double env = Math.Exp(-dt * 11.0);
                                    double freq = 520.0 + (s * 88.0);
                                    mix += Math.Sin(2.0 * Math.PI * freq * dt) * 0.16 * env;
                                }
                            }
                        }

                        // ACT 2 (1.85s - 3.6s): Rockstar-Style Emblem Deep Bass Drop
                        if (t >= 1.85 && t < 3.7)
                        {
                            double dt = t - 1.85;
                            double dropEnv = Math.Exp(-dt * 2.6);
                            double sweepFreq = 95.0 * Math.Exp(-dt * 1.4) + 38.0;
                            mix += Math.Sin(2.0 * Math.PI * sweepFreq * dt) * 0.65 * dropEnv;
                            mix += Math.Sin(2.0 * Math.PI * 196.0 * dt) * 0.12 * Math.Exp(-dt * 4.5);
                        }

                        // ACT 3 (3.2s - 12.0s): GTA 6 Vice City Sunset Synthwave (Pulsing Bass + Warm Analog Pads + Soft Arp)
                        if (t >= 3.2)
                        {
                            double st = t - 3.2;
                            double fadeIn = Math.Min(1.0, st / 1.2);
                            double fadeOut = Math.Min(1.0, (totalSeconds - t) / 0.8);
                            double masterEnv = fadeIn * fadeOut;

                            int bar = (int)(st / 2.2) % chords.Length;
                            double barPos = st % 2.2;

                            // Deep 80s sidechain-style pulsing sub bass
                            double beatPos = (st * 2.0) % 1.0; // 120 BPM half-time pulse
                            double pulseEnv = Math.Exp(-beatPos * 3.8);
                            double bassFreq = bassRoots[bar];
                            double bassWave = 0.7 * Math.Sin(2.0 * Math.PI * bassFreq * st) +
                                              0.3 * Math.Sin(2.0 * Math.PI * (bassFreq * 2.0) * st);
                            mix += bassWave * 0.42 * pulseEnv * masterEnv;

                            // Warm detuned analog pad chord
                            double pad = 0.0;
                            foreach (double f in chords[bar])
                            {
                                pad += Math.Sin(2.0 * Math.PI * f * st) * 0.55;
                                pad += Math.Sin(2.0 * Math.PI * (f * 1.004) * st) * 0.35; // Slight chorus detune
                            }
                            mix += (pad / 4.0) * 0.34 * masterEnv;

                            // Dreamy slow Vice City synth arpeggio
                            double arpStep = st * 4.0;
                            int noteIdx = (int)arpStep % 4;
                            double noteDt = (arpStep - Math.Floor(arpStep)) / 4.0;
                            double arpFreq = chords[bar][noteIdx] * 2.0;
                            double arpEnv = Math.Exp(-noteDt * 10.0);
                            mix += Math.Sin(2.0 * Math.PI * arpFreq * st) * 0.11 * arpEnv * masterEnv;

                            // Subtle tape swell on slide transitions
                            if (barPos < 0.35 && st > 1.0)
                            {
                                double n = (rng.NextDouble() * 2.0 - 1.0);
                                lpNoise = lpNoise * 0.94 + n * 0.06;
                                mix += lpNoise * 0.14 * (1.0 - (barPos / 0.35)) * masterEnv;
                            }
                        }

                        // Soft clip limiter so audio stays warm and never distorts
                        mix = Math.Tanh(mix * 1.15);
                        short sample = (short)(mix * short.MaxValue * 0.82);
                        writer.Write(sample);
                    }
                }

                _bgmStream.Position = 0;
                _bgmPlayer = new SoundPlayer(_bgmStream);
                _bgmPlayer.Play();
            }
            catch
            {
                // Fail silently if audio device is unavailable
            }
        }

        public static void StopGtaLoadingAudio()
        {
            try
            {
                _bgmPlayer?.Stop();
                _bgmPlayer?.Dispose();
                _bgmPlayer = null;

                _bgmStream?.Dispose();
                _bgmStream = null;
            }
            catch { }
        }

        private static void WriteWavHeader(BinaryWriter writer, int sampleRate, int dataSize)
        {
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
        }

        public static void PlayStartup()
        {
            PlayTone(523, 130, 0.85);
            PlayTone(659, 130, 0.85);
            PlayTone(784, 240, 0.90);
        }

        public static void PlayMenuClick()
        {
            PlayTone(880, 65, 0.65);
        }

        public static void PlaySuccess()
        {
            PlayTone(587, 130, 0.85);
            PlayTone(880, 240, 0.90);
        }

        public static void PlayWarning()
        {
            PlayTone(440, 170, 0.85);
            PlayTone(330, 250, 0.90);
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