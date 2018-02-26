using System.Text;

namespace QFXparser
{
    public class Token
    {
        private StringBuilder _content;

        public Token()
        {
            _content = new StringBuilder();
        }

        public bool IsElement { get; set; } = false;

        public string Content
        {
            get
            {
                return _content.ToString().Trim();
            }
        }

        public void AddCharacter(char character)
        {
            _content.Append(character);
        }

        public void ClearToken()
        {
            _content.Clear();
            IsElement = false;
        }
    }
}
