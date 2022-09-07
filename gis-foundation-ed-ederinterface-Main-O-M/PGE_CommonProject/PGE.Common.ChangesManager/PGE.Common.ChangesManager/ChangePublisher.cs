using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.Delivery.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

namespace PGE.Common.ChangesManager
{
    public abstract class ChangePublisher
    {
        protected static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        public IList<IChangeSubscriber> ChangeSubscribers
        {
            get;
            set;
        }

        protected ChangeDictionariesList _changeDictionaries = new ChangeDictionariesList();

        public ChangeDictionariesList ChangeDictionaries
        {
            get
            {
                return _changeDictionaries;
            }
        }


        public ChangePublisher()
        {

        }

        public void Attach(IChangeSubscriber changeSubscriber)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ChangeSubscribers.Add(changeSubscriber);
        }

        public void Detach(IChangeSubscriber changeSubscriber)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ChangeSubscribers.Remove(changeSubscriber);
        }
        public void DetachAll()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            foreach (IChangeSubscriber changeSubscriber in ChangeSubscribers)
            {
                changeSubscriber.Dispose();
            }
            ChangeSubscribers.Clear();
        }

        public void Notify()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            foreach (IChangeSubscriber changeSubscriber in ChangeSubscribers)
            {
                changeSubscriber.Update(_changeDictionaries);
            }
        }
    }
}
