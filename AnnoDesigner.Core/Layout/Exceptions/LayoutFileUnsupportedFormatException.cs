using System;

namespace AnnoDesigner.Core.Layout.Exceptions
{
    public class LayoutFileUnsupportedFormatException : Exception
    {
        public LayoutFileUnsupportedFormatException()
        {
        }

        public LayoutFileUnsupportedFormatException(string message)
            : base(message)
        {
        }

        public LayoutFileUnsupportedFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
