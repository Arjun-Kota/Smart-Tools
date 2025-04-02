using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using System.Linq;
using System.Drawing;

namespace Smart_Tools
{
    public class Smart_ToolsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Smart_ToolsComponent()
          : base("Smart Merge", "SM",
            "Automatically connects selected components to a Merge component.",
            "Smart Tools", "Workflow")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Smart Merge", "SM", "Enable auto-merge process", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // No output needed as the component modifies the canvas directly
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool smartMerge = false;
            if (!DA.GetData(0, ref smartMerge) || !smartMerge) return;

            IGH_Component mergeComp = null;
            List<IGH_DocumentObject> selectedObjs = OnPingDocument().SelectedObjects().ToList();

            // Find the Merge component in the selected objects
            foreach (IGH_DocumentObject obj in selectedObjs)
            {
                if (obj.Name == "Merge")
                {
                    mergeComp = obj as IGH_Component;
                    break;
                }
            }

            if (mergeComp == null) return; // Exit if no Merge component is found

            // Delay execution to prevent errors
            Grasshopper.Instances.ActiveCanvas.Document.ScheduleSolution(1, doc =>
            {
                List<IGH_DocumentObject> sortedObjs = selectedObjs
                    .Where(obj => obj != mergeComp) // Exclude Merge component
                    .OrderBy(obj => obj.Attributes.Pivot.Y)
                    .ThenBy(obj => obj.Attributes.Pivot.X)
                    .ToList();

                int position = 0;

                foreach (IGH_DocumentObject obj in sortedObjs)
                {
                    IGH_Param source = null;

                    if (obj is IGH_Component ghc && ghc.Params.Output.Count > 0)
                        source = ghc.Params.Output[0]; // Get first output
                    else if (obj is IGH_Param ghp)
                        source = ghp;
                    else if (obj is GH_Panel pan)
                        source = pan;

                    if (source != null)
                    {
                        while (position < mergeComp.Params.Input.Count && mergeComp.Params.Input[position].SourceCount > 0)
                        {
                            position++;
                        }

                        if (position < mergeComp.Params.Input.Count)
                        {
                            IGH_Param new_param = mergeComp.Params.Input[position];
                            new_param.AddSource(source);
                        }
                    }
                }

                mergeComp.ExpireSolution(true); // Force Grasshopper to update
            });
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("289a214c-9178-4c7d-b917-fc971bcc5b34");
    }
}