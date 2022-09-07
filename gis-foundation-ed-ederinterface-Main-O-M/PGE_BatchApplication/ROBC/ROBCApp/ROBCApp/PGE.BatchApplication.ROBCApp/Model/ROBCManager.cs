using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROBCApp.Model
{
    public class ROBCManager
    {
        public CircuitModel FindCircuit(int CircuitId)
        {
            return FindCircuitAPI(CircuitId);
        }

        private CircuitModel FindCircuitAPI(int CircuitId)
        {
            var model = new CircuitModel();
            return model;
        }
    }
}
