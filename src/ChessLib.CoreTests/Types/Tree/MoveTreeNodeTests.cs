using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.PgnExport;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core.Tests.Types.Tree
{
    [TestFixture]
    public class MoveTreeNodeTests
    {
        [TestCaseSource(typeof(TreeData), nameof(TreeData.GetInitialMoveSetTestCases))]
        public bool InitialMoveFlag_OnVariationFirstNode_InitialMoveFlagShouldGetSet(Game g)
        {
            return g.Current.Node.IsInitialMoveOfContinuation;
        }

        [TestCaseSource(typeof(TreeData), nameof(TreeData.GetVariationMoveSetTestCases))]
        public bool VariationFlag_OnVariationFirstNode_ShouldGetSet(Game g)
        {
            return g.Current.Node.IsVariation;
        }



    }
}
