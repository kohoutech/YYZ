/* ----------------------------------------------------------------------------
YYZ - a source code compiler
Copyright (C) 1997-2020  George E Greaney

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YYZ.Parse
{
    class Scanner
    {
        string filename;
        byte[] srcbuf;
        int srcpos;
        int line;
        int col;

        Dictionary<string, TokenType> keywordTable;

        public Scanner(string _filename)
        {
            filename = _filename;
            
            keywordTable = new Dictionary<string, TokenType>();
            keywordTable["exports"] = TokenType.EXPORTS;
            keywordTable["proc"] = TokenType.PROC;
            keywordTable["var"] = TokenType.VAR;
        }

        public bool openSource()
        {
            try
            {
                srcbuf = File.ReadAllBytes(filename);
                srcpos = 0;
                line = 1;
                col = 1;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public Token getToken()
        {
            char ch;
            Token tok = null;

            //skip any whitespace to next token 
            bool done = srcpos >= srcbuf.Length;
            while (!done)
            {
                ch = (char)srcbuf[srcpos];
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\x0a':
                    case '\x0d':
                        srcpos++;
                        col++;
                        if (ch == '\x0a')
                        {
                            line++;
                            col = 1;
                        }
                        done = srcpos >= srcbuf.Length;
                        break;
                    default:
                        done = true;
                        break;
                }

            }

            Location loc = new Location(line, col);
            if (srcpos == srcbuf.Length)
            {
                return new Token(TokenType.EOF, loc);
            }

            ch = (char)srcbuf[srcpos++];
            col++;
            if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch == '_'))
            {
                StringBuilder sb = new StringBuilder();
                done = false;
                do
                {
                    sb.Append(ch);
                    if (srcpos < srcbuf.Length)
                    {
                        ch = (char)srcbuf[srcpos++];
                        col++;
                    }
                    else
                    {
                        done = true;
                    }
                } while ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || (ch == '_'));

                srcpos--;
                col--;
                string ident = sb.ToString();

                if (keywordTable.ContainsKey(ident))
                {
                    tok = new Token(keywordTable[ident], loc);
                }
                else
                {
                    tok = new Token(TokenType.IDENT, loc);
                    tok.ident = ident;
                }
            }
            else if (ch >= '0' && ch <= '9')
            {
                StringBuilder sb = new StringBuilder();
                done = false;
                do
                {
                    sb.Append(ch);
                    if (srcpos < srcbuf.Length)
                    {
                        ch = (char)srcbuf[srcpos++];
                        col++;
                    }
                    else
                    {
                        done = true;
                    }
                } while (ch >= '0' && ch <= '9');

                srcpos--;
                col--;
                long val = Int64.Parse(sb.ToString());
                tok = new Token(TokenType.INTCONST, loc);
                tok.intval = val;
            }
            else switch (ch)
                {
                    case '(':
                        tok = new Token(TokenType.LPAREN, loc);
                        break;
                    case ')':
                        tok = new Token(TokenType.RPAREN, loc);
                        break;
                    case '{':
                        tok = new Token(TokenType.LBRACE, loc);
                        break;
                    case '}':
                        tok = new Token(TokenType.RBRACE, loc);
                        break;
                    case ':':
                        tok = new Token(TokenType.COLON, loc);
                        break;
                    case ';':
                        tok = new Token(TokenType.SEMICOLON, loc);
                        break;
                    case '=':
                        tok = new Token(TokenType.EQUALS, loc);
                        break;
                }

            if (srcpos < srcbuf.Length)
            {
            }
            return tok;
        }
    }
}
