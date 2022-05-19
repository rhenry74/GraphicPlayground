using SharpDX;
using SharpDX.DirectSound;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public class DialogHostMediator : IHost
    {
        private Form _Form;
        private Dictionary<string,string> _Stats;
        private DirectSound _DirectSound;

        public DialogHostMediator(Form form, DirectSound directSound)
        {
            _Form = form;
            _Stats = new Dictionary<string, string>();
            _DirectSound = directSound;
        }

        public int Width => _Form.Width;

        public int Height => _Form.Height;

        public Dictionary<string, string> Stats => _Stats;
        public void SetStatictic(string key, string val)
        {
            if (_Stats.ContainsKey(key))
            {
                _Stats[key] = val;
            }
            else
            {
                _Stats.Add(key, val);
            }
        }

        public void SetStatictic(string key, double val)
        {
            var s = string.Format("{0}", val);
            if (_Stats.ContainsKey(key))
            {
                _Stats[key] = s;
            }
            else
            {
                _Stats.Add(key, s);
            }
        }

        private Queue<MouseEventArgs> MouseQueue = new Queue<MouseEventArgs>();
        public void AddMouseEvent(MouseEventArgs e)
        {
            MouseQueue.Enqueue(e);
        }

        public MouseEventArgs PeakMouse()
        {
            if (MouseQueue.Count == 0)
            {
                return null;
            }
            return MouseQueue.Peek();
        }

        public MouseEventArgs DequeueMouse()
        {
            return MouseQueue.Dequeue();
        }

        Dictionary<string, SecondarySoundBuffer> _SoundBuffers = new Dictionary<string, SecondarySoundBuffer>();

        public string LoadWav(string fileName)
        {
            //// Default WaveFormat Stereo 44100 16 bit
            WaveFormat waveFormat = new WaveFormat();

            // Create SecondarySoundBuffer
            var secondaryBufferDesc = new SoundBufferDescription();
            secondaryBufferDesc.BufferBytes = waveFormat.ConvertLatencyToByteSize(60000);
            secondaryBufferDesc.Format = waveFormat;
            secondaryBufferDesc.Flags = BufferFlags.GetCurrentPosition2 | BufferFlags.ControlPositionNotify | BufferFlags.GlobalFocus |
                                        BufferFlags.ControlVolume | BufferFlags.StickyFocus;
            secondaryBufferDesc.AlgorithmFor3D = Guid.Empty;
            var secondarySoundBuffer = new SecondarySoundBuffer(_DirectSound, secondaryBufferDesc);

            // Get Capabilties from secondary sound buffer
            var capabilities = secondarySoundBuffer.Capabilities;

            // Lock the buffer
            DataStream dataPart2;
            var dataPart1 = secondarySoundBuffer.Lock(0, capabilities.BufferBytes, LockFlags.EntireBuffer, out dataPart2);

            // Fill the buffer with some sound
            int numberOfSamples = capabilities.BufferBytes / waveFormat.BlockAlign;
            for (int i = 0; i < numberOfSamples; i++)
            {
                double vibrato = Math.Cos(2 * Math.PI * 10.0 * i / waveFormat.SampleRate);
                short value = (short)(Math.Cos(2 * Math.PI * (220.0 + 4.0 * vibrato) * i / waveFormat.SampleRate) * 16384); // Not too loud
                dataPart1.Write(value);
                dataPart1.Write(value);
            }

            // Unlock the buffer
            secondarySoundBuffer.Unlock(dataPart1, dataPart2);

            _SoundBuffers.Add(Path.GetFileNameWithoutExtension(fileName), secondarySoundBuffer);
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public void PlayBuffer(string name)
        {
            // Play the song
            _SoundBuffers[name].Play(0, PlayFlags.TerminateByTime);
        }
    }

    
}
