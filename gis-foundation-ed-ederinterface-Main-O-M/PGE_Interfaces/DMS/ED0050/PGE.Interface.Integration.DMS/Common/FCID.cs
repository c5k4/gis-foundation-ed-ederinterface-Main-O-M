using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// Class used to map Feature Class IDs to static IDs
    /// </summary>
    public class FCID : IDictionary<int,int>
    {
        /// <summary>
        /// ED Generic Electric Junction
        /// DMS Node
        /// </summary>
        public const int Junction = 0;
        /// <summary>
        /// ED Primary Overhead Conductor
        /// DMS Line
        /// </summary>
        public const int PrimaryOH = 1;
        /// <summary>
        /// ED Primary Underground Conductor
        /// DMS Line
        /// </summary>
        public const int PrimaryUG = 2;
        /// <summary>
        /// ED Busbar
        /// DMS Line
        /// </summary>
        public const int DistBusBar = 3;
        /// <summary>
        /// ED Electric Stitch Point, Source Device
        /// DMS Source, Node
        /// </summary>
        public const int StitchPoint = 4;
        /// <summary>
        /// ED Device Group, non electric feature used to logically link devices into a group
        /// DMS Site, Node
        /// </summary>
        public const int DeviceGroup = 5;
        /// <summary>
        /// ED Transformer
        /// DMS Load, Node
        /// </summary>
        public const int Transformer = 6;
        /// <summary>
        /// ED Capacitor
        /// DMS Capacitor, Node
        /// </summary>
        public const int Capacitor = 7;
        /// <summary>
        /// ED Switch
        /// DMS Device, Node
        /// </summary>
        public const int Switch = 8;
        /// <summary>
        /// ED StepDown
        /// DMS Device, Node
        /// </summary>
        public const int StepDown = 9;
        /// <summary>
        /// ED Fuse
        /// DMS Device, Node
        /// </summary>
        public const int Fuse = 10;
        /// <summary>
        /// ED Primary Meter
        /// DMS Load, Node
        /// </summary>
        public const int PrimaryMeter = 11;
        /// <summary>
        /// ED Dynamic Protective Device
        /// DMS Device, Node
        /// </summary>
        public const int DynamicProtectiveDevice = 12;
        /// <summary>
        /// ED Voltage Regulator
        /// DMS Device, Node
        /// </summary>
        public const int VoltageRegulator = 13;
        /// <summary>
        /// ED Open Point
        /// DMS Device, Node
        /// </summary>
        public const int OpenPoint = 14;
        /// <summary>
        /// ED Tie
        /// DMS Node
        /// </summary>
        public const int Tie = 15;
        /// <summary>
        /// ED Primary Generation
        /// DMS Source, Node
        /// </summary>
        public const int PrimaryGen = 16;
        /// <summary>
        /// ED Fault Indicator
        /// DMS Device, Node
        /// </summary>
        public const int FaultInd = 17;
        /// <summary>
        /// ED Overhead ConductorInfo table related to ED Primary Overhead Conductor
        /// </summary>
        public const int OHInfo = 18;
        /// <summary>
        ///  ED Underground ConductorInfo table related to ED Primary Underground Conductor
        /// </summary>
        public const int UGInfo = 19;
        /// <summary>
        /// ED Controller table related to Capacitor, Voltage Regulator, and Dynamic Protective Device
        /// </summary>
        public const int Controller = 20;

        /// <summary>
        /// Substation Overhead Conductor
        /// DMS Line
        /// </summary>
        public const int SUBPrimaryOH = 21;
        /// <summary>
        /// Substation Underground Conductor
        /// DMS Line
        /// </summary>
        public const int SUBPrimaryUG = 22;
        /// <summary>
        /// Substation Busbar
        /// DMS Line
        /// </summary>
        public const int SUBBusBar = 23;
        /// <summary>
        /// Substation Stitch Point
        /// DMS Source, Node
        /// </summary>
        public const int SUBStitchPoint = 24;
        /// <summary>
        /// Substation Generic Junction
        /// DMS Node
        /// </summary>
        public const int SUBJunction = 25;
        /// <summary>
        /// Substation Riser
        /// DMS Node
        /// </summary>
        public const int SUBRiser = 26;
        /// <summary>
        /// Substation Current Transformer
        /// DMS Node
        /// </summary>
        public const int SUBCurrentTransformer = 27;
        /// <summary>
        /// Substation Potential Transformer
        /// DMS Device, Node
        /// </summary>
        public const int SUBPotentialTransformer = 28;
        /// <summary>
        /// Substation Service Transformer
        /// DMS Load, Node
        /// </summary>
        public const int SUBStationTransformer = 29;
        /// <summary>
        /// Substation Transformer Bank
        /// DMS Device, Node
        /// </summary>
        public const int SUBTransformer = 30;
        /// <summary>
        /// Substation Switch
        /// DMS Device, Node
        /// </summary>
        public const int SUBSwitch = 31;
        /// <summary>
        /// Substation Fuse
        /// DMS Device, Node
        /// </summary>
        public const int SUBFuse = 32;
        /// <summary>
        /// Substation Capacitor
        /// DMS Capacitor, Node
        /// </summary>
        public const int SUBCapacitor = 33;
        /// <summary>
        /// Substation Cell
        /// Currently not used
        /// </summary>
        public const int SUBCell = 34;
        /// <summary>
        /// Substation Reactor
        /// DMS Device, Node
        /// </summary>
        public const int SUBReactor = 35;
        /// <summary>
        /// Substation Tie
        /// DMS Node
        /// </summary>
        public const int SUBTie = 36;
        /// <summary>
        /// Substation Voltage Regulator
        /// DMS Device, Node
        /// </summary>
        public const int SUBVoltageRegulator = 37;
        /// <summary>
        /// Substation Interrupting Device
        /// DMS Device, Node
        /// </summary>
        public const int SUBInterruptingDevice = 38;
        /// <summary>
        /// Substation MTU
        /// DMS Device, Node
        /// </summary>
        public const int SUBMTU = 39;

        /// <summary>
        /// ED Transformer Unit table related to ED Transformer
        /// </summary>
        public const int TransformerUnit = 40;
        /// <summary>
        /// ED Voltage Regulator Unit table related to ED Voltage Regulator
        /// </summary>
        public const int VoltageRegulatorUnit = 41;
        /// <summary>
        /// Substation and ED Circuit Source Table, related to Substation Stitch Point, and ED Stitch Point
        /// </summary>
        public const int SUBCircuitSource = 42;
        /// <summary>
        /// Substation Overhead ConductorInfo table related to Substation Overhead Conductor
        /// </summary>
        public const int SUBOHInfo = 43;
        /// <summary>
        /// Substation Underground ConductorInfo table related to Substation Underground Conductor
        /// </summary>
        public const int SUBUGInfo = 44;

        /// <summary>
        /// ED DC Rectifier
        /// DMS Node (secondary)
        /// </summary>
        public const int DCRectifier = 45;
        /// <summary>
        /// ED Delivery Point
        /// DMS Node (secondary)
        /// </summary>
        public const int DeliveryPoint = 46;
        /// <summary>
        /// ED Secondary Generation
        /// DMS Node (secondary)
        /// </summary>
        public const int SecondaryGeneration = 47;
        /// <summary>
        /// ED Service Location
        /// DMS Node (secondary)
        /// </summary>
        public const int ServiceLocation = 48;
        /// <summary>
        /// ED Smart Meter
        /// DMS Node (secondary)
        /// </summary>
        public const int SmartMeterNetworkDevice = 49;
        /// <summary>
        /// ED Street Light
        /// DMS Node (secondary)
        /// </summary>
        public const int StreetLight = 50;

        /// <summary>
        /// Substation Link
        /// DMS Device, Node
        /// </summary>
        public const int SUBLink = 51;
        /// <summary>
        /// Substation Extra Winding
        /// DMS Device, Node
        /// </summary>
        public const int SUBExtraWinding = 52;
        /// <summary>
        /// Substation Transformer Unit table related to Substation Transformer Bank
        /// </summary>
        public const int SUBTransformerUnit = 53;
        /// <summary>
        /// Substation Transformer Rating table related to Substation Transformer Unit table
        /// </summary>
        public const int SUBTransformerRating = 54;
        /// <summary>
        /// Substation Load Tap Changer table related to Substation Transformer Bank
        /// </summary>
        public const int SUBLoadTapChanger = 55;
        /// <summary>
        /// Substation Voltage Regulator Unit table related to Substation Voltage Regulator
        /// </summary>
        public const int SUBVoltageRegulatorUnit = 56;


        /// <summary>
        /// ED Primary Riser
        /// DMS Node
        /// </summary>
        public const int PrimaryRiser = 57;

        public const int DCConductor = 58;
        public const int SecondaryOH = 59;
        public const int SecondaryUG = 60;
        public const int TransformerLead = 61;
        public const int SUBLightningArrestor = 62;
        public const int SCADA = 63;
        public const int NetworkProtector = 64;

        /*Changes for ENOS to SAP migration - DMS .. Start */
        public const int ServicePoint = 65;
        public const int GenerationInfo = 66;
        /*Changes for ENOS to SAP migration - DMS .. End*/

        private Dictionary<int, int> _fcids;
        private static FCID _value;
        private static Dictionary<int, int> _map;
        
        /// <summary>
        /// Loads all the FCID mappings from the configuration file
        /// </summary>
        public FCID()
        {
            _fcids = new Dictionary<int, int>();
            //load this from a config file instead
            _fcids.Add(FCID.Junction, Configuration.FCIDS["JU"]);
            _fcids.Add(FCID.PrimaryOH, Configuration.FCIDS["PO"]);
            _fcids.Add(FCID.PrimaryUG, Configuration.FCIDS["PU"]);
            _fcids.Add(FCID.DistBusBar, Configuration.FCIDS["DI"]);
            _fcids.Add(FCID.StitchPoint, Configuration.FCIDS["ST"]);
            _fcids.Add(FCID.DeviceGroup, Configuration.FCIDS["DV"]);
            _fcids.Add(FCID.Transformer, Configuration.FCIDS["TR"]);
            _fcids.Add(FCID.Capacitor, Configuration.FCIDS["CP"]);

            _fcids.Add(FCID.Switch, Configuration.FCIDS["SW"]);
            _fcids.Add(FCID.StepDown, Configuration.FCIDS["SE"]);
            _fcids.Add(FCID.Fuse, Configuration.FCIDS["FU"]);
            _fcids.Add(FCID.PrimaryMeter, Configuration.FCIDS["PM"]);
            _fcids.Add(FCID.DynamicProtectiveDevice, Configuration.FCIDS["DP"]);
            _fcids.Add(FCID.VoltageRegulator, Configuration.FCIDS["VR"]);
            _fcids.Add(FCID.OpenPoint, Configuration.FCIDS["OP"]);
            _fcids.Add(FCID.Tie, Configuration.FCIDS["TI"]);
            /*Changes for ENOS to SAP migration - DMS . Start */
            //_fcids.Add(FCID.PrimaryGen, Configuration.FCIDS["PG"]);
            /*Changes for ENOS to SAP migration - DMS . End */
            _fcids.Add(FCID.FaultInd, Configuration.FCIDS["FI"]);
            _fcids.Add(FCID.OHInfo, Configuration.FCIDS["OI"]);
            _fcids.Add(FCID.UGInfo, Configuration.FCIDS["UI"]);
            _fcids.Add(FCID.Controller, Configuration.FCIDS["CO"]);
            _fcids.Add(FCID.SCADA, Configuration.FCIDS["SC"]);
            _fcids.Add(FCID.TransformerUnit, Configuration.FCIDS["TU"]);
            _fcids.Add(FCID.VoltageRegulatorUnit, Configuration.FCIDS["VU"]);
            _fcids.Add(FCID.PrimaryRiser, Configuration.FCIDS["RI"]);
            _fcids.Add(FCID.NetworkProtector, Configuration.FCIDS["NP"]);

            _fcids.Add(FCID.SUBPrimaryOH, Configuration.SUBFCIDS["OH"]);
            _fcids.Add(FCID.SUBPrimaryUG, Configuration.SUBFCIDS["UG"]);
            _fcids.Add(FCID.SUBBusBar, Configuration.SUBFCIDS["BU"]);
            _fcids.Add(FCID.SUBStitchPoint, Configuration.SUBFCIDS["SI"]);
            _fcids.Add(FCID.SUBJunction, Configuration.SUBFCIDS["JU"]);
            _fcids.Add(FCID.SUBRiser, Configuration.SUBFCIDS["RI"]);
            _fcids.Add(FCID.SUBPotentialTransformer, Configuration.SUBFCIDS["PT"]);
            _fcids.Add(FCID.SUBCurrentTransformer, Configuration.SUBFCIDS["CT"]);
            _fcids.Add(FCID.SUBStationTransformer, Configuration.SUBFCIDS["ST"]);
            _fcids.Add(FCID.SUBTransformer, Configuration.SUBFCIDS["TR"]);
            _fcids.Add(FCID.SUBSwitch, Configuration.SUBFCIDS["SW"]);
            _fcids.Add(FCID.SUBFuse, Configuration.SUBFCIDS["FU"]);
            _fcids.Add(FCID.SUBCapacitor, Configuration.SUBFCIDS["CA"]);
            _fcids.Add(FCID.SUBCell, Configuration.SUBFCIDS["CE"]);
            _fcids.Add(FCID.SUBReactor, Configuration.SUBFCIDS["RE"]);
            _fcids.Add(FCID.SUBTie, Configuration.SUBFCIDS["TI"]);
            _fcids.Add(FCID.SUBVoltageRegulator, Configuration.SUBFCIDS["VO"]);
            _fcids.Add(FCID.SUBInterruptingDevice, Configuration.SUBFCIDS["ID"]);
            _fcids.Add(FCID.SUBMTU, Configuration.SUBFCIDS["MT"]);
            _fcids.Add(FCID.SUBCircuitSource, Configuration.SUBFCIDS["CS"]);
            _fcids.Add(FCID.SUBOHInfo, Configuration.SUBFCIDS["OI"]);
            _fcids.Add(FCID.SUBUGInfo, Configuration.SUBFCIDS["UI"]);
            _fcids.Add(FCID.SUBLink, Configuration.SUBFCIDS["LI"]);
            _fcids.Add(FCID.SUBExtraWinding, Configuration.SUBFCIDS["EX"]);
            _fcids.Add(FCID.SUBTransformerUnit, Configuration.SUBFCIDS["TU"]);
            _fcids.Add(FCID.SUBTransformerRating, Configuration.SUBFCIDS["TA"]);
            _fcids.Add(FCID.SUBLoadTapChanger, Configuration.SUBFCIDS["LO"]);
            _fcids.Add(FCID.SUBVoltageRegulatorUnit, Configuration.SUBFCIDS["VU"]);
            _fcids.Add(FCID.SUBLightningArrestor, Configuration.SUBFCIDS["LA"]);

            _fcids.Add(FCID.DCRectifier, Configuration.NonFCIDS["DC"]);
            _fcids.Add(FCID.DeliveryPoint, Configuration.NonFCIDS["DE"]);
            _fcids.Add(FCID.SecondaryGeneration, Configuration.NonFCIDS["SG"]);
            /*Changes for ENOS to SAP migration - DMS . Start */
            //_fcids.Add(FCID.ServiceLocation, Configuration.NonFCIDS["SL"]);
            /*Changes for ENOS to SAP migration - DMS . End */
            _fcids.Add(FCID.SmartMeterNetworkDevice, Configuration.NonFCIDS["SM"]);
            _fcids.Add(FCID.StreetLight, Configuration.NonFCIDS["ST"]);

            _fcids.Add(FCID.DCConductor, Configuration.NonFCIDS["SD"]);
            _fcids.Add(FCID.SecondaryOH, Configuration.NonFCIDS["SO"]);
            _fcids.Add(FCID.SecondaryUG, Configuration.NonFCIDS["SU"]);
            _fcids.Add(FCID.TransformerLead, Configuration.NonFCIDS["TL"]);
            /*Changes for ENOS to SAP migration - DMS . Start */
            _fcids.Add(FCID.ServiceLocation, Configuration.FCIDS["SL"]);
            _fcids.Add(FCID.ServicePoint, Configuration.FCIDS["SP"]);
            _fcids.Add(FCID.GenerationInfo, Configuration.FCIDS["GI"]);
            /*Changes for ENOS to SAP migration - DMS .. End*/
            
        }

        /// <summary>
        /// Dictionary mapping a FCID, the dictionary key, to its static value, the dictionary value
        /// </summary>
        public static Dictionary<int, int> Map
        {
            get
            {
                if (_map == null)
                {
                    _map = new Dictionary<int, int>();
                    _map.Add(Value[FCID.Junction], FCID.Junction);
                    _map.Add(Value[FCID.PrimaryOH], FCID.PrimaryOH);
                    _map.Add(Value[FCID.PrimaryUG], FCID.PrimaryUG);
                    _map.Add(Value[FCID.DistBusBar], FCID.DistBusBar);
                    _map.Add(Value[FCID.StitchPoint], FCID.StitchPoint);
                    _map.Add(Value[FCID.DeviceGroup], FCID.DeviceGroup);
                    _map.Add(Value[FCID.Transformer], FCID.Transformer);
                    _map.Add(Value[FCID.Capacitor], FCID.Capacitor);
                    _map.Add(Value[FCID.Switch], FCID.Switch);
                    _map.Add(Value[FCID.StepDown], FCID.StepDown);
                    _map.Add(Value[FCID.Fuse], FCID.Fuse);
                    _map.Add(Value[FCID.PrimaryMeter], FCID.PrimaryMeter);
                    _map.Add(Value[FCID.DynamicProtectiveDevice], FCID.DynamicProtectiveDevice);
                    _map.Add(Value[FCID.VoltageRegulator], FCID.VoltageRegulator);
                    _map.Add(Value[FCID.OpenPoint], FCID.OpenPoint);
                    _map.Add(Value[FCID.Tie], FCID.Tie);
                    /*Changes for ENOS to SAP migration - DMS .. Start */
                    //_map.Add(Value[FCID.PrimaryGen], FCID.PrimaryGen);
                    /*Changes for ENOS to SAP migration - DMS .. End */
                    _map.Add(Value[FCID.FaultInd], FCID.FaultInd);
                    _map.Add(Value[FCID.OHInfo], FCID.OHInfo);
                    _map.Add(Value[FCID.UGInfo], FCID.UGInfo);
                    _map.Add(Value[FCID.Controller], FCID.Controller);
                    _map.Add(Value[FCID.TransformerUnit], FCID.TransformerUnit);
                    _map.Add(Value[FCID.PrimaryRiser], FCID.PrimaryRiser);
                    _map.Add(Value[FCID.SCADA], FCID.SCADA);
                    _map.Add(Value[FCID.NetworkProtector], FCID.NetworkProtector);

                    _map.Add(Value[FCID.SUBPrimaryOH], FCID.SUBPrimaryOH);
                    _map.Add(Value[FCID.SUBPrimaryUG], FCID.SUBPrimaryUG);
                    _map.Add(Value[FCID.SUBBusBar], FCID.SUBBusBar);
                    _map.Add(Value[FCID.SUBStitchPoint], FCID.SUBStitchPoint);
                    _map.Add(Value[FCID.SUBJunction], FCID.SUBJunction);
                    _map.Add(Value[FCID.SUBRiser], FCID.SUBRiser);
                    _map.Add(Value[FCID.SUBPotentialTransformer], FCID.SUBPotentialTransformer);
                    _map.Add(Value[FCID.SUBCurrentTransformer], FCID.SUBCurrentTransformer);
                    _map.Add(Value[FCID.SUBStationTransformer], FCID.SUBStationTransformer);
                    _map.Add(Value[FCID.SUBTransformer], FCID.SUBTransformer);
                    _map.Add(Value[FCID.SUBSwitch], FCID.SUBSwitch);
                    _map.Add(Value[FCID.SUBFuse], FCID.SUBFuse);
                    _map.Add(Value[FCID.SUBCapacitor], FCID.SUBCapacitor);
                    _map.Add(Value[FCID.SUBCell], FCID.SUBCell);
                    _map.Add(Value[FCID.SUBReactor], FCID.SUBReactor);
                    _map.Add(Value[FCID.SUBTie], FCID.SUBTie);
                    _map.Add(Value[FCID.SUBVoltageRegulator], FCID.SUBVoltageRegulator);
                    _map.Add(Value[FCID.SUBInterruptingDevice], FCID.SUBInterruptingDevice);
                    _map.Add(Value[FCID.SUBMTU], FCID.SUBMTU);
                    _map.Add(Value[FCID.SUBLink], FCID.SUBLink);
                    _map.Add(Value[FCID.SUBExtraWinding], FCID.SUBExtraWinding);
                    _map.Add(Value[FCID.SUBLightningArrestor], FCID.SUBLightningArrestor);


                    _map.Add(Value[FCID.DCRectifier], FCID.DCRectifier);
                    _map.Add(Value[FCID.DeliveryPoint], FCID.DeliveryPoint);
                    _map.Add(Value[FCID.SecondaryGeneration], FCID.SecondaryGeneration);
                    _map.Add(Value[FCID.ServiceLocation], FCID.ServiceLocation);
                    _map.Add(Value[FCID.SmartMeterNetworkDevice], FCID.SmartMeterNetworkDevice);
                    _map.Add(Value[FCID.StreetLight], FCID.StreetLight);

                    /*Changes for ENOS to SAP migration - DMS .. Start */
                    _map.Add(Value[FCID.ServicePoint], FCID.ServicePoint);
                    _map.Add(Value[FCID.GenerationInfo], FCID.GenerationInfo);
                    /*Changes for ENOS to SAP migration - DMS .. End*/
                }

                return FCID._map;
            }

        }

        /// <summary>
        /// A static instance of the FCID class used by other classes to look up FCID mappings
        /// </summary>
        public static FCID Value
        {
            get
            {
                if (_value == null)
                {
                    _value = new FCID();
                }

                return FCID._value;
            }
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(int key, int value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Check if there is a mapping for the FCID
        /// </summary>
        /// <param name="key">The FCID</param>
        /// <returns>True if there is a mapping</returns>
        public bool ContainsKey(int key)
        {
            return _fcids.ContainsKey(key);
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        public ICollection<int> Keys
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(int key, out int value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        public ICollection<int> Values
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Look up the static value for a FCID. This is not settable.
        /// </summary>
        /// <param name="key">The FCID</param>
        /// <returns>The static value the FCID maps to</returns>
        public int this[int key]
        {
            get
            {
                return _fcids[key];
            }
            
            set
            {
                //mappings should only every be set once
                throw new NotImplementedException();
            }
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Not Implemented
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
