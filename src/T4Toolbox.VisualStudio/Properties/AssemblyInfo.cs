using Microsoft.VisualStudio.Shell;
using System;

[assembly: CLSCompliant(true)]

// Help Visual Studio resolve references to the T4Toolbox assembly.
[assembly: ProvideCodeBase(
    AssemblyName = "T4Toolbox",
    PublicKeyToken = "3a2ec55667349e26",
    Version = "15.0.0.0",
    CodeBase = @"$PackageFolder$\T4Toolbox.dll")]

// This is currently required because MPF enumerates attributes applied to T4ToolboxPackage class when instantiating
// the T4ToolboxOptionsPage. This can be eliminated if I can figure out how to register directive processors via MEF instead
// of ProvideDirectiveProcessorAttribute.
[assembly: ProvideCodeBase(
    AssemblyName = "T4Toolbox.DirectiveProcessors",
    PublicKeyToken = "3a2ec55667349e26",
    Version = "15.0.0.0",
    CodeBase = @"$PackageFolder$\T4Toolbox.DirectiveProcessors.dll")]

// Help Visual Studio resolve references to the T4Toolbox.VisualStudio assembly.
[assembly: ProvideCodeBase(
    AssemblyName = "T4Toolbox.VisualStudio",
    PublicKeyToken = "3a2ec55667349e26",
    Version = "15.0.0.0",
    CodeBase = @"$PackageFolder$\T4Toolbox.VisualStudio.dll")]
