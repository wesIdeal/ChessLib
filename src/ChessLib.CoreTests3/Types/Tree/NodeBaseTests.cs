using NUnit.Framework;
using ChessLib.Core.Types.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLib.Core.Types.Tree.Tests
{
    [TestFixture]
    public class NodeBaseTests
    {
        [Test(TestOf = typeof(TreeRoot<>))]
        public void Constructor_Should_Throw_Exception_If_Given_Null_Starting_Node()
        {
            var exception = Assert.Throws<TreeException>(() =>
            {
                var temp = new TreeRoot<string>(null);
            });
        }

        [Test(TestOf = typeof(TreeRoot<>))]
        public void Constructor_Should_Throw_Correct_Exception_If_Given_Null_Starting_Node()
        {
            try
            {
                var tree = new TreeRoot<string>(null);
                throw new Exception("Tree should not be set.");
            }

            catch (TreeException treeException)
            {
                Assert.AreEqual(TreeErrorType.StartingNodeCannotBeNull, treeException.TreeErrorType);
            }
        }

        TreeRoot<string> tree;

        [SetUp]
        public void Setup()
        {
            tree = new TreeRoot<string>("This");
        }
        [Test(TestOf = typeof(NodeBase<string>))]
        public void Next_Should_Return_Null_If_No_Subsequent_Node_Exists()
        {
            Assert.AreEqual(tree.Next, null);
        }

        [Test(TestOf = typeof(NodeBase<string>))]
        public void Next_Should_Return_Next_Node_If_Subsequent_Node_Exists()
        {
            var node = "is";
            tree.AddNode(node);
            Assert.AreEqual(node, tree.Next.Value);
        }

        [Test(TestOf = typeof(NodeBase<string>))]
        public void Add_Next_Should_Add_New_Tree_As_First_Continuation()
        {
            var node = "is";
            var root = tree;
            var firstContinuation = tree.AddNode(node);
            Assert.AreEqual(node, root.Continuations.First().Value);
        }

        [Test(TestOf = typeof(NodeBase<string>))]
        public void Previous_Should_Return_Null_If_Root()
        {

            Assert.AreEqual(null, tree.Previous);
        }

        [Test(TestOf = typeof(NodeBase<string>))]
        public void Previous_Should_Return_Previous_If_Not_Root()
        {
            var node = "is";
            var root = tree;
            var firstContinuation = tree.AddNode(node);
            Assert.AreEqual(tree, firstContinuation.Previous);
        }

    }
}