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

using YYZ.OIL;

namespace YYZ.Parse
{
    public class Parser
    {
        string filename;
        Scanner scanner;

        public OilModule module;
        OilProc curProc;

        SymTblStack symstack;

        public Parser(string _filename)
        {
            filename = _filename;
            scanner = null;
            module = null;
            curProc = null;

            symstack = new SymTblStack();
            SymbolTable globalsyms = new SymbolTable();
            symstack.push(globalsyms);

            //initialize build in types
            globalsyms.add("void", new OilType(OilType.Typ.VOID));
            globalsyms.add("int", new OilType(OilType.Typ.INT));
            globalsyms.add("float", new OilType(OilType.Typ.FLOAT));
            globalsyms.add("char", new OilType(OilType.Typ.CHAR));
        }


        public OilModule parseProgram()
        {
            scanner = new Scanner(filename);
            scanner.openSource();
            scanner.getToken();

            parseModule();
            module.spill("oil.spill.txt");
            return module;
        }

        public void parseModule()
        {
            module = new OilModule(filename);

            bool done = false;
            do
            {
                switch (scanner.token.ttype)
                {
                    case TokenType.PROC:
                        parseProcDecl();
                        break;
                    case TokenType.EXPORTS:
                        parseExports();
                        break;
                    case TokenType.EOF:
                        done = true;
                        break;
                }
            } while (!done);
        }

        public void parseVarDecl()
        {
            scanner.consume(TokenType.VAR);

            while (scanner.token.ttype == TokenType.IDENT)
            {
                OilVar vardef = new OilVar();
                vardef.name = scanner.token.ident;
                symstack.top().add(vardef.name, vardef);
                scanner.consume(TokenType.IDENT);

                scanner.consume(TokenType.COLON);

                string typename = scanner.token.ident;
                OilType vartype = (OilType)symstack.find(typename);
                vardef.typ = vartype;
                scanner.consume(TokenType.IDENT);

                scanner.consume(TokenType.SEMICOLON);
                if (curProc != null)
                {
                    curProc.varList.Add(vardef);
                }
                else
                {
                    module.varList.Add(vardef);
                }
            }
        }

        public void parseProcDecl()
        {
            curProc = new OilProc();

            scanner.consume(TokenType.PROC);

            curProc.name = scanner.token.ident;
            symstack.top().add(curProc.name, curProc);
            scanner.consume(TokenType.IDENT);

            scanner.consume(TokenType.LPAREN);

            scanner.consume(TokenType.RPAREN);

            scanner.consume(TokenType.COLON);

            string typename = scanner.token.ident;
            OilType rettype = (OilType)symstack.find(typename);
            curProc.retType = rettype;
            scanner.consume(TokenType.IDENT);

            parseProcBody();

            module.procList.Add(curProc);
            curProc = null;
        }

        public void parseProcBody()
        {
            bool done = false;
            do
            {
                switch (scanner.token.ttype)
                {
                    case TokenType.VAR:
                        parseVarDecl();
                        break;
                    case TokenType.LBRACE:
                        done = true;
                        break;
                }
            } while (!done);

            parseBlock();
        }

        private void parseBlock()
        {
            OilBlock block = new OilBlock();
            scanner.consume(TokenType.LBRACE);

            while (scanner.token.ttype != TokenType.RBRACE)
            {
                OilStatement stmt = parseStatement();
                block.stmtList.Add(stmt);
            }
            scanner.consume(TokenType.RBRACE);
            curProc.block = block;
        }

        public OilStatement parseStatement()
        {
            OilStatement s1 = parseAssignment();
            scanner.consume(TokenType.SEMICOLON);
            return s1;
        }

        private OilAssign parseAssignment()
        {
            OilExpression e1 = parseExpression();
            scanner.consume(TokenType.EQUAL);
            OilExpression e2 = parseExpression();

            OilAssign s1 = new OilAssign(e1, e2);
            return s1;
        }

        public OilExpression parseExpression()
        {
            OilExpression e1 = null;
            switch (scanner.token.ttype)
            {
                case TokenType.IDENT:
                    {
                        e1 = new OilVarRef(scanner.token.ident);
                        scanner.consume(TokenType.IDENT);
                        break;
                    }
                case TokenType.INTCONST:
                    {
                        e1 = new OilIntConst(scanner.token.intval);
                        scanner.consume(TokenType.INTCONST);
                        break;
                    }
            }
            return e1;
        }

        public void parseExports()
        {
            scanner.consume(TokenType.EXPORTS);
            
            module.exportList.Add(scanner.token.ident);
            scanner.consume(TokenType.IDENT);

            scanner.consume(TokenType.SEMICOLON);
        }
    }
}
