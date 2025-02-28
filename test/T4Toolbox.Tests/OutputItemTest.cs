// <copyright file="OutputItemTest.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.Tests
{
    using System;
    using System.Text;

    /// <summary>
    /// A test class for <see cref="OutputItem"/>.
    /// </summary>
    public class OutputItemTest
    {
        #region CopyToOutputDirectory

        [Fact]
        public void CopyToOutputDirectoryIsDoNotCopyByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(CopyToOutputDirectory.DoNotCopy, output.CopyToOutputDirectory);
        }

        [Fact]
        public void CopyToOutputDirectoryCanBeSet()
        {
            var output = new OutputItem();

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyAlways;
            Assert.Equal(CopyToOutputDirectory.CopyAlways, output.CopyToOutputDirectory);

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyIfNewer;
            Assert.Equal(CopyToOutputDirectory.CopyIfNewer, output.CopyToOutputDirectory);

            output.CopyToOutputDirectory = CopyToOutputDirectory.DoNotCopy;
            Assert.Equal(CopyToOutputDirectory.DoNotCopy, output.CopyToOutputDirectory);
        }

        [Fact]
        public void CopyToOutputDirectoryIsStoredAsMetadata()
        {
            var output = new OutputItem();

            output.CopyToOutputDirectory = CopyToOutputDirectory.DoNotCopy;
            Assert.Equal(string.Empty, output.Metadata[ItemMetadata.CopyToOutputDirectory]);

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyAlways;
            Assert.Equal("Always", output.Metadata[ItemMetadata.CopyToOutputDirectory]);

            output.CopyToOutputDirectory = CopyToOutputDirectory.CopyIfNewer;
            Assert.Equal("PreserveNewest", output.Metadata[ItemMetadata.CopyToOutputDirectory]);
        }

        #endregion

        #region CustomTool

        [Fact]
        public void CustomToolIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.CustomTool);
        }

        [Fact]
        public void CustomToolCanBeSet()
        {
            var output = new OutputItem { CustomTool = "TextTemplatingFileGenerator" };
            Assert.Equal("TextTemplatingFileGenerator", output.CustomTool);
        }

        [Fact]
        public void CustomToolIsStoredAsMetadata()
        {
            var output = new OutputItem { CustomTool = "TextTemplatingFileGenerator" };
            Assert.Equal("TextTemplatingFileGenerator", output.Metadata[ItemMetadata.Generator]);
        }

        [Fact]
        public void CustomToolThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.CustomTool = null);
        }

        #endregion

        #region CustomToolNamespace

        [Fact]
        public void CustomToolNamespaceIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.CustomToolNamespace);
        }

        [Fact]
        public void CustomToolNamespaceCanBeSet()
        {
            var output = new OutputItem { CustomToolNamespace = "T4Toolbox" };
            Assert.Equal("T4Toolbox", output.CustomToolNamespace);
        }

        [Fact]
        public void CustomToolNamespaceIsStoredAsMetadata()
        {
            var output = new OutputItem { CustomToolNamespace = "T4Toolbox" };
            Assert.Equal("T4Toolbox", output.Metadata[ItemMetadata.CustomToolNamespace]);
        }

        [Fact]
        public void CustomToolNamespaceThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.CustomToolNamespace = null);
        }

        #endregion

        #region Directory

        [Fact]
        public void DirectoryIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.Directory);
        }

        [Fact]
        public void DirectoryThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.Directory = null);
        }

        #endregion

        #region Encoding

        [Fact]
        public void EncodingIsUtf8ByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(Encoding.UTF8, output.Encoding);
        }

        [Fact]
        public void EncodingCanBeSet()
        {
            var output = new OutputItem { Encoding = Encoding.ASCII };
            Assert.Equal(Encoding.ASCII, output.Encoding);
        }

        [Fact]
        public void EncodingThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.Encoding = null);
        }

        #endregion

        #region File

        [Fact]
        public void FileIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.File);
        }

        [Fact]
        public void FileCanBeSet()
        {
            var output = new OutputItem { File = "Test.cs" };
            Assert.Equal("Test.cs", output.File);
        }

        [Fact]
        public void FileUpdatesDirectoryPropertyWhenItIncludesDirectoryName()
        {
            var output = new OutputItem { File = @"Folder\Test.cs" };
            Assert.Equal("Test.cs", output.File);
            Assert.Equal("Folder", output.Directory);
        }

        [Fact]
        public void FilePreservesDirectoryPropertyWhenItDoesNotIncludeDirectoryName()
        {
            var output = new OutputItem { Directory = "Folder", File = "NewTest.cs" };
            Assert.Equal("Folder", output.Directory);
        }

        [Fact]
        public void FileThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.File = null);
        }

        #endregion

        #region ItemType

        [Fact]
        public void ItemTypeIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.ItemType);
        }

        [Fact]
        public void ItemTypeCanBeSet()
        {
            var output = new OutputItem { ItemType = ItemType.Compile };
            Assert.Equal(ItemType.Compile, output.ItemType);
        }

        [Fact]
        public void ItemTypeThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.ItemType = null);
        }

        #endregion

        #region Metadata

        [Fact]
        public void MetadataIsEmptyByDefault()
        {
            var output = new OutputItem();
            Assert.Empty(output.Metadata);
        }

        [Fact]
        public void MetadataIsNotCaseSensitive()
        {
            var output = new OutputItem();
            output.Metadata["TEST"] = "value";
            Assert.Equal("value", output.Metadata["test"]);
        }

        #endregion

        #region Path

        [Fact]
        public void PathReturnsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.Path);
        }

        [Fact]
        public void PathReturnsFileName()
        {
            var output = new OutputItem { File = "Test.cs" };
            Assert.Equal("Test.cs", output.Path);
        }

        [Fact]
        public void PathCombinesDirectoryAndFileName()
        {
            var output = new OutputItem { File = "Test.cs", Directory = "Folder" };
            Assert.Equal(@"Folder\Test.cs", output.Path);
        }

        [Fact]
        public void PathCombinesProjectDirectoryAndFileName()
        {
            var output = new OutputItem { File = "Test.cs", Project = @"Project\Test.proj" };
            Assert.Equal(@"Project\Test.cs", output.Path);
        }

        [Fact]
        public void PathCombinesDirectoryAndProjectDirectory()
        {
            var output = new OutputItem { File = "Test.cs", Directory = "Folder", Project = @"Project\Test.proj" };
            Assert.Equal(@"Project\Folder\Test.cs", output.Path);
        }

        [Fact]
        public void PathReturnsIgnoresProjectWhenDirectoryIsRooted()
        {
            var output = new OutputItem { File = @"C:\Folder\Test.cs", Project = @"Project\Test.proj" };
            Assert.Equal(@"C:\Folder\Test.cs", output.Path);
        }

        #endregion

        #region Project

        [Fact]
        public void ProjectIsEmptyStringByDefault()
        {
            var output = new OutputItem();
            Assert.Equal(string.Empty, output.Project);
        }

        [Fact]
        public void ProjectCanBeSet()
        {
            var output = new OutputItem { Project = "Test.proj" };
            Assert.Equal("Test.proj", output.Project);
        }

        [Fact]
        public void ProjectThrowsArgumentNullExceptionWhenNewValueIsNull()
        {
            var output = new OutputItem();
            Assert.Throws<ArgumentNullException>(() => output.Project = null);
        }

        #endregion

        #region References

        [Fact]
        public void ReferencesIsEmptyByDefault()
        {
            var output = new OutputItem();
            Assert.Empty(output.References);
        }

        #endregion
    }
}
