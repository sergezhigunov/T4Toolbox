// <copyright file="GeneratorTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;

    /// <summary>
    /// This is a test class for <see cref="Generator"/> and is intended to contain all of its unit tests.
    /// </summary>
    public class GeneratorTest : IDisposable
    {
        private const string TestMessage = "Test Message";

        private readonly FakeGenerator generator = new FakeGenerator();

        private readonly FakeTransformation transformation;

        public GeneratorTest()
        {
            this.transformation = new FakeTransformation();
            TransformationContext.Initialize(this.transformation, this.transformation.GenerationEnvironment);
        }

        public void Dispose()
        {
            TransformationContext.Cleanup();
            this.transformation.Dispose();
        }

        #region Context

        [Fact]
        public void ContextReturnsTransformationContextByDefault()
        {
            Assert.Same(TransformationContext.Current, this.generator.Context);
        }

        [Fact]
        public void ContextCanBeSet()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            {
                this.generator.Context = context;
                Assert.Same(context, this.generator.Context);
            }
        }

        [Fact]
        public void ContextThrowsArgumentNullExceptionWhenNewValueIsNull() // Because allowing it would magically change property value to TransformationContext.Current.
        {
            Assert.Throws<ArgumentNullException>(() => this.generator.Context = null);
        }

        #endregion

        #region Error(string)

        [Fact]
        public void ErrorAddsNewErrorToErrorsCollection()
        {
            this.generator.Error(TestMessage);
            Assert.Equal(1, this.generator.Errors.Count);
            Assert.Equal(TestMessage, this.generator.Errors[0].ErrorText);
            Assert.Equal(false, this.generator.Errors[0].IsWarning);
        }

        [Fact]
        public void ErrorThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.generator.Error(null));
        }

        #endregion

        #region Error(string, params object[])

        [Fact]
        public void ErrorFormatAddsNewErrorToErrorsCollection()
        {
            this.generator.Error("{0}", TestMessage);
            Assert.Equal(1, this.generator.Errors.Count);
            Assert.Equal(TestMessage, this.generator.Errors[0].ErrorText);
            Assert.Equal(false, this.generator.Errors[0].IsWarning);
        }

        [Fact]
        public void ErrorFormatThrowsArgumentNullExceptionWhenFormatIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.generator.Error(null, null));
        }

        #endregion

        #region Errors

        [Fact]
        public void ErrorsIsNotNull()
        {
            Assert.NotNull(this.generator.Errors);
        }

        #endregion

        #region Run

        [Fact]
        public void RunValidatesGenerator()
        {
            bool validated = false;
            this.generator.Validated = () => validated = true;
            this.generator.Run();
            Assert.True(validated);
        }

        [Fact]
        public void RunSkipsRunCoreIfValidateGeneratesErrors()
        {
            this.generator.Validated = () => this.generator.Error("Test Error");

            bool runCoreExecuted = false;
            this.generator.RanCore = () => runCoreExecuted = true;

            this.generator.Run();
            Assert.False(runCoreExecuted);
        }

        [Fact]
        public void RunExecutesRunCoreIfValidateGeneratesWarnings()
        {
            this.generator.Validated = () => this.generator.Warning(TestMessage);

            bool runCoreExecuted = false;
            this.generator.RanCore = () => runCoreExecuted = true;

            this.generator.Run();
            Assert.True(runCoreExecuted);
        }

        [Fact]
        public void RunReportsTransformationExceptionsAsError()
        {
            const string ErrorMessage = "Test Error";
            this.generator.Validated = () => { throw new TransformationException(ErrorMessage); };

            this.generator.Run();
            Assert.Equal(1, this.generator.Errors.Count);
            Assert.Equal(ErrorMessage, this.generator.Errors[0].ErrorText);
        }

        [Fact]
        public void RunReportsErrorsToTransformation()
        {
            this.generator.Validated = () => this.generator.Warning(TestMessage);
            this.generator.Run();
            Assert.Equal(1, this.transformation.Errors.Count);
            Assert.Equal(TestMessage, this.transformation.Errors[0].ErrorText);
        }

        [Fact]
        public void RunClearsPreviousErrorsToAvoidReportingThemToTransformationMoreThanOnce()
        {
            this.generator.Error(TestMessage);
            this.generator.Run();
            Assert.Equal(0, this.generator.Errors.Count);
        }

        [Fact]
        public void RunReportsInputFileInErrorsOfInputFileBasedTransformation()
        {
            this.transformation.Session[TransformationContext.InputFileNameKey] = "Input.cs";
            this.generator.Validated = () => this.generator.Error(TestMessage);
            this.generator.Run();

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.Equal("Input.cs", error.FileName);
        }

        [Fact]
        public void RunReportsTemplateFileInErrorsOfTemplateFileBasedTransformation()
        {
            this.transformation.Host.TemplateFile = "Template.tt";
            this.generator.Validated = () => this.generator.Error(TestMessage);
            this.generator.Run();

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.Equal("Template.tt", error.FileName);
        }

        #endregion

        #region Warning(string)

        [Fact]
        public void WarningAddsWarningToErrorsCollection()
        {
            const string WarningMessage = TestMessage;
            this.generator.Warning(WarningMessage);
            Assert.Equal(1, this.generator.Errors.Count);
            Assert.Equal(WarningMessage, this.generator.Errors[0].ErrorText);
            Assert.Equal(true, this.generator.Errors[0].IsWarning);
        }

        public void WarningThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.generator.Warning(null));
        }

        #endregion

        #region Warning(string, params object[])

        [Fact]
        public void WarningFormatAddsWarningToErrorsCollection()
        {
            this.generator.Warning("{0}", TestMessage);
            Assert.Equal(1, this.generator.Errors.Count);
            Assert.Equal(TestMessage, this.generator.Errors[0].ErrorText);
            Assert.Equal(true, this.generator.Errors[0].IsWarning);
        }

        [Fact]
        public void WarningFormatThrowsArgumentNullExceptionWhenFormatIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.generator.Warning(null, null));
        }

        #endregion
    }
}
