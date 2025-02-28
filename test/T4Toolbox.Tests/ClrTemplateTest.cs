// <copyright file="ClrTemplateTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.IO;

    /// <summary>
    /// A test class for <see cref="ClrTemplate" />.
    /// </summary>
    public class ClrTemplateTest 
    {
        #region DefaultNamespace

        [Fact]
        public void DefaultNamespaceReturnsRootNamespaceForInputFileInRootOfProject()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;
                transformation.Host.TemplateFile = Path.Combine(Environment.CurrentDirectory, "Template.tt");
                transformation.Host.GetMetadataValue = (hierarhcy, inputFile, metadataName) => string.Empty;
                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    switch (propertyName)
                    {
                        case "RootNamespace":
                            return "TestNamespace";
                        case "MSBuildProjectFullPath":
                            return Path.Combine(Environment.CurrentDirectory, "Project.proj");
                        default:
                            return string.Empty;
                    }
                };

                Assert.Equal("TestNamespace", template.DefaultNamespace);
            }
        }

        [Fact]
        public void DefaultNamespaceCombinesRelativeInputFilePathWithRootNamespace()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;
                transformation.Host.TemplateFile = Path.Combine(Environment.CurrentDirectory, "SubFolder\\Template.tt");
                transformation.Host.GetMetadataValue = (hierarhcy, inputFile, metadataName) => string.Empty;
                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    switch (propertyName)
                    {
                        case "RootNamespace":
                            return "TestNamespace";
                        case "MSBuildProjectFullPath":
                            return Path.Combine(Environment.CurrentDirectory, "Project.proj");
                        default:
                            return string.Empty;
                    }
                };

                Assert.Equal("TestNamespace.SubFolder", template.DefaultNamespace);
            }
        }

        [Fact]
        public void DefaultNamespaceUsesProjectItemLinkPathInsteadOfPhysicalFilePath()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;

                transformation.Host.TemplateFile = Path.Combine(Environment.CurrentDirectory, "Template.tt");

                transformation.Host.GetMetadataValue = (hierarhcy, inputFile, metadataName) =>
                {
                    switch (metadataName)
                    {
                        case "Link":
                            return "SubFolder\\Template.tt";
                        default:
                            return string.Empty;
                    }
                };

                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    switch (propertyName)
                    {
                        case "RootNamespace":
                            return "TestNamespace";
                        case "MSBuildProjectFullPath":
                            return Path.Combine(Environment.CurrentDirectory, "Project.proj");
                        default:
                            return string.Empty;
                    }
                };

                Assert.Equal("TestNamespace.SubFolder", template.DefaultNamespace);
            }
        }

        #endregion

        [Fact]
        public void RootNamespaceReturnsPropertyValueSuppliedByProvider()
        {
            using (var transformation = new FakeTransformation())
            using (var context = new TransformationContext(transformation, transformation.GenerationEnvironment))
            using (var template = new TestClrTemplate())
            {
                template.Context = context;

                const string ExpectedValue = "TestNamespace";
                string actualPropertyName = null;
                transformation.Host.GetPropertyValue = (hierarchy, propertyName) =>
                {
                    actualPropertyName = propertyName;
                    return ExpectedValue;
                };

                Assert.Equal(ExpectedValue, template.RootNamespace);
                Assert.Equal("RootNamespace", actualPropertyName);
            }
        }

        // ClrTemplate is an abstract class. Need a concrete descendant to test it.
        private class TestClrTemplate : ClrTemplate
        {
            public override string TransformText()
            {
                return string.Empty;
            }
        }
    }
}