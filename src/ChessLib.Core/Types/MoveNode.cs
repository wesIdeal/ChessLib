using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types.Interfaces;




public enum TreeErrorType
{
    NoneSpecified,
    StartingNodeCannotBeNull
}

public class TreeException : Exception
{
    public new string Message { get; }
    public TreeErrorType TreeErrorType { get; }


    public TreeException(TreeErrorType treeErrorType)
    {
        TreeErrorType = treeErrorType;
        switch (treeErrorType)
        {
            case TreeErrorType.StartingNodeCannotBeNull:
                Message = "Starting node must be non-null.";
                break;
            case TreeErrorType.NoneSpecified:
            default:
                Message = "General tree exception";
                break;
        }
    }
}