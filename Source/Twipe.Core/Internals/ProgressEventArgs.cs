using System;

namespace Twipe.Core.Internals
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(float progress)
        {
            Progress = progress;
        }

        public ProgressEventArgs(float progress, string message)
        {
            Progress = progress;
            Message = message;
        }

        public float Progress { get; protected set; }

        public string Message { get; protected set; }
    }
}