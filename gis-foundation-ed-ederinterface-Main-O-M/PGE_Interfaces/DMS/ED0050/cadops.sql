drop table capacitor;
create table CAPACITOR
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	ANNO_POS_1 NUMBER (3), -- Y Position of annotation -- geographic view. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 6. Reserved for future use.
	ANNO_POS_2 NUMBER (3), -- Y Position of annotation -- schematic view. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 12. Reserved for future use.
	BITS NUMBER (10), -- Y Normal capacitor status for 24 hours of the day, one bit for each hour: 0=open, 1=closed, Bit 31 -- switchable?
	CAFPOS NUMBER (10), -- Y Internal key used to increase performance.
	CAID VARCHAR2 (40), -- Y Unique identifier.
	CANAME VARCHAR2 (40), -- Y Name/description.
	CA_KEY NUMBER (10), -- Y Not used.
	CS_KEY NUMBER (3), -- Y FEEDERALL case study key.
	DESCRIPTIVE_LOC VARCHAR2 (60), -- Y Descriptive location.
	DT_KEY NUMBER (10), -- Y Reference to type data. Used to drive symbology. [->DEVTYPE.DTFPOS]
	NO_KEY NUMBER (10), -- Y Node where capacitor is located [->NODE.NFPOS].
	PH3PRES NUMBER (3), -- Y Presence of capacitor on phase (3 digits, representing phases A, B and C. 111=all phases).
	RATING_1 NUMBER (8, 2), -- Y Bank size (kVAR) - 3 phase total. [*, BLF]
	RATING_2 NUMBER (8, 2), -- Y Bank size (kVAR) - phase A.
	RATING_3 NUMBER (8, 2), -- Y Bank size (kVAR) - phase B.
	RATING_4 NUMBER (8, 2), -- Y Bank size (kVAR) - phase C.
	SCADA_BITS NUMBER (6), -- Y Reserved for future use.
	STATE NUMBER (3), -- Y 1=normal, 2=planned addition, 3=planned deletion. Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	STATUS NUMBER (3), -- Y Current status of each phase (3 digits, representing phases A, B and C. 0=open, 1=closed. 100=phase A closed, B and C open.
	SWTYPE VARCHAR2 (5), -- Y Where switch on capacitor is not explicitly modeled, this indicates the type of the switch. This is an attribute of the installation, not of the capacitor.
	SYM_POS_1 NUMBER (3), -- Y Symbol position -- geographic view. 1-12 represent clock positions relative to the symbol. Reserved for future use.
	SYM_POS_2 NUMBER (3), -- Y Symbol position -- schematic view. 1-12 represent clock positions relative to the symbol. Reserved for future use.
	TAG NUMBER (10), -- Y Tag level bit field.
	UPDATE_ERROR NUMBER (4), -- Y Used for update and return code (0=when passed to ABB, 1=when added to ABB database with no errors, #=error return code).
	primary key (caid)
); 

drop table device;
create table DEVICE
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	ANNOPOS_1 NUMBER (3), -- Y Position of annotation -- geographic. 0=no annotation. 1-12 represent clock positions relative to the symbol. Default: 6. Reserved for future use.
	ANNOPOS_2 NUMBER (3), -- Y Position of annotation -- schematic. 0=no annotation. 1-12 represent clock positions relative to the symbol. Default: 12. Reserved for future use.
	ATO_KEY NUMBER (10), -- Y Indicates device is associated with ATO. [-> ATO.ATFPOS]
	BREAKAMPS NUMBER (6, 2), -- Y Maximum interrupting rating in amps for all interrupting devices (fuse, relay, recloser, breaker/switch). An analysis warning will be given if the calculated current through the device exceeds this limit. 
	BYPASS_STATUS NUMBER (3), -- Y Indicates which phases are bypassed.
	CADOPS_CONTROL NUMBER (1), -- Y Not used.
	COILRATE NUMBER (6, 2), -- Y Reclose trip coil rating (amp).
	CONSTIMEF NUMBER (6, 2), -- Y Electronic recloser TCC #1 (FAST) constant time adder (0-0.2 seconds).
	CONSTIMES NUMBER (6, 2), -- Y Electronic recloser TCC #2 (SLOW) constant time adder (0-0.2 seconds).
	CONTROLLED NUMBER (10), -- Y The node where the voltage is regulated by the transformer/regulator points to NODE table NFPOS.
	CS_KEY NUMBER (3), -- Y Not used.
	CURRENTCTA NUMBER (8, 4), -- Y Current transformer (CT) rating (phase A).
	CURRENTCTB NUMBER (8, 4), -- Y Current transformer (CT) rating (phase B).
	CURRENTCTC NUMBER (8, 4), -- Y Current transformer (CT) rating (phase C).
	CURTRANTAP NUMBER (6, 2), -- Y Relay -- current transformer tap setting.
	C_TRANSF NUMBER (6, 2), -- Y Relay -- secondary current.
	DDISTRICT NUMBER (10), -- Y District number responsible for device.
	DESCRIPTIVE_LOC VARCHAR2 (255), -- Y Descriptive location.
	DETAIL_VIEW_NAME VARCHAR2 (12), -- Y Name of table in Electric File containing more details on the device. External ASCII file will define this data requirement.
	DEVCAT NUMBER (3), -- Y Device category number (switch, breaker, or fuse, etc.), with DT_KEY points to DEVTYPE table.
	DEVICESIZE NUMBER (8, 2), -- Y Continuous current rating in amps for all interrupting devices (fuse, relay, recloser, breaker/switch). An overloaded condition will be given if the calculated current through the device exceeds this limit.
	DFPOS NUMBER (10), -- Y Internal key used to increase performance.
	DID VARCHAR2 (40), -- Y Unique identifier (set up by the interface program).
	DNAME VARCHAR2 (45), -- Y Name/description.
	DT_KEY NUMBER (10), -- Y Device type number (e.g. what type of breaker). Points to DEVTYPE table, or REGTYPE table if DEVCAT indicates the device is a line transformer or regulator (DTFPOS in DEVTYPE table or RTFPOS in REGTYPE table).
	FASTCURV VARCHAR2 (4), -- Y Fast curve code for TCC library.
	FAULT_DISTANCE NUMBER (10, 2), -- Y Distance to fault, if the device type indicates this is a fault distance reporting device. Units are defined by FAULT_DIST_UNITS in the DEVTYPE table.
	FUZESIZE NUMBER (6, 2), -- Y Fuse rating.
	INSTALL_DEVSIZE NUMBER (8, 2), -- Y Temporary installed device size.
	INSTTRIP NUMBER (6, 2), -- Y Relay setting -- instantaneous trip (secondary amps).
	INTERLOCK_NO NUMBER (6), -- Y Used by device interlock function. [->INTERLOCK.INTERLOCK_NO]
	LABEL_SEP NUMBER (1), -- Y Not used.
	LINE_END NUMBER (3), -- Y Which end of the line section is the device on: 1=NO_KEY_1 of the line, 2=NO_KEY_2 of the line.
	LI_KEY NUMBER (10), -- Y Line section the device is on, points back to LINE table (FPOS).
	LOCKACT NUMBER (6), -- Y Electronic recloser lockout-active shot number (1-4).
	LOCKMUL NUMBER (6), -- Y Electronic recloser High current lockout-multiple of min trip (1-30).
	LTH_1 NUMBER (5), -- Y Distance from base node (for display on
					  -- geographic one-line) in percent.
	LTH_2 NUMBER (5), -- Y Distance from base node (for display on schematic one-line) in percent.
	MAXBANDWA NUMBER (8, 2), -- Y Maximum bandwidth setting of LDC (120 V base) (phase A).
	MAXBANDWB NUMBER (8, 2), -- Y Maximum bandwidth setting of LDC (120 V base) (phase B).
	MAXBANDWC NUMBER (8, 2), -- Y Maximum bandwidth setting of LDC (120 V base) (phase C).
	MAXLOADBA NUMBER (8, 4), -- Y Maximum tap setting allowed by LDC (load bonus) (phase A).
	MAXLOADBB NUMBER (8, 4), -- Y Maximum tap setting allowed by LDC (load bonus) (phase B).
	MAXLOADBC NUMBER (8, 4), -- Y Maximum tap setting allowed by LDC (load bonus) (phase C).
	MINBANDWA NUMBER (8, 2), -- Y Minimum bandwidth setting of LDC (120 V base) (phase A).
	MINBANDWB NUMBER (8, 2), -- Y Minimum bandwidth setting of LDC (120 V base) (phase B).
	MINBANDWC NUMBER (8, 2), -- Y Minimum bandwidth setting of LDC (120 V base) (phase C).
	MINIRSPF NUMBER (3), -- Y Electronic recloser TCC #1 (FAST) Minimum response time (0-42 cycles).
	MINIRSPS NUMBER (3), -- Y Electronic recloser TCC #2 (SLOW) Minimum response time (0-42 cycles).
	MINLOADBA NUMBER (8, 4), -- Y Minimum tap setting allowed by LDC (load bonus) (phase A).
	MINLOADBB NUMBER (8, 4), -- Y Minimum tap setting allowed by LDC (load bonus) (phase B).
	MINLOADBC NUMBER (8, 4), -- Y Minimum tap setting allowed by LDC (load bonus) (phase C).
	MSPOS NUMBER (5), -- Y Not used.
	MULTVALF NUMBER (6, 2), -- Y Electronic recloser TCC #1 (FAST) Multiplier value (0.01-2.00).
	MULTVALS NUMBER (6, 2), -- Y Electronic recloser TCC #2 (SLOW) Multiplier value (0.01-2.00).
	NORM_STATE NUMBER (6), -- Y Normal status of the device, 3 digits (ABC). 0=Open, 1=Closed.
	PHASE NUMBER (4), -- Y Indicates which phases are built for the device, which is independent of the line phases. 111 indicates ABC.
	PHASE_GROUND VARCHAR2 (1), -- Y Indicates if the protective device is a [P]hase,[G]round, or [B]oth phase and ground device. Default is P.
	POTENTPTA NUMBER (8, 4), -- Y Potential transformer (PT) ratio (phase A).
	POTENTPTB NUMBER (8, 4), -- Y Potential transformer (PT) ratio (phase B).
	POTENTPTC NUMBER (8, 4), -- Y Potential transformer (PT) ratio (phase C).
	PREFERRED NUMBER (1), -- Y 0=device is not a preferred device for ATO, 1=device is preferred ATO device.
	QFASTTRIPS NUMBER (3), -- Y Recloser -- number of fast trips.
	QSLOWTRIPS NUMBER (3), -- Y Recloser -- number of slow trips.
	RECBREAKT NUMBER (6), -- Y Reclose breaker time (cycles).
	RECLTIM1 NUMBER (3), -- Y Reclose time for first trip (seconds).
	RECLTIM2 NUMBER (3), -- Y Reclose time for second trip (seconds).
	RECLTIM3 NUMBER (3), -- Y Reclose time for third trip (seconds).
	REQVOLTS NUMBER (8, 4), -- Y Heavy load desired voltage (pu) at the controlled node. Negative value indicates that the
							-- transformer/regulator is gang operated, 0=fixed taps).
	REQVOLTS_L NUMBER (5, 4), -- Y Light load desired voltage (in pu) at the controlled node (negative value indicates that the transformer/regulator is gang operated, 0=fixed taps).
	RESETTIME NUMBER (3), -- Y Relay -- Reset time (mechanical relays) in second.
	RSETTINGA NUMBER (8, 4), -- Y R (resistance) setting of LDC (phase A).
	RSETTINGB NUMBER (8, 4), -- Y R (resistance) setting of LDC (phase B).
	RSETTINGC NUMBER (8, 4), -- Y R (resistance) setting of LDC (phase C).
	SA_KEY NUMBER (6), -- Y Not used.
	SCADA_BITS NUMBER (6), -- Y Quality code bits for ganged devices that are monitored by SCADA systems. Values
						   -- 8192=telemetry error, 4096=manually entered, 2048=scan disabled (deactivated).
	SCADA_CONTROL NUMBER (1), -- Y Indicates that the device is monitored by SCADA.
	SCADA_CONTROL_OVERRIDE NUMBER (22, 1), -- Y Not used.
	SCADA_STATE NUMBER (3), -- Y Not used.
	SCADA_TAG_OVERRIDE NUMBER (22, 10), -- Y Not used.
	SC_KEY NUMBER (6), -- Y Not used.
	SENSING_PHASE VARCHAR2 (1), -- Y Sensing phase for a gang operated regulator/transformer. Required if gang operated regulator/transformer. A,B,C.
	SLOPE NUMBER (5), -- Y Symbol orientation in schematic.
	SLOWCURV VARCHAR2 (4), -- Y Slow curve code for TCC library.
	SS_KEY NUMBER (6), -- Y Not used.
	STATE NUMBER (3), -- Y 1=normal, 2=planned addition, 3=planned deletion. Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	STATUS NUMBER (6), -- Y Current status of the device, 3 digits (ABC): 0=Open, 1=Closed. The initial status must be input. DTC SW will maintain the status thereafter.
	SW_OPER NUMBER (1), -- Y Not used.
	TAG NUMBER (10), -- Y Tag level bit field.
	TAPSIDE NUMBER (3), -- Y Which side is the tap on (1 or 2): 1=NO_KEY_1 of the line, 2=NO_KEY_2 of the line.
	TAP_1 NUMBER (3), -- Y Input tap setting used for balanced analysis.
	TAP_2 NUMBER (3), -- Y Input tap setting on phase a.
	TAP_3 NUMBER (3), -- Y Input tap setting on phase b.
	TAP_4 NUMBER (3), -- Y Input tap setting on phase c.
	TIMEDIAL NUMBER (4, 2), -- Y Relay setting -- time dial unit.
	TRACSH NUMBER (6), -- Y Electronic recloser High current trip-active shot number (1-4).
	TRANSRATIO NUMBER (3), -- Y Relay -- current transformer turn ratio (pu).
	TRMUOMT NUMBER (6), -- Y Electronic recloser High current trip-multiple of min trip (1-30).
	TRTRTID NUMBER (6), -- Y Electronic recloser High current trip-trip time delay (1-9 cycles).
	XD_1 NUMBER (10), -- Y X-coordinate -- geographic.
	XD_2 NUMBER (10), -- Y X-coordinate -- schematic.
	XSETTINGA NUMBER (8, 4), -- Y X (reactance) setting of LDC (phase A).
	XSETTINGB NUMBER (8, 4), -- Y X (reactance) setting of LDC (phase B).
	XSETTINGC NUMBER (8, 4), -- Y X (reactance) setting of LDC (phase C).
	YD_1 NUMBER (10), -- Y Y-coordinate -- geographic.
	YD_2 NUMBER (10), -- Y Y-coordinate -- schematic. Note: If XD_1, YD_1, XD_2, YD_2 are zero, LTH_1, LTH_2 will be used.
	primary key (did)
);

drop table line;
create table LINE
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	ANNOPOS_1 NUMBER (2), -- Y Position of annotation (geographic). 1-12 represent clock positions relative to the symbol (default 6). 0=no annotation. Reserved for future use.
	ANNOPOS_2 NUMBER (2), -- Y Position of annotation (0-12) (schematic). 1-12 represent clock positions relative to the symbol (default 12). 0=no annotation. Reserved for future use.
	BITS NUMBER (10), -- Y Additional status information. Determines visibility in geographic and schematic views, as
					  -- well as role the conductor plays in the network - main circuit, tap, etc.
	BUS_BAR NUMBER (1), -- Y A value of 1 indicates that the line is part of a Substation Bus Bar.
	COLOR NUMBER (3), -- Y Not used.
	CS_KEY NUMBER (3), -- Y Not used.
	ELECTRICAL VARCHAR2 (1), -- Y Not used.
	FPOS NUMBER (10) not null, -- N Internal key used to increase performance.
	GOFFSET_1 NUMBER (3), -- Y Display offset from center of node 1 in geographic map (if busbar).
	GOFFSET_2 NUMBER (3), -- Y Display offset from center of node 2 in geographic map (if bus bar).
	ID VARCHAR2 (40) not null, -- N Unique Identifier.
	LENGTH NUMBER (10, 3), -- Y Length of line section for analysis (user units).
						   -- Units are specified separately for each customer.
	LINE_TYPE NUMBER (10), -- Y Conductor and construction type, points to the linetype table (LTFPOS) [->LINETYPE.LFPOS]. The line type data specifies the ampacity and impedances (or the conductor and construction spacing).
	NAME VARCHAR2 (40), -- Y Name/description.
	NO_KEY_1 NUMBER (10), -- Y Connected from node, points to node table (NFPOS).
	NO_KEY_2 NUMBER (10), -- Y Connected to node, points to node table (NFPOS).
	OFFSET_1 NUMBER (3), -- Y Display offset from center of node 1 in schematic map (if bus bar).
	OFFSET_2 NUMBER (3), -- Y Display offset from center of node 2 in schematic map (if bus bar).
	PA_KEY_1 NUMBER (10), -- Y First path point -- geographic, points to the PATH table. Other path points along the same line are linked together in the PATH table (see description of the PATH table). [->PATH.PAFPOS]. Note: path
						  -- points cannot be shared between views - each path point must only appear in one linked list.
	PA_KEY_2 NUMBER (10), -- Y First path point -- schematic, points to the PATH table. Other path points along the same line are linked together in the PATH table. [->PATH.PAFPOS]. Note: path points cannot be shared between views - each path point must only appear in one linked list.
	PHASE_PERM NUMBER (4), -- Y Phases that were built, position of conductors. 4 digit code represents conductor positions of phases a, b, c and neutral, respectively. Each digit must be from 0 to 5: 0=the phase or neutral is not built, 1-4 indicates position in the conductor.
	PHASE_PRESENT NUMBER (3), -- Y Not used.
	STATE NUMBER (3), -- Y Indicates whether line state is one of: (1=normal, 2=planned addition, 3=planned deletion). Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	SUB_AREA NUMBER (3), -- Y Operation authorization ID.
	SWITCHABLE NUMBER (1), -- Y Flag indicates whether a switch can be installed on this line in the future (planning software) (1=YES, 0=NO).
	TAG NUMBER (10), -- Y Tag level bit field.
	UPDATE_ERROR NUMBER (4), -- Y Used for update and return code (0=when passed to ABB, 1=when added to ABB database with no errors, #=error return code).
	primary key (id)
);


drop table load;
create table LOAD
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	ANNOPOS_1 NUMBER (3), -- Y Position of annotation -- geographic. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 6. Reserved for future use.
	ANNOPOS_2 NUMBER (3), -- Y Position of annotation -- schematic. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 12. Reserved for future use.
	BILLED_1 NUMBER (10, 2), -- Y Peak (monthly or seasonal) Last billed energy (kWh) -- 3 phase total. (A+B+C)
	BILLED_2 NUMBER (10, 2), -- Y Peak (monthly or seasonal) Last billed energy (kWh) -- phase A.
	BILLED_3 NUMBER (10, 2), -- Y Peak (monthly or seasonal) Last billed energy (kWh) -- phase B.
	BILLED_4 NUMBER (10, 2), -- Y Peak (monthly or seasonal) Last billed energy (kWh) -- phase C.
	BITS NUMBER (3), -- Y Type of load calibration: 16=scalable spot load, 32=spot load, 64=any other load.
	CONNECT_AMP NUMBER (3), -- Y Connect ampacity for load.
	COUNTPRIORITY NUMBER (5), -- Y The count of priority customers on this transformer.
	CS_KEY NUMBER (3), -- Y Not used.
	CUSTNO_1 NUMBER (10, 3), -- Y Count of total customers.
	CUSTNO_2 NUMBER (10, 3), -- Y Count of customers who use phase A.
	CUSTNO_3 NUMBER (10, 3), -- Y Count of customers who use phase B.
	CUSTNO_4 NUMBER (10, 3), -- Y Count of customers who use phase C.
	DESCRIPTIVE_LOC VARCHAR2 (255), -- Y Descriptive location.
	DISTRICT NUMBER (10), -- Y District number where load is located. Used in distributed ASTORM architecture.
	INSTALL_RATING_1 NUMBER (8, 2), -- Y Temporary installed device size.
	INSTALL_RATING_2 NUMBER (8, 2), -- Y Temporary installed device size.
	INSTALL_RATING_3 NUMBER (8, 2), -- Y Temporary installed device size.
	INSTALL_RATING_4 NUMBER (8, 2), -- Y Temporary installed device size.
	LABEL_SEP NUMBER (1), -- Y Not used.
	LITE_FACTOR NUMBER (1), -- Y Not used.
	LITE_UTIL_FACTOR NUMBER (3, 2), -- Y Indicates the loading level at light condition. If load is off at light condition, value is 0. This value is used to adjust the connected kVA size for allocating the load at light condition. Default: 1.0.
	LOFPOS NUMBER (10), -- Y Internal key used to increase performance
	LOID VARCHAR2 (40), -- Y Unique identifier. This can be the service transformer ID.
	LONAME VARCHAR2 (40), -- Y Name/description.
	LOTYPE NUMBER (3), -- Y Type of load, used for symbol display.
	NO_KEY NUMBER (10), -- Y Node where load is, points to node table (NFPOS). If 0, the load (service transformer) is assumed not installed.
	NUM1 NUMBER (10), -- Y Not used.
	PEAK_UTIL_FACTOR NUMBER (3, 2), -- Y Percent of kVA to be used for allocation of load at
									-- peak condition (per unit). Default: 1.0. [**]
	PFACT_1 NUMBER (8, 5), -- Y Power factor (3 phase total). [**]
	PFACT_2 NUMBER (8, 5), -- Y Power factor phase A. [**]
	PFACT_3 NUMBER (8, 5), -- Y Power factor phase B. [**]
	PFACT_4 NUMBER (8, 5), -- Y Power factor phase C. [**]
	PHASE NUMBER (3), -- Y Three digits represent the A, B and C phases served by the transformer (1=phase served, 0=no). EXAMPLE: 100=single-phase transformer on phase A; 111=three-phase transformer.
	RATING_1 NUMBER (8, 2), -- Y Maximum rating of the transformer connected kVA -- 3 phase total.
	RATING_2 NUMBER (8, 2), -- Y Maximum rating of the transformer connected kVA -- phase A.
	RATING_3 NUMBER (8, 2), -- Y Maximum rating of the transformer connected kVA -- phase B.
	RATING_4 NUMBER (8, 2), -- Y Maximum rating of the transformer connected kVA -- phase C.
	SCADA_BITS NUMBER (6), -- Y Not used.
	SG_KEY NUMBER (6), -- Y Symbol group key, points to symbol table. (geographic). Default: 48. [+]
	STATE NUMBER (3), -- Y 1=normal, 2=planned addition, 3=planned deletion. Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	STATUS NUMBER (3), -- Y This field is for modeling individual transformer outages (0=open, 1=closed).
	SWTYPE VARCHAR2 (5), -- Y Switch type of switchable load. Not a reference key.
	SYM_POS_1 NUMBER (3), -- Y Symbol position -- geographic. 1-12 represent clock positions relative to the symbol. Reserved for future use.
	SYM_POS_2 NUMBER (3), -- Y Symbol position -- schematic. 1-12 represent clock positions relative to the symbol. Reserved for future use.
	TAG NUMBER (10), -- Y Internal use only. Will be removed in future releases.
	TAP_1 NUMBER (3), -- Y Transformer tap setting (balanced analysis).
	TAP_2 NUMBER (3), -- Y Transformer tap setting (phase a).
	TAP_3 NUMBER (3), -- Y Transformer tap setting (phase b).
	TAP_4 NUMBER (3), -- Y Transformer tap setting (phase c).
	TT_KEY NUMBER (10), -- Y Load transformer type, points to regtype table (RTFPOS).This field is only needed if the user wants to model each transformer separately and if DTC SW needs to maintain the tap setting of the transformer, etc.
	UPDATE_ERROR NUMBER (4), -- Y Used for update and return code (0=when passed to ABB, 1=when added to ABB database with no errors, #=error return code).
	VOLT_BUS_COLOR NUMBER (3), -- Y Not used.
	primary key (loid)
);


drop table node;
create table NODE
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	BUS_ANG_1 NUMBER (3), -- Y Angle of rotation of bus bar in the geographic view, in degrees. Bus bar nodes only.
	BUS_ANG_2 NUMBER (3), -- Y Angle of rotation of bus bar in the schemtaic view, in degrees. Bus bar nodes only.
	BUS_BAR NUMBER (1), -- Y Node is part of substation BUS BAR.
	BUS_LTH NUMBER (3), -- Y Display length (in case the node is a bus bar) in schematic map. Positive value represents a horizontal bar. Negative value represents a vertical bar.
	BUS_LTH_2 NUMBER (3), -- Y Display length (in case the node is a bus bar) in geographic map. Positive value represents horizontal bar. Negative value represents a vertical bar.
	CON_TYPE NUMBER (1), -- Y Defines the connection code for the load: 1=Wye connected load, 2=Delta connected load.
	CS_KEY NUMBER (3), -- Y Not used.
	DISTRICT NUMBER (10), -- Y District number.
	ELECTRICAL VARCHAR2 (1), -- Y Not used.
	GPOS_1 NUMBER (2), -- Y Disable. Reserved for future use. Default: 3.
	GPOS_2 NUMBER (2), -- Y Disable. Reserved for future use. Default: 3.
	LABEL_SEP NUMBER (1), -- Y Not used.
	MAXCAPACITOR NUMBER (8, 2), -- Y Maximum acceptable capacitor bank size (kVAR) at a bus.
	MINCAPACITOR NUMBER (8, 2), -- Y Minimum acceptable capacitor bank size (kVAR) at a bus. Default: 0.
	NANNOPOS_1 NUMBER (2), -- Y Position of annotation -- geographic. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 6. This field overrides the default set in nodetype.
	NANNOPOS_2 NUMBER (2), -- Y Position of annotation -- schematic. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 12. This field overrides the default set in nodetype.
	NBITS NUMBER (5), -- Y Status information (low voltage, etc.). Default: 12288.
	NFPOS NUMBER (10) not null, -- N Internal key used to increase performance.
	NID VARCHAR2 (40) not null, -- N Unique identifier (generated by interface program).
	NNAME VARCHAR2 (40), -- Y Name/description.
	PFMINLAGGING NUMBER (6, 4), -- Y Analysis warning limit -- Most lagging power factor (pu).
	PFMINLEADING NUMBER (6, 4), -- Y Analysis warning limit -- Most leading power factor (pu).
	REM1 VARCHAR2 (10), -- Y Not used.
	REM2 VARCHAR2 (10), -- Y Not used.
	REM3 VARCHAR2 (10), -- Y Not used.
	SECONDARY NUMBER (1), -- Y 1=node is secondary, 0=NOT secondary.
	SG_KEY_1 NUMBER (3), -- Y Symbol group key, along with VARIANTS_1 points to symbol table (geographic). Default: 41.
	STATE NUMBER (3), -- Y 1=normal, 2=planned addition, 3=planned deletion. Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	SUB_AREA NUMBER (3), -- Y Operation authorization ID.
	SUB_ID NUMBER (10), -- Y Substation Identifier for fetching purposes. Set by program, does not need to be provided by user. Used to select area associated with a source for analysis. [-> SOURCE.SOFPOS]
	TAG NUMBER (10), -- Y Tag level bit field.
	UPDATE_ERROR NUMBER (4), -- Y Used for update and return code (0=when passed to ABB, 1=when added to ABB database with no errors, #=error return code).
	VARIANTS_1 NUMBER (3), -- Y Not used.
	VARIANTS_2 NUMBER (3), -- Y Not used.
	VLEVEL NUMBER (3), -- Y Nominal voltage, determined by DTC SW.
	VMAXPU NUMBER (6, 4), -- Y Analysis warning limit -- Maximum acceptable voltage (pu).
	VMINPU NUMBER (6, 4), -- Y Analysis warning limit -- Minimum acceptable voltage (pu).
	WNN NUMBER (5), -- Y WIDGET node number for compatibility with old version.]
	XN_1 NUMBER (10), -- Y X-coordinate -- geographic.
	XN_2 NUMBER (10), -- Y X-coordinate -- schematic.
	YN_1 NUMBER (10), -- Y Y-coordinate -- geographic. If both XN_1 and YN_1 are zero or null, the node will not be displayed in the geographic one-line diagram. This overrides the flag set in the nodetype table. If both end nodes of a line are not displayed, the line will also not be displayed.
	YN_2 NUMBER (10), -- Y Y-coordinate -- schematic. If both XN_2 and YN_2 are zero or null, the node will not be displayed in the schematic one-line diagram. This overrides the flag set in the nodetype table. If both end nodes of a line are not displayed, the line will also not be displayed.
	primary key (nid)
);


drop table path;
create table PATH
(
	CS_KEY NUMBER (3), -- Y Not used.
	NEXTP NUMBER (10), -- Y Next path point, indicates end of line section if NEXTP=0 (point back to PAFPOS).
	PAFPOS NUMBER (10), -- Y PATH key referenced by PA_KEY_1 or PA_KEY_2 of the line table.
	SUB_AREA NUMBER (3), -- Y Sub area that path point is in.
	XDIFF NUMBER (10), -- Y X-coordinate of the path point.
	YDIFF NUMBER (10), -- Y Y-coordinate of the path point.
	primary key (pafpos)
);


drop table site;
create table SITE
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	ADDRESS VARCHAR2 (45), -- Y Not used.
	ANNOPOS_1 NUMBER (3), -- Y Position of label 1.
	ANNOPOS_2 NUMBER (3), -- Y Position of label 2.
	CS_KEY NUMBER (3), -- Y Not used.
	GSITEH NUMBER (10), -- Y Height (geographic display).
	GSITEW NUMBER (10), -- Y Width (geographic display).
	GSITE_ANG NUMBER (3), -- Y Angle of rotation of site box in geographic view.
						  -- Expressed in degrees, in a counterclockwise direction.
	GXSITE NUMBER (10), -- Y Left coordinate (geographic display).
	GYSITE NUMBER (10), -- Y Upper coordinate (geographic display).
	LB_FILE VARCHAR2 (30), -- Y Landbase file name for site internals.
	NO_KEY NUMBER (10), -- Y Node to which site is attached. [->NODE.NFPOS]
	REAL_SITE_KEY NUMBER (10), -- Y Not used.
	SG_KEY NUMBER (6), -- Y [-> SYMBOL.SG_KEY]
	SG_KEY_FP NUMBER (6), -- Y Not used.
	SIFPOS NUMBER (10), -- Y Primary key.
	SIID VARCHAR2 (40), -- Y Unique text ID.
	SINAME VARCHAR2 (40), -- Y Non-Unique text name.
	SITEH NUMBER (10), -- Y Height (schematic display).
	SITEW NUMBER (10), -- Y Width (schematic display).
	SSITE_ANG NUMBER (3), -- Y Angle of rotation of site box in schematic view.
						  -- Expressed in degrees, in a counterclockwise direction.
	STATE NUMBER (3), -- Y 1=normal, 2=planned addition, 3=planned deletion. Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	TYPE NUMBER (3), -- Y 0=Substation, 1=Internal Switching Center. Also refer to Appendix A.
	UPDATE_ERROR NUMBER (4), -- Y Used for update and return code (0=when passed to ABB, 1=when added to ABB database with no errors, #=error return code).
	USERCOMMENT VARCHAR2 (100), -- Y Not used.
	XSITE NUMBER (10), -- Y Left coordinate (schematic display).
	YSITE NUMBER (10), -- Y Upper coordinate (schematic display).
	primary key (siid)
);


drop table source;
create table SOURCE
(
	ABB_INT_ID NUMBER (10), -- Y Unique identifier for each object type. Used as a link between case study and master databases for determining updates.
	ANNOPOS_1 NUMBER (3), -- Y Position of annotation -- geographic. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 6.
	ANNOPOS_2 NUMBER (3), -- Y Position of annotation -- schematic. 0=no annotation, 1-12 represent clock positions relative to the symbol. Default: 12.
	CAP_APPL_ANAL NUMBER (1), -- Y Availability of capacitor application analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	CS_KEY NUMBER (3), -- Y Not used.
	DISTRICT NUMBER (10), -- Y Not used.
	EMERGCAP NUMBER (6, 2), -- Y Emergency capacity (MVA/phase). An analysis warning will be given if the total load connected to this source, including losses, exceeds this limit.
	KVARGEN NUMBER (8, 2), -- Y kVAR generation (local gen.).
	KVAROUT_1 NUMBER (8, 2), -- Y kVAR output - 3 phase total.]
	KVAROUT_2 NUMBER (8, 2), -- Y kVAR output - phase A.
	KVAROUT_3 NUMBER (8, 2), -- Y kVAR output - phase B.
	KVAROUT_4 NUMBER (8, 2), -- Y kVAR output - phase C.
	KWGEN NUMBER (8, 2), -- Y kW generation (local gen.).]
	KWLOSS NUMBER (8, 2), -- Y kW loss.
	KWOUT_1 NUMBER (8, 2), -- Y kW output - 3 phase total.
	KWOUT_2 NUMBER (8, 2), -- Y kW output - phase A.
	KWOUT_3 NUMBER (8, 2), -- Y kW output - phase B.
	KWOUT_4 NUMBER (8, 2), -- Y kW output - phase C.
	MAXCAP NUMBER (8, 2), -- Y Normal capacity (MVA/phase). An analysis alarm will be given if the total load connected to this source, including losses, exceeds this limit.
	MAXKVAR NUMBER (8, 2), -- Y Maximum kVAR output. This field is used for load flow calculations.
	MAX_OVERLOAD NUMBER (8, 2), -- Y Maximum overload current referenced to the line in percent.
	MAX_PERCENT_DROP NUMBER (8, 2), -- Y Maximum percent voltage drop referenced to the source in percent.
	MINKVAR NUMBER (8, 2), -- Y Minimum kVAR output. This field is used for load flow calculations.
	MO_START_ANAL NUMBER (1), -- Y Availability of motor start analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	NEGSEQX NUMBER (6, 4), -- Y Negative sequence reactance.
	NET_LF_ANAL NUMBER (1), -- Y Availability of network load flow analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	NOMPHANG NUMBER (6, 2), -- Y Source voltage angle (degree) at heavy load.
	NOMPHANG_A NUMBER (6, 2), -- Y Source voltage angle phase A (degree) at heavy load.
	NOMPHANG_B NUMBER (6, 2), -- Y Source voltage angle phase B (degree) at heavy load.
	NOMPHANG_C NUMBER (6, 2), -- Y Source voltage angle phase C (degree) at heavy load.
	NOMPHANG_L NUMBER (6, 2), -- Y Source voltage angle (degree) at light load.
	NOMPHANG_L_A NUMBER (6, 2), -- Y Source voltage angle phase A (degree) at light load.
	NOMPHANG_L_B NUMBER (6, 2), -- Y Source voltage angle phase B (degree) at light load.
	NOMPHANG_L_C NUMBER (6, 2), -- Y Source voltage angle phase C (degree) at light load.
	NO_KEY NUMBER (10), -- Y Node where the source is, points to node table (NFPOS).
	O_CAP_APPL_ANAL NUMBER (1), -- Y Availability of optimized capacitor application analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	O_MO_START_ANAL NUMBER (1), -- Y Availability of optimized motor start analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	O_NET_LF_ANAL NUMBER (1), -- Y Availability of optimized network load flow analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	O_PROD_DEV_ANAL NUMBER (1), -- Y Availability of optimized protective device analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	O_SHORT_CIR_ANAL NUMBER (1), -- Y Availability of optimized short circuit analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	O_SING_PHA_ANAL NUMBER (1), -- Y Availability of optimized single phase load flow analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	O_THREE_PHA_ANAL NUMBER (1), -- Y Availability of optimized three phase load flow analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	POSSEQR NUMBER (8, 4), -- Y Positive sequence resistance (ohm).
	POSSEQX NUMBER (8, 4), -- Y Positive sequence reactance (ohm).
	PROD_DEV_ANAL NUMBER (1), -- Y Availability of protective device analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	RATIO_PI NUMBER (5, 2), -- Y Ratio of real constant impedance load.
	RATIO_PR NUMBER (5, 2), -- Y Ratio of real constant power load.
	RATIO_QI NUMBER (5, 2), -- Y Ratio of reactive constant impedance load.
	RATIO_QR NUMBER (5, 2), -- Y Ratio of reactive constant power load.
	SCADA_BITS NUMBER (6), -- Y Not used.
	SCOLIND NUMBER (3), -- Y Source color for line coloring. One option for line coloring which can be selected online is line color by source. In this option, each line will be drawn in the color of the source it is connected to.
	SG_KEY NUMBER (8), -- Y Symbol group key, points to symbol table (geographic). Default: 1.
	SHORT_CIR_ANAL NUMBER (1), -- Y Availability of short circuit analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	SING_PHA_ANAL NUMBER (1), -- Y Availability of single phase load flow analysis results. 0=no results, 1=heavy load results, 2=light load results, 3=both (heavy and light).
	SOFPOS NUMBER (10), -- N Internal key used to increase performance.
	SOID VARCHAR2 (40), -- Y Unique identifier.
	SONAME VARCHAR2 (40), -- Y Name description.
	STATE NUMBER (3), -- Y 1=normal, 2=planned addition, 3=planned deletion. Network Manager DMS ignores planned additions, while Network Planner ignores planned deletions.
	STATUS NUMBER (3), -- Y Indicates status for each phase of source (3 digits represent phases A, B and C). 0=out of service, 1=OK.
	STYPE NUMBER (3), -- Y Type of source: 0=pseudo source (used for line coloring only); 1=swing bus; 2=P, Q generation; 3=P, V (3 phase); 4=P, V phase A; 5=P, V phase B; 6=P, V phase C.
	SUBTRAX NUMBER (6, 4), -- Y Subtransient reactance.
	SVPU_A NUMBER (6, 4), -- Y Source voltage phase A (pu) at heavy load.
	SVPU_B NUMBER (6, 4), -- Y Source voltage phase B (pu) at heavy load.
	SVPU_C NUMBER (6, 4), -- Y Source voltage phase C (pu) at heavy load.
	SVPU_L_A NUMBER (5, 4), -- Y Source voltage phase A (pu) at light load.
	SVPU_L_B NUMBER (5, 4), -- Y Source voltage phase B (pu) at light load.
	SVPU_L_C NUMBER (5, 4), -- Y Source voltage phase C (pu) at light load.
	SVPU_L_T NUMBER (5, 4), -- Y Equivalent single phase voltage in pu at light load.
	SVPU_T NUMBER (6, 4), -- Y Source voltage (pu) at heavy load. This field represents the starting voltage at the source for calculating the voltage of each connected node. It can be changed on line.
	SYM_POS_1 NUMBER (3), -- Y Symbol position -- geographic.
	SYM_POS_2 NUMBER (3), -- Y Symbol position -- schematic.
	TAG NUMBER (10), -- Y No longer used.
	THREE_PHA_ANAL NUMBER (1), -- Y Not used.
	TRANSIX NUMBER (6, 4), -- Y Transient reactance.
	UPDATE_ERROR NUMBER (4), -- Y Used for update and return code (0=when passed to ABB, 1=when added to ABB database with no errors, #=error return code).
	UPSTREAM_LINE NUMBER (10), -- Y Not used.
	USAGE NUMBER (1), -- Y Flag to identify whether source participates in load flow analysis and/or optimization. 0=both load flow and optimization, 1=only load flow, 2=only optimization, 4=neither load flow nor optimization.
	VLEVEL NUMBER (3), -- Y Nominal voltage level. The table VOLTAGELV defines the nominal kV for each level.
	VOLT_COMP NUMBER (5, 2), -- Y Voltage compensation factor with respect to active load in per unit.
	ZEROSEQR NUMBER (8, 4), -- Y Zero sequence resistance (ohm).
	ZEROSEQX NUMBER (8, 4), -- Y Zero sequence impedance (ohm).
	primary key (soid)
);
