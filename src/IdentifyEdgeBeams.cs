using Elements;
using Elements.Geometry;
using Elements.Spatial.CellComplex;
using System.Collections.Generic;
using System.Linq;

namespace IdentifyEdgeBeams
{
    public static class IdentifyEdgeBeams
    {
        /// <summary>
        /// The IdentifyEdgeBeams function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A IdentifyEdgeBeamsOutputs instance containing computed results and the model with any new elements.</returns>
        public static IdentifyEdgeBeamsOutputs Execute(Dictionary<string, Model> inputModels, IdentifyEdgeBeamsInputs input)
        {
            var structure = inputModels["Structure"];
            var bays = inputModels["Bays"];

            var cellComplex = bays.AllElementsOfType<CellComplex>().FirstOrDefault();

            IEnumerable<ElementInstance> elements = null;
            switch (input.SelectionType)
            {
                case IdentifyEdgeBeamsInputsSelectionType.Columns:
                    elements = structure.AllElementsOfType<ElementInstance>()
                                        .Where(ei => ei.BaseDefinition is Column);
                    break;
                case IdentifyEdgeBeamsInputsSelectionType.Edge_Beams:
                    elements = structure.AllElementsOfType<ElementInstance>()
                                        .Where(ei => ei.BaseDefinition is Beam)
                                        .Where(b => b.AdditionalProperties.ContainsKey("ExternalEdgeId"));
                    break;
            }

            var model = new Model();

            var selectedMaterial = new Material("selected", Colors.Yellow, unlit: true);
            var definitions = elements.Select(e => e.BaseDefinition).Distinct();
            foreach (var d in definitions)
            {
                d.Material = selectedMaterial;
            }
            model.AddElements(elements);

            var output = new IdentifyEdgeBeamsOutputs()
            {
                Model = model
            };

            return output;
        }
    }
}