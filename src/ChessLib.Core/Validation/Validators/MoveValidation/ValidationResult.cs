using ChessLib.Core.Types.Exceptions;

namespace ChessLib.Core.Validation.Validators.MoveValidation
{
    public enum ValidationSeverity
    {
        None,
        Warning,
        Error
    }

    internal class ValidationResult
    {
        public MoveError ValidationIssue { get; set; }
        public ValidationSeverity Severity { get; set; }

        public ValidationResult(MoveError validationIssue)
        {
            ValidationIssue = validationIssue;
            Severity = ValidationSeverity.Error;
        }

        public ValidationResult(MoveError validationIssue = MoveError.NoneSet,
            ValidationSeverity severity = ValidationSeverity.None)
        {
            ValidationIssue = validationIssue;
            Severity = severity;
        }
    }
}