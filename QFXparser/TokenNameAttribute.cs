using System;

namespace QFXparser
{
    internal class NodeNameAttribute : Attribute
    {
        private string _openTag;
        private string _closeTag;

        public NodeNameAttribute(string openTag, string closeTag = "")
        {
            _openTag = openTag;
            _closeTag = closeTag;
        }

        public string OpenTag
        {
            get
            {
                return _openTag;
            }
        }

        public string CloseTag
        {
            get
            {
                return _closeTag;
            }
        }
    }
}