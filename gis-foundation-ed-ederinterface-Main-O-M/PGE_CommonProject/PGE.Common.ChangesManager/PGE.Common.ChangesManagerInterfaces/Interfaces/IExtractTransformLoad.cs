using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Common.ChangesManagerShared.Interfaces
{
    public interface IExtractTransformLoad
    {
        void Transfer();
        void Dispose();
    }
}
