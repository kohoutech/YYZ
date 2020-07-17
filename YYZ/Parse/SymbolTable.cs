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
    public class SymbolTable
    {
        public Dictionary<string, OilNode> symtbl;

        public SymbolTable()
        {
            symtbl = new Dictionary<string, OilNode>();
        }

        public void add(string sym, OilNode def)
        {
            symtbl[sym] = def;
        }

        public OilNode find(string sym)
        {
            if (symtbl.ContainsKey(sym))
            {
                return symtbl[sym];
            }
            return null;
        }
    }

    public class SymTblStack
    {
        public List<SymbolTable> stack;

        public SymTblStack()
        {
            stack = new List<SymbolTable>();
        }

        public SymbolTable top()
        {
            return stack[stack.Count - 1];
        }

        public void push(SymbolTable tbl)
        {
            stack.Add(tbl);
        }

        public SymbolTable pop()
        {
            SymbolTable tbl = null;
            if (stack.Count > 0)
            {
                tbl = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
            }
            return tbl;
        }

        public OilNode find(string sym)
        {
            OilNode def = null;
            int level = stack.Count-1;
            bool found = false;
            do
            {
                SymbolTable tbl = stack[level];
                def = tbl.find(sym);
                level--;
                found = (def != null) || (level >= 0);

            } while (!found);
            return def;
        }
    }
}
