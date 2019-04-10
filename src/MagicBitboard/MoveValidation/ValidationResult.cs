using ChessLib.Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.MagicBitboard.MoveValidation
{
    public enum ValidationSeverity { None, Warning, Error };
     class ValidationResult
    {
        public ValidationResult(MoveExceptionType validationIssue)
        {
            ValidationIssue = validationIssue;
            Severity = ValidationSeverity.Error;
        }

        public ValidationResult(MoveExceptionType validationIssue = MoveExceptionType.NoneSet, ValidationSeverity severity = ValidationSeverity.None)
        {
            ValidationIssue = validationIssue;
            Severity = severity;
        }

        public MoveExceptionType ValidationIssue { get; set; }
        public ValidationSeverity Severity { get; set; }
    }
}
