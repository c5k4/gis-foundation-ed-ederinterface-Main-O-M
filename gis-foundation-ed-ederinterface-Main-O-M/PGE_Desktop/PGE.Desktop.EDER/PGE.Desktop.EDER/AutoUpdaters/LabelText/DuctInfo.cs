using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Miner.Interop;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// Immutable class for duct definition objects to facilitate linq queries.
    /// </summary>
    public class DuctInfo
    {

        /// <summary>
        /// Use  DuctInfo(IMMDuctDefinition def)
        /// </summary>
        protected DuctInfo() { }

        /// <summary>
        /// Gets the duct diameter.
        /// </summary>
        public float Diameter { get; private set; }
        
        /// <summary>
        /// Gets the duct material.
        /// </summary>
        public string Material { get; private set; }
  
        /// <summary>
        /// Gets a value indicating whether this <see cref="DuctInfo"/> is available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if available; otherwise, <c>false</c>.
        /// </value>
        public bool Available { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuctInfo"/> class.  
        /// </summary>
        /// <param name="def">The def.</param>
        public DuctInfo(IMMDuctDefinition def)
        {
            Diameter = def.diameter;
            Material = def.material ?? string.Empty;

            Available = def.availability;

        }

        public override string ToString()
        {
            return Diameter.ToString() + "\" " + Material;
        }
    }
}
