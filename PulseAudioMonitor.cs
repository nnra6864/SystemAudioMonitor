using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class PulseAudioMonitor
    {
        private Process _paMonitorProcess;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;

        public float Loudness { get; private set; }

        public async Task StartMonitoring(string name = "", string streamName = "Output Device Monitor", string device = "@DEFAULT_MONITOR@", int volume = 65536, int rate = 44100, int bufferSize = 2048)
        {
            if (_isRunning) return;
            _isRunning               = true;
            _cancellationTokenSource = new();

            _paMonitorProcess = new()
            {
                StartInfo = new()
                {
                    FileName               = "parec",
                    Arguments              = $"-n '{name}' --stream-name '{streamName}' -d {device} --volume {volume} --rate={rate} --channels=1 --format=float32le --latency-msec=1",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                }
            };

            _paMonitorProcess.Start();

            await Task.Run(() => MonitorAudio(_cancellationTokenSource.Token, bufferSize), _cancellationTokenSource.Token);
        }

        private void MonitorAudio(CancellationToken token, int bufferSize)
        {
            var stream = _paMonitorProcess.StandardOutput.BaseStream;
            var buffer = new byte[bufferSize];

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) continue;

                    float volume = GetAudioLevel(buffer, bytesRead);
                    Loudness = volume;
                }
            }
            catch (IOException)
            {
                
            }
            finally
            {
                _isRunning = false;
            }
        }

        private float GetAudioLevel(byte[] buffer, int length)
        {
            var sampleCount = length / 4; // 32-bit (4 bytes per sample)
            float maxSample = 0;

            for (int i = 0; i < sampleCount; i++)
            {
                var sample = BitConverter.ToSingle(buffer, i * 4);
                maxSample = Math.Max(maxSample, Math.Abs(sample));
            }

            return maxSample;
        }

        public void StopMonitoring()
        {
            if (!_isRunning) return;
            _cancellationTokenSource?.Cancel();

            if (!_paMonitorProcess.HasExited)
            {
                _paMonitorProcess.Kill();
                _paMonitorProcess.Dispose();
            }

            _isRunning = false;
        }
    }
}