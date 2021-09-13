using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation;
using ChessLib.Core.Validation.MoveValidation.CastlingRules;
using ChessLib.Core.Validation.MoveValidation.EnPassantRules;
using ChessLib.Core.Validation.MoveValidation.MoveRules;
using ChessLib.Core.Validation.MoveValidation.PromotionRules;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation
{
    [TestFixture(TestOf = typeof(MoveValidator))]
    public class MoveValidatorTests
    {
        
        [TestCaseSource(nameof(GetCompileTestCases))]
        public void CompileShouldGetCorrectRules(MoveType moveType, IEnumerable<Type> expectedTypes)
        {
            var validator = new MoveValidator();
            var actualRules = validator.CompileRules(moveType).Select(x => x.GetType());
            var expectedRuleTypes = expectedTypes.OrderBy(x=>x.GUID).ToArray();
            var actualRuleTypes = actualRules.OrderBy(x=>x.GUID).ToArray();
            Assert.AreEqual(expectedRuleTypes.Count(), actualRuleTypes.Count(), "Expected rule count is not equal to actual count.");
            var zRules = expectedRuleTypes.Zip(actualRuleTypes, (exp, act) => new { expected = exp, actual = act, areEqual = exp == act }).ToArray();
            var misMatchError = "Rule set has inequalities, expected vs actual";
            Assert.AreEqual(expectedRuleTypes.Length, zRules.Count(), misMatchError);
            Assert.IsTrue(zRules.All(x => x.areEqual), misMatchError);
        }

        [TestCaseSource(nameof(GetCompileIntegrationTests))]
        public void Validate_ShouldCallCompileCorrectly(Mock<MoveValidator> validatorMock, Mock<IMoveRule> ruleMock, Board board, Move move)
        {
            validatorMock.Object.Validate(board, move, out ulong[][] _);
            validatorMock.Verify();
        }

        [TestCaseSource(nameof(GetCompileIntegrationTests))]
        public void Validate_ShouldCallRuleValidationCorrectly(Mock<MoveValidator> validatorMock, Mock<IMoveRule> ruleMock, Board board, Move move)
        {
            validatorMock.Object.Validate(board, move, out ulong[][] _);
            ruleMock.Verify();
        }

        [TestCaseSource(nameof(GetValidationReturnTestCases))]
        public MoveError Validate_ShouldReturnCorrectResult(Mock<MoveValidator> validatorMock, Board board, Move move)
        {
          var rv =  validatorMock.Object.Validate(board, move, out ulong[][] _);
          return rv;
        }



        protected static IEnumerable<TestCaseData> GetValidationReturnTestCases()
        {
            var e2 = "e2".ToBoardIndex();
            var e4 = "e4".ToBoardIndex();

            foreach (var moveError in EnumsNET.Enums.GetValues<MoveError>())
            {
                var mock = new Mock<MoveValidator>() { CallBase = true };
                var genericValidator = new Mock<IMoveRule>();
                var board = new Board();
                var move = MoveHelpers.GenerateMove(e2, e4, MoveType.Normal);
                genericValidator.Setup(x => x.Validate(board, move)).Returns(moveError);
                mock.SetupGet(x => x.rules).Returns(new List<IMoveRule>(new[] { genericValidator.Object }));
                mock.Setup(x => x.CompileRules(It.IsAny<MoveType>())).Returns(new List<IMoveRule>());
                yield return new TestCaseData(mock, board, move)
                    .SetName($"Should return validation result: Expecting value of {moveError}")
                    .Returns(moveError);
            }
        }

        protected static IEnumerable<TestCaseData> GetCompileIntegrationTests()
        {
           
            var e2 = "e2".ToBoardIndex();
            var e4 = "e4".ToBoardIndex();
           
            foreach (var moveType in EnumsNET.Enums.GetValues<MoveType>())
            {
                var mock = new Mock<MoveValidator>() { CallBase = true };
                var genericValidator = new Mock<IMoveRule>();
                var board = new Board();
                var move = MoveHelpers.GenerateMove(e2, e4, moveType);
                genericValidator.Setup(x => x.Validate(board, move)).Returns(MoveError.NoneSet).Verifiable();
                mock.SetupGet(x => x.rules).Returns(new List<IMoveRule>(new []{genericValidator.Object}));
                mock.Setup(x => x.CompileRules(It.Is<MoveType>(x => x == moveType))).Returns(new List<IMoveRule>()).Verifiable();
                yield return new TestCaseData(mock, genericValidator,  board, move).SetName($"Calling with {moveType}");
            }
        }
        private static readonly Type[] ubiquitousValidators = new Type[]
            { typeof(ActiveColorValidator), typeof(MoveDestinationValidator), typeof(NotInCheckAfterMoveValidator) };
        protected static IEnumerable<TestCaseData> GetCompileTestCases()
        {

            foreach (var moveType in EnumsNET.Enums.GetValues<MoveType>())
            {
                var ruleTypes = ubiquitousValidators.ToList();
                if (moveType == MoveType.EnPassant)
                {
                    ruleTypes.Add(typeof(EnPassantDestinationValidator));
                }
                else if (moveType == MoveType.Castle)
                {
                    ruleTypes.Add(typeof(CastlingMoveIsAvailableValidator));
                    ruleTypes.Add(typeof(KingDestinationValidator));
                    ruleTypes.Add(typeof(NotInCheckBeforeMoveValidator));
                    ruleTypes.Add(typeof(AttackNotBlockingMoveValidator));
                    ruleTypes.Add(typeof(NoPieceBlocksCastlingMoveValidator));
                }
                else if (moveType == MoveType.Promotion)
                {
                    ruleTypes.Add(typeof(SourceIsPawnValidator));
                }

                string typeStr = string.Join(", ", ruleTypes.Select(x => x.Name));
                yield return new TestCaseData(moveType, ruleTypes)
                    .SetName($"{moveType} - {typeStr}");
            }
        }
    }
}