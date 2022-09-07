using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.ChangesManager.Interfaces
{
    public interface IChangeSubscriber
    {
        void Update(ChangePublisher changesPublisher);
    }
}
