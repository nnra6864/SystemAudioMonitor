using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace NnUtils.Modules.SystemAudioMonitor
{
    public class PulseAudioMonitor : AudioMonitor
    {
        private Thread _thread;
        private Process _paMonitorProcess;

        public readonly string Name;
        public readonly string StreamName;
        public readonly string Device;
        public readonly int Volume;
        public readonly int Rate;
        public readonly int BufferSize;
        public readonly int UpdateInterval;

        public PulseAudioMonitor(string name = "", string streamName = "Output Device Monitor", string device = "@DEFAULT_MONITOR@",
            int volume = 65536, int rate = 44100, int bufferSize = 2048, int updateInterval = 100)
        {
            Name           = name;
            StreamName     = streamName;
            Device         = device;
            Volume         = volume;
            Rate           = rate;
            BufferSize     = bufferSize;
            UpdateInterval = updateInterval;
        }

        public override void Start()
        {
            if (_thread is { IsAlive: true }) return;

            _paMonitorProcess = new()
            {
                StartInfo = new()
                {
                    FileName               = "parec",
                    Arguments              = $"-n '{Name}' --stream-name '{StreamName}' -d {Device} --volume {Volume} --rate={Rate} --channels=1 --format=float32le --latency-msec=1",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                }
            };

            _paMonitorProcess.Start();

            _thread = new(() => MonitorAudio(BufferSize));
            _thread.Start();
            //Task.Run(() => MonitorAudio(_cancellationTokenSource.Token, BufferSize), _cancellationTokenSource.Token);
        }

        private void MonitorAudio(int bufferSize)
        {
            var stream = _paMonitorProcess.StandardOutput.BaseStream;
            var buffer = new byte[bufferSize];

            try
            {
                while (true)
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

        private void StopMonitoring()
        {
            if (_thread is not { IsAlive: true }) return;

            _thread.Abort();
            _paMonitorProcess.Kill();
            _paMonitorProcess.Dispose();
        }

        public override void Dispose()
        {
            StopMonitoring();
            Debug.Log("HERE");
        }
    }
}