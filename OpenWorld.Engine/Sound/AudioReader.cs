﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace OpenWorld.Engine.Sound
{
    /// <summary>
    /// Defines a pack of data to return to the user.
    /// </summary>
    public class AudioData
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="data">The raw audio data</param>
        /// <param name="format">The format of the audio data</param>
        /// <param name="frequency">The frequency of the audio data</param>
        public AudioData(byte[] data, ALFormat format,int frequency)
        {
            Buffer = data;
            Format = format;
            Frequency = frequency;
        }

        public Byte[] Buffer { get; private set; }
        public ALFormat Format { get; private set; }
        public int Frequency { get; private set; }
    }
    
    /// <summary>
    /// Reader reading audio data from a stream.
    /// </summary>
    public abstract class AudioReader : BinaryReader
    {
        protected AudioReader(Stream input) : base(input) { }
        protected AudioReader(Stream input, Encoding encoding) : base(input, encoding) { }

        /// <summary>
        /// Reads the audio data in the stream.
        /// </summary>
        /// <returns>Raw Audio Buffer</returns>
        public abstract AudioData ReadAudioData();

        
    }
}