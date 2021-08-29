using System;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidatorAttribute : CategoryAttribute
    {
        public const string _name = "Validator";

        public ValidatorAttribute() : this(_name)
        {
        }

        public ValidatorAttribute(string name) : base($"{_name}.{name}")
        {
        }
    }
}