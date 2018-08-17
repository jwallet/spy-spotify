using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Models;
using NAudio.Lame;
using NAudio.Wave;
using File = System.IO.File;

namespace EspionSpotify
{
    public interface IRecorder
    {
        int CountSeconds { get; set; }
        bool Running { get; set; }

        void Run();
    }
}