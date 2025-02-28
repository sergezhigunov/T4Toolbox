// <copyright file="TemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;

    public class TemplateTest : IDisposable
    {
        private const string TestFile = "Test.txt";

        private const string TestMessage = "Test Message";

        private const string TestOutput = "Test Template Output";

        private FakeTransformation transformation;

        public TemplateTest()
        {
            this.transformation = new FakeTransformation();
            TransformationContext.Initialize(this.transformation, this.transformation.GenerationEnvironment);
        }

        public void Dispose()
        {
            if (this.transformation != null)
            {
                TransformationContext.Cleanup();
                this.transformation.Dispose();
                this.transformation = null;
            }
        }

        #region Context

        [Fact]
        public void ContextReturnsTransformationContextByDefault()
        {
            using (var template = new FakeTemplate())
            {
                Assert.Same(TransformationContext.Current, template.Context);
            }
        }

        [Fact]
        public void ContextCanBeSet()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new FakeTemplate())
            {
                template.Context = context;
                Assert.Same(context, template.Context);
            }
        }

        [Fact]
        public void ContextThrowsArgumentNullExceptionWhenNewValueIsNull() // Because allowing it would magically change property value to TransformationContext.Current.
        {
            using var template = new FakeTemplate();
            Assert.Throws<ArgumentNullException>(() => template.Context = null);
        }

        #endregion

        #region Enabled

        [Fact]
        public void EnabledIsTrueByDefault()
        {
            using (var template = new FakeTemplate())
            {
                Assert.True(template.Enabled);
            }
        }

        [Fact]
        public void EnabledCanBeSet()
        {
            using (var template = new FakeTemplate())
            {
                template.Enabled = false;
                Assert.Equal(false, template.Enabled);
            }
        }

        #endregion

        #region Error(string)

        [Fact]
        public void ErrorAddsNewErrorToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Error(TestMessage);
                Assert.Equal(1, template.Errors.Count);
                Assert.Equal(TestMessage, template.Errors[0].ErrorText);
                Assert.Equal(false, template.Errors[0].IsWarning);
            }
        }

        [Fact]
        public void ErrorThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            using (var template = new FakeTemplate())
            {
                Assert.Throws<ArgumentNullException>(() => template.Error(null));
            }
        }

        #endregion

        #region Error(string, params object[])

        [Fact]
        public void ErrorFormatAddsNewErrorToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Error("{0}", TestMessage);
                Assert.Equal(1, template.Errors.Count);
                Assert.Equal(TestMessage, template.Errors[0].ErrorText);
                Assert.Equal(false, template.Errors[0].IsWarning);
            }
        }

        [Fact]
        public void ErrorFormatThrowsNullArgumentExceptionWhenFormatIsNull()
        {
            using (var template = new FakeTemplate())
            {
                Assert.Throws<ArgumentNullException>(() => template.Error(null, null));
            }
        }

        #endregion

        #region Errors

        [Fact]
        public void ErrorsIsNotNull()
        {
            using (var template = new FakeTemplate())
            {
                Assert.NotNull(template.Errors);
            }
        }

        #endregion

        #region Render

        [Fact]
        public void RenderDoesNotTransformTemplateWhenEnabledIsFalse()
        {
            using (var template = new FakeTemplate())
            {
                template.Enabled = false;
                bool transformed = false;
                template.TransformedText = () => transformed = true;
                template.Render();
                Assert.False(transformed);
            }
        }

        [Fact]
        public void RenderRaisesRenderingEvent()
        {
            using (var template = new FakeTemplate())
            {
                bool eventRaised = false;
                template.Rendering += delegate { eventRaised = true; };
                template.Render();
                Assert.Equal(true, eventRaised);
            }
        }

        [Fact]
        public void RenderTransformsTemplateWhenEnabledIsSetByRenderingEventHandler()
        {
            using (var template = new FakeTemplate())
            {
                template.Enabled = false;
                bool transformed = false;
                template.Rendering += delegate { template.Enabled = true; };
                template.TransformedText = () => transformed = true;
                template.Render();
                Assert.True(transformed);
            }
        }

        [Fact]
        public void RenderReportsTransformationExceptionsAsErrors()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => { throw new TransformationException(TestMessage); };
                template.Render();
                AssertSingleError(template.Errors, TestMessage);
            }
        }

        [Fact]
        public void RenderReportsTemplateValidationErrorsToTransformation()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Render();
                AssertSingleError(this.transformation.Errors, TestMessage);
            }
        }

        [Fact]
        public void RenderReportsInputFileInErrorsOfInputFileBasedTransformation()
        {
            this.transformation.Session[TransformationContext.InputFileNameKey] = "Input.cs";

            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Render();
            }

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.Equal("Input.cs", error.FileName);
        }

        [Fact]
        public void RenderReportsTemplateFileInErrorsOfTemplateFileBasedTransformation()
        {
            this.transformation.Host.TemplateFile = "Template.tt";

            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Render();
            }

            var error = this.transformation.Errors.Cast<CompilerError>().Single();
            Assert.Equal("Template.tt", error.FileName);
        }

        #endregion

        #region RenderToFile

        [Fact]
        public void RenderToFileSetsOutputFile()
        {
            using (var template = new FakeTemplate())
            {
                template.RenderToFile(TestFile);
                Assert.Equal(TestFile, template.Output.File);
            }
        }

        [Fact]
        public void RenderToFileRendersTheTemplate()
        {
            OutputFile[] outputFiles = null;
            this.transformation.Host.UpdatedOutputFiles = (input, outputs) => outputFiles = outputs;

            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.Write(TestOutput);
                template.RenderToFile(TestFile);
            }

            this.Dispose(); // Force the end of transformation

            OutputFile outputFile = outputFiles.Single(output => output.File == TestFile);
            Assert.Equal(TestOutput, outputFile.Content.ToString());
        }

        #endregion

        #region Session

        [Fact]
        public void SessionReturnsTransformationSession()
        {
            using (var template = new FakeTemplate())
            {
                Assert.Same(this.transformation.Session, template.Session);
            }
        }

        #endregion

        #region Transform

        [Fact]
        public void TransformRunsCodeGeneratedByDirectiveProcessors()
        {
            using (var template = new FakeTemplate())
            {
                bool initialized = false;
                template.Initialized = () => initialized = true;
                template.Transform();
                Assert.True(initialized);
            }
        }

        [Fact]
        public void TransformValidatesTemplate()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Transform();
                Assert.Equal(1, template.Errors.Count);
            }
        }

        [Fact]
        public void TransformDoesNotCatchTransformationException()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => { throw new TransformationException(); };
                Assert.Throws<TransformationException>(() => template.Transform());
            }
        }

        [Fact]
        public void TransformDoesNotGenerateOutputWhenValidateReportsErrors()
        {
            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.WriteLine(TestOutput);
                template.Validated = () => template.Error(TestMessage);
                Assert.Equal(string.Empty, template.Transform());
            }
        }

        [Fact]
        public void TransformGeneratesOutputWhenValidateReportsWarnings()
        {
            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.Write(TestOutput);
                template.Validated = () => template.Warning(TestMessage);
                Assert.Equal(TestOutput, template.Transform());
            }
        }

        [Fact]
        public void TransformDoesNotValidateOutputProperties()
        {
            using (var template = new FakeTemplate())
            {
                template.Output.Project = "Test.proj";
                template.Transform();
                Assert.Equal(0, template.Errors.Count);
            }
        }

        [Fact]
        public void TransformClearsPreviousOutputToAllowGeneratingMultipleOutputsFromSingleTemplate()
        {
            using (var template = new FakeTemplate())
            {
                template.TransformedText = () => template.Write("First Output");
                template.Transform();

                template.TransformedText = () => template.Write(TestOutput);
                Assert.Equal(TestOutput, template.Transform());
            }
        }

        [Fact]
        public void TransformClearsPreviousErrorsToAllowTransformingSameTemplateMultipleTimes()
        {
            using (var template = new FakeTemplate())
            {
                template.Validated = () => template.Error(TestMessage);
                template.Transform();

                template.Validated = null;
                template.TransformedText = () => template.Write(TestOutput);
                Assert.Equal(TestOutput, template.Transform());
            }
        }

        #endregion

        #region Warning(string)

        [Fact]
        public void WarningAddsNewWarningToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Warning(TestMessage);
                Assert.Equal(1, template.Errors.Count);
                Assert.Equal(TestMessage, template.Errors[0].ErrorText);
                Assert.Equal(true, template.Errors[0].IsWarning);
            }
        }

        [Fact]
        public void WarningThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            using (var template = new FakeTemplate())
            {
                Assert.Throws<ArgumentNullException>(() => template.Warning(null));
            }
        }

        #endregion

        #region Warning(string, params object[])

        [Fact]
        public void WarningFormatAddsNewWarningToErrorsCollection()
        {
            using (var template = new FakeTemplate())
            {
                template.Warning("{0}", TestMessage);
                Assert.Equal(1, template.Errors.Count);
                Assert.Equal(TestMessage, template.Errors[0].ErrorText);
                Assert.Equal(true, template.Errors[0].IsWarning);
            }
        }

        [Fact]
        public void WarningFormatThrowsArgumentNullExceptionWhenFormatIsNull()
        {
            using (var template = new FakeTemplate())
            {
                Assert.Throws<ArgumentNullException>(() => template.Warning(null, null));
            }
        }

        #endregion

        private static void AssertSingleError(CompilerErrorCollection errors, params string[] keywords)
        {
            var error = errors.Cast<CompilerError>().Single();
            foreach (string keyword in keywords)
            {
                Assert.Contains(keyword, error.ErrorText);
            }
        }
    }
}
