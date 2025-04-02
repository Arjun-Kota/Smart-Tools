using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace Smart_Tools
{
    public class Smart_ToolsInfo : GH_AssemblyInfo
    {
        public override string Name => "Smart Tools";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("0f1a2d7a-fa59-4424-bd74-208d2b41d5bf");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}