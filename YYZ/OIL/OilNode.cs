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

using YYZ.Parse;

namespace YYZ.OIL
{
    public enum OilNodeType
    {
        MODULE,
        VAR,
        PARAM,
        PROC,
        TYPE,
        BLOCK,
        ASSIGN,
        VARREF,
        INTCONST
    }

    public class OilNode
    {
        public OilNodeType type;
        public Location loc;

        public virtual void printNode(StreamWriter oilfile)
        {
            //nothing
        }
    }

    //- declaractions ---------------------------------------------------------

    public class OilModule : OilNode
    {
        public string filename;
        public List<OilProc> procList;
        public List<OilVar> varList;
        public List<string> exportList;

        public OilModule(string fn)
        {
            type = OilNodeType.MODULE;
            filename = fn;
            varList = new List<OilVar>();
            procList = new List<OilProc>();
            exportList = new List<string>();
        }

        public void spill(string fn)
        {
            StreamWriter oilfile = new StreamWriter(fn);
            this.printNode(oilfile);
            oilfile.Close();
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(module " + filename + ")");
            oilfile.WriteLine("--------------------------------------------------");
            foreach (OilVar ov in varList)
            {
                ov.printNode(oilfile);
            }
            oilfile.WriteLine("--------------------------------------------------");
            foreach (OilProc op in procList)
            {
                op.printNode(oilfile);
            }
            oilfile.WriteLine("--------------------------------------------------");
            oilfile.WriteLine("(exports)");
            foreach (string exp in exportList)
            {
                oilfile.WriteLine(exp);
            }
        }
    }

    public class OilVar : OilNode
    {
        public string name;
        public OilType typ;

        public OilVar()
        {
            type = OilNodeType.VAR;
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(var def " + name + ", type = " + typ.ToString() + ")");
        }
    }

    public class OilParam : OilNode
    {
        public OilParam()
        {
            type = OilNodeType.PARAM;
        }

        public override void printNode(StreamWriter oilfile)
        {
            //nothing
        }
    }

    public class OilProc : OilNode
    {
        public string name;
        public List<OilParam> paramList;
        public List<OilVar> varList;
        public OilType retType;
        public OilBlock block;

        public OilProc()
        {
            type = OilNodeType.PROC;
            name = "";
            paramList = new List<OilParam>();
            varList = new List<OilVar>();
            retType = null;
            block = null;
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(proc " + name + ", rettype = " + retType.ToString() + ")");
            foreach (OilParam op in paramList)
            {
                op.printNode(oilfile);
            }
            foreach (OilVar ov in varList)
            {
                ov.printNode(oilfile);
            }
            block.printNode(oilfile);
        }
    }

    public class OilType : OilNode
    {
        public enum Typ
        {
            VOID,
            INT,
            FLOAT,
            CHAR
        }

        public Typ typ;

        public OilType (Typ _typ)
        {
            type = OilNodeType.TYPE;
            typ = _typ;
        }

        static string[] typnames = { "void", "int", "float", "char" };

        public override string ToString()
        {
            return typnames[(int)typ];
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(type " + typnames[(int)typ] + ")");
        }
    }

    //- statements ------------------------------------------------------------

    public class OilBlock : OilNode
    {
        public List<OilStatement> stmtList;

        public OilBlock()
        {
            type = OilNodeType.BLOCK;
            stmtList = new List<OilStatement>();
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(block)");
            foreach (OilStatement os in stmtList)
            {
                os.printNode(oilfile);
            }
        }
    }

    public class OilStatement : OilNode
    {
        //abstract base class
    }

    public class OilAssign : OilStatement
    {
        public OilExpression left;
        public OilExpression right;

        public OilAssign(OilExpression l, OilExpression r)
        {
            type = OilNodeType.ASSIGN;
            left = l;
            right = r;
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(assignment)");
            left.printNode(oilfile);
            right.printNode(oilfile);
        }
    }

    //- expressions -----------------------------------------------------------

    public class OilExpression : OilNode
    {
        //abstract base class
    }

    public class OilVarRef : OilExpression
    {
        public string name;

        public OilVarRef(string _name)
        {
            type = OilNodeType.VARREF;
            name = _name;
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(var ref [" + name + "])");
        }
    }

    public class OilIntConst : OilExpression
    {
        public long val;

        public OilIntConst(long _val)
        {
            type = OilNodeType.INTCONST;
            val = _val;
        }

        public override void printNode(StreamWriter oilfile)
        {
            oilfile.WriteLine("(int const [" + val + "])");
        }
    }
}
