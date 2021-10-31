using System.Reflection;

namespace QFXparser.Parsing
{
    internal class PropertyResult
    {
        public MemberInfo Member { get; set; }
        public NodeType Type { get; set; }
    }
}
