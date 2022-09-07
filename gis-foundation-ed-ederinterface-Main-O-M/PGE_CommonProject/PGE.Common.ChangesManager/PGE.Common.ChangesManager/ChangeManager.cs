using System;
using System.Collections.Generic;
using System.Reflection;
using PGE.Common.ChangesManagerShared.Interfaces;

namespace PGE.Common.ChangesManager
{
    /// <summary>
    /// Responsible for gathering all changes and alerting all subscribed parties
    /// NB: yes, this is subject/observer, not pub-sub
    /// </summary>
    public class ChangeManager : ChangePublisher
    {

        // Already have a _logger from ChangesPublisher

        private IList<IChangeDetector> _changeDetectors = new List<IChangeDetector>();

        public IList<IChangeDetector> ChangeDetectors
        {
            get
            {
                return _changeDetectors;
            }
            set
            {
                _changeDetectors = value;
            }
        }

        public ChangeManager()
        {
        }

        public void Dispose()
        {
            if (_changeDetectors != null) _changeDetectors.Clear();
            _changeDetectors = null;
            if (_changeDictionaries != null) _changeDictionaries.Clear();
        }

        public void ProcessChanges()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            foreach (IChangeDetector changeDetector in _changeDetectors)
            {
                changeDetector.Read();
                _changeDictionaries.Add(changeDetector.Inserts);
                _changeDictionaries.Add(changeDetector.Updates);
                _changeDictionaries.Add(changeDetector.Deletes);
            }
            _changeDictionaries.CalculateEditCount();

            Notify();
            DetachAll();

            _changeDictionaries.Clear();
            _changeDictionaries = null;

            // Handles things like rolling versions/fgdbs
            foreach (IChangeDetector changeDetector in _changeDetectors)
            {
                changeDetector.Cleanup();
            }

            _changeDetectors.Clear();
            _changeDetectors = null;
        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added
        /// Process Change Detection using CD API
        /// </summary>
        /// <param name="toDate"></param>

        public void ProcessChangesCD(DateTime toDate,DateTime fromDate)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            foreach (IChangeDetector changeDetector in _changeDetectors)
            {
                changeDetector.ReadCD(toDate, fromDate);
                _changeDictionaries.Add(changeDetector.Inserts);
                _changeDictionaries.Add(changeDetector.Updates);
                _changeDictionaries.Add(changeDetector.Deletes);
            }
            _changeDictionaries.CalculateEditCount();

            Notify();
            DetachAll();

            _changeDictionaries.Clear();
            _changeDictionaries = null;

            // Handles things like rolling versions/fgdbs
            foreach (IChangeDetector changeDetector in _changeDetectors)
            {
                if(changeDetector is FeatureCompareChangeDetector)
                    changeDetector.Cleanup();
            }

            _changeDetectors.Clear();
            _changeDetectors = null;
        }

        public void ProcessChangesCDBatch(DateTime toDate, DateTime fromDate, string TableName, string BatchID)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            foreach (IChangeDetector changeDetector in _changeDetectors)
            {
                changeDetector.ReadCDBatch(toDate, fromDate, TableName, BatchID);
                _changeDictionaries.Add(changeDetector.Inserts);
                _changeDictionaries.Add(changeDetector.Updates);
                _changeDictionaries.Add(changeDetector.Deletes);
            }
            _changeDictionaries.CalculateEditCount();

            _logger.Info("After Dictionary Population Batch :" + BatchID);
            _logger.Info(PGE.Common.ChangeDetectionAPI.LOGGINGDiagno.PRINTMemory());

            Notify();

            _logger.Info("Before Dictionary Release Batch :" + BatchID);
            _logger.Info(PGE.Common.ChangeDetectionAPI.LOGGINGDiagno.PRINTMemory());

            //DetachAll();
            foreach (ChangesManagerShared.ChangeDictionary changeDictionary in _changeDictionaries)
            {
                foreach(string FCName in changeDictionary.changedFeatures.Keys)
                {
                    
                    foreach(var ins in changeDictionary.changedFeatures[FCName].Action.Insert)
                    {
                        ins.Dispose();
                    }
                    foreach (var ins in changeDictionary.changedFeatures[FCName].Action.Update)
                    {
                        ins.Dispose();
                    }
                    foreach (var ins in changeDictionary.changedFeatures[FCName].Action.Delete)
                    {
                        ins.Dispose();
                    }
                    ReleaseComReference(changeDictionary.changedFeatures[FCName].FeatClass);
                }
                changeDictionary.changedFeatures = null;
            }

            _changeDictionaries.Clear();

            //_changeDictionaries = null;

            // Handles things like rolling versions/fgdbs
            foreach (IChangeDetector changeDetector in _changeDetectors)
            {
                if (changeDetector is FeatureCompareChangeDetector)
                    changeDetector.Cleanup();
            }

            //_changeDetectors.Clear();
            //_changeDetectors = null;
        }

        private void ReleaseComReference(object obj)
        {
            try
            {
                if (obj != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
