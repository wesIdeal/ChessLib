using ChessLib.Data.Types.Exceptions;

namespace ChessLib.Data.Validators.MoveValidation
{
    public enum ValidationSeverity { None, Warning, Error };
    class ValidationResult
    {
        public ValidationResult(MoveError validationIssue)
        {
            ValidationIssue = validationIssue;
            Severity = ValidationSeverity.Error;
        }

        public ValidationResult(MoveError validationIssue = MoveError.NoneSet, ValidationSeverity severity = ValidationSeverity.None)
        {
            ValidationIssue = validationIssue;
            Severity = severity;
        }

        public MoveError ValidationIssue { get; set; }
        public ValidationSeverity Severity { get; set; }
    }
}
