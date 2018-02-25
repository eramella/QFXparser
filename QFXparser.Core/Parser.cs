using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QFXparser.Core
{
    public class Parser
    {
        public static IEnumerable<Token> Parse(string fileText)
        {

            Token token = new Token();

            foreach (var character in fileText)
            {
                if (character == '<')
                {
                    if (!token.IsElement)
                    {
                        if (!string.IsNullOrEmpty(token.Content))
                        {
                            yield return token;
                        }
                        token.ClearToken();
                    }

                    token.IsElement = true;
                }
                else if (character == '>')
                {
                    if (!string.IsNullOrEmpty(token.Content))
                    {
                        yield return token;
                    }
                    token.ClearToken();
                }
                else
                {
                    token.AddCharacter(character);
                }
            }
        }
    }
}
