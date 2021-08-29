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

        [TestCaseSource(typeof(TreeData), nameof(TreeData.GetEndOfVariationTestCases))]
        public bool LastContinuationMove_ShouldReturnCorrectValue(MoveTreeNode<PostMoveState> node)
        {
            return node.IsLastMoveOfContinuation;
        }

    }

    //using NUnit.Framework;
    //using ChessLib.Core.Types.Tree;
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Text;

    //namespace ChessLib.Core.Types.Tree.Tests
    //{
    //[TestFixture]
    //public class NodeBaseTests
    //{


     
    //    [Test(TestOf = typeof(NodeBase<string>))]
    //    public void Next_Should_Return_Null_If_No_Subsequent_Node_Exists()
    //    {
    //        Assert.AreEqual(tree.Next, null);
    //    }

    //    [Test(TestOf = typeof(NodeBase<string>))]
    //    public void Next_Should_Return_Next_Node_If_Subsequent_Node_Exists()
    //    {
    //        var node = "is";
    //        tree.AddNode(node);
    //        Assert.AreEqual(node, tree.Next.Value);
    //    }

    //    [Test(TestOf = typeof(NodeBase<string>))]
    //    public void Add_Next_Should_Add_New_Tree_As_First_Continuation()
    //    {
    //        var node = "is";
    //        var root = tree;
    //        var firstContinuation = tree.AddNode(node);
    //        Assert.AreEqual(node, root.Continuations.First().Value);
    //    }

    //    [Test(TestOf = typeof(NodeBase<string>))]
    //    public void Previous_Should_Return_Null_If_Root()
    //    {

    //        Assert.AreEqual(null, tree.Previous);
    //    }

    //    [Test(TestOf = typeof(NodeBase<string>))]
    //    public void Previous_Should_Return_Previous_If_Not_Root()
    //    {
    //        var node = "is";
    //        var root = tree;
    //        var firstContinuation = tree.AddNode(node);
    //        Assert.AreEqual(tree, firstContinuation.Previous);
    //    }

    //}
    //}
}
