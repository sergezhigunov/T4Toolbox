// <copyright file="AssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(T4Toolbox.AssemblyInfo.Name)]
[assembly: CLSCompliant(true)]

namespace T4Toolbox
{
    /// <summary>
    /// Defines constants for common assembly information.
    /// </summary>
    /// <remarks>
    /// This class is defined here instead of the CommonAssemblyInfo.cs because we need to use the
    /// <see cref="InternalsVisibleToAttribute"/> to enable access to internal members for testing.
    /// With internals visible to all projects in the solution, defining this class in every project
    /// generates compiler errors due to naming collisions.
    /// </remarks>
    internal abstract class AssemblyInfo
    {
        /// <summary>
        /// Name of the T4 Toolbox assembly.
        /// </summary>
        public const string Name = "T4Toolbox";

        /// <summary>
        /// Name of the product.
        /// </summary>
        public const string Product = "T4 Toolbox";

        /// <summary>
        /// Description of the product.
        /// </summary>
        public const string Description = "Extends code generation capabilities of Text Templates.";

#if SIGN_ASSEMBLY
        public const string PublicKey = ", PublicKey=002400000480000094000000060200000024000052534131000400000100010011CC907C694D8D5A9F25C44F0D03922B57CB7BC49DD1CC308030DA72F79DC08661EE909C8187330AD6F5C111CE95C2DA494C148562109DFF2B2134D77018EF2C1C6E120052A70C5B31C4127D7038A3A7DB16376E287C79223CF01FD040B2584D7DE446D0927B1489461BFA5C1DE8A5BF5FDB8B68E06F99E5B95BC9CB5FBFB4CA";
        public const string PublicKeyToken = "3a2ec55667349e26";
#else
        public const string PublicKey = "";
        public const string PublicKeyToken = "";
#endif

        /// <summary>
        /// Version of the T4 Toolbox assembly.
        /// </summary>
        public const string Version = "15.0.0.0";
    }
}