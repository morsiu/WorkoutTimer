using System;
using System.Linq;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Timer.WorkoutTracking.Sound.NAudio
{
    public sealed class NAudioSoundFactory : ISoundFactory, IDisposable
    {
        private const int Channels = 1;
        private const int SampleRate = 44100;
        private readonly WaveOutEvent _outputDevice;
        private readonly MixingSampleProvider _mixer;
        private bool _disposed;
        private bool _initialized;

        public NAudioSoundFactory()
        {
            _outputDevice = new WaveOutEvent();
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels));
            _mixer.ReadFully = true;
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _outputDevice?.Dispose();
        }
        
        public ISound SeriesOfSound(Frequency frequency, TimeSpan duration, TimeSpan pause, int count)
        {
            ThrowWhenDisposed();
            Initialize();
            return new SeriesOfSampleSound(_mixer, frequency, duration, pause, count);
        }
        
        public ISound Sound(Frequency frequency, TimeSpan duration)
        {
            ThrowWhenDisposed();
            Initialize();
            return new SampleSound(_mixer, frequency, duration);
        }

        private void ThrowWhenDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(NAudioSoundFactory));
        }

        private void Initialize()
        {
            if (!_initialized)
            {
                _outputDevice.Init(_mixer);
                _outputDevice.Play();
                _initialized = true;
            }
        }

        private sealed class SampleSound : ISound
        {
            private readonly MixingSampleProvider _mixer;
            private readonly Frequency _frequency;
            private readonly TimeSpan _duration;

            public SampleSound(MixingSampleProvider mixer, Frequency frequency, TimeSpan duration)
            {
                _mixer = mixer;
                _frequency = frequency;
                _duration = duration;
            }

            public void PlayAsynchronously()
            {
                _mixer.AddMixerInput(
                    new OffsetSampleProvider(
                        new SignalGenerator(SampleRate, Channels)
                        {
                            Frequency = _frequency, Gain = 0.3, Type = SignalGeneratorType.Sin
                        })
                    {
                        Take = _duration
                    });
            }
        }

        private sealed class SeriesOfSampleSound : ISound
        {
            private readonly MixingSampleProvider _mixer;
            private readonly Frequency _frequency;
            private readonly TimeSpan _duration;
            private readonly TimeSpan _pause;
            private readonly int _count;

            public SeriesOfSampleSound(
                MixingSampleProvider mixer,
                Frequency frequency,
                TimeSpan duration,
                TimeSpan pause,
                int count)
            {
                _mixer = mixer;
                _frequency = frequency;
                _duration = duration;
                _pause = pause;
                _count = count;
            }

            private ISound SampleSound() => new SampleSound(_mixer, _frequency, _duration);

            public async void PlayAsynchronously()
            {
                var sample = SampleSound();
                foreach (var _ in Enumerable.Repeat(1, _count))
                {
                    sample.PlayAsynchronously();
                    await Task.Delay(_duration + _pause);
                }
            }
        }
    }
}