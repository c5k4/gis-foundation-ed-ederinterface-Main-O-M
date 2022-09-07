using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.CWOSL.Interfaces
{
    public interface IExtractTransformLoad
    {
        void ProcessEtl();
        void Dispose();
        //ME Q1 : 2020
        void ReconcileAndPostVersion();
    }
}
