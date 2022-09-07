using System;
using System.Runtime.InteropServices;
using Miner.Interop;

namespace PGE.BatchApplication.DLMTools.Utility
{
    internal class AUDisabler : IDisposable
    {
        IMMAutoUpdater _mmAu = null;
        mmAutoUpdaterMode _originalMode;

        bool _auState;
        internal bool AUsEnabled
        {
            get
            {
                return _auState;
            }
            set
            {
                if (!value)
                {
                    _originalMode = _mmAu.AutoUpdaterMode;
                    _mmAu.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                }
                else
                    _mmAu.AutoUpdaterMode = _originalMode;

                _auState = value;
            }
        }

        internal AUDisabler()
        {
            Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object obj = Activator.CreateInstance(type);
            _mmAu = (IMMAutoUpdater) obj;
        }

        public void Dispose()
        {
            if (_mmAu != null) Marshal.FinalReleaseComObject(_mmAu);
        }
    }
}