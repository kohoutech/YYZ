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

namespace YYZ.CodeGen
{
    public class GenNode
    {
        static Dictionary<OilNode, GenNode> nodeTable = new Dictionary<OilNode, GenNode>();

        public GenNode(OilNode node)
        {
            nodeTable[node] = this;
        }
    }

    public class GenVar : GenNode
    {
        OilVar oilvar;

        public GenVar(OilVar _oilvar) : base(_oilvar)
        {
            oilvar = _oilvar;
        }
    }

    public class GenProc : GenNode
    {
        public OilProc oilproc;

        public List<GenVar> varList;
        public GenBlock block;

        public GenProc(OilProc _oilproc) : base(_oilproc)
        {
            oilproc = _oilproc;
            varList = new List<GenVar>();
            foreach (OilVar ov in oilproc.varList)
            {
                varList.Add(new GenVar(ov));
            }
        }
    }

    //- statements ------------------------------------------------------------

    public class GenBlock : GenNode
    {
        public OilBlock oilblock;

        public List<GenStatement> stmtList;

        public GenBlock(OilBlock _oilblock) : base(_oilblock)
        {
            oilblock = _oilblock;
            stmtList = new List<GenStatement>();
            GenStatement stmt = null;
            foreach (OilStatement os in oilblock.stmtList)
            {
                stmt = GenStatement.getStmtNode(os);
            }
        }
    }

    public class GenStatement : GenNode
    {
        public static GenStatement getStmtNode(OilStatement oilstmt)
        {
            switch (oilstmt.type)
            {
                case OilNodeType.ASSIGN:
                    return new GenAssign((OilAssign)oilstmt);
            }
            return null;
        }

        public GenStatement(OilStatement _oilstmt) : base(_oilstmt)
        {
        }
    }

    public class GenAssign : GenStatement
    {
        OilAssign oilassign;
        public GenExpression left;
        public GenExpression right;

        public GenAssign(OilAssign _oilassign) : base(_oilassign)
        {
            oilassign = _oilassign;
            left = GenExpression.getExprNode(oilassign.left);
            right = GenExpression.getExprNode(oilassign.right);
        }
    }

    //- expressions -----------------------------------------------------------

    public class GenExpression : GenNode
    {
        public static GenExpression getExprNode(OilExpression oilexpr)
        {
            switch (oilexpr.type)
            {
                case OilNodeType.VARREF:
                    return new GenVarRef((OilVarRef)oilexpr);

                case OilNodeType.INTCONST:
                    return new GenIntConst((OilIntConst)oilexpr);
            }
            return null;
        }

        public GenExpression(OilExpression _oilexpr) : base(_oilexpr)
        {
        }
    }

    public class GenVarRef : GenExpression
    {
        public OilVarRef oilvarref;

        public GenVarRef(OilVarRef _oilvarref) : base(_oilvarref)
        {
            oilvarref = _oilvarref;
        }
    }

    public class GenIntConst : GenExpression
    {
        public OilIntConst oilintconst;

        public GenIntConst(OilIntConst _oilintconst) : base(_oilintconst)
        {
            oilintconst = _oilintconst;
        }
    }
}
