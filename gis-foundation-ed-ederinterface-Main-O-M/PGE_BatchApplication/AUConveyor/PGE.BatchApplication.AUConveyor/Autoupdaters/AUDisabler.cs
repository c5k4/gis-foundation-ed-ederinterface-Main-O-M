using System;
using System.Runtime.InteropServices;
using Miner.Interop;

namespace PGE.BatchApplication.AUConveyor.Autoupdaters
{
    /// <summary>
    /// Contains the logic necessary to disable and re-enable all AUs during execution.
    /// </summary>
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

            _mmAu = obj as IMMAutoUpdater;
        }

        public void Dispose()
        {
            if (_mmAu != null) while (Marshal.ReleaseComObject(_mmAu) > 0) { }
        }
    }
}
