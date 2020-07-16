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

namespace YYZ.Parse
{
    enum TokenType
    {
        IDENT,
        INTCONST,

        //keywords
        EXPORTS,
        PROC,
        VAR,

        //punctuation
        LPAREN,
        RPAREN,
        COLON,
        SEMICOLON,
        LBRACE,
        RBRACE,
        EQUAL,

        EOF
    }

    class Token
    {
        string[] TOKENNAME = { "ident", "int const", "EXPORTS", "PROC", "VAR", "(", ")", ":", ";", "{", "}", "=", "eof" };

        public TokenType ttype;
        public Location loc;
        public string ident;
        public long intval;

        public Token(TokenType _ttype, Location _loc)
        {
            ttype = _ttype;
            loc = _loc;
        }

        public override string ToString()
        {
            string val = "";
            switch (ttype)
            {
                case TokenType.IDENT:
                    val = "(" + ident + ")";
                    break;
                case TokenType.INTCONST:
                    val = "(" + intval + ")";
                    break;
            }
            return "token = " + TOKENNAME[(int)ttype] + val + " " + loc.ToString();
        }
    }

    class Location
    {
        int line;
        int col;

        public Location(int _line, int _col)
        {
            line = _line;
            col = _col;
        }

        public override string ToString()
        {
            return "location = (" + line + "," + col + ")";
        }

    }
}
