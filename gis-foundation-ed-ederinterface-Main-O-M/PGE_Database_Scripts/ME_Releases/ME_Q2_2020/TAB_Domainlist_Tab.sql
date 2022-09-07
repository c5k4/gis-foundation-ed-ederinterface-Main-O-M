REM
REM tjj4 9/13/17 updated Division Names for Sonoma and Humboldt
REM              removed linefeed chars that were causing desktop sqlplus to fail

CREATE TABLE EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME        VARCHAR2(255),
    DOMAIN_CODE        VARCHAR2(255),
    DOMAIN_DESCRIPTION VARCHAR2(255)
  ) ;
COMMIT;
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'AIW',
    'Aetna Insulated Wire Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'ALC',
    'Alcan Cable Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'ALT',
    'Alcatel Cable Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'ACP',
    'Aloca Conductor Products'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'ANA',
    'Anaconda Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'BIC',
    'BICC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'CAB',
    'CableC Coporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'CAM',
    'Camden Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'CWC',
    'Canada Wire and Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'CAR',
    'Caraluma Wire and Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'COL',
    'Collyer Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'CPI',
    'Conductor Products Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'CMY',
    'Conductors Monterrey'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'ESS',
    'Essex Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'FPS',
    'Forte Power Systems - Southwire'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'GCC',
    'General Cable Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'HWC',
    'Hendrix Wire and Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'HIT',
    'Hitachi Cable LTD (KGK Locke)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'KAC',
    'Kaiser Aluminum Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'KER',
    'Kerite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'NOR',
    'Noranda Aluminum Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'OKO',
    'Okonite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'PAR',
    'Paranite Cable Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'PHD',
    'Phelps Dodge'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'PHI',
    'Phillips Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'PCC',
    'Pirelli Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'RMC',
    'Reynolds Metals Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'ROM',
    'Rome Cable Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'SEW',
    'Service Wire Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'SIM',
    'Simplex Wire Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'SIN',
    'Sinplex Cable Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'SOM',
    'Somerset Cable Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'SWC',
    'Southwire Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Manufacturer',
    'YON',
    'Yonkers Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Temperature Rise',
    '55',
    '55 C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Temperature Rise',
    '65',
    '65 C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Temperature Rise',
    '80',
    '80 C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Temperature Rise',
    '115',
    '115 C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Temperature Rise',
    '150',
    '150 C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Temperature Rise',
    '60',
    '60 C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Current Transformer Type',
    'DONUT',
    'Donut Type CT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Current Transformer Type',
    'BAR',
    'Bar Type CT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Current Transformer Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Object Class Model Name Domain',
    'Domain Independent Object Class Model 
Name',
    'Domain Independent Object Class Model Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Object Class Model Name Domain',
    'Electric Object Class Model Name',
    'Electric 
Object Class Model Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Object Class Model Name Domain',
    'Designer Object Class Model Name',
    'Designer 
Object Class Model Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Object Class Model Name Domain',
    'Identify Tool Object Class Model 
Name',
    'Identify Tool Object Class Model Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Object Class Model Name Domain',
    'PGE ED Object Class Model Name',
    'PGE ED 
Object Class Model Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Not Applicable - Single',
    '0',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '4',
    'A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '2',
    'B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '1',
    'C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '6',
    'AB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '5',
    'AC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '3',
    'BC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '7',
    'ABC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Phase Designation',
    '8',
    'N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB LTC Interrupt Type',
    'VAC',
    'Vacuum Reactance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB LTC Interrupt Type',
    'ARC',
    'Arcing Reactance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB LTC Interrupt Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Yes/No Indicator','Y','Yes');
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Yes/No Indicator',
    'N',
    'No'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Yes/No Indicator',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Device Control Type',
    'HYD',
    'Hydraulic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Device Control Type',
    'ELE',
    'Electronic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '7',
    'Current Limiting Fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '8',
    'Current Limiting Fuse w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '11',
    'Energy Limiting Fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '12',
    'Energy Limiting Fuse w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '9',
    'Fault Tamer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '10',
    'Fault Tamer w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '2',
    'Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '5',
    'Liquid Fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '6',
    'Liquid Fuse w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '3',
    'Self Protected'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '4',
    'Self Protector w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB High Side Protection',
    '1',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cell Type',
    'Maint',
    'Maintenance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cell Type',
    'Norm',
    'Normal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    '1',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    '2',
    'Idle/Underloaded - Priority Change Out - 
Idle or Vacant Building'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    '3',
    'Underloaded - OK for Change Out'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    '4',
    'Underloaded - Low Priority - Customer 
Shutdown Restrictions'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    '5',
    'Underloaded - Multiple Customers'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    '9999',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'A',
    'Idle/Underloaded - Verified - Future Use'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'B',
    'Idle/Underloaded - Designated for Future 
Use Per Marketing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'C',
    'Idle/Overloaded/Underloaded - Verified Work 
Order Prepared'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'D',
    'Idle - Premapped Entry - Transformer Not 
Installed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'E',
    'Overloaded - Wrong LF - Metered Demand 
Within Limits'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'F',
    'Underloaded - Wrong LF - Metered Demand 
Within Limits'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'G',
    'Underloaded - Sized to Reduce Flicker or to 
Maintain Voltage'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'H',
    'Underloaded - Verified - Available for 
Replacement'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'I',
    'Underloaded - Minimum Size Transformer 
Being Used'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'J',
    'Idle - Seasonally Idle'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'K',
    'Idle/Underloaded - Verified - Not 
Economical to Replace'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'M',
    'Underloaded - Expected Customer Load Growth 
- Delay 1 Year'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'N',
    'Underloaded - Not Economical to Replace - 
Style Obstructions'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'O',
    'Underloaded - Oil Leak'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'P',
    'Underloaded - PCB Content'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'Q',
    'Underloaded - No Change Out - Non-Linear 
Load Problem'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Field Investigation Code',
    'Z',
    'Transformer Changed Out'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Construction Type',
    '1PH',
    '1 Phase Pot Head'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Construction Type',
    '1PR',
    '1 Phase Riser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Construction Type',
    '2PH',
    '2 Phase Pot Head'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Construction Type',
    '2PR',
    '2 Phase Riser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Construction Type',
    '3PH',
    '3 Phase Pot Head'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Construction Type',
    '3PR',
    '3 Phase Riser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '250',
    '250 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '2300',
    '2,300 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '6000',
    '6,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '8000',
    '8,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '8450',
    '8,450 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '10000',
    '10,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '10500',
    '10,500 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '12500',
    '12,500 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '13100',
    '13,100 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '14000',
    '14,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '15000',
    '15,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '18000',
    '18,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '20000',
    '20,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '22000',
    '22,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '23000',
    '23,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '25000',
    '25,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '31000',
    '31,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '31500',
    '31,500 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '32000',
    '32,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '34000',
    '34,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '36000',
    '36,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '37000',
    '37,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '40000',
    '40,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '41000',
    '41,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '42000',
    '42,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '53000',
    '53,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '64000',
    '64,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '65000',
    '65,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '12000',
    '12,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '16000',
    '16,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '48000',
    '48,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '0',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '1200',
    '1,200 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '600',
    '600 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '400',
    '400 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '4000',
    '4000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '2000',
    '2000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '30000',
    '30,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '28300',
    '28,300 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '100000',
    '100,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '125000',
    '125,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '21000',
    '21,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '29000',
    '29,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '26000',
    '26,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '50000',
    '50,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '200',
    '200 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '61000',
    '61,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '38000',
    '38,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '7000',
    '7,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '1600',
    '1,600 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '15500',
    '15,500 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '63000',
    '63,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Current Rating',
    '28000',
    '28,000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '132',
    'PIT NO 5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '138',
    'TIGER CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '151',
    'WISHON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '201',
    'SF X'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '202',
    'NAPA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '203',
    'WOODLAND'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '204',
    'OAKLAND D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '205',
    'WILLIAMS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '206',
    'ROCKLIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '207',
    'SF E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '208',
    'TAFT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '209',
    'SF G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '210',
    'SF H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '211',
    'OAKLAND L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '212',
    'PETALUMA A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '213',
    'SUISUN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '214',
    'LIVERMORE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '215',
    'SANTA ROSA A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '216',
    'PITTSBURG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '217',
    'WILLOW CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '218',
    'FORESTHILL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '219',
    'SAN ARDO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '220',
    'WALNUT CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '221',
    'NOVATO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '222',
    'SF J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '223',
    'TRINIDAD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '224',
    'LOS ALTOS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '225',
    'SPAULDING'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '226',
    'SF L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '227',
    'WISE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '228',
    'SF N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '229',
    'TAMARACK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '230',
    'TULUCAY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '231',
    'MENDOTA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '232',
    'FORT SEWARD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '233',
    'SF P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '234',
    'SF Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '235',
    'SANGER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '236',
    'SAN JOAQUIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '237',
    'STOCKTON A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '238',
    'EEL RIVER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '239',
    'OILFIELDS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '240',
    'JUDAH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '241',
    'LOW GAP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '242',
    'STAGG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '243',
    'SHADY GLEN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '244',
    'PLEASANT GROVE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '245',
    'VALLEJO B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '246',
    'PLACER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '247',
    'RUSS RANCH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '248',
    'IGNACIO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '249',
    'YOSEMITE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '250',
    'TARAVAL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '251',
    'NORIEGA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '252',
    'RICHMOND Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '253',
    'POWER HOUSE NO 3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '254',
    'VOLTA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '255',
    'VALLEJO C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '256',
    'PENRYN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '257',
    'MOLINO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '258',
    'OCEAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '259',
    'SUMMIT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '260',
    'WESTLAKE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '261',
    'PORTOLA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '262',
    'CAWELO B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '263',
    'SAN LUIS OBISPO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '264',
    'DALY CITY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '265',
    'HIGHWAY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '266',
    'WILLITS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '267',
    'WESTLEY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '268',
    'ZACA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '269',
    'MILLBRAE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '270',
    'VIERRA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '271',
    'KERMAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '272',
    'SONOMA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '273',
    'RESERVATION ROAD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '274',
    'WILLOWS A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '275',
    'FITCH MOUNTAIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '276',
    'MADERA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '277',
    'WILKINS SLOUGH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '278',
    'MCKITTRICK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '280',
    'SF Y'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '281',
    'WHEATLAND'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '282',
    'STANISLAUS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '283',
    'SPRING GAP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '284',
    'RIDGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '285',
    'VIEJO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '286',
    'SERRAMONTE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '287',
    'UPPER LAKE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '288',
    'TRACY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '289',
    'SCHINDLER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '290',
    'SEMITROPIC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '291',
    'WYANDOTTE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '292',
    'STONE CORRAL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '293',
    'TEJON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '294',
    'TIVY VALLEY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '295',
    'TULARE LAKE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '296',
    'WASCO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '297',
    'WEEDPATCH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '298',
    'WARD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '299',
    'LOCKHEED NO.1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '300',
    'LOCKHEED NO.2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '301',
    'MORRO BAY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '302',
    'WOODACRE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '303',
    'PALMER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '304',
    'GARCIA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '305',
    'TEMPLETON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '306',
    'COVELO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '307',
    'PERRY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '308',
    'ROUGH AND READY ISLA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '309',
    'GREENBRAE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '310',
    'SPANISH CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '311',
    'SAN LEANDRO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '312',
    'COUNTRY CLUB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '313',
    'WEST SACRAMENTO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '314',
    'SOLANO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '315',
    'WATERLOO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '316',
    'SALMON CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '317',
    'MADISON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '318',
    'PALO SECO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '319',
    'ZAMORA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '320',
    'WEST POINT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '321',
    'MORMON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '322',
    'STOCKTON ACRES'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '323',
    'NORTH BRANCH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '324',
    'WHITNEY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '325',
    'TRES VIAS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '326',
    'SOTO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '327',
    'OAK PARK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '328',
    'VICTOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '329',
    'PUEBLO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '330',
    'SOQUEL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '331',
    'RUSSELL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '332',
    'WINTERS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '333',
    'WATSONVILLE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '334',
    'SPRUCE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '335',
    'WALDO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '336',
    'HIGHLANDS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '337',
    'SARATOGA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '338',
    'WOOD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  ( 'SubstationID', '339', 'SWIFT');
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '340',
    'HAMMONDS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '341',
    'LAKEVIEW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '342',
    'PANAMA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '343',
    'SILVERADO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '344',
    'PLAINFIELD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '345',
    'OPAL CLIFFS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '346',
    'SAN LORENZO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '347',
    'RICHMOND R'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '348',
    'WHEELER RIDGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '349',
    'MC ARTHUR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '350',
    'SEACLIFF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '351',
    'ERTA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '352',
    'OREGON TRAIL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '353',
    'RAWSON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '354',
    'SANTA RITA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '355',
    'SMYRNA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '356',
    'STILLWATER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '357',
    'TYLER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '358',
    'VINA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '359',
    'VACA DIXON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '360',
    'WHITMORE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '361',
    'WILDWOOD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '362',
    'WEST LANE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '363',
    'WHISMAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '364',
    'POSO MOUNTAIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '365',
    'SHINGLE SPRINGS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '366',
    'STROUD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '367',
    'WOLFE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '368',
    'PUTAH CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '369',
    'SALT SPRINGS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '370',
    'WESTPARK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '371',
    'TUDOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '372',
    'PIT NO 1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '373',
    'WEST FRESNO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '374',
    'WALL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '375',
    'VASCO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '376',
    'RACETRACK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '377',
    'VASONA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '378',
    'VIRGINIA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '379',
    'SMARTVILLE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '380',
    'TRIMBLE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '381',
    'WAYNE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '384',
    'STUART'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '385',
    'BABEL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '387',
    'FMC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '388',
    'IONE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '389',
    'MONTAGUE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '390',
    'AMES'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '391',
    'WILLOW PASS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '392',
    'MANCHESTER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '393',
    'GATES'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '395',
    'COLUMBUS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '396',
    'BULLARD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '398',
    'EL PECO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '400',
    'ALPAUGH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '401',
    'BAY MEADOWS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '402',
    'BELL HAVEN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '403',
    'BELMONT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '404',
    'DOS PALOS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '405',
    'SANTA NELLA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '406',
    'DUNLAP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '407',
    'STOCKDALE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '408',
    'EMERALD LAKE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '409',
    'GLENWOOD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '410',
    'HALF MOON BAY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '411',
    'HILLSDALE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '412',
    'MC CALL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '413',
    'MENLO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '414',
    'RALSTON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '415',
    'SINCLAIR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '416',
    'ROSSMOOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '418',
    'SAN CARLOS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '419',
    'SAN MATEO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '420',
    'GOOSE LAKE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '421',
    'MARICOPA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '423',
    'SAN RAMON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '424',
    'WATERSHED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '425',
    'WOODSIDE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '426',
    'POINT PINOLE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '427',
    'CASSIDY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '428',
    'LOST HILLS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '429',
    'WELLFIELD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '430',
    'WILSON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '431',
    'ORTIGA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '432',
    'BALFOUR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '434',
    'VALLEY VIEW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '435',
    'FREMONT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '437',
    'SAN PABLO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '438',
    'GRANT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '440',
    'TWISSELMAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '441',
    'MC MULLIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '442',
    'OAKHURST'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '443',
    'COARSEGOLD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '444',
    'RAINBOW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '445',
    'MARIPOSA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '446',
    'NEWHALL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '447',
    'DUMBARTON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '448',
    'BERRENDA C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '450',
    'VINEYARD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '451',
    'TECUYA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '453',
    'WAHTOKE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '454',
    'GANSO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '455',
    'FIGARDEN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '456',
    'TUPMAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '457',
    'RENFRO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '458',
    '7TH STANDARD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '459',
    'BRENTWOOD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '460',
    'SAND CREEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '461',
    'STOREY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '464',
    'WRIGHT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '465',
    'TIDEWATER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '466',
    'TASSAJARA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '467',
    'SOBRANTE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '468',
    'BLACKWELL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '469',
    'RESEARCH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '470',
    'CRESSEY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '472',
    'DIXON LANDING'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '476',
    'ROSEDALE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '477',
    'JACOBS CORNER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '480',
    'RANCHERS COTTON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '482',
    'CADET'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '490',
    'GALLO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '491',
    'WOODCHUCK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '492',
    'LAS PALMAS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '493',
    'GARDNER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '494',
    'BOSWELL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '500',
    'AVENAL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '512',
    'BORDEN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '525',
    'RIVER ROCK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '526',
    'KERN POWER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '527',
    'CELERON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '528',
    'CARNATION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '529',
    'WOODWARD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '531',
    'PENTLAND'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '532',
    'TEVIS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '533',
    'SHARON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '536',
    'LE GRAND'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '537',
    'ORO LOMA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '539',
    'BONITA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '542',
    'TEXACO EMIDIO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '544',
    'PACIFIC PIPE GRAPEVINE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '545',
    'CAL WATER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '800',
    'DOWNIEVILLE DIESEL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '801',
    'SIERRA CITY GENERATOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '802',
    'FRASER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '803',
    'ECHO SUMMIT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '804',
    'WASHINGTON CITY GEN.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '805',
    'ASBESTOS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '809',
    'MCAVOY TAP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '810',
    'SHORE ACRES BANK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '811',
    'SCE MCFARLAND'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '812',
    'PIPER BANK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '813',
    'SCE TEHACHAPI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '815',
    'BETHEL BANK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '818',
    'BOLTHOUSE FARMS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '819',
    'SCE TEJON TIE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '823',
    'ORWOOD TRACT TAP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '832',
    'JERSEY ISLAND'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '853',
    'CRANE VALLEY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '859',
    'TULE POWER HOUSE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '866',
    'BATAVIA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '867',
    'MAINE PRAIRIE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '882',
    'COAST RD.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '885',
    'SO. CAL. EDISON #2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '886',
    'SO. CAL. EDISON #3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '887',
    'QUARRY RD.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '888',
    'TOKAY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SubstationID',
    '900',
    'DIABLO CANYON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'LocalOfficeNumber',
    '0',
    '0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Shape',
    'ANGBAR',
    'Angle Bar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Shape',
    'TUBULAR',
    'Tubular'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Source Type',
    'LINE',
    'Line Source'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Source Type',
    'CIRCUIT',
    'Circuit Source'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '5',
    '5 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '15',
    '15 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '20',
    '20 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '25',
    '25 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '30',
    '30 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '40',
    '40 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '50',
    '50 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '60',
    '60 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '65',
    '65 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '70',
    '70 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '75',
    '75 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '80',
    '80 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '100',
    '100 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '125',
    '125 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '135',
    '135 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '140',
    '140 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '150',
    '150 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '175',
    '175 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '185',
    '185 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '200',
    '200 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '225',
    '225 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '250',
    '250 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '280',
    '280 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '300',
    '300 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '400',
    '400 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '560',
    '560 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '600',
    '600 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '630',
    '630 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '800',
    '800 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '900',
    '900 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '1200',
    '1200 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '1550',
    '1550 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '1600',
    '1600 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '1800',
    '1800 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '2000',
    '2000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '3000',
    '3000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '3150',
    '3150 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '4000',
    '4000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '5000',
    '5000 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '10',
    '10 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '33',
    '33 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '35',
    '35 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '170',
    '170 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '220',
    '220 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '500',
    '500 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '720',
    '720 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Continuous Current Rating',
    '1400',
    '1400 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Unit Spare',
    'NO',
    'No'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Unit Spare',
    'SHARED',
    'Shared'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Unit Spare',
    'DEDICATED',
    'Dedicated'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AB',
    'AB Chance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AT',
    'Acme Tool'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AN',
    'Alcan'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AA',
    'Alcoa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AC',
    'Allis-Chalmers'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AE',
    'American Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AD',
    'Anaconda'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'AS',
    'Asea Brown (Abb) Bravari'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'CA',
    'Carte'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'CM',
    'Cent-Maloney'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'CE',
    'Central'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'CO',
    'Colman'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'CI',
    'Colt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'CP',
    'Cooper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'DS',
    'Delta-Star/Hill/Hk.Porter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'DT',
    'Distribution Trf'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'DE',
    'Dowser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'EE',
    'Eastern Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'EA',
    'Elastic Stop Nut (Esna)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'EM',
    'Elastimold'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'EO',
    'Electric Service (Esco)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'EL',
    'Electrical Equipment Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'EN',
    'Energyline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'ER',
    'Ermco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'FE',
    'Ferranti Packard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'FS',
    'Fisher Pierce'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'GW',
    'G '
    || chr(38)
    ||' W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'GP',
    'Gardiner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'FP',
    'Gardiner/Pe/Fp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'GC',
    'General Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'GE',
    'General Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'HO',
    'Holophane'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'HI',
    'Howard Industries'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'IT',
    'I T E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'IA',
    'Inertia'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'IS',
    'Innovative Switchgear Solutions Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'JO',
    'Josyln'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'KP',
    'K P F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'KA',
    'Kaiser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'KC',
    'Kearny Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'KE',
    'Kelman'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'KU',
    'Kuhlman'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'LM',
    'Kyle-Line Mat'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'LC',
    'Lincontrol'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'MS',
    'Maysteel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'ME',
    'Mcgraw Edison'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'MO',
    'Moloney'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'NE',
    'Nelson'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'NS',
    'Non-Standard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'OE',
    'Obsolete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'OB',
    'Ohio Brass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'OK',
    'Okonite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'PM',
    'Pac El Motor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'PT',
    'Pennsylvania'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'PD',
    'Phelps-Dodge'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'PP',
    'Power Partners'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'RT',
    'R T E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'RM',
    'Reynolds'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'RO',
    'Rome'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SC',
    'S '
    || chr(38)
    ||' C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SA',
    'Sangamo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SO',
    'Scott'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SB',
    'Shalbetter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SI',
    'Siemens'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SP',
    'Specialty Transformer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SD',
    'Square D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'ST',
    'Standard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'TC',
    'TC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'TR',
    'Trayer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'TU',
    'Turner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'XX',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'WA',
    'Wagner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'WU',
    'Western Utilities'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'WE',
    'Westinghouse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'WT',
    'Westronics'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'ZI',
    'Zinsco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'HM',
    'Horstmann'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SL',
    'Schweitzer Engineering Labs'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SE',
    'Sentient'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'SS',
    'Silver Spring Networks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Manufacturer',
    'TG',
    'Tollgrade'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB LTC Onload Offload',
    'ON',
    'On-Load'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB LTC Onload Offload',
    'OFF',
    'Off-Load'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Normal Status',
    '1',
    'Closed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Normal Status',
    '0',
    'Open'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Normal Status',
    '2',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Control Type',
    'TIME',
    'Time'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Control Type',
    'TEMP',
    'Temprature'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Control Type',
    'VOLTS',
    'Voltage'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Control Type',
    'AMPS',
    'Current'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Control Type',
    'KVAR',
    'KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Riser Usage - Primary',
    '1',
    'Primary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'R',
    'R'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'S',
    'S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'T',
    'T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'X',
    'X'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'R1',
    'R1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'S1',
    'S1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'T1',
    'T1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Climate Zone',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Time Multiplier',
    'esriFieldTypeInteger',
    'TimeMultiplier'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '10',
    'Air switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '38',
    'Automatic Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '99',
    'Co-Generation Equipment'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '14',
    'Disconnect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '44',
    'ES55 Disconnect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '8',
    'Load Check Point'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '40',
    'Miscellaneous Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '4',
    'Other Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '17',
    'PDAC Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '7',
    'Remote Telephone'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '18',
    'SCADA Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '35',
    'Switch - 600 Amp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '3',
    'Underarm Sidebreak (US)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '1',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '2',
    'Vacuum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '5',
    'VCR Oiled'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Overhead Switch Type',
    '6',
    'TSC Oiled'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Reactor Usage Type',
    'PHS',
    'Phase'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Reactor Usage Type',
    'NEU',
    'Neutral'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Reactor Usage Type',
    'CURLIM',
    'Current Limiting'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Reactor Usage Type',
    'CAPSWI',
    'Capacitor Switching'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Arrestor Type',
    'MOV',
    'MOV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Arrestor Type',
    'MO',
    'MO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Arrestor Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Curve Type',
    'esriFieldTypeInteger',
    'CurveType'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Energized',
    'ENER',
    'Energized'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Energized',
    'DEEN',
    'De-Energized'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Current Transformer Usage',
    'COUPCAP',
    'Coupling Capacitor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Current Transformer Usage',
    'LINE',
    'Line'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Current Transformer Usage',
    'BUS',
    'Bus'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Type',
    'FIXD',
    'Fixed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Type',
    'SWTD',
    'Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Type',
    'SWVS',
    'Switched w/var Sensor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '4',
    '4%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '6',
    '6%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '10',
    '10%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '7.5',
    '7.5%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '5',
    '5%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '8',
    '8%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '2',
    '2%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '9.85',
    '9.85%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Boost/Buck Percent',
    '12.5',
    '12.5%'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '120',
    '120 V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '240',
    '240 V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '277480',
    '277/480'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '277',
    '277 V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '240480',
    '240/480'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '208',
    '208 V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '120240',
    '120/240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '120208',
    '120/208'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Secondary Nominal Voltage',
    '480',
    '480 V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Device Usage Type',
    'CAPCON',
    'Capacitor Control'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Device Usage Type',
    'STABRK',
    'Station Breaker'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Device Usage Type',
    '3',
    'Station Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Encasement Type',
    'BURIED',
    'Direct Buried'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Encasement Type',
    'CONDUIT',
    'In Conduit'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Encasement Type',
    'DUCTBANK',
    'Ductbank'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Encasement Type',
    'RACEWAY',
    'Raceway'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'ActualDuctSize',
    'ActualDuctSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'AT_ConductorMaterial',
    'AT_ConductorMaterial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'AT_ConductorSize',
    'AT_ConductorSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'AT_NeutralMaterial',
    'AT_NeutralMaterial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'AT_NeutralSize',
    'AT_NeutralSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'AT_PhaseDesignation',
    'AT_PhaseDesignation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'AT_Shape',
    'AT_Shape'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'Class',
    'Class'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'ConductorMaterial',
    'ConductorMaterial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'ConductorSize',
    'ConductorSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'CYMDISTEXPORT',
    'CYMDISTEXPORT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'Diameter',
    'Diameter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctAvailability',
    'DuctAvailability'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctBankConfiguration',
    'DuctBankConfiguration'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctBankHeight',
    'DuctBankHeight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctBankWidth',
    'DuctBankWidth'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctID',
    'DuctID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctMaterial',
    'DuctMaterial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctSize',
    'DuctSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FacilityID',
    'FacilityID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FdrMgrNontraceable',
    'FdrMgrNontraceable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FeederID',
    'FeederID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FeederID2',
    'FeederID2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FeederInfo',
    'FeederInfo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FeederName',
    'FeederName'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FeederSourceInfo',
    'FeederSourceInfo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'GroundReactance',
    'GroundReactance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'GroundResistance',
    'GroundResistance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'GuyCount',
    'GuyCount'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'Height',
    'Height'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'HighSideConfiguration',
    'HighSideConfiguration'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'HighSideGroundReactance',
    'HighSideGroundReactance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'HighSideGroundResistance',
    'HighSideGroundResistance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'InsulationType',
    'InsulationType'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'JointAttachmentOwner',
    'JointAttachmentOwner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'LassoText',
    'LassoText'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'LassoTextField',
    'LassoTextField'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'LeadLength',
    'LeadLength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'LowSideConfiguration',
    'LowSideConfiguration'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'LowSideGroundReactance',
    'LowSideGroundReactance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'LowSideGroundResistance',
    'LowSideGroundResistance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'LowSideVoltage',
    'LowSideVoltage'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'Material',
    'Material'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMElectricTraceWeight',
    'MMElectricTraceWeight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktAmpacity',
    'MMSecCktAmpacity'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktCode',
    'MMSecCktCode'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktCost',
    'MMSecCktCost'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktCUCode',
    'MMSecCktCUCode'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktDescription',
    'MMSecCktDescription'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktInsulation',
    'MMSecCktInsulation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktMaterial',
    'MMSecCktMaterial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktR_Per1000UnitsLength',
    'MMSecCktR_Per1000UnitsLength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktR0_Per1000UnitsLength',
    'MMSecCktR0_Per1000UnitsLength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktSize',
    'MMSecCktSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerCode',
    'MMSecCktTransformerCode'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerDescription',
    'MMSecCktTransformerDescription'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktTransformerKVA',
    'MMSecCktTransformerKVA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerMaxXToR',
    'MMSecCktTransformerMaxXToR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerMaxZ',
    'MMSecCktTransformerMaxZ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerMinXToR',
    'MMSecCktTransformerMinXToR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerMinZ',
    'MMSecCktTransformerMinZ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktTransformerPhases',
    'MMSecCktTransformerPhases'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'MMSecCktUnderground',
    'MMSecCktUnderground'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktX_Per1000UnitsLength',
    'MMSecCktX_Per1000UnitsLength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSecCktX0_Per1000UnitsLength',
    'MMSecCktX0_Per1000UnitsLength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'NeutralMaterial',
    'NeutralMaterial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'NeutralSize',
    'NeutralSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'NormalPosition_A',
    'NormalPosition_A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'NormalPosition_B',
    'NormalPosition_B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'NormalPosition_C',
    'NormalPosition_C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'NumberConductorsPerPhase',
    'NumberConductorsPerPhase'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'OperatingVoltage',
    'OperatingVoltage'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'PhaseDesignation',
    'PhaseDesignation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'RatedKVA',
    'RatedKVA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'ServiceCurrentRating',
    'ServiceCurrentRating'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'SubstationID',
    'SubstationID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'SubtypeCD',
    'SubtypeCD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'ULSTextField',
    'ULSTextField'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'UsageType',
    'UsageType'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'WireSize',
    'WireSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'WireStrength',
    'WireStrength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'NominalVoltage',
    'NominalVoltage'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DeviceType',
    'Device Type'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'ManufacturerModel',
    'Manufacturer Model'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'Rating',
    'Rating'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'SerialNumber',
    'Serial Number'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSECCKTTRANSFORMERFULLLOADLOSSMAX',
    'MMSECCKTTRANSFORMERFULLLOADLOSSMAX'
  ) ;
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSECCKTTRANSFORMERNOLOADLOSSMIN',
    'MMSECCKTTRANSFORMERNOLOADLOSSMIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSECCKTTRANSFORMERNOLOADLOSSMAX',
    'MMSECCKTTRANSFORMERNOLOADLOSSMAX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'MMSECCKTTRANSFORMERFULLLOADLOSSMIN',
    'MMSECCKTTRANSFORMERFULLLOADLOSSMIN'
  ) ;
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'SYNERGEE_PHASECONDUCTOR1',
    'SYNERGEE_PHASECONDUCTOR1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'SYNERGEE_PHASECONDUCTOR2',
    'SYNERGEE_PHASECONDUCTOR2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'SYNERGEE_NEUTRALCONDUCTOR1',
    'SYNERGEE_NEUTRALCONDUCTOR1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model 
Name',
    'SYNERGEE_NEUTRALCONDUCTOR2',
    'SYNERGEE_NEUTRALCONDUCTOR2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DuctName',
    'DuctName'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'HASSUBDUCTS',
    'HasSubducts'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FACILITYDIAGRAM',
    'FACILITYDIAGRAM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DUCTOCCUPIED',
    'DUCTOCCUPIED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'DIRECTION',
    'Conduit Direction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Electric Field Model Name',
    'FEATUREID',
    'Feature ID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '1',
    'Alameda'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '2',
    'Alpine'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '3',
    'Amador'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '4',
    'Butte'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '5',
    'Calaveras'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '6',
    'Colusa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '7',
    'Contra Costa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '8',
    'Del Norte'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '9',
    'El Dorado'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '10',
    'Fresno'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '11',
    'Glenn'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '12',
    'Humbolt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '13',
    'Imperial'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '14',
    'Inyo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '15',
    'Kern'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '16',
    'Kings'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '17',
    'Lake'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '18',
    'Lassen'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '19',
    'Los Angeles'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '20',
    'Madera'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '21',
    'Marin'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '22',
    'Mariposa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '23',
    'Mendocino'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '24',
    'Merced'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '25',
    'Modoc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '26',
    'Mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '27',
    'Monterey'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '28',
    'Napa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '29',
    'Nevada'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '30',
    'Orange'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '31',
    'Placer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '32',
    'Plumas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '33',
    'Riverside'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '34',
    'Sacramento'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '35',
    'San Benito'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '36',
    'San Bernardino'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '37',
    'San Diego'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '38',
    'San Francisco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '39',
    'San Joaquin'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '40',
    'San Luis Obispo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '41',
    'San Mateo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '42',
    'Santa Barbara'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '43',
    'Santa Clara'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '44',
    'Santa Cruz'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '45',
    'Shasta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '46',
    'Sierra'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '47',
    'Siskiyou'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '48',
    'Solano'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '49',
    'Sonoma'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '50',
    'Stanislaus'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '51',
    'Sutter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '52',
    'Tehama'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '53',
    'Trinity'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '54',
    'Tulare'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '55',
    'Tuolumne'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '56',
    'Ventura'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '57',
    'Yolo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB County Name',
    '58',
    'Yuba'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Relay Code',
    'GRD',
    'GRD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Relay Code',
    'PHA',
    'PHA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Relay Code',
    'GRDBackup',
    'GRDBackup'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Relay Code',
    'PHABackup',
    'PHABackup'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'BY',
    'Bayonet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'E',
    'E Link'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'EJO',
    'EJO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'ELSG',
    'ELSG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'FX',
    'FX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'K',
    'K Link'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'N',
    'N Link'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'NXD',
    'NXD Provision'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'PM',
    'PM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'PX',
    'PX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'SL',
    'Slow Link'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'SMD',
    'SMD Provision'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'SMU',
    'SMU_20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'T',
    'T Link'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'EJ1',
    'EJ1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'H',
    'H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Type',
    'CL',
    'CL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '50',
    '50 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '51',
    '51 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '52',
    '52 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '53',
    '53 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '54',
    '54 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '55',
    '55 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '56',
    '56 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {400}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '57',
    '57 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '58',
    '58 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {600}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '59',
    '59 - cu 3/c p'
    || chr(38)
    ||'l (5 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '60',
    '60 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '61',
    '61 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '62',
    '62 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '63',
    '63 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '64',
    '64 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '65',
    '65 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '66',
    '66 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {400}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '67',
    '67 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '68',
    '68 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {600}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '69',
    '69 - cu 3/c p'
    || chr(38)
    ||'l (15 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '77',
    '77 - cu 3/c p'
    || chr(38)
    ||'l shielded(15 kV) 
{500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '79',
    '79 - cu 3/c p'
    || chr(38)
    ||'l shielded(15 kV) 
{750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '80',
    '80 - cu 3/c p'
    || chr(38)
    ||'l gas(15 kV) 
{750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '91',
    '91 - cu 3/c ro'
    || chr(38)
    ||'jwa (5 kV) 
{1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '92',
    '92 - cu 3/c ro'
    || chr(38)
    ||'jwa (5 kV) 
{250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '93',
    '93 - cu 3/c ro'
    || chr(38)
    ||'jwa (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '94',
    '94 - cu 3/c ro'
    || chr(38)
    ||'jwa (15 kV) 
{250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '95',
    '95 - 3/c cu ro'
    || chr(38)
    ||'jwa (15 kV) 
{500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '96',
    '96 - 3/c cu ro'
    || chr(38)
    ||'jwa (15 kV) 
{750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '100',
    '100 - cu 1/c p'
    || chr(38)
    ||'l (5 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '101',
    '101 - cu 1/c p'
    || chr(38)
    ||'l (5 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '102',
    '102 - cu 1/c p'
    || chr(38)
    ||'l (5 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '103',
    '103 - cu 1/c p'
    || chr(38)
    ||'l (5 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '104',
    '104 - cu 1/c p'
    || chr(38)
    ||'l (5 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '105',
    '105 - cu 1/c p'
    || chr(38)
    ||'l (5 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '106',
    '106 - cu 1/c p'
    || chr(38)
    ||'l (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '107',
    '107 - cu 1/c p'
    || chr(38)
    ||'l (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '108',
    '108 - cu 1/c p'
    || chr(38)
    ||'l (15 kV) 
{2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '109',
    '109 - 1/c cu p'
    || chr(38)
    ||'l (15 kV) 
{250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '110',
    '110 - cu 1/c p'
    || chr(38)
    ||'l (15 kV) 
{500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '111',
    '111 - cu 1/c p'
    || chr(38)
    ||'l (15 kV) 
{750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '112',
    '112 - cu 1/c ro'
    || chr(38)
    ||'n (5 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '113',
    '113 - cu 1/c ro'
    || chr(38)
    ||'n (5 kV) 
{2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '114',
    '114 - cu 1/c ro'
    || chr(38)
    ||'n (5 kV) 
{250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '115',
    '115 - cu 1/c ro'
    || chr(38)
    ||'n (5 kV) 
{500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '116',
    '116 - cu 1/c ro'
    || chr(38)
    ||'n (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '117',
    '117 - 1/c cu ro'
    || chr(38)
    ||'n (15 kV) 
{2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '118',
    '118 - cu 1/c ro'
    || chr(38)
    ||'n (15 kV) 
{250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '119',
    '119 - cu 1/c ro'
    || chr(38)
    ||'n (15 kV) 
{500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '120',
    '120 - cu 1/c pe-pvc (5 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '121',
    '121 - cu 1/c pe-pvc (5 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '122',
    '122 - cu 1/c pe-pvc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '123',
    '123 - cu 1/c pl'
    || chr(38)
    ||'n (25 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '124',
    '124 - cu 1/c pl'
    || chr(38)
    ||'n (25 kV) 
{250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '125',
    '125 - cu 1/c pl'
    || chr(38)
    ||'n (25 kV) 
{500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '130',
    '130 - cu {6}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '131',
    '131 - cu {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '132',
    '132 - cu {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '133',
    '133 - cu {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '134',
    '134 - cu {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '135',
    '135 - cu {3/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '136',
    '136 - cu {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '137',
    '137 - cu {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '138',
    '138 - cu {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '139',
    '139 - misc. types {6}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '140',
    '140 - cu 1/c pe-pvc (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '141',
    '141 - cu 1/c pec (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '142',
    '142 - cu 1/c pec (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '143',
    '143 - cu 1/c pec (5 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '144',
    '144 - cu 1/c pec (5 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '145',
    '145 - cu 1/c pec (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '150',
    '150 - cu 1/c xlp-pvc (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '151',
    '151 - cu 1/c xlp-pvc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '152',
    '152 - cu 1/c xlp-pvc (15 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '153',
    '153 - cu 1/c xlp-pvc (15 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '154',
    '154 - cu 1/c xlp-pvc (15 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '155',
    '155 - cu 1/c xlp-pvc (15 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '161',
    '161 - cu 1/c xlp-pvc (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '162',
    '162 - cu 1/cxlp-pvc (22 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '163',
    '163 - 1/c cu xlp-pvc (22 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '164',
    '164 - 1/c cu xlp-pvc (22 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '165',
    '165 - cu 1/c xlp-pvc (22 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '166',
    '166 - cu 1/c xlc (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '170',
    '170 - cu 1/c xlc (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '171',
    '171 - cu 1/c xlc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '180',
    '180 - cu 1/c xlp-conc-pvc (15 kV) {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '181',
    '181 - cu 1/c xlp-conc-pvc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '182',
    '182 - cu 1/c xlp-conc-pvc (15 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '183',
    '183 - cu 1/c xlp-conc-pvc (15 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '184',
    '184 - cu 1/c xlp-conc-pvc (15 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '185',
    '185 - cu 1/c xlp-conc-pvc (15 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '186',
    '186 - cu 1/c xlp-conc-pvc (15 kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '187',
    '187 - cu 1/c epr-conc-encap-pe 15 kV {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '188',
    '188-cu 1/c epr-conc-encap-pe 15kV {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '189',
    '189-cu 1/c epr-conc-encap-pe 15 kV {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '190',
    '190-cu 1/c epr-conc-encap-pe 15 kV {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '191',
    '191 - cu 1/c xlp-conc-pvc (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '192',
    '192 - cu 1/c xlp-conc-pvc (22 kV) {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '193',
    '193 - cu 1/c xlp-conc-pvc (22 kV) {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '194',
    '194 - cu 1/cxlp-conc-pvc (22 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '195',
    '195 - cu 1/c xlp-conc-pvc (22 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '196',
    '196 - 1/c cu xlp-conc-pvc (22 kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '199',
    '199 - cu 1/c mpg (35 kV) {900}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '201',
    '201 - al xlp-pvc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '202',
    '202 - al xlp-pvc (15 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '203',
    '203 - al xlp-pvc (15 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '204',
    '204 - al xlp-pvc (15 kV) {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '205',
    '205 - al xlp-pvc (15 kV) {700}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '206',
    '206 - al xlp-pvc (15 kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '207',
    '207 - al xlp-pvc (15 kV) {1250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '211',
    '211 - al xlp-pvc (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '212',
    '212 - al xlp-pvc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '213',
    '213 - al xlp-pvc (22 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '214',
    '214 - al xlp-pvc (22 kV) {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '215',
    '215 - al xlp-pvc (22 kV) {700}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '216',
    '216 - al xlp-pvc (22 kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '217',
    '217 - al xlp-pvc (22 kV) {1250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '218',
    '218 - al xlp-pvc (35kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '219',
    '219 - al xlp-pvc (35 kV) {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '220',
    '220 - al xlp-pvc (35 kV) {700}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '221',
    '221 - al pec (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '222',
    '222 - al pec (15 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '231',
    '231 - al pec (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '232',
    '232 - al pec (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '233',
    '233 - al pec (22 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '234',
    '234 - al pe-conc-hdpe (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '235',
    '235 - al pe-conc-hdpe (15 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '236',
    '236 - al pe-conc-hdpe (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '237',
    '237 - al pe-conc-hdpe (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '241',
    '241 - al xlc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '242',
    '242 - al xlc (15 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '243',
    '243 - al xlp-conc (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '244',
    '244 - al xlp-conc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '251',
    '251 - al 600 V - xlp {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '252',
    '252 - al 600 V - xlp  {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '253',
    '253 - al 600 V - xlp {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '254',
    '254 - al 600 V - xlp {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '255',
    '255 - al 600 V - xlp {700}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '261',
    '261 - al xlp-conc-cic (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '262',
    '262 - al xlp-conc-cic (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '263',
    '263 - al xlp-conc-cic (22 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '269',
    '269 - al xlp-conc-encap pe 15 kV {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '270',
    '270 - al xlp-conc-encap pe 15 kV {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '271',
    '271 - al xlp-conc-pvc (15 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '272',
    '272 - al xlp-conc-pvc (15 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '273',
    '273 - al xlp-conc-pvc (15 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '274',
    '274 - al xlp-conc-pvc (15 kV) {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '275',
    '275 - al xlp-conc-pvc (15 kV) {700}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '276',
    '276 - al xlp-conc-pvc (15 kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '277',
    '277 - al xlp-conc-pvc (15 kV) {1250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '278',
    '278 - al xlp-conc-hdxlp (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '279',
    '279 - al xlp-conc-encap pe 22 kV {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '280',
    '280 - al xlp-conc-encap pe 22 kV {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '281',
    '281 - al xlp-conc-pvc (22 kV) {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '282',
    '282 - al xlp-conc-pvc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '283',
    '283 - al xlp-conc-pvc (22 kV) {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '284',
    '284 - al xlp-conc-pvc (22 kV) {350}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '285',
    '285 - al xlp-conc-pvc (22 kV) {700}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '286',
    '286 - al xlp-conc-pvc (22 kV) {1000}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '287',
    '287 - al xlp-conc-pvc (22 kV) {1250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '288',
    '288 - al epr-conc-encap-pe 25  kV {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '289',
    '289 - al epr-conc-encap-pe 25  kV {600}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '290',
    '290 - al epr-conc-encap-pe 25  kV {1100}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '291',
    '291 - al xlp-pbconc.-pvc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '292',
    '292 - al trxlp-conc.-pvc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '293',
    '293 - al kerite-conc.-pvc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '294',
    '294 - al epr-conc.-pvc (22 kV) {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '298',
    '298 - mpg (35 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '299',
    '299 - mpg (35 kV) {1750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '390',
    '390 - cu 3/c xlp-jwa (15 kV) {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '391',
    '391 - cu 3/c xlp-jwa (15 kV) {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '777',
    'Series Capacitor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '888',
    'Secondary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - UG',
    '9999',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Not Applicable - Double',
    '0',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '28',
    'Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '26',
    'Central'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '16',
    'Chico'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '9',
    'Colusa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '80',
    'De Anza'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '93',
    'Delta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '30',
    'Diablo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '21',
    'El Dorado'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '35',
    'Eureka'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '36',
    'Fortuna'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '70',
    'Fresno'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '34',
    'Garberville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '14',
    'Glenn'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '8',
    'Hollister'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '66',
    'Kern'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '4',
    'King City'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '18',
    'Las Plumas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '11',
    'Marysville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '24',
    'Mission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '7',
    'Monterey'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '95',
    'Mother Lode'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '22',
    'Nevada'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '40',
    'North Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '13',
    'Oroville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '17',
    'Paradise'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '3',
    'Paso Robles'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '58',
    'Peninsula'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '20',
    'Placer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '52',
    'Sacramento'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '5',
    'Salinas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '60',
    'San Francisco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '78',
    'San Jose'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '2',
    'San Luis Obispo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '1',
    'Santa Maria'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '44',
    'Santa Rosa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '87',
    'Shasta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '56',
    'Skyline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '48',
    'Solano'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '91',
    'Stanislaus'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '85',
    'Tehema'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '88',
    'Trinity'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '46',
    'Ukiah'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '42',
    'Vallejo-Napa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '38',
    'Willow Creek'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '50',
    'Yolo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB District Name',
    '72',
    'Yosemite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'XLPE',
    'Cross-linked Polyethelene'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'PILC',
    'Paper Insulated Lead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'Kerite',
    'Kerite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'CIC',
    'Cable in Conduit'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'EPR',
    'Ethylene Propylene Rubber'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'JCK',
    'Jacketed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'LC',
    'Laterally Cor. Copper Shield'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'PL',
    'Paper and Lead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'PWRCBL',
    'Power Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'RL',
    'Rubber and Lead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Cable Construction Type',
    'WI',
    'Water Impervious'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Time Adder',
    'esriFieldTypeInteger',
    'TimeAdder'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'AAC',
    'AAC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'AR',
    'ACSR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'A',
    'AL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'C',
    'CU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'CUWD',
    'Copper Weld'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'HDCU',
    'Hard Drawn Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'SDCU',
    'Soft Drawn Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'STCU',
    'Stranded Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'STL',
    'Steel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Material',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Conductor Material',
    'AL',
    'Aluminium'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Conductor Material',
    'CU',
    'Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Conductor Material',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'MAIN',
    'Main'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'TRANS',
    'Transfer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'SPAR',
    'Spare'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'AUX',
    'Auxillary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'TERT',
    'Tertirary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'GRD',
    'Ground'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'NA',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Usage',
    'NEU',
    'Neutral'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Not Applicable - String',
    'NA',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Material',
    'AL',
    'Aluminium'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Bus Bar Material',
    'CU',
    'Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '50',
    '50 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '100',
    '100 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '200',
    '200 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '300',
    '300 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '600',
    '600 kVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '900',
    '900 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '1200',
    '1200 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Unit KVAR',
    '1800',
    '1800 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH/UG Indicator',
    'OH',
    'OH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH/UG Indicator',
    'UG',
    'UG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '0',
    'Central Coast'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '1',
    'De Anza'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '2',
    'Diablo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '3',
    'East Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '4',
    'Fresno'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '5',
    'Humboldt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '6',
    'Kern'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '7',
    'Los Padres'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '8',
    'Mission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '9',
    'North Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '10',
    'North Valley'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '11',
    'Peninsula'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '12',
    'Sacramento'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '13',
    'San Francisco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '14',
    'Sierra'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '15',
    'San Jose'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '16',
    'Sonoma'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '17',
    'Stockton'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '18',
    'Yosemite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '19',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '20',
    'North Coast'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '21',
    'Eureka'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Division Name',
    '22',
    'Skyline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Cable Size',
    '1000',
    '1000 KCM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Cable Size',
    '750',
    '750 KCM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Cable Size',
    '600',
    '600 KCM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Cable Size',
    '500',
    '500 KCM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Cable Size',
    '350',
    '350 KCM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB UG Cable Size',
    '1/0',
    '1/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    'P',
    'Pilot'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '1',
    '1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '2',
    '2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '3',
    '3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '4',
    '4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '5',
    '5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '6',
    '6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '7',
    '7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '8',
    '8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '9',
    '9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '10',
    '10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '11',
    '11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '12',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '13',
    '13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '14',
    '14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '15',
    '15'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '16',
    '16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '17',
    '17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '18',
    '18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '19',
    '19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '20',
    '20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '21',
    '21'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '22',
    '22'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '23',
    '23'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '24',
    '24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '25',
    '25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '26',
    '26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '27',
    '27'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conversion Work Package',
    '28',
    '28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Easement Type',
    '1',
    'Public Utility Easement'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Easement Type',
    '2',
    'AT'
    || chr(38)
    ||'T Pacific Bell'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Easement Type',
    '3',
    'AT'
    || chr(38)
    ||'T Nevada Bell'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Easement Type',
    '4',
    'AT'
    || chr(38)
    ||'T - AT'
    || chr(38)
    ||'T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Easement Type',
    '5',
    'AT'
    || chr(38)
    ||'T Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Description',
    'Description'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'GDBExtraction',
    'GDBExtraction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'InsetFrameTitle',
    'InsetFrameTitle'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'MapGridName',
    'MapGridName'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'MapSheetAngle',
    'MapSheetAngle'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'MapSheetName',
    'MapSheetName'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'MeasuredLength',
    'MeasuredLength'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model 
Name',
    'MMAbandonIndicator',
    'MMAbandonIndicator'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model 
Name',
    'MMConduitIndicator',
    'MMConduitIndicator'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Path',
    'Path'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'QAExport',
    'QAExport'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'ServerOID',
    'ServerOID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'SymbolRotation',
    'SymbolRotation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Comments',
    'Comments'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Owner',
    'Owner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Date',
    'Date'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Chargeable',
    'Chargeable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'LabelText',
    'LabelText'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'LocatableField',
    'LocatableField'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'InsetFrameSortIndex',
    'Inset Frame Sort 
Index'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'InsetFrameScale',
    'Inset Frame Scale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'ProtectionType',
    'ProtectionType'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'ID',
    'ID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'FacilityID',
    'FacilityID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'ValveSize',
    'ValveSize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model 
Name',
    'GraphicFavoriteName',
    'GraphicFavoriteName'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Field Model Name',
    'Units',
    'Units'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '0',
    'Proposed - Install'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '1',
    'Proposed - Change'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '2',
    'Proposed - Remove'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '3',
    'Proposed - Deactivated'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '5',
    'In Service'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '10',
    'Deactivated'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '20',
    'Removed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '30',
    'Idle'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Construction Status',
    '99',
    'Pseudo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Insulation Type',
    'BARE',
    'Bare'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Insulation Type',
    'XLP',
    'XLP (Cross-Linked Poly)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Insulation Type',
    'EPR',
    'EPR (Ethelene Propylene Rubber)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Insulation Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '0',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '2',
    '2 - cu {6}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '3',
    '3 - cu {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '4',
    '4 - cu {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '5',
    '5 - cu {1}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '6',
    '6 - cu {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '7',
    '7 - cu {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '8',
    '8 - cu {3/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '9',
    '9 - cu {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '10',
    '10 - cu {250}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '11',
    '11 - cu {500}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '12',
    '12 - cu {750}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '13',
    '13 - al {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '14',
    '14 - al {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '15',
    '15 - al {266}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '19',
    '19 - ar {3/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '20',
    '20 - al {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '21',
    '21 - al {3/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '22',
    '22 - al {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '23',
    '23 - al {267}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '24',
    '24 - al {336}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '25',
    '25 - al {397}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '26',
    '26 - al {715}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '27',
    '27 - al {954}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '28',
    '28 - al {1113}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '29',
    '29 - al {2/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '30',
    '30 - ar {4}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '31',
    '31 - ar {2}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '32',
    '32 - ar {1/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '33',
    '33 - ar {4/0}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '34',
    '34 - ar {267}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '35',
    '35 - ar {336}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '36',
    '36 - ar {397}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '37',
    '37 - ar {795}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '38',
    '38 - ar {954}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '39',
    '39 - ar {1113}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '40',
    '40 - cw {8a}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '41',
    '41 - cw {6a}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '42',
    '42 - cw {4a}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '43',
    '43 - cw {2a}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '44',
    '44 - cw {1f}'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '888',
    'Secondary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Conductor Code - OH',
    '9999',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Load Break',
    '0',
    'No'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Load Break',
    '1',
    'Yes'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Load Break',
    '9',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Season',
    'S',
    'Summer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Season',
    'W',
    'Winter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Season',
    'B',
    'Both'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Season',
    'N',
    'Neither'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Job Prefix',
    'WO',
    'Work Order'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Job Prefix',
    'SO',
    'Service Order'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Job Prefix',
    'SD',
    'Source Document'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Job Prefix',
    'GM',
    'General Maintenance'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Use',
    '3',
    'Disconnects'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Use',
    '5',
    'Bypass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Use',
    '7',
    'Bus1Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Use',
    '9',
    'Bus2Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Medium Type',
    'Vacuum',
    'Vacuum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Medium Type',
    'Oil',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Medium Type',
    'Air',
    'Air'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Medium Type',
    'SF6',
    'SF6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Material',
    'Plastic',
    'Plastic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Material',
    'Metal',
    'Metal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Material',
    'Wood',
    'Wood'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Material',
    'Unknown',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '0',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '1',
    'COSE 2 Second Definite Time'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '2',
    'COLE(INV)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '3',
    'CO-9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '4',
    'CO-10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '5',
    'IAC-11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '6',
    'IAC-51'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '7',
    'IAC-53'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '8',
    'IAC-77'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '9',
    'CO-11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '10',
    'CO-8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '11',
    'ITE51Y'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '12',
    'ITE51E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '13',
    'BE-VIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '14',
    'CO-ADI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '15',
    'CO-ADV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '16',
    'BE-EIN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '17',
    'CO-2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '18',
    'CO-5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '19',
    'CO-6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '20',
    'CO-7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '21',
    'BE1-51_B1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '22',
    'BE1-51_B1E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '23',
    'BE1-51_B2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '24',
    'BE1-51_B2E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '25',
    'BE1-51_B3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '26',
    'BE1-51_B3E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '27',
    'BE1-51_B4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '28',
    'BE1-51_B4E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '29',
    'BE1-51_B5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '30',
    'BE1-51_B5E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '31',
    'BE1-51_B6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '32',
    'BE1-51_B6E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '33',
    'BE1-51_B7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '34',
    'BE1-51_B7E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '35',
    'BE1-51A_W1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '36',
    'BE1-51A_W2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '37',
    'BE1-51A_W3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '38',
    'BE1-51A_W4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '41',
    'BE1-51A_W5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '42',
    'BE1-51A_W6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '43',
    'IAC-54'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '44',
    'IAC-55'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '45',
    'IAC-56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '46',
    'IAC-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '47',
    'IAC-66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '48',
    'IAC-75F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '49',
    'IAC-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '50',
    'IAC-81'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '51',
    'IAC-90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '52',
    'IAC-95F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '53',
    'GEC-EI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '54',
    'GEC-VI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '55',
    'CR-2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '56',
    'CR-5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '57',
    'CR-6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '58',
    'CR-7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '59',
    'CR-8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '60',
    'CR-9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '61',
    'CR-10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '62',
    'CR-11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '63',
    'SEL-EI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '64',
    'SEL-I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '65',
    'SEL-MI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '66',
    'SEL-VI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '67',
    'F60-VI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '68',
    'F60-EI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '69',
    'F60-MI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Circuit Relay Type',
    '99',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Usage',
    '1',
    'Primary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Usage',
    '2',
    'Secondary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Usage',
    '3',
    'Service'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Usage',
    '4',
    'Streetlight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Usage',
    '5',
    'Tenant'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Usage',
    '6',
    'DC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '1/0',
    '1/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '4/0',
    '4/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '447',
    '447'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '477',
    '477'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '750',
    '750'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '795',
    '795'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '1590',
    '1590'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Conductor Size',
    '2500',
    '2500'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Switch Type',
    'OIL',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Switch Type',
    'VAC',
    'Vacuum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Switch Type',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Switch Type',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Switch Type',
    'NA',
    'NA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'VERTBRK',
    'Vertical Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DBLBRK',
    'Double Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'TILT',
    'Tilting Insulator'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'SIDEBRK',
    'Side Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'CENTBRK',
    'Center Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'VERTREACH',
    'Vertical Reach'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DE',
    'Double End'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'EB',
    'End Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'GND',
    'Ground'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'REGBP',
    'Regulator Bypass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'KB',
    'Knife Blade'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'LB',
    'Load Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'MDO',
    'Metal Draw Out'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'INT',
    'Interrupter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'QB',
    'Quick Break'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'KPF',
    'KPF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DT',
    'Double Throw'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'SAH',
    'Switch with Arcing Horns'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DBAH',
    'Double Break with Arcing Horns'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'CBAH',
    'Center Break with Arcing Horns'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'KPFAH',
    'KPF with Arcing Horns'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DTAH',
    'Double Throw with Arcing horns'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DBLB',
    'Double Break with Load Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'CBLB',
    'Center Break with Load Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'KPFLB',
    'KPF with Load Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DTLB',
    'Double Throw with Load Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'CBQB',
    'Center Break with Quick Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DBQB',
    'Double Break with Quick Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'KPFQB',
    'KPF with Quick Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'DTQB',
    'Double Throw with Quick Breaks'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Construction Style',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Fluid Type',
    'Silicon',
    'Silicon Based'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Fluid Type',
    'Mineral',
    'Mineral Based'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Fluid Type',
    'Petrol',
    'Petroleum Based'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Fluid Type',
    'Dry',
    'Dry'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Fluid Type',
    'Unk',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Gang Poles',
    '3',
    '3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Switch Gang Poles',
    '6',
    '6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Winding Type',
    'AL',
    'Aluminium'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Winding Type',
    'CU',
    'Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB RegulatorType',
    'Step',
    'Step'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB RegulatorType',
    'Induction',
    'Induction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Splice Manufacturer',
    'OK',
    'Okinite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Splice Manufacturer',
    'RTE',
    'RTE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Splice Manufacturer',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Splice Manufacturer',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Summer Zone',
    'S',
    'Summer Interior'
  );

INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Summer Zone',
    'X',
    'Summer Coastal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay',
    'GDST',
    'Ground Dist. Relay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay',
    'GRND',
    'Ground Relay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay',
    'PDST',
    'Phase Dist. Relay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay',
    'PHSE',
    'Phase Relay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Gen PrimeMover',
    'HYD',
    'Hydro'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Gen PrimeMover',
    'SOL',
    'Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Gen PrimeMover',
    'BIO',
    'Biofuel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Gen PrimeMover',
    'NUC',
    'Nuclear'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Database Model Name Domain',
    'Domain Independent Database Model Name',
    'Domain 
Independent Database Model Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '0',
    'U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '1',
    'I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '2',
    'R'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '-10',
    'P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '16',
    'E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '32',
    'A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM Work Function Abbr',
    '8192',
    'Ex'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Size',
    '3',
    '3"'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Size',
    '4',
    '4"'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Size',
    '6',
    '6"'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Riser Size',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Construction Code',
    '1',
    'Horizontal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Construction Code',
    '2',
    'Vertical'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB OH Line Construction Code',
    '3',
    'Triangular'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '25',
    '25 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '35',
    '35 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '50',
    '50 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '70',
    '70 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '100',
    '100 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '200',
    '200 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '280',
    '280 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '400',
    '400 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '560',
    '560 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Control Coil Size',
    '600',
    '600 Amps'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'FOA',
    'FOA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'OA',
    'OA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ODAF',
    'ODAF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONAN',
    'ONAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'FA',
    'FA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'FFA',
    'FFA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'FAA',
    'FAA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONAF',
    'ONAF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'OFAF',
    'OFAF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONAF1',
    'ONAF1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONAF2',
    'ONAF2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONAR',
    'ONAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'OAAN',
    'OAAN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'SC',
    'SC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONFA',
    'ONFA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'FA/FA',
    'FA/FA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONANF2',
    'ONANF2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transformer Cooling Class',
    'ONANF1',
    'ONANF1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Mechanism Type',
    'Manual',
    'Manual'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Mechanism Type',
    'MotorOperated',
    'Motor Operated'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Interrupting Mechanism Type',
    'Auto',
    'Automatic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '70000',
    '70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '110000',
    '110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '60000',
    '60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '230000',
    '230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '115000',
    '115'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '500000',
    '500'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Transmission Nominal Voltage',
    '18000',
    '18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Railroad Type',
    '1',
    'Standard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Railroad Type',
    '2',
    'Tunnel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'ABB',
    'ABB/Westinghouse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'BB',
    'Brown Boveri'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'CPR',
    'Cooper/McGraw Edison'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'GE',
    'General Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'MA',
    'Multi-Amp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'SHN',
    'Shneider'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Relay Control Manufacturer',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB ANSI Type',
    'A',
    'A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB ANSI Type',
    'B',
    'B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Attachment Type',
    '1',
    'Tenant - PG'
    || chr(38)
    ||'E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Attachment Type',
    '2',
    'Tenant - OU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Attachment Type',
    '3',
    'Joint Owner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Auto Manual',
    'A',
    'Automatic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Auto Manual',
    'M',
    'Manual'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Auto Manual',
    'NA',
    'NA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'LabelText2',
    'LabelText2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CABLETESTDATE',
    'PGE_CABLETESTDATE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CGC12',
    'PGE_CGC12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CIRCUITID',
    'PGE_CIRCUITID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CIRCUITID2',
    'PGE_CIRCUITID2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CITY',
    'PGE_CITY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_COMBONORMALPOSTION',
    'PGE_COMBONORMALPOSTION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORCODE',
    'PGE_CONDUCTORCODE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORCOUNT',
    'PGE_CONDUCTORCOUNT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORINSULATION',
    'PGE_CONDUCTORINSULATION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORMATERIAL',
    'PGE_CONDUCTORMATERIAL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORRATING',
    'PGE_CONDUCTORRATING'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORSIZE',
    'PGE_CONDUCTORSIZE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORTYPE',
    'PGE_CONDUCTORTYPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONDUCTORUSE',
    'PGE_CONDUCTORUSE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONNECTIONCODE',
    'PGE_CONNECTIONCODE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CONSTRUCTIONTYPE',
    'PGE_CONSTRUCTIONTYPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_COUNTY',
    'PGE_COUNTY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DETECTIONSCHEME',
    'PGE_DETECTIONSCHEME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DISTRIBUTIONMAPNO',
    'PGE_DISTRIBUTIONMAPNO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model 
Name',
    'PGE_DISTRIBUTIONMAPNUMBER',
    'PGE_DISTRIBUTIONMAPNUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DISTRICT',
    'PGE_DISTRICT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DIVISION',
    'PGE_DIVISION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DMSFIELD',
    'PGE_DMSFIELD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model 
Name',
    'PGE_FAULTINDICATORYEARINSTALLED',
    'PGE_FAULTINDICATORYEARINSTALLED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_GENERATORNAME',
    'PGE_GENERATORNAME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_GROUNDINGINDICATOR',
    'PGE_GROUNDINGINDICATOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITCITY',
    'PGE_INHERITCITY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITCOUNTY',
    'PGE_INHERITCOUNTY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITDISTRICT',
    'PGE_INHERITDISTRICT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITDIVISION',
    'PGE_INHERITDIVISION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITLOCALOFFICE',
    'PGE_INHERITLOCALOFFICE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITMAPOFFICE',
    'PGE_INHERITMAPOFFICE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITREGION',
    'PGE_INHERITREGION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INHERITZIP',
    'PGE_INHERITZIP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INSTALLATIONDATE',
    'PGE_INSTALLATIONDATE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_INSTALLJOBYEAR',
    'PGE_INSTALLJOBYEAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_JOBNUMBER',
    'PGE_JOBNUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LASTFEDFEEDER',
    'PGE_LASTFEDFEEDER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LATITUDE',
    'PGE_LATITUDE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LEGACYCOORDSYSTEM',
    'PGE_LEGACYCOORDSYSTEM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LOCALOFFICE',
    'PGE_LOCALOFFICE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LOCATOR_DIVISION',
    'PGE_LOCATOR_DIVISION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LOCATOR_OPNUM',
    'PGE_LOCATOR_OPNUM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LONGITUDE',
    'PGE_LONGITUDE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_MAILNAME',
    'PGE_MAILNAME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_MANUFACTURER',
    'PGE_MANUFACTURER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_MAPNUMBER',
    'PGE_MAPNUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_MAPOFFICE',
    'PGE_MAPOFFICE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_MAPSCALE',
    'PGE_MAPSCALE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_MAPTYPE',
    'PGE_MAPTYPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_NUMBEROFPHASES',
    'PGE_NUMBEROFPHASES'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_OPERATINGNUMBER',
    'PGE_OPERATINGNUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_OPERATINGVOLTAGE',
    'PGE_OPERATINGVOLTAGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ORIGINALSSL',
    'PGE_ORIGINALSSL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_OUTPUTVOLTAGE',
    'PGE_OUTPUTVOLTAGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_PROTECTIONTYPE',
    'PGE_PROTECTIONTYPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_RADIOMANUFACTURER',
    'PGE_RADIOMANUFACTURER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_REGION',
    'PGE_REGION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_RELATECOUNT',
    'PGE_RELATECOUNT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_RTUTYPE',
    'PGE_RTUTYPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SCADACOMM',
    'PGE_SCADACOMM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SCADACOMMUNICATION',
    'PGE_SCADACOMMUNICATION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SCADATYPE',
    'PGE_SCADATYPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SECONDARYVOLTAGE',
    'PGE_SECONDARYVOLTAGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SERIALNUMBER',
    'PGE_SERIALNUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SOURCESIDEDEVICEID',
    'PGE_SOURCESIDEDEVICEID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_STATUS',
    'PGE_STATUS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SUBTYPECD',
    'PGE_SUBTYPECD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SUPERVISORYCONTROL',
    'PGE_SUPERVISORYCONTROL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model 
Name',
    'PGE_SYMBOL_NUMBER_FCNAME',
    'PGE_SYMBOL_NUMBER_FCNAME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SYMBOL_NUMBER_XML',
    'PGE_SYMBOL_NUMBER_XML'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SYMBOLNUMBER',
    'PGE_SYMBOLNUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_TOTALKVAR',
    'PGE_TOTALKVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_UPPERCASE',
    'PGE_UPPERCASE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_WINDSPEEDCODE',
    'PGE_WINDSPEEDCODE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_WINDSPEEDRERATEYEAR',
    'PGE_WINDSPEEDRERATEYEAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_YEARMANUFACTURED',
    'PGE_YEARMANUFACTURED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ZGBPCTIMP',
    'PGE_ZGBPCTIMP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ZHLPCTIMP',
    'PGE_ZHLPCTIMP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ZHPCTIMP',
    'PGE_ZHPCTIMP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ZIP',
    'PGE_ZIP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ZLPCTIMP',
    'PGE_ZLPCTIMP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ZTPCTIMP',
    'PGE_ZTPCTIMP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ALLOWEDIT',
    'PGE_ALLOWEDIT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_LEGACYMAPID',
    'PGE_LEGACYMAPID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_TRIGGERMAPCHANGE',
    'PGE_TRIGGERMAPCHANGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_HASDATA',
    'PGE_HASDATA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ANNO_EXP_FIELD',
    'PGE_ANNO_EXP_FIELD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_JOINTOWNERNAME',
    'PGE_JOINTOWNERNAME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CUSTOMEROWNED',
    'PGE_CUSTOMEROWNED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_VERSIONNAME',
    'PGE_VERSIONNAME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ASSETCOPY',
    'PGE_ASSETCOPY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_REPLACEGUID',
    'PGE_REPLACEGUID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_AGREEMENTNUM',
    'PGE_AGREEMENTNUM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DEACTIVATEINDICATOR',
    'PGE_DEACTIVATEINDICATOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_ENABLED',
    'PGE_ENABLED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CIRCUITNAME',
    'PGE Circuit Name'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model 
Name',
    'PGE_XSECTIONANNOCIRCUITID',
    'PGE_XSECTIONANNOCIRCUITID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model 
Name',
    'PGE_XSECTIONANNOCIRCUITID2',
    'PGE_XSECTIONANNOCIRCUITID2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DEADPHASEINDICATOR',
    'PGE_DEADPHASEINDICATOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_AUTOBOOSTER',
    'PGE_AUTOBOOSTER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_STRUCTURESIZE',
    'PGE_STRUCTURESIZE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'LABELTEXT3',
    'LABELTEXT3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'LABELTEXT4',
    'LABELTEXT4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SCALE',
    'PGE_SCALE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_FONTSIZE',
    'PGE_FONTSIZE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_STRUCTURENUMBER',
    'PGE_STRUCTURENUMBER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DIRECTBURYIDC',
    'PGE_DIRECTBURYIDC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_DUCTSYNCATTR',
    'PGE_DUCTSYNCATTR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_CircuitColor',
    'PGE_CircuitColor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_PROTECTIVESSD',
    'PGE_PROTECTIVESSD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_AUTOPROTECTIVESSD',
    'PGE_AUTOPROTECTIVESSD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SUBSTATIONID',
    'PGE_SUBSTATIONID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PGE ED Field Model Name',
    'PGE_SUBSTATIONNAME',
    'PGE_SUBSTATIONNAME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '60',
    '60 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '95',
    '95 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '125',
    '125 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '150',
    '150 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '200',
    '200 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '450',
    '450 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '650',
    '650 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '900',
    '900 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '1130',
    '1130 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    '1800',
    '1800 KV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB BIL Rating',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating As Device',
    'LR',
    'Line Recloser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating As Device',
    'SW',
    'Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating As Device',
    'ST',
    'Sectionalizer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Installation Type OH',
    'OH',
    'Overhead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - IND',
    '208',
    '208'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - IND',
    '240',
    '240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - IND',
    '460',
    '460'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - IND',
    '480',
    '480'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Shunt Reactor Material',
    'CNCT',
    'Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Shunt Reactor Material',
    'FIBR',
    'Fiberglass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Shunt Reactor Material',
    'POR',
    'Porcelain'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Shunt Reactor Material',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'ED',
    'Electric Distribution'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'ET',
    'Electric Transmission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'FO',
    'Fiber Optics'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'TC',
    'TeleCommunication'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'V',
    'Vent'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'GS',
    'Gas Line'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'FOTC',
    'Fiber Optics and TeleCommunications'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fill Type',
    'TI',
    'Temperature Indicator'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'EnabledDomain',
    '0',
    'False'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'EnabledDomain',
    '1',
    'True'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'ArcFMSystemTable',
    'ArcFMSystemTable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'GDBExtraction',
    'GDBExtraction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'InsetFrameSource',
    'InsetFrameSource'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MapGrid',
    'MapGrid'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MMAbandonable',
    'MMAbandonable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MMAbandoned',
    'MMAbandoned'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'MMAbandonRelate',
    'MMAbandonRelate'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MMConduitable',
    'MMConduitable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MMDoNotPost',
    'MMDoNotPost'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MMRemovable',
    'MMRemovable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'MMRemoved',
    'MMRemoved'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'NOQAExport',
    'NOQAExport'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'SplitTarget',
    'SplitTarget'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'ParcelPoly',
    'ParcelPoly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'Inspection',
    'Inspection'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'Jumper',
    'Jumper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'LocatableObject',
    'LocatableObject'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'MP_INVALIDATELAYER',
    'MP_InvalidateLayer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'NonDesignerObject',
    'Non-Designer 
Object'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'OverviewWindow',
    'OverviewWindow'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'PublicAnnotationTarget',
    'PublicAnnotationTarget'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'ArcFMHiddenTable',
    'ArcFM Hidden 
Table'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'STREETCENTERLINE',
    'STREETCENTERLINE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'InspectorSource',
    'InspectorSource'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model 
Name',
    'InspectorDestination',
    'InspectorDestination'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Domain Independent Object Class Model Name',
    'SendToTargets',
    'SendToTargets'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'N',
    'North'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'E',
    'East'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'S',
    'South'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'W',
    'West'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'NE',
    'Northeast'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'NW',
    'Northwest'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'SE',
    'Southeast'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Direction',
    'SW',
    'Southwest'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'AL',
    'Aluminum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'CNCT',
    'Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'CNTR',
    'Center Bore'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'FIBR',
    'Fiberglass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'OTHR',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'STEL',
    'Steel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'THBR',
    'Through Bore'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'TOWR',
    'Tower'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'TREE',
    'Tree'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'WGUY',
    'Wood Guy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'WOOD',
    'Wood'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'WOST',
    'Wood Stub'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'PUSH',
    'Push Pole'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'PoleType',
    'GUYP',
    'Guy Pole'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '1',
    'BPE250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '2',
    '3406'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '3',
    'SR4B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '4',
    '1250 GQNA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '5',
    '432RSL4021'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '6',
    'MT250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '8',
    'HCI734F2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '9',
    'VGF48GSID'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '10',
    'MTG53'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    '7',
    'HCI 434D/444D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - SYN',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '0',
    '0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '2',
    '2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '3',
    '3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '4',
    '4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '5',
    '5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '6',
    '6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '7',
    '7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '9',
    '9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '10',
    '10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '12',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '13',
    '13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '16',
    '16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '18',
    '18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '20',
    '20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '24',
    '24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '25',
    '25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '26',
    '26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '28',
    '28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '29',
    '29'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '30',
    '30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '31',
    '31'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '33',
    '33'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '35',
    '35'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '36',
    '36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '40',
    '40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '43',
    '43'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '44',
    '44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '45',
    '45'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '46',
    '46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '48',
    '48'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '52',
    '52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '56',
    '56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '57',
    '57'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '60',
    '60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '63',
    '63'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '64',
    '64'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '67',
    '67'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '68',
    '68'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '69',
    '69'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '71',
    '71'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '72',
    '72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '73',
    '73'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '86',
    '86'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '88',
    '88'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '106',
    '106'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '107',
    '107'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '108',
    '108'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '120',
    '120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '125',
    '125'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '129',
    '129'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '132',
    '132'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '136',
    '136'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '140',
    '140'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '145',
    '145'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '147',
    '147'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '150',
    '150'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '152',
    '152'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '153',
    '153'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '154',
    '154'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '155',
    '155'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '156',
    '156'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '157',
    '157'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '158',
    '158'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '160',
    '160'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '162',
    '162'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '163',
    '163'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '164',
    '164'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '165',
    '165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '167',
    '167'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '170',
    '170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '172',
    '172'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '173',
    '173'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '175',
    '175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '176',
    '176'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '178',
    '178'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '180',
    '180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '181',
    '181'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '182',
    '182'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '183',
    '183'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '185',
    '185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '187',
    '187'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '189',
    '189'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '190',
    '190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '191',
    '191'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '192',
    '192'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '193',
    '193'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '194',
    '194'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '195',
    '195'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '196',
    '196'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '197',
    '197'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '198',
    '198'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '199',
    '199'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '200',
    '200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '201',
    '201'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '202',
    '202'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '203',
    '203'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '204',
    '204'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '205',
    '205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '206',
    '206'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '207',
    '207'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '208',
    '208'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '209',
    '209'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '210',
    '210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '211',
    '211'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '212',
    '212'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '213',
    '213'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '214',
    '214'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '215',
    '215'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '216',
    '216'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '217',
    '217'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '218',
    '218'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '219',
    '219'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '220',
    '220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '221',
    '221'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '222',
    '222'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '224',
    '224'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '227',
    '227'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '228',
    '228'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '230',
    '230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '231',
    '231'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '233',
    '233'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '234',
    '234'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '235',
    '235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '237',
    '237'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '238',
    '238'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '240',
    '240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '245',
    '245'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '246',
    '246'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '249',
    '249'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '250',
    '250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '254',
    '254'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '255',
    '255'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '260',
    '260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '261',
    '261'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '262',
    '262'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '263',
    '263'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '264',
    '264'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '266',
    '266'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '270',
    '270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '272',
    '272'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '274',
    '274'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '275',
    '275'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '277',
    '277'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '278',
    '278'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '279',
    '279'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '280',
    '280'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '282',
    '282'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '283',
    '283'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '284',
    '284'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '285',
    '285'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '286',
    '286'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '287',
    '287'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '289',
    '289'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '291',
    '291'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '293',
    '293'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '294',
    '294'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '295',
    '295'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '296',
    '296'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '297',
    '297'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '298',
    '298'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '300',
    '300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '301',
    '301'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '303',
    '303'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '306',
    '306'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '307',
    '307'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '308',
    '308'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '309',
    '309'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '311',
    '311'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '313',
    '313'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '314',
    '314'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '316',
    '316'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '317',
    '317'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '318',
    '318'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '319',
    '319'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '320',
    '320'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '321',
    '321'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '322',
    '322'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '325',
    '325'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '327',
    '327'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '328',
    '328'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '330',
    '330'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '332',
    '332'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '333',
    '333'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '335',
    '335'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '339',
    '339'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '340',
    '340'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '341',
    '341'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '342',
    '342'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '343',
    '343'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '344',
    '344'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '349',
    '349'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '350',
    '350'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '352',
    '352'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '354',
    '354'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '355',
    '355'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '357',
    '357'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '358',
    '358'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '360',
    '360'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '361',
    '361'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '362',
    '362'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '363',
    '363'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '364',
    '364'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '365',
    '365'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '366',
    '366'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '367',
    '367'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '368',
    '368'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '369',
    '369'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '372',
    '372'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '374',
    '374'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '375',
    '375'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '376',
    '376'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '377',
    '377'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '378',
    '378'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '379',
    '379'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '382',
    '382'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '384',
    '384'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '385',
    '385'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '387',
    '387'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '388',
    '388'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '389',
    '389'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '390',
    '390'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '391',
    '391'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '392',
    '392'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '393',
    '393'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '394',
    '394'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '395',
    '395'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '396',
    '396'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '397',
    '397'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '398',
    '398'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '399',
    '399'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '400',
    '400'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '401',
    '401'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '403',
    '403'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '404',
    '404'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '405',
    '405'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '406',
    '406'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '407',
    '407'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '408',
    '408'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '409',
    '409'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '410',
    '410'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '411',
    '411'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '412',
    '412'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '413',
    '413'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '414',
    '414'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '415',
    '415'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '417',
    '417'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '418',
    '418'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '419',
    '419'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '420',
    '420'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '421',
    '421'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '422',
    '422'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '424',
    '424'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '425',
    '425'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '427',
    '427'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '428',
    '428'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '430',
    '430'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '431',
    '431'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '434',
    '434'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '436',
    '436'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '437',
    '437'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '438',
    '438'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '440',
    '440'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '442',
    '442'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '445',
    '445'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '448',
    '448'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '457',
    '457'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '461',
    '461'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '462',
    '462'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '467',
    '467'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '468',
    '468'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '476',
    '476'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '477',
    '477'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '478',
    '478'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '480',
    '480'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '481',
    '481'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '484',
    '484'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '488',
    '488'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '489',
    '489'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '491',
    '491'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '492',
    '492'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '493',
    '493'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '496',
    '496'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '498',
    '498'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '500',
    '500'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '505',
    '505'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '509',
    '509'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '510',
    '510'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '516',
    '516'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '518',
    '518'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '522',
    '522'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '526',
    '526'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '530',
    '530'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '531',
    '531'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '534',
    '534'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '537',
    '537'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '543',
    '543'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '545',
    '545'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '576',
    '576'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '583',
    '583'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '593',
    '593'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '600',
    '600'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '615',
    '615'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '704',
    '704'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '1000',
    '1000'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '2208',
    '2208'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '2430',
    '2430'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '2440',
    '2440'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '2864',
    '2864'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'NPVoltage - DC',
    '5565',
    '5565'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '1',
    '70 L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '2',
    'IR70L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '4',
    '37577100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '5',
    '365TTDS1703'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '6',
    'PRIMELINE BB365TTDS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '7',
    'W01044-A - A-002'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '8',
    'CM-60H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '9',
    'CM-75H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    '10',
    'H24GL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - IND',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Detection Scheme',
    'CRNT',
    'Current Scheme'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Detection Scheme',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Detection Scheme',
    'VLTG',
    'Voltage Scheme'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Service Territory Provider',
    '0',
    'PG'
    || chr(38)
    ||'E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Service Territory Provider',
    '1',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Fill Type',
    'OIL',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Capacitor Fill Type',
    'VAC',
    'Vacuum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Dyn Protect Device Type Breaker',
    'CB',
    'Circuit breaker'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH Insulation District',
    'AA',
    'Most Severe'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH Insulation District',
    'A',
    'More Severe'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH Insulation District',
    'B',
    'Severe'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH Insulation District',
    'C',
    'Less Severe'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH Insulation District',
    'D',
    'Least Severe'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Grid Index Type',
    '25',
    '1 = 25 Scale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Grid Index Type',
    '50',
    '1 = 50 Scale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Grid Index Type',
    '100',
    '1= 100 Scale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Grid Index Type',
    '250',
    '1 = 250 Scale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Grid Index Type',
    '500',
    '1 = 500 Scale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH District Type',
    'ED',
    'Electric Distribution'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'OH District Type',
    'ET',
    'Electric Transmission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Arrestor Class',
    's',
    'Station'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Arrestor Class',
    'I',
    'Intermediate'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Arrestor Class',
    'DI',
    'Riser Pole'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Restricted Area Type',
    '1',
    'Military - Active'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Restricted Area Type',
    '2',
    'Military - Inactive'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Restricted Area Type',
    '3',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Line Of Business',
    'ED',
    'Electric Distribution'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Line Of Business',
    'ET',
    'Electric Transmission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Line Of Business',
    'GD',
    'Gas Distribution'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Line Of Business',
    'GT',
    'Gas Transmission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Line Of Business',
    'LB',
    'Land Base'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Active Indicator',
    'A',
    'Active'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Active Indicator',
    'IA',
    'Inactive'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Summer Temp District',
    'C',
    'Coastal District'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Summer Temp District',
    'I',
    'Interior District'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Primary Voltage District',
    'C1',
    'Class 1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Primary Voltage District',
    'C2',
    'Class 2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Primary Voltage District',
    'C3',
    'Class 3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Utility Type',
    'ED',
    'Electric Distribution'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Utility Type',
    'ET',
    'Electric Transmission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Utility Type',
    'GD',
    'Gas Distribution'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Utility Type',
    'GT',
    'Gas Transmission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '1',
    'Cable TV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '2',
    'Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '3',
    'Gas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '4',
    'Sewer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '5',
    'Storm'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '6',
    'Telephone'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '7',
    'Water'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Foreign Utility Type',
    '8',
    'FiberOptic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM EDM Property Type',
    '0',
    'General'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'MM EDM Property Type',
    '1',
    'Site Condition'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating Mechanism',
    'HOOK',
    'Hook Stick'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating Mechanism',
    'MANUAL',
    'Manual'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating Mechanism',
    'MOTOR',
    'Motor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating Mechanism',
    'SPRING',
    'Spring'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Operating Mechanism',
    'HYDRAULIC',
    'Hydraulic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '28',
    'Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '26',
    'Central'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '16',
    'Chico'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '9',
    'Colusa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '80',
    'De Anza'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '93',
    'Delta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '30',
    'Diablo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '21',
    'El Dorado'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '35',
    'Eureka'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '36',
    'Fortuna'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '70',
    'Fresno'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '34',
    'Garberville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '14',
    'Glenn'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '8',
    'Hollister'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '66',
    'Kern'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '4',
    'King City'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '18',
    'Las Plumas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '11',
    'Marysville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '24',
    'Mission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '7',
    'Monterey'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '95',
    'Mother Lode'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '22',
    'Nevada'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '40',
    'North Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '13',
    'Oroville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '17',
    'Paradise'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '3',
    'Paso Robles'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '58',
    'Peninsula'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '20',
    'Placer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '52',
    'Sacramento'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '5',
    'Salinas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '60',
    'San Francisco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '78',
    'San Jose'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '2',
    'San Luis Obispo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '1',
    'Santa Maria'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '44',
    'Santa Rosa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '87',
    'Shasta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '56',
    'Skyline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '48',
    'Solano'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '91',
    'Stanislaus'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '85',
    'Tehema'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '88',
    'Trinity'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '46',
    'Ukiah'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '42',
    'Vallejo-Napa'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '38',
    'Willow Creek'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '50',
    'Yolo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '72',
    'Yosemite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'District Name',
    '76',
    'Coast'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Feature Missing',
    'Feature Missing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Feature Extra',
    'Feature Extra'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Orientation Incorrect',
    'Orientation Incorrect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Attribute Missing',
    'Attribute Missing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Attribute Incorrect',
    'Attribute Incorrect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Annotation Content',
    'Annotation Content'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Annotation Placement',
    'Annotation Placement'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Annotation Overstrike',
    'Annotation Overstrike'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Connectivity Error',
    'Connectivity Error'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Feature Incorrect',
    'Feature Incorrect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Legacy Source Issue',
    'Legacy Source Issue'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Attribute Automated Error',
    'Attribute Automated Error'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Connectivity Automated Error',
    'Connectivity Automated Error'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Feature Incorrect Absolute Placement',
    'Feature Incorrect Absolute 
Placement'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Feature Incorrect Relative Position',
    'Feature Incorrect Relative 
Position'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Feature Aesthetic Placement',
    'Feature Aesthetic Placment'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Data Source Issue',
    'Data Source Issue'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'PAR Interpretation',
    'PAR Interpretation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Related Object Missing',
    'Related Object Missing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Related Object Attribute Missing',
    'Related Object Attribute 
Missing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Related Object Attribute Incorrect',
    'Related Object Attribute 
Incorrect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Relationship incorrect',
    'Relatioinship incorrect'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Error Type',
    'Relationship missing',
    'Relationship missing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'AERO',
    'Aero Wind Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ALPS',
    'Alps Technology Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ANDA',
    'Andalay Solar, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ASEA',
    'ASE America'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ASTP',
    'AstroPower, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ATLE',
    'Atlantis Energy, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'BGWC',
    'Bergey Windpower Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'BOSC',
    'Bosch Solar Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'BPSR',
    'BP Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'BYNE',
    'Baoding Yingli New Energy Res Co'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CANS',
    'Canadian Solar Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CAPS',
    'Capstone'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CEEG',
    'CEEG (Shanghai) Solar Science and Technology 
Co., Ltd.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CHIN',
    'Chint Solar (Zhejiang) Co Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CHNS',
    'Changzhou Nesl Solartech'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CNRE',
    'Open Energy Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CNTA',
    'Centrosolar America'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CONA',
    'Conergy AG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'CTSE',
    'Changzhou Trina Solar Energy Co., Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'D4EN',
    'Day4 Energy Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'DCOR',
    'DyoCore'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'DELS',
    'DelSolar Co., Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'DSLR',
    'Dunasolar Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ECOA',
    'EcoAdvantage Power, Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ECOS',
    'Ecosolargy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ENGP',
    'Energy Photovoltaics, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ENWP',
    'Endurance Wind Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ETSI',
    'ET Solar Industry, Ltd.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'EVRS',
    'Evergreen Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'FARM',
    'Farm Boy Energy Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'FSIC',
    'First Solar, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'FSTS',
    'FirstSolar, LLC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'GENE',
    'GE Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'HYHI',
    'Hyundai Heavy Industries'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ISOF',
    'Isofoton'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'JLSC',
    'Jiangsu Linyang Solarfun Co Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'KNKC',
    'Kaneka Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'KYOA',
    'Kyocera Solar, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'LIGI',
    'Ligitek Photovoltaic Co Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'LUMA',
    'LUMA Resources'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'LUMS',
    'Lumos'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'MAGE',
    'MAGE Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'MARI',
    'Mariah Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'MCSL',
    'MC Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'MIAS',
    'MiaSole'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'MTSH',
    'Mitsubishi Electric Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'MTXS',
    'Matrix Solar/Photowatt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'NEXP',
    'NexPower Technology Corp.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'NGBO',
    'Ningbo Solar Electric Power Co Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'NIGT',
    'Ningbo Ginlong Technologies'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'PLTC',
    'Powerlight Corp.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'PVPL',
    'PV Powered LLC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'RCSM',
    'REC ScanModule'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'RECS',
    'REC Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'RITK',
    'Ritek'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SAFS',
    'Solon Ag Fuer Solartechnik'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SAMS',
    'Samsung Electronics'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SCHS',
    'Scheuten Solar USA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SCHU',
    'Schuco USA LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SCIK',
    'Schuco International KG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SHAP',
    'Shott Applied Power Corp.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SHGP',
    'Shasta Green Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SHLS',
    'Shell Solar Industries'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SHOT',
    'Schott Solar, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SHRP',
    'Sharp Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SIES',
    'Siemens Solar Industries'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SILC',
    'Siliken California Corp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SILR',
    'Silray'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SITC',
    'Solar Integrated Technologies'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SKYL',
    'Skyline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SLIN',
    'Solec International, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SLWC',
    'Solar World California, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SNPS',
    'Sun Perfect Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SNTC',
    'Suntech Power Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SNWZ',
    'Sunwize'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SNYO',
    'Sanyo Electric Co. Ltd.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOFP',
    'Solarfun Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOFR',
    'Solar Frontier'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOLA',
    'Solarex'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOLC',
    'Solaria Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOLO',
    'Solon'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOLS',
    'Solar Semiconductor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOLY',
    'Solyndra Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SOPI',
    'Solartech Power Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SPWR',
    'Solar Power, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SSES',
    'Shanghai Solar Energy S'
    || chr(38)
    ||'T Co 
Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SUNG',
    'SUNGEN International'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SUNP',
    'SunPower Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SWCI',
    'SolarWorld California, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'SWWP',
    'Southwest Windpower'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'TNEC',
    'Tianwei New Energy (Chengdu) PV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'TRNS',
    'Trina Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'UNEN',
    'Unitron Energy Pvt Ltd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'UPSO',
    'Upsolar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'USOV',
    'United Solar Ovonic LLC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'USSC',
    'United Solar Systems Corp.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'VSTS',
    'Vestas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'WE',
    'Westinghouse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'WEMU',
    'Worldwide Energy '
    || chr(38)
    ||' Mfg Usa Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'WNTB',
    'Wind Turbine Industries'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'YESI',
    'Yes! Solar Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'YGRE',
    'Yingli Green Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'YSWG',
    'REDriven, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ZERQ',
    'Zerquest Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'ZSLE',
    'Zhejiang Sunflower Light Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - DC',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Data Conversion Supplier',
    'A',
    'Avineon'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Data Conversion Supplier',
    'I',
    'InfoTech'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Data Conversion Supplier',
    'S',
    'Schematics'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'BLEN',
    'Bluepoint Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'CATP',
    'Caterpiller'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'CUMM',
    'Cummins'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'HE',
    'Hess'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'INGR',
    'Ingersoll-Rand'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'KATO',
    'Kato'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'ONN',
    'Onan'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'STFD',
    'Stamford'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'WK',
    'Waukesha'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - SYN',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'GENE',
    'General Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'HE',
    'Hess'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'INGR',
    'Ingersoll-Rand'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'KATO',
    'Kato'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'KOH',
    'Kohler'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'MARA',
    'Marathon'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'MTS',
    'Mitsubishi'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'RELE',
    'Reliance Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'TEC',
    'Tecogen'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Manufacturer - IND',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1',
    'ASE-100-ATF/17-34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '2',
    'ASE-300-DGF/17-285'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '3',
    'ASE-300-DGF/17-300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '4',
    'ASE-300-DGF/17-315'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '5',
    'ASE-300-DGF/42-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '6',
    'ASE-300-DGF/50-260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '7',
    'ASE-300-DGF/50-285'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '8',
    'ASE-300-DGF/50-300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '9',
    'ASE-300-DGF/50-315'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '10',
    'AP-100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '11',
    'AP-1006'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '12',
    'AP-110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '13',
    'AP-1106'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '14',
    'AP-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '15',
    'AP-1206'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '16',
    'AP-50-GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '17',
    'AP-55-GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '18',
    'AP-6105'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '19',
    'AP-65'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '20',
    'AP-7105'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '21',
    'AP-75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '22',
    'AP6-170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '23',
    'APi-055-GCA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '24',
    'APi-165-MCB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '25',
    'APX-140'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '26',
    'APX-75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '27',
    'APX-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '28',
    'APX-90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '29',
    'LAP-425'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '30',
    'LAP-440'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '31',
    'LAP-460'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '32',
    'LAP-480'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '33',
    'LAPX-300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '34',
    'AP-F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '35',
    'AP-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '36',
    'AP-H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '37',
    'SM-II'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '38',
    'SP-A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '39',
    'SP-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '40',
    'SP-C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '41',
    'SX-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '42',
    'SX-E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '43',
    'BWC 1500'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '44',
    'BWC EXCEL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '45',
    'BWC XL.1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '46',
    '3125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '47',
    '3150B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '48',
    '7170S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '49',
    '7180S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '50',
    '785S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '51',
    '790DB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '52',
    '790S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '53',
    '790U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '54',
    '8451'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '55',
    '8501'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '56',
    '850I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '57',
    '8551'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '58',
    'BP 175 I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '59',
    'BP SM3 150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '60',
    'BP SM3 160S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '61',
    'BP SX 140S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '62',
    'BP SX 150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '63',
    'BP SX3 150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '64',
    'BP SX3 160B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '65',
    'BP SX3 160S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '66',
    'BP175 B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '67',
    'BP2140S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '68',
    'BP2150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '69',
    'BP270U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '70',
    'BP270UL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '71',
    'BP275U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '72',
    'BP275UL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '73',
    'BP3123XR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '74',
    'BP3140B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '75',
    'BP3140S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '76',
    'BP3150B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '77',
    'BP3150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '78',
    'BP3160B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '79',
    'BP3160QS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '80',
    'BP3160S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '81',
    'BP360U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '82',
    'BP375S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '83',
    'BP375U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '84',
    'BP380S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '85',
    'BP380U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '86',
    'BP4150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '87',
    'BP4160S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '88',
    'BP4170B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '89',
    'BP4170S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '90',
    'BP4175B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '91',
    'BP4175I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '92',
    'BP4175S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '93',
    'BP475S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '94',
    'BP475U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '95',
    'BP480S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '96',
    'BP480U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '97',
    'BP5160S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '98',
    'BP5170S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '99',
    'BP580U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '100',
    'BP585KD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '101',
    'BP585U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '102',
    'BP585UL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '103',
    'BP590UL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '104',
    'BP970B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '105',
    'BP970I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '106',
    'BP980B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '107',
    'BP980I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '108',
    'BP990B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '109',
    'BP990I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '110',
    'MST-43I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '111',
    'MST-43LV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '112',
    'MST-43MV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '113',
    'MST-45LV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '114',
    'MST-45MV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '115',
    'MST-50I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '116',
    'MST-50LV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '117',
    'MST-50MV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '118',
    'MSX-110'
  );

INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '119',
    'MSX-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '120',
    'MSX-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '121',
    'MSX-50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '122',
    'MSX-56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '123',
    'MSX-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '124',
    'MSX-64'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '125',
    'MSX-77'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '126',
    'MSX-80U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '127',
    'MSX-83'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '128',
    'SP130-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '129',
    'SX 140S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '130',
    'SX-110S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '131',
    'SX-110U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '132',
    'SX-120S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '133',
    'SX-120U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '134',
    'SX-150B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '135',
    'SX-40D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '136',
    'SX-40M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '137',
    'SX-40U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '138',
    'SX-50D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '139',
    'SX-50M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '140',
    'SX-50U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '141',
    'SX-55D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '142',
    'SX-55U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '143',
    'SX-60D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '144',
    'SX-60U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '145',
    'SX-65D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '146',
    'SX-65U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '147',
    'SX-75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '148',
    'SX-75TS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '149',
    'SX-75TU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '150',
    'SX-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '151',
    'SX-85'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '152',
    'SX140B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '153',
    'SX160B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '154',
    'SX170B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '155',
    'TF-80B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '156',
    'TF-80I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '157',
    'TF-90B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '158',
    'TF-90I'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '159',
    'VLX-53'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '160',
    'VLX-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '161',
    'CNE-35L-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '162',
    'Day4 48MC 170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '163',
    'Day4 48MC 180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '164',
    'Day4 48MC 190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '165',
    'DS-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '166',
    'DS-40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '167',
    'EPV-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '168',
    'EPV-40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '169',
    'E-25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '170',
    'E-28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '171',
    'E-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '172',
    'E-50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '173',
    'E-56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '174',
    'E-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '175',
    'EC-102'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '176',
    'EC-110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '177',
    'EC-115'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '178',
    'EC-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '179',
    'EC-47'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '180',
    'EC-51'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '181',
    'EC-55'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '182',
    'EC-94'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '183',
    'ES-112'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '184',
    'ES-170-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '185',
    'ES-180-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '186',
    'ES-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '187',
    'FS-40D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '188',
    'FS-50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '189',
    'FS-50C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '190',
    'FS-50Z'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '191',
    'GEPV-055-GCA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '192',
    'GEPV-055-GCB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '193',
    'GEPV-110-MCA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '194',
    'GEPV-165-MCA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '195',
    'GEPV-173'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '196',
    'GEPVp-066-GC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '197',
    'GEPVp-200-M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '198',
    'PV-165 MCB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '199',
    'I-150 S/24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '200',
    'I-165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '201',
    'CSA201'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '202',
    'G-SA060'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '203',
    'GSA221'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '204',
    'KC-125G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '205',
    'KC-167G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '206',
    'KC120-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '207',
    'KC125GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '208',
    'KC130GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '209',
    'KC158G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '210',
    'KC170GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '211',
    'KC175GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '212',
    'KC187G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '213',
    'KC190GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '214',
    'KC200GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '215',
    'KC50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '216',
    'KC60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '217',
    'KC70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '218',
    'KC80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '219',
    'Samurai SU53BU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '220',
    'PW 1650'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '221',
    'PW1000-100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '222',
    'PW1000-105'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '223',
    'PW1000-90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '224',
    'PW1000-95'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '225',
    'PW1250-115'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '226',
    'PW1250-125'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '227',
    'PW1250-135'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '228',
    'PW1650-155'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '229',
    'PW1650-165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '230',
    'PW1650-175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '231',
    'PW750-70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '232',
    'PW750-75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '233',
    'PW750-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '234',
    'PW750-90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '235',
    'BP970B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '236',
    'MLB3416-115'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '237',
    'PV-MF120EC3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '238',
    'PV-MF125E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '239',
    'PV-MF130E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '240',
    'PV-MF150EB3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '241',
    'PV-MF155EB3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '242',
    'PV-MF165EB3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '243',
    'PV-MF170EB3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '244',
    'PV-MF170EB4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '245',
    'PL-AP-120L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '246',
    'PL-AP-130'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '247',
    'PL-AP-75 Double Module'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '248',
    'PL-ASE-100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '249',
    'PL-BP-2150S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '250',
    'PL-BP-TF-80L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '251',
    'PL-FS-415-A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '252',
    'PL-MST-43'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '253',
    'PL-MSX-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '254',
    'PL-ND-6AE1DA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '255',
    'PL-PLT-63L-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '256',
    'PL-PW-750'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '257',
    'PL-PW-750 Double Module'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '258',
    'PL-SP-135'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '259',
    'PL-SP-150-24L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '260',
    'PL-SP-150-CPL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '261',
    'PL-SP-75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '262',
    'PL-SP-75 Double Module'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '263',
    'PL-SY-HIP-190BA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '264',
    'PL-SY-HIP-H552BA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '265',
    'PVP 1800'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '266',
    'PVP 2800'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '267',
    'PVP1100E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '268',
    'PVP3200-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '269',
    'HIP-180BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '270',
    'HIP-186BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '271',
    'HIP-190BA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '272',
    'HIP-190BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '273',
    'HIP-195BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '274',
    'HIP-200BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '275',
    'HIP-200BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '276',
    'HIP-205BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '277',
    'HIP-205BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '278',
    'HIP-G751BA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '279',
    'HIP-G751BA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '280',
    'HIP-H552BA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '281',
    'HIP-H552BA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '282',
    'HIP-J54BA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '283',
    'HIP-J54BA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '284',
    'S125-SP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '285',
    'S158-SP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '286',
    'ND-160U1Z'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '287',
    'ND-160U3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '288',
    'ND-162U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '289',
    'ND-167U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '290',
    'ND-167U3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '291',
    'ND-167U3A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '292',
    'ND-187U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '293',
    'ND-200U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '294',
    'ND-205U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '295',
    'ND-208U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '296',
    'ND-250U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '297',
    'ND-60RU1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '298',
    'ND-62RU1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '299',
    'ND-70ELU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '300',
    'ND-70ERU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '301',
    'ND-72ELU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '302',
    'ND-L3E1U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '303',
    'ND-N2ECU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '304',
    'ND-NOECU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '305',
    'ND-Q0E2U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '306',
    'ND167U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '307',
    'NE-165U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '308',
    'NE-170U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '309',
    'NE-80E1U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '310',
    'NE-K125U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '311',
    'NE-K125U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '312',
    'NE-Q5E1U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '313',
    'NE-Q5E2U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '314',
    'NT-175U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '315',
    'NT-180U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '316',
    'NT-185U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '317',
    'NT-188U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '318',
    'NT-R5E1U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '319',
    'NT-S5E1U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '320',
    'Sharp NT-188U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '321',
    'SP130-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '322',
    'SP140-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '323',
    'SQ140-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '324',
    'SQ150-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '325',
    'SQ160-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '326',
    'SQ165-P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '327',
    'SQ165-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '328',
    'SQ175-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '329',
    'SQ85-P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '330',
    'ASE-250DGF/50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '331',
    'ASE-270DGF/50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '332',
    'SAPC-123'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '333',
    'SAPC-165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '334',
    'SAPC-175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '335',
    'SAPC-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '336',
    'SM-110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '337',
    'SM10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '338',
    'SM20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '339',
    'SM46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '340',
    'SM46J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '341',
    'SM50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '342',
    'SM50-H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '343',
    'SM50-HJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '344',
    'SM50-J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '345',
    'SM55'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '346',
    'SM55-J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '347',
    'SM6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '348',
    'SP-140PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '349',
    'SP130-24P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '350',
    'SP140-24P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '351',
    'SP140-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '352',
    'SP150-24P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '353',
    'SP150-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '354',
    'SP18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '355',
    'SP36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '356',
    'SP65'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '357',
    'SP70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '358',
    'SP75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '359',
    'SR100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '360',
    'SR50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '361',
    'SR90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '362',
    'ST36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '363',
    'ST40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '364',
    'SW145 mono/P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '365',
    'SW155 mono/P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '366',
    'SW155 mono/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '367',
    'SW155 poly/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '368',
    'SW165 mono/P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '369',
    'SW165 mono/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '370',
    'SW165 poly/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '371',
    'SW175 mono/P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '372',
    'SW175 mono/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '373',
    'SW175 poly/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '374',
    'SW185 mono/T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '375',
    'PVXS120SU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '376',
    'PVXS120SU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '377',
    'S-055'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '378',
    'S-100D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '379',
    'SQ-080'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '380',
    'SQ-090'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '381',
    '175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '382',
    '502'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '383',
    '503'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '384',
    'AIR403'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '385',
    'H40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '386',
    'H80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '387',
    'SPR-200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '388',
    'SPR-210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '389',
    'SPR-215'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '390',
    'SPR-220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '391',
    'SPR5200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '392',
    'SRP-210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '393',
    'STP 170S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '394',
    'STP 175S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '395',
    '155L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '396',
    'SW 115'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '397',
    'SW 155L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '398',
    'SW165L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '399',
    'ASR-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '400',
    'ASR-64'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '401',
    'ES-124'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '402',
    'PVL-116(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '403',
    'PVL-116(PM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '404',
    'PVL-124'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '405',
    'PVL-128(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '406',
    'PVL-128(PM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '407',
    'PVL-136'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '408',
    'PVL-29(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '409',
    'PVL-29(PM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '410',
    'PVL-58(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '411',
    'PVL-58(PM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '412',
    'PVL-64(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '413',
    'PVL-64(PM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '414',
    'PVL-68'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '415',
    'PVL-87(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '416',
    'PVL-87(PM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '417',
    'SHR-15'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '418',
    'SHR-17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '419',
    'SSR-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '420',
    'SSR-120(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '421',
    'SSR-120J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '422',
    'SSR-120J(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '423',
    'SSR-128'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '424',
    'SSR-128(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '425',
    'SSR-128J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '426',
    'SSR-128J(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '427',
    'SSR-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '428',
    'SSR-60(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '429',
    'SSR-60J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '430',
    'SSR-60J(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '431',
    'SSR-64'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '432',
    'SSR-64(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '433',
    'SSR-64J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '434',
    'SSR-64J(DM)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '435',
    'US-116'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '436',
    'US-32'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '437',
    'US-39'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '438',
    'US-42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '439',
    'US-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '440',
    'US-64'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '441',
    'V.15 Single Phase'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '442',
    '23-10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '443',
    'C30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '444',
    'C60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '445',
    '330'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '446',
    'BP4170H'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '447',
    'SPR-205-BLK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '448',
    'SX3195B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '449',
    'ASE-250DGF/17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '450',
    'ASE-250DGF/50-230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '451',
    'ASE-250DGF/50-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '452',
    'ASE-250DGF/50-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '453',
    'ASE-270DGF/17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '454',
    'ASE-270DGF/50-260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '455',
    'ASE-270DGF/50-270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '456',
    'ASE-300-DGF/25-145'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '457',
    'ASE-300-DGF/50-265'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '458',
    'ASE-300DGF/50-280'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '459',
    'ASE-300DGF/50-290'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '460',
    'ASE-250DGF/17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '461',
    'ASE-250DGF/17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '462',
    'ES-180-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '463',
    'PV-MF170UD4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '464',
    'PV-MF180UD4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '465',
    'PV-MF185UD4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '466',
    'SX3200B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '467',
    'SX3200W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '468',
    'SAPC-170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '469',
    'S 175MU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '470',
    'ASE-100-ATF/17-34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '471',
    'ASE-250DGF/17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '472',
    'ASE-250DGF/50-230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '473',
    'ASE-250DGF/50-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '474',
    'ASE-250DGF/50-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '475',
    'ASE-270DGF/17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '476',
    'ASE-270DGF/50-260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '477',
    'ASE-270DGF/50-270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '478',
    'ASE-300-DGF/17-285'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '479',
    'ASE-300-DGF/17-300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '480',
    'ASE-300-DGF/17-315'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '481',
    'ASE-300-DGF/25-145'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '482',
    'ASE-300-DGF/34-195'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '483',
    'ASE-300-DGF/42-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '484',
    'ASE-300-DGF/50-260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '485',
    'ASE-300-DGF/50-265'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '486',
    'ASE-300-DGF/50-280'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '487',
    'ASE-300-DGF/50-285'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '488',
    'ASE-300-DGF/50-290'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '489',
    'ASE-300-DGF/50-300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '490',
    'ASE-300-DGF/50-310'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '491',
    'ASE-300-DGF/50-315'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '492',
    'ASE-300-DGF/50-320'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '493',
    'ASE-330-DGF/50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '494',
    'ASE-330-DGF/50-330'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '495',
    'ASE-330-DGF/50-340'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '496',
    'SAPC-123'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '497',
    'SAPC-165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '498',
    'SAPC-170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '499',
    'SAPC-175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '500',
    'SAPC-80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '501',
    'HIP-167BA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '502',
    'HIP-175BA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '503',
    'HIP-175BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '504',
    'HIP-180BA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '505',
    'HIP-180BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '506',
    'HIP-180DA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '507',
    'HIP-180DA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '508',
    'HIP-180DA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '509',
    'HIP-186BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '510',
    'HIP-186DA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '511',
    'HIP-186DA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '512',
    'HIP-186DA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '513',
    'HIP-190BA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '514',
    'HIP-190BA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '515',
    'HIP-190BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '516',
    'HIP-190DA2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '517',
    'HIP-195BA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '518',
    'HIP-195DA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '519',
    'HIP-200DA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '520',
    'ND-200U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '521',
    'SX3190B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '522',
    'BP3115S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '523',
    'BP3115U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '524',
    'BP3125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '525',
    'BP3125U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '526',
    'SX4175N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '527',
    'ES-195-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '528',
    'ES-195-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '529',
    'TDB125X125-72-P 150W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '530',
    'TDB125X125-72-P 155W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '531',
    'TDB125X125-72-P 160W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '532',
    'TDB125X125-72-P 165W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '533',
    'TDB125X125-72-P 170W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '534',
    'TDB125X125-72-P 175W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '535',
    'Day4 48MC 160'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '536',
    'Day4 48MC 165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '538',
    'Day4 48MC 175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '540',
    'Day4 48MC 185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '542',
    'PVL-93'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '543',
    'SP225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '544',
    'SP450'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '545',
    'SP480'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '546',
    'Meridian SU39BU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '547',
    'Meridian SU53BU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '549',
    'Samurai SU39BU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '550',
    'FS-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '551',
    'FS-255'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '552',
    'FS-257'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '553',
    'FS-260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '554',
    'FS-262'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '556',
    'FS-267'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '557',
    'FS-270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '558',
    'FS-40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '559',
    'FS-40D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '560',
    'FS-45'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '561',
    'FS-45D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '562',
    'FS-50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '563',
    'FS-50C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '564',
    'FS-50D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '565',
    'FS-50Z'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '566',
    'FS-55'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '567',
    'FS-55D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '569',
    'FS-60D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '570',
    'FS-272'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '571',
    'FS-275'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '572',
    'SP170FM22'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '574',
    'SP200FP32'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '575',
    'NE-160U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '576',
    'Skystream 3.7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '577',
    'S-165D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '578',
    'ND-216U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '579',
    'PV-UD185MF5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '580',
    'SPR-225-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '581',
    'SPR-230-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '582',
    'SW165 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '583',
    'ND-216U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '584',
    'SF190-27-M170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '585',
    'YL175(156)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '586',
    'SPR-305-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '587',
    'SPR-210-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '588',
    'SF190-27-M200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '589',
    'ND-N2ECUF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '590',
    'ND-72ERUF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '591',
    'SPR-210-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '592',
    'SPR-215-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '593',
    'SPR-220-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '594',
    'PL-EVER-ES-190P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '595',
    'OE-34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '596',
    'AES-SS-100-W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '597',
    'SPR-205-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '598',
    'KD205GX-LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '599',
    'PL-SUNP-SPR-210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '600',
    'SPR-215-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '601',
    'KD205GX-L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '602',
    'PL-EVER-ES-180P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '603',
    'KD135GX-LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '604',
    'NE-165U5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '605',
    'KD180GX-LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '606',
    'UE 42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '607',
    'AES-SS-100-C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  ( 'Gen Model - DC','608','ND-224U2');
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '609',
    'SF190-27-M210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '610',
    'ND-224U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '611',
    'PV-UD190MF5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '612',
    'STP 180S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '613',
    'ST170-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '614',
    'ST165-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '615',
    'ST175-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '616',
    'SX3175B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '617',
    'ND-208U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '618',
    'SX3200N_Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '619',
    'FD10.0-20K'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '620',
    'SPR-220-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '621',
    'ND-198U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '622',
    'SX3175N_Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '623',
    'SX3190S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '624',
    'HIP-200BA19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '625',
    'S180 SPU-4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '626',
    'KC130TM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '627',
    'PV-MF155EB4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '628',
    'FH-10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '629',
    'FH-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '630',
    'HIP-195BA19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '631',
    'SPR-200-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '632',
    'SCM215'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '633',
    'ES-A 200-fa2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '634',
    'FD8.0-10K'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '635',
    'ES-A 205-fa2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '636',
    'ND-N6E1U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '637',
    'STP 210S-18/Ub-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '638',
    'ES-A 210-fa2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '639',
    'ES-A 195-fa2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '640',
    'SCM220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '641',
    'PL-ND-6KE1DA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '642',
    'KD210GX-LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '643',
    'ES-A 190-fa2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '644',
    'GEPVc-170-MSA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '645',
    'ND-V230A1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '646',
    'STP 165S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '647',
    'SI680G1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '648',
    'HIP-190BA19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '649',
    'STP 210-18/Ub-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '650',
    'CS6P-190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '651',
    'SL-001-157U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '652',
    'SL-001-165U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '653',
    'SL-001-173U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '654',
    'HIP-205BA19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '655',
    'SX3190W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '656',
    'ND-162U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '657',
    'SCM225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '658',
    'S165-SP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '659',
    'ES-190-VL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '660',
    'CS6P-200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '661',
    'ND-62RU2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '662',
    'ES-A 205-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '663',
    'ND-216UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '664',
    'SGP20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '665',
    'STP270-24/Vb-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '666',
    'CS6A-190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '667',
    'REC225AE-USA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '668',
    'REC215AE-USA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '669',
    'Energy Series ES200W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '670',
    'ES-A 200-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '671',
    'S-001'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '672',
    'ET-P672270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '673',
    'SPV 210 SMAU-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '674',
    'REC215A-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '675',
    'NT-175UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '676',
    'HIP-205NKHA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '677',
    'SL-001-165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '678',
    'HIP-210NKHA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '679',
    'ET-P654205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '680',
    'ND-224UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '681',
    'AWE-25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '682',
    'HIP-215NKHA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '683',
    'ND-U230C1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '684',
    'ET-M572180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '685',
    'FS-275'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '686',
    'TSM-180D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '687',
    'TSM-175D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '688',
    'ND-198UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '689',
    'S16.165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '690',
    'S16.170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '691',
    'S16.175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '692',
    'S16.180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '693',
    'S16.185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '694',
    'S18.210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '695',
    'S18.215'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '696',
    'S18.220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '697',
    'S18.225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '698',
    'S18.230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '699',
    'ATI-2000(210)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '700',
    'ATI-2000(220)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '701',
    'ATI-2000(230)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '702',
    'KC180-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '703',
    'KC205-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '704',
    'KC210-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '705',
    'ASEC-175G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '706',
    'ASEC-175G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '707',
    'ASEC-180G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '708',
    'ASEC-180G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '709',
    'ASEC-185G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '710',
    'ASEC-185G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '711',
    'ASEC-190G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '712',
    'ASEC-190G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '713',
    'ASEC-195G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '714',
    'ASEC-195G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '715',
    'ASEC-200G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '716',
    'ASEC-200G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '717',
    'ASEC-205G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '718',
    'ASEC-205G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '719',
    'ASEC-210G6M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '720',
    'ASEC-210G6S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '721',
    'BP175B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '722',
    'BP3210N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '723',
    'BP3220N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '724',
    '365TS EnergyTile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '725',
    'CS5C-80M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '726',
    'CS5C-90M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '727',
    'CS5A-150M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '728',
    'CS6A-150P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '729',
    'CS6A-150PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '730',
    'CS6A-155P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '731',
    'CS6A-155PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '732',
    'CS5A-160M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '733',
    'CS6A-160P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '734',
    'CS6A-160PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '735',
    'CS6P-160PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '736',
    'CS5A-165M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '737',
    'CS6A-165P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '738',
    'CS6P-165PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '739',
    'CS5A-170M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '740',
    'CS6A-170P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '741',
    'CS6P-170PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '742',
    'CS5A-175M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '743',
    'CS6A-175P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '744',
    'CS6P-175PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '745',
    'CS5A-180M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '746',
    'CS6A-180P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '747',
    'CS6P-180P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '748',
    'CS6P-180PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '749',
    'CS6A-185P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '750',
    'CS6P-185P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '751',
    'CS6P-185PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '752',
    'CS6A-190P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '753',
    'CS6P-190P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '754',
    'CS6P-190PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '755',
    'CS6P-195P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '756',
    'CS6P-195PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '757',
    'CS6P-200P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '758',
    'CS6P-200PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '759',
    'CS6P-205P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '760',
    'CS6P-210P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '761',
    'CS6P-215P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '762',
    'CS5P-220M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '763',
    'CS6P-220P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '764',
    'CS5P-225M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '765',
    'CS6P-225P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '766',
    'CS5P-230M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '767',
    'CS6P-230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '768',
    'CS5P-235M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '769',
    'CS5P-240M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '770',
    'SST 160-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '771',
    'SST 165-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '772',
    'SST 170-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '773',
    'TSM-165DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '774',
    'TSM-170D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '775',
    'TSM-170DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '776',
    'TSM-170DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '777',
    'TSM-170PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '778',
    'TSM-175DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '779',
    'TSM-175DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '780',
    'TSM-175PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '781',
    'TSM-180DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '782',
    'TSM-180DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '783',
    'TSM-180PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '784',
    'TSM-185DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '785',
    'TSM-185DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '786',
    'TSM-185PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '787',
    'TSM-190DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '788',
    'TSM-190PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '789',
    'TSM-220DA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '790',
    'TSM-220PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '791',
    'TSM-230DA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '792',
    'TSM-230PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '793',
    'TSM-240DA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '794',
    'TSM-240PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '795',
    'CHSM-175M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '796',
    'CHSM-230M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '797',
    'Day4 48MC 155'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '798',
    'D6P230A3E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '799',
    'D6P220A3E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '800',
    'D6P210A3E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '803',
    'ET-M572165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '804',
    'ET-M572170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '805',
    'ET-M572175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '806',
    'ET-M572185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '807',
    'ET-P654190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '808',
    'ET-P654195'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '809',
    'ET-P654200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '810',
    'ET-P654210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '811',
    'ET-P672250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '812',
    'ET-P672255'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '813',
    'ET-P672260'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '814',
    'ET-P672265'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '815',
    'ES-B-170-fa1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '816',
    'ES-B-170-fb1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '817',
    'ES-B-180-fa1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '818',
    'ES-B-180-fb1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '819',
    'ES-A-190-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '820',
    'ES-B-190-fa1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '821',
    'ES-B-190-fb1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '822',
    'ES-A-195-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '823',
    'ES-B-195-fa1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '824',
    'ES-B-195-fb1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '825',
    'ES-B-200-fa1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '826',
    'ES-B-200-fb1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '827',
    'ES-A-210-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '828',
    'ES-170-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '829',
    'ES-190-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '830',
    'ES-190-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '831',
    'ES-200-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '832',
    'ES-200-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '833',
    'FS-272'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '834',
    'FS-277'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '835',
    'GEPVp-066-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '836',
    'JT175(35)S1580?808'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '837',
    'JT180(36)S1580?808'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '838',
    'JT185(36)S1580?808'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '839',
    'JT210(29)P1655?992'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '840',
    'JT215(30)P1655?992'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '841',
    'JT220(30)P1655?992'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '842',
    'JT225(30)P1655?992'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '843',
    'JT230(30)P1655?992'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '844',
    'JMP-85W-S5-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '845',
    'JMP-160W-S5-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '846',
    'JMP-170W-S5-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '847',
    'KD130GX-L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '848',
    'KD130GX-LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '849',
    'KD135GX-L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '850',
    'KD180GX-L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '851',
    'KD210GX-L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '852',
    'SU53BU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '853',
    'LM210BB00'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '854',
    'LM220BB00'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '855',
    'LM230BB00'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '856',
    'MAGE Powertec 175/5 ME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '857',
    'PV-EE120MF5F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '858',
    'PV-EE125MF5F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '859',
    'PV-EE130MF5F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '860',
    'PV-MF160EB4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '861',
    'PV-MF165EB4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '862',
    'PV-MF175UD4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '863',
    'PV-UD170MF5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '864',
    'PV-UD175MF5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '865',
    'PV-UD180MF5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '866',
    'NH-100UX 3A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '867',
    'NH-100UX 4A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '868',
    'NH-100UX 5A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '869',
    'MP-170WP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '870',
    'TDB125?125-72-P 180W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '871',
    '165(35) D UL811?1581'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '872',
    '230(35) D UL997?1953'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '873',
    'OE-48'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '874',
    'REC205AE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '875',
    'REC205AE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '876',
    'SCM205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '877',
    'REC210AE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '878',
    'REC210AE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '879',
    'SCM210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '880',
    'REC215AE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '881',
    'REC215AE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '882',
    'REC220AE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '883',
    'REC220AE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '884',
    'REC225AE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '885',
    'REC225AE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '886',
    'REC230AE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '887',
    'REC230AE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '888',
    'HIP-180BA19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '889',
    'HIP-186BA19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '890',
    'HIP-190DA3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '891',
    'HIP-195NKHA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '892',
    'HIP-195NKHA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '893',
    'HIP-200NKHA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '894',
    'HIP-200NKHA5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '895',
    'HIP-205NKHA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '896',
    'HIP-210NKHA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '897',
    'HIP-215NKHA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '898',
    'Poly 202'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '899',
    'Poly 210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '900',
    'Poly 217'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '901',
    'Poly 220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '902',
    'Poly 225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '903',
    'ASE-300-DGF/50-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '904',
    'ASE-300-DGF/50-270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '905',
    'MPE 310 MP 02'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '906',
    'MPE 320 MP 02'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '907',
    'S 165-SPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '908',
    'S 165-SPU-4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '909',
    'S 170-SPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '910',
    'S 170-SPU-4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '911',
    'S 175-SPU-4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '912',
    'SPV 200 SMAU-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '913',
    'ND-62RU1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '914',
    'ND-62RUC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '915',
    'ND-65RU1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '916',
    'ND-65RUC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '917',
    'ND-72ELUC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '918',
    'ND-72ELUF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '919',
    'ND-72ERU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '920',
    'ND-72ERUC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '921',
    'ND-V075CL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '922',
    'ND-V075CR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '923',
    'ND-123UJF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '924',
    'ND-130UJF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '925',
    'ND-N2ECUC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '926',
    'ND-162U1Y'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '927',
    'ND-162U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '928',
    'ND-167U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '929',
    'ND-167U1Y'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '930',
    'ND-167U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '931',
    'ND-167UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '932',
    'ND-176U1Y'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '933',
    'ND-176UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '934',
    'ND-181U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '935',
    'ND-181U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '936',
    'ND-181U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '937',
    'ND-187U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '938',
    'ND-187U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '939',
    'ND-187UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '940',
    'ND-200U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '941',
    'ND-200UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '942',
    'ND-208U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '943',
    'ND-208UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '944',
    'ND-U216C1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '945',
    'ND-220U1F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '946',
    'ND-220UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '947',
    'ND-U224C1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '948',
    'NE-165UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '949',
    'NE-170UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '950',
    'NT-170U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '951',
    'NT-170UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '952',
    'SLK60P6L 215 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '953',
    'SLK60P6L 220 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '954',
    'SLK60P6L 225 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '955',
    'SLK60P6L 230 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '956',
    'SLK60P6L 235 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '958',
    'SP165FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '959',
    'SP170FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '960',
    'SP175FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '961',
    'SP190FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '962',
    'SP190FP52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '963',
    'SP195FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '964',
    'SP195FP12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '965',
    'SP195FP52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '966',
    'SP200FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '967',
    'SP200FP12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '968',
    'SP200FP52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '969',
    'SP205FM52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '970',
    'SP205FP12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '971',
    'SP205FP52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '972',
    'SPM210P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '973',
    'SPM215P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '974',
    'SPM220P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '975',
    'SPM225P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '976',
    'SPM230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '977',
    'SW175 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '978',
    'P220/6+/01 215Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '979',
    'P220/6+/01 220Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '980',
    'P220/6+/01 225Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '981',
    'P220/6+/01 230Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '982',
    'P220/6+/01 235Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '983',
    'SL-001-135'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '984',
    'SL-001-135N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '985',
    'SL-001-135U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '986',
    'SL-001-150'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '987',
    'SL-001-150N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '988',
    'SL-001-150U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '989',
    'SL-001-157'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '990',
    'SL-001-157N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '991',
    'SL-001-165N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '992',
    'SL-001-173'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '993',
    'SL-001-173N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '994',
    'SL-001-182'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '995',
    'SL-001-182N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '996',
    'SL-001-182U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '997',
    'SL-001-191'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '998',
    'SL-001-191N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '999',
    'SL-001-191U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1000',
    'SL-001-200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1001',
    'SL-001-200N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1002',
    'SL-001-200U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1003',
    'PL-PLT-63L-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1004',
    'SPR-200-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1005',
    'SPR-217-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1006',
    'SPR-290-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1007',
    'SPR-310-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1008',
    'SPR-315E-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1009',
    'STP115D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1010',
    'STP115D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1011',
    'STP120D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1012',
    'STP120D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1013',
    'STP125D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1014',
    'STP125D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1015',
    'STP130D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1016',
    'STP130D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1017',
    'STP135D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1018',
    'STP135D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1019',
    'STP160S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1020',
    'STP160S-24/Ab-1 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1021',
    'STP165S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1022',
    'STP165S-24/Ab-1 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1023',
    'STP170S-24/Ab-1 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1024',
    'STP175S-24/Ab-1 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1025',
    'STP180S-24/Ab-1 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1026',
    'STP180-18/Ub-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1027',
    'STP190-18/UB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1028',
    'STP200-18/UB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1029',
    'STP230D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1030',
    'STP230D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1031',
    'STP240D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1032',
    'STP240D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1033',
    'STP250-24/Vb-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1034',
    'STP250D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1035',
    'STP250D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1036',
    'STP260-24/Vb-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1037',
    'STP260D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1038',
    'STP260D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1039',
    'STP270D-12/VEC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1040',
    'STP270D-12/VED'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1041',
    'STP280-24/Vb-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1042',
    'PVL-29'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1043',
    'PVL-31'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1044',
    'PVL-33'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1045',
    'PVL-58'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1046',
    'PVL-62'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1047',
    'PVL-64'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1048',
    'PVL-66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1049',
    'PVL-68'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1050',
    'PVL-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1051',
    'PVL-116'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1052',
    'PVL-124'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1053',
    'PVL-128'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1054',
    'PVL-131'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1055',
    'PVL-136'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1056',
    'PVL-144'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1057',
    'AS-5M-170W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1058',
    'AS-5M-175W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1059',
    'AS-5M-180W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1060',
    'ES190W-NW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1061',
    'ES190W-RW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1062',
    'ES195W-NW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1063',
    'ES195W-RW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1064',
    'ES200W-NW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1065',
    'ES200W-RW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1066',
    'ES205W-NW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1067',
    'ES205W-RW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1068',
    'Zp170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1069',
    'Windspire'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1070',
    'EBA-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1071',
    'SGP5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1072',
    'NU-U230F3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1073',
    'UGE-4K'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1074',
    'NU-U235F1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1075',
    'SSI-M6-235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1076',
    'SSI-M6-230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1077',
    'SSI-M6-220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1078',
    'SSI-M6-215'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1079',
    'EBH-25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1080',
    'YL230P-29b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1081',
    'NU-U235F3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1082',
    'BP3230T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1083',
    'S80015dc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1084',
    'NU-U240F1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1085',
    'KD210GX-LPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1086',
    'MPE 170 MS 05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1087',
    'MPE 175 MS 05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1088',
    'MPE 180 MS 05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1089',
    'MPE 185 MS 05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1090',
    'MPE 200 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1091',
    'MPE 205 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1092',
    'MPE 210 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1093',
    'MPE 215 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1094',
    'MPE 220 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1095',
    'MPE 220 PS 09'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1096',
    'MPE 225 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1097',
    'MPE 225 PS 09'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1098',
    'MPE 230 PS 04'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1099',
    'MPE 230 PS 09'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1100',
    'IRTV14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1101',
    'EBH-5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1102',
    'E230B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1103',
    'CHSM6610M-230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1104',
    'SunBase 1.0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1105',
    'ET-P660230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1106',
    'ET-M572180B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1107',
    'KD235GX-LPB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1108',
    'Powertec Plus 185/5 MH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1109',
    'HIP-200BA20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1110',
    'P6-54 205W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1111',
    'SW225 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1112',
    'SW225 mono black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1113',
    'SW245 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1114',
    'SGM-230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1115',
    'SER-228P (Serengeti)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1116',
    'SPR-318E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1117',
    'SPR-315E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1118',
    'STP275-24/Vd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1119',
    'YL235P-29b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1120',
    'YL225P-29b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1121',
    'BP3215B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1122',
    'REC220PE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1123',
    'TSM-230PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1124',
    'CS5A-185M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1125',
    'SST250-60M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1126',
    'Conergy P 230PA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1127',
    'Conergy P 235PA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1128',
    'Powertec Plus 230/6PJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1129',
    'REC230PE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1130',
    'LPC241SM-02'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1131',
    'HIT-N220A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1132',
    'P6-54 200W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1133',
    'Poly 230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1134',
    'PL-SY-HIP-190CA1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1135',
    'STP185S-24/Adb+'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1136',
    'WS-175-1-DC0-0-A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1137',
    'ET-M572185B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1138',
    'HiS-M224SG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1139',
    'REC230PE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1140',
    'SF160-24-P170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1141',
    'SW230 mono black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1142',
    'SPR-238E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1143',
    'TSM-230PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1144',
    'TSM-235PA05.05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1145',
    'SF160-24-M170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1146',
    'STP205-18/Ud'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1147',
    'CS5A-185MX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1148',
    'BP3616N1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1149',
    'LS285-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1150',
    'SQ155-PC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1151',
    'SOLON Black 230/15/01 240WP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1152',
    'SLK60P6L BLK/BLK 235Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1153',
    'LS250-60M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1154',
    'STP185S-24/Adb'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1155',
    'WS_175_1_DC0_0_B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1156',
    'ND-U230Q1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1157',
    'REC225PE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1158',
    'PM230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1159',
    'PV-UE125MF5N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1160',
    'SCM205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1161',
    'c-Si M 60-230-16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1162',
    'DJ-270P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1163',
    'GS-S-230-CS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1164',
    'LRSS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1165',
    'MR-107'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1166',
    'SAPC-165S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1167',
    'SLK60P6L SLV/WHT 235Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1168',
    'Sunmodule SW 235 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1169',
    'CRM235S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1170',
    'SPR-320E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1171',
    'SPR-230E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1172',
    'STP190S-24/Ad+'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1173',
    'TSM-225PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1174',
    'SLK60P6L BLK/BLK 230Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1175',
    'SLK60P6L BLK/WHT 210Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1176',
    'Solaria 220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1177',
    'REC220PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1178',
    'REC235PE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1179',
    '31-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1180',
    '1STH-230-P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1181',
    'BP3175N_Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1182',
    'DJ-175D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1183',
    'TWES-(230)60P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1184',
    'ET-M572180B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1185',
    'ET-P660230B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1186',
    'TKSA-23001'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1187',
    'TKSA-23501'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1188',
    'Powertec Plus 180/5MJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1189',
    'REC225PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1190',
    'REC230PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1191',
    'SF190-27-P200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1192',
    'PL-SUNP-SPR-318E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1193',
    'SPR-11401f-3-208 Delta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1194',
    'T5-SPR-318E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1195',
    'YL255P-32b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1196',
    'CS6P-235PX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1197',
    'HiS-M227SG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1198',
    'HiS-M230SG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1199',
    'TPB156x156-60-P 230W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1200',
    'CRM235S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1201',
    'SLK60P6L BLK/BLK 225Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1202',
    'SLK60P6L SLV/WHT 225Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1203',
    'SLK60P6L SLV/WHT 230Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1204',
    'SF85-US-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1205',
    'SF220-30-P235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1206',
    'Upsolar UP-M225P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1207',
    'SF125x125-72-M-175W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1208',
    'BP485U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1209',
    'CS6P-240M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1210',
    'SST295-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1211',
    'CHSM6610P-230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1212',
    'Powertec Plus 190/5MI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1213',
    'STP295-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1214',
    'NU-U245P1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1215',
    'Solaria 230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1216',
    'SN-110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1217',
    'AB1-55-A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1218',
    'CS5A-190M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1219',
    'CS6P-215PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1220',
    'TWES-(220)60P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1221',
    'ET-P660235B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1222',
    'GSM6-225D-E1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1223',
    'PS-185M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1224',
    'TPB156x156-60-P 230W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1225',
    'REC215PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1226',
    'P6-54 195W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1227',
    'NU-U240F2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1228',
    'Sunmodule SW 240 Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1229',
    'HU662AWK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1230',
    'STP185S-24/Ad+'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1231',
    'SF156x156-60-M-230W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1232',
    'K230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1233',
    'DM230M2-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1234',
    'TWES-(175)72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1235',
    'REC235PE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1236',
    'Poly 235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1237',
    'M75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1238',
    'SW235 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1239',
    'SW250 Mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1240',
    'SL-200-182'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1241',
    'CRM235S125M-96'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1242',
    'YL220P-29b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1243',
    'E-3120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1244',
    'NEX-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1245',
    'DM280M2-36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1246',
    'TWES-(180)72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1247',
    'REC240PE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1248',
    'NU-U235F4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1249',
    'STP225-20/Wd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1250',
    '1STH-240-WH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1251',
    'CS6P-220PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1252',
    'CP6230SW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1253',
    'CSUN190D-24D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1254',
    'Conergy P 225PA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1255',
    'GB60P6-220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1256',
    'Powertec Plus 230/6PI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1257',
    'MS130GG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1258',
    'LPC244SM-08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1259',
    'SPV 215 SMAU-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1260',
    'MPE 235 PS 09'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1261',
    'S-165D-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1262',
    'NU-U235F2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1263',
    'SRMA-180WP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1264',
    'SW230 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1265',
    'SW235 mono black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1266',
    'SW240 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1267',
    'SGM-270P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1268',
    'SPR-425E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1269',
    'STP225-18/Ud'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1270',
    'TWES-(230)60P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1271',
    'CS5P-250M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1272',
    'CS6P-230M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1273',
    'GES-P230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1274',
    'GB60P6-225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1275',
    'SF160-24-M185B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1276',
    'MTPVp-235-MSC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1277',
    'TPB156x156-72-P 280W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1278',
    'REC240PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1279',
    'REC240PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1280',
    'REC240PE-US (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1281',
    'HIT-N225A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1282',
    'API-185M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1283',
    'SFX3-i210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1284',
    'SL-200-173'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1285',
    'SL-200-191'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1286',
    'ST-175-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1287',
    'YL255C-30b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1288',
    'YL260P-35b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1289',
    'BP3235T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1290',
    'CSI-AC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1291',
    'CP6235SW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1292',
    'DJ-280P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1293',
    'ET-P654200B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1294',
    'PLM-250M-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1295',
    'SW240 mono black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1296',
    'STP280-VRM-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1297',
    'YL260C-30b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1298',
    'AT5-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1299',
    'SLG S300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1300',
    '1STH-245-WH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1301',
    'CS6P-230PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1302',
    'CS6P-250M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1303',
    'CP6240BW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1304',
    'CHSM6610M-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1305',
    'CHSM6610P-235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1306',
    'SDM-170/(185)-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1307',
    'GS-S-240-Fab1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1308',
    'ET-M572185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1309',
    'ET-M572185B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1310',
    'GV-MOD001-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1311',
    'SF160-24-M190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1312',
    'SF220-30-M235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1313',
    'SF220-30-P230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1314',
    'SF220-30-P235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1315',
    'GPM240-B-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1316',
    'JS140P36-12V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1317',
    'Powertec Plus 230/6PH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1318',
    'PV-MLU255HC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1319',
    'Nanosolar Untility Panel 150W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1320',
    'REC235PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1321',
    'REC235PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1322',
    'CRM180S125M-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1323',
    'CRM285S156P-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1324',
    'ND-224QCJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1325',
    'NU-Q235F4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1326',
    'NU-Q240F2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1327',
    'SLK60P6L BLK/WHT 245Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1328',
    'SLK60P6L SLV/WHT 240Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1329',
    'SLK60P6L SLV/WHT 245Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1330',
    'SW225 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1331',
    'SW245 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1332',
    'SF220-230-P240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1333',
    'SOLON Blue 220/01 230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1334',
    'CRM230S156P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1335',
    'SER-235P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1336',
    'SPR-225E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1337',
    'SPR-240E-WHT-U-ACPV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1338',
    'SPR-435NE-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1339',
    'STP170S-24/Adb+'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1340',
    'STP190S-24/Adb+'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1341',
    'UP-M230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1342',
    'UP-M280P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1343',
    'HELX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1344',
    'c-Si M 60-225-16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1345',
    'BYD 235P6-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1346',
    'CS5P-230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1347',
    'CS5P-245M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1348',
    'CS5P-255M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1349',
    'CS6P-215PX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1350',
    'CS6X-295M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1351',
    'CS6X-290M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1352',
    'BP6235SW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1353',
    'CP6230SW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1354',
    'Apollo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1355',
    '230-0400'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1356',
    'M215-60-SLL-S2x-NA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1357',
    'GS-S-250-Fab5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1358',
    'ET-P660235B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1359',
    '6T 240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1360',
    'LDK-230D-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1361',
    'LDK-245D-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1362',
    'LDK-290P-24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1363',
    'LS245-60M-J'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1364',
    'MTPVP-240-MSC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1365',
    'PLM-280P-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1366',
    'PLM-6B-235P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1367',
    'CRM280S156P-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1368',
    'ND-230UCJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1369',
    'ND-U230Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1370',
    'SLK60P6L BLK/BLK 240Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1371',
    'SF220-30-P240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1372',
    'SOLON Blue 270/09 280'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1373',
    'SPR-327NE-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1374',
    'PLUTO240-Wde'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1375',
    'PLUTO240-Wde'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1376',
    'PLUTO245-Wde'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1377',
    'STP200-18/Ud'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1378',
    'UP-M270P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1379',
    'WS_185_1_DC0_0_A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1380',
    'GSM-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1381',
    'SF125x125-72-M-180W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1382',
    'API-185MPM220P01.0_235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1383',
    'BYD 240P6-36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1384',
    'EPV-40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1385',
    'ET-P660240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1386',
    'JAM6-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1387',
    'LG235M1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1388',
    'ND-L235Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1389',
    'ND-240QCJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1390',
    'NU-Q235F2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1391',
    'SF85-US-P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1392',
    'SOLON Corvus 235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1393',
    'SE14-50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1394',
    'SPR-225E-BLK-U-ACPV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1395',
    'TP660P-235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1396',
    'TP660M-235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1397',
    'WLW-235-1-DC1-0-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1398',
    'GT3.3-NA-DS-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1399',
    'XZERES 442'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1400',
    'Conergy PH 225P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1401',
    'Conergy PH 240P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1402',
    'Conergy PH 235P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1403',
    'SPR-240E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1404',
    'SPR-225E-BLK-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1405',
    'SPR-76R-BLK-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1406',
    'ND-224UCJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1407',
    'ND-U235Q1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1408',
    'KD215GX-LPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1409',
    'KD240GX-LPB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1410',
    'BP6235BB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1411',
    'BP4180B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1412',
    'ET-P672275'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1413',
    'SW255 Mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1414',
    'SW240 Mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1415',
    'KD245GX-LPB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1416',
    'ET-P660230B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1417',
    'OE-34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1418',
    'OE-50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1419',
    'PM220P01.0_235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1420',
    'BIPV052-S11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1421',
    'BIPV052-S86'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1422',
    'BIPV052-T86'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1423',
    'BP170B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1424',
    'BP3170B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1425',
    'BP3170N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1426',
    'BP3170N_Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1427',
    'BP3170T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1428',
    'BP3175B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1429',
    'BP3175T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1430',
    'BP3180N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1431',
    'BP3180N_Q'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1432',
    'BP3210T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1433',
    'BP3220T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1434',
    'BP3225N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1435',
    'BP3225T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1436',
    'BP3230N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1437',
    'BP3237T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1438',
    'BP365TS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1439',
    'SX-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1440',
    'SX140S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1441',
    'SX3190N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1442',
    'SX3195N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1443',
    'SX3195W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1444',
    'SX3200N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1445',
    'BI-156-200W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1446',
    'CS5A-180MX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1447',
    'CS5A-195M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1448',
    'CS5P-225P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1449',
    'CS6P-230PX'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1450',
    'CS6X-280M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1451',
    'CHSM6610P-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1452',
    'DJ-180D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1453',
    'DJ-220P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1454',
    'DJ-230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1455',
    'CSE275P-3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1456',
    'D220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1457',
    'CP6235BW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1458',
    'Conergy P 185M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1459',
    'CSG220M2-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1460',
    'D6P175A2E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1461',
    'D6P180A2E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1462',
    'D6P185A2E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1463',
    'D6P215A3E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1464',
    'D6P225A3E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1465',
    'SDM-170/(180)-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1466',
    'ECO230S156P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1467',
    'SDM-170/(175)-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1468',
    'SDM-170/(180)-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1469',
    'SDM-170/(185)-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1470',
    'SMI-220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1471',
    'EPV-42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1472',
    'D380-72-2LL-S1x (-NA)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1473',
    'ET-P660225B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1474',
    'ET-P660235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1475',
    'ET-P660240B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1476',
    'ET-P660245B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1477',
    'ET-P672275'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1478',
    'ET-P672280'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1479',
    'EC-110-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1480',
    'EC-120-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1481',
    'ES-155-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1482',
    'ES-155-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1483',
    'ES-160-RL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1484',
    'ES-160-SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1485',
    'ES-180-VL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1486',
    'ES-195-VL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1487',
    'ES-A 190-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1488',
    'ES-A 195-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1489',
    'ES-A 210-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1490',
    'ES-A-215-fa3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1491',
    'ES-B 200-fa1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1492',
    'ES-C-120-fa4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1493',
    'FTS235M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1494',
    'FS-265'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1495',
    'FS-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1496',
    'CS-P-220-DJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1497',
    'CS-P-230-DJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1498',
    'ET-M572170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1499',
    'ET-M572180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1500',
    'ET-P654205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1501',
    'GV16K-480-3D (24598)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1502',
    'GV2-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1503',
    'SF190-27-M195'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1504',
    'SF190-27-P195'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1505',
    'HiS-M206SF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1506',
    'HiS-S250MG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1507',
    'IP-230 GG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1508',
    'IS-220/32'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1509',
    'JKM240M-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1510',
    'JKM240M-60B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1511',
    'JKM240P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1512',
    'JKM270P-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1513',
    'SF190-27-M190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1514',
    'SF190-27-M205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1515',
    'SF190-27-P205'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1516',
    'GES-P230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1517',
    'GSA211'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1518',
    'KC120-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1519',
    'KC125GT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1520',
    'KD130GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1521',
    'KD135GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1522',
    'KD135GX-LPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1523',
    'KD140GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1524',
    'KD180GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1525',
    'KD185GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1526',
    'KD185GX-LPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1527',
    'KD190GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1528',
    'KD200GX-LPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1529',
    'KD205GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1530',
    'KD205GX-LPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1531',
    'KD210GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1532',
    'KD215GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1533',
    'KD225GX-LFB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1534',
    'KD225GX-LPB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1535',
    'KD230GX-LFB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1536',
    'KD230GX-LPB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1537',
    'KD235GX-LFB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1538',
    'KD240GX-LFB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1539',
    'LDK-230P-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1540',
    'LG220P1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1541',
    'LG225P1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1542',
    'LG235P1W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1543',
    'LG240M1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1544',
    'LSX 190-72M-W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1545',
    'Powertec 230/6PJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1546',
    'Powertec Plus 180/5 ME'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1547',
    'Powertec Plus 185/5MJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1548',
    'Powertec Plus 240/6MF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1549',
    'Powertec Plus 240/6MI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1550',
    'Powertec Plus 240/6MO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1551',
    'MBPV CAAP BB 225W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1552',
    'MF180UD4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1553',
    'PV-MF110EC3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1554',
    'PV-MF110EC4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1555',
    'PV-MF130EA2LF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1556',
    'PV-UJ225GA6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1557',
    'TDB125x125-72-P 175W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1558',
    'TPB125x125-96-P 240W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1559',
    'TPB156x156-72-P 240W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1560',
    'NXT18m'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1561',
    'NXT20e'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1562',
    'GS8048'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1563',
    'PS230M-20/U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1564',
    'PS230P-20/U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1565',
    'PS240M-20/U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1566',
    'PS240P-20/U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1567',
    'SCM210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1568',
    'SCM215'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1569',
    'SCM220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1570',
    'SCM225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1571',
    'REC220PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1572',
    'REC225PE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1573',
    'REC230PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1574',
    'REC245PE-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1575',
    'LPC244SM-02'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1576',
    'LPC247SM-08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1577',
    'MPE 190 MS 05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1578',
    'MPE 240 PS 09'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1579',
    'S-165 SPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1580',
    'S165 SPU-4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1581',
    'S170-SPU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1582',
    'SP 165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1583',
    'CRM230S156P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1584',
    'CRM235S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1585',
    'SDM-170/(180)-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1586',
    'NT-170U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1587',
    'SQ75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1588',
    'ASE-300DGF/50-280'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1589',
    'ASE-300DGF/50-290'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1590',
    'ASE-300DGF/50-310'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1591',
    'ND-200U2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1592',
    'ND-230QCJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1593',
    'ND-L240Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1594',
    'ND-L245Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1595',
    'ND-Q235Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1596',
    'ND-Q240Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1597',
    'ND-U235Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1598',
    'NE-160U1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1599',
    'NU-Q230F4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1600',
    'NU-U180FC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1601',
    'NU-U208FC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1602',
    'NU-U240P2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1603',
    'SLK60P6L BLK/WHT 225Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1604',
    'SLK60P6L BLK/WHT 230Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1605',
    'SR-180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1606',
    'SI816GI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1607',
    'SW 165 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1608',
    'SW 175 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1609',
    'SW220 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1610',
    'SW230 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1611',
    'SW235 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1612',
    'SW240 mono'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1613',
    'SW245 Mono Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1614',
    'SW250 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1615',
    'CRM115S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1616',
    'CRM145S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1617',
    'CRM175S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1618',
    'CRM195S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1619',
    'CRM245S125M-96'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1620',
    'CRM255S125M-96'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1621',
    'CRM60S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1622',
    'CRM85S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1623',
    'STP050D-5/ZCB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1624',
    'STP050D-5/ZCF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1625',
    'STP050D-5/ZCG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1626',
    'STP150-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1627',
    'STP150S-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1628',
    'STP160S-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1629',
    'STP170-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1630',
    'STP170S-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1631',
    'STP175-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1632',
    'STP175S-24/AB-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1633',
    'STP180S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1634',
    'STP180S-24/Adb+'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1635',
    'STP185S-24/Ab-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1636',
    'STP200-18/Ub-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1637',
    'STP210-18/Ub-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1638',
    'STP210-18/Ud'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1639',
    'STP210S-18/Ub-1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1640',
    'STP270-24/Vd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1641',
    'STP280-24/Vd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1642',
    'SW 180'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1643',
    'SW175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1644',
    'HIP-180BA20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1645',
    'HIP-186BA20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1646',
    'HIP-190BA20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1647',
    'HIP-195BA20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1648',
    'HIP-195NKHA6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1649',
    'HIP-200NKHA6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1650',
    'HIP-205BA20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1651',
    'HIP-205NKHA6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1652',
    'HIP-210NKHA6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1653',
    'HIP-215NKHA6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1654',
    'HIT-N195A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1655',
    'HIT-N200A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1656',
    'HIT-N205A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1657',
    'HIT-N210A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1658',
    'HIT-N215A01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1659',
    'SF160-24-M170'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1660',
    'SF160-24-M175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1661',
    'SF160-24-P165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1662',
    'MST-43MV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1663',
    'CMT-190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1664',
    'P220/6+/01 220Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1665',
    'P220/6+/01 225 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1666',
    'P220/6+/01 230Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1667',
    'P220/6+/01 235Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1668',
    'SOLON Black 230/15 235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1669',
    'SOLON Black 235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1670',
    'SOLON Blue 270/09/01 270 Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1671',
    'SSI-M6-225'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1672',
    'SL-150-191'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1673',
    'SL-200-200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1674',
    'SL-200-210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1675',
    'SP200FP32'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1676',
    'SPI-M200-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1677',
    'SPT16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1678',
    'CRM175S125S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1679',
    'CRM230S156P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1680',
    'CRM235S156P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1681',
    'S-125D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1682',
    'SST185-72M-Roof'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1683',
    'SST240-60M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1684',
    'SST285-72M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1685',
    'SN-120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1686',
    'SE14-52'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1687',
    'Q110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1688',
    'SGM-180D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1689',
    'PL-SUNP-SPR-290'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1690',
    'PL-SUNP-SPR-305'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1691',
    'PL-SUNP-SPR-308E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1692',
    'PL-SUNP-SPR-310'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1693',
    'PL-SUNP-SPR-315E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1694',
    'SER-228P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1695',
    'SPR-208-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1696',
    'SPR-215-BLK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1697',
    'SPR-295E-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1698',
    'SPR-305E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1699',
    'SPR-305E-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1700',
    'SPR-308E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1701',
    'SPR-308NE-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1702',
    'SPR-310-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1703',
    'SPR-310E-WHT-U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1704',
    'SPR-3301f-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1705',
    'T5-SPR-290'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1706',
    'T5-SPR-305'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1707',
    'T5-SPR-305E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1708',
    'T5-SPR-308E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1709',
    'T5-SPR-310'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1710',
    'T5-SPR-315'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1711',
    'TW180(35)D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1712',
    'TW230(28)P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1713',
    'TSM-165DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1714',
    'TSM-170D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1715',
    'TSM-170DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1716',
    'TSM-170DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1717',
    'TSM-170PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1718',
    'TSM-175D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1719',
    'TSM-175DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1720',
    'TSM-175DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1721',
    'TSM-175PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1722',
    'TSM-180D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1723',
    'TSM-180DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1724',
    'TSM-180DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1725',
    'TSM-180PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1726',
    'TSM-185DA01'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1727',
    'TSM-185DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1728',
    'TSM-185PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1729',
    'TSM-190DA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1730',
    'TSM-190PA03'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1731',
    'TSM-220DA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1732',
    'TSM-220PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1733',
    'TSM-230DA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1734',
    'TSM-235PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1735',
    'TSM-240DA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1736',
    'TSM-240PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1737',
    'PVM220PS-Q6LTT220'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1738',
    'ES-62T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1739',
    'PVL-144'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1740',
    'PVL-62'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1741',
    'SSR-136'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1742',
    'AS-5M36-185W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1743',
    'W1600 - 165'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1744',
    'XR36-291'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1745',
    'YL175(156)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1746',
    'YL175P-23b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1747',
    'YL280P-35b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1748',
    'SF125x125-72-M-185W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1749',
    'SF156x156-60-M-235W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1750',
    'T2-R15'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1751',
    'Twister 1000-T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1752',
    '800080'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1753',
    'ACS-250-M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1754',
    'PM270AA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1755',
    'CS6P-225PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1756',
    'CS6P-235M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1757',
    'CS6P-250P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1758',
    'CS6X-300M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1759',
    'ECO235S156P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1760',
    'ET-P660235WBZ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1761',
    'ET-P660250B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1762',
    'SF220-30-P240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1763',
    'LDK-250D-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1764',
    'LG245S1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1765',
    'MEMC-M240LMA-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1766',
    'Nanosolar Utility Panel 200W (NS-200)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1767',
    'NU-U240P1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1768',
    'SLK60P6L SLV/WHT 255Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1769',
    'SMX230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1770',
    'SL-150-182'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1771',
    'TDB125x125-72-P 190W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1772',
    'TP660P-240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1773',
    'TSM-285PA14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1774',
    'WLW-235-1-AC1-A-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1775',
    'WLW-235-1-AC0-D-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1776',
    'GW 133-11 KW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1777',
    'P-3000-G'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1778',
    'S79U240 ulr'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1779',
    'ASW-230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1780',
    'PM220P01.0_240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1781',
    'PM240P00_240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1782',
    'PM250M00_240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1783',
    'PM250M00_255'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1784',
    'BIPV052-T16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1785',
    'CS6P-235P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1786',
    'CS6P-240P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1787',
    'CS6P-245M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1788',
    'CS6X-280P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1789',
    'E235B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1790',
    'E235BWZ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1791',
    'E275'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1792',
    'CHSM5612M-185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1793',
    'CHSM6610M-235'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1794',
    'CNPV-205M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1795',
    'CNPV-245P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1796',
    'CNPV-250M'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1797',
    'D6M240B3A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1798',
    'TWES-(225)60P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1799',
    'ET-P672290'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1800',
    'GS-P-280-Fab1 280W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1801',
    'SF220-30-P245B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1802',
    'HiS-S245MG(BK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1803',
    'HiS-S248MG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1804',
    'AD235P6-Ab'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1805',
    'JKM280P-72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1806',
    'JKM280P-72B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1807',
    'KD220GX-LFBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1808',
    'LG250S1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1809',
    'LG255S1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1810',
    'LSX180-72M-C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1811',
    'Powertec Plus 230/6 PH-US'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1812',
    'MEMC-M240AMA-20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1813',
    'PV-MLU260HC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1814',
    'MTPVp-230-MSB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1815',
    'MTPVp-235-MSB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1816',
    'MTS-215-P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1817',
    'PS250m-20U'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1818',
    'Q.Smart-90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1819',
    'REC 205 AE-US-Silver'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1820',
    'REC245PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1821',
    'P6-54 190W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1822',
    'MPE 230 PS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1823',
    'MPE 240 PS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1824',
    'MPE 250 MS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1825',
    'MPE 255 MS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1826',
    'MPE 260 MS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1827',
    'CRM240S125M-96'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1828',
    'ND-230UC1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1829',
    'ND-235QCJ'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1830',
    'ND-Q235F4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1831',
    'SLK60P6L SLV/WHT 250Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1832',
    'SLK60P6L SLV/WHT 220Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1833',
    'API-230P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1834',
    'SW220 Poly'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1835',
    'SW250 Mono Black'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1836',
    'SR156P-230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1837',
    'OPT260-60-4-100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1838',
    'SPR-245NE-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1839',
    'SPR-300E-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1840',
    'PLUTO240-Wdm'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1841',
    'STP190S-24/Adb'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1842',
    'STP295-24/Vd'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1843',
    'TSM-185DA01A.05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1844',
    'TSM-230PA05.08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1845',
    'TSM-240PA05.10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1846',
    'TSM-275PA14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1847',
    'TSM-280PA14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1848',
    'WLW-235-1-DC0-0-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1849',
    'YL240P-29b'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1850',
    'PM250MA0_255'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1851',
    'BIPV052-S16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1852',
    'BIPV054-S16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1853',
    'BYD 230P6-30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1854',
    'CS5P-235P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1855',
    'CS6X-300P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1856',
    'BP6-235BW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1857',
    'EP6 240 BW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1858',
    'DJ-210P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1859',
    'CHSM6610M-250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1860',
    'Conergy PM 235P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1861',
    'Conergy PM 240P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1862',
    'Conergy PM 245P'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1863',
    'D6P230B3A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1864',
    'ET-A-P660230B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1865',
    'ET-P672290B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1866',
    'ES-E-210-fc3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1867',
    'AC2-1-B-6-5-xx'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1868',
    'SF160-24-M185L-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1869',
    'SF260-36-P285'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1870',
    'LG260S1C-G2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1871',
    'Powertec Plus 235/6PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1872',
    'Powertec Plus 235/6PH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1873',
    'Powertec Plus 245/6 MNBS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1874',
    'Powertec Plus 250/6 MNCS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1875',
    'MTPVp-240-MSB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1876',
    'MTPVp-240-MSD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1877',
    'PLM-240P-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1878',
    'REC250PE (BLK)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1879',
    'PV-MBA1CG247'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1880',
    'Perform Poly 240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1881',
    'PERFORM POLY 245'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1882',
    'Poly 240'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1883',
    'MPE 235 PS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1884',
    'MPE 245 PS 08'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1885',
    'ND-H240Q2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1886',
    'SLK60P6L BLK/BLK 245Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1887',
    'SLK60P6L SLV/WHT 210Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1888',
    'SLK60P6L SLV/WHT 250Wp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1889',
    'Solaria 270'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1890',
    'SOLON Black 230/15 230'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1891',
    'SR-175'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1892',
    'OPT250-60-4-1B0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1893',
    'SPR-225NE-BLK-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1894',
    'SPR-230NE-BLK-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1895',
    'SPR-320NE-WHT-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1896',
    'SNPM-G60-02-190'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1897',
    'STP245S-20/Wdb'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1898',
    'TSM-240PA05.18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1899',
    'TSM-245PA05'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1900',
    'PVM220PS-Q6LTT210'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '1901',
    'WLW-235-1-AC2-D-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '555',
    'FS-265'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '568',
    'FS-60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    '573',
    'SP200FP12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Gen Model - DC',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '19920',
    '19.92'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '2400',
    '2.4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '4160',
    '4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '21000',
    '21'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '17000',
    '17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '34500',
    '34.5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '7200',
    '7.2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Primary Nominal Voltage',
    '12000',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '0',
    'Pilot'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '1',
    '1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '2',
    '2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '3',
    '3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '4',
    '4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '5',
    '5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '6',
    '6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '7',
    '7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '8',
    '8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '9',
    '9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '10',
    '10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '11',
    '11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '12',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '13',
    '13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '14',
    '14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '15',
    '15'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '16',
    '16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '17',
    '17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '18',
    '18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '19',
    '19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '20',
    '20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '21',
    '21'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '22',
    '22'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '23',
    '23'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '24',
    '24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '25',
    '25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '26',
    '26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '27',
    '27'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion WorkPackage Land',
    '28',
    '28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Job Type',
    'OH',
    'OH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Job Type',
    'UG',
    'UG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Job Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Status Land',
    '1',
    'Proposed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Status Land',
    '2',
    'In Construction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Status Land',
    '3',
    'As-Built'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Zip',
    '80',
    '80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Corrosion Area',
    'S',
    'Severe Corrosion'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Corrosion Area',
    'M',
    'Moderate Corrosion'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Corrosion Area',
    'O',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'GOOD',
    'Unrestricted Access'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'READ',
    'Readily Accessible'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'HARD',
    'Not Readily Accessible'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'UNC1',
    'Uncollectible - First Attempt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'UNC2',
    'Uncollectible - Second Attempt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'UNC3',
    'Uncollectible - Third Attempt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU01',
    'Customer - Refusal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU02',
    'Customer - Hostile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU03',
    'Customer - Locked Gate'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU04',
    'Customer - In a fenced enclosure or backyard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU05',
    'Customer - City-county-state-federal refuse access'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU06',
    'Customer - Vicious or mean dogs'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU07',
    'Customer - Call for appointment'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'CU08',
    'Customer - Unable to secure appropriate permits'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG01',
    'Agriculture - Dense crop planting'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG02',
    'Agriculture - Flooded or watered field'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG03',
    'Agriculture - Organic farm'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG04',
    'Agriculture - Wait until after harvest'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG05',
    'Agriculture - Pasture Land'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG06',
    'Agriculture - Open field'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG07',
    'Agriculture - Planted field'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'AG08',
    'Agriculture - Orchard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR01',
    'Terrain - Access by foot only'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR02',
    'Terrain - Steep incline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR03',
    'Terrain - Unsafe roads'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR04',
    'Terrain - Unsafe foot access'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR05',
    'Terrain - Preserve or protected lands'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR06',
    'Terrain - In wet lands or flooded'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR07',
    'Terrain - Restricted by vegetation growth'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR08',
    'Terrain - Hillside'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'WE01',
    'Weather - No winter access due to snow or ice'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'WE02',
    'Weather - Flooded due to rain or runoff'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'WE03',
    'Weather - No winter access due to rain or runoff'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'OTHR',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Access Info',
    'TR09',
    'Terrain - Marsh or Wetlands'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'R',
    'R'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'S',
    'S'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'T',
    'T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'X',
    'X'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'R1',
    'R1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'S1',
    'S1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'T1',
    'T1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Climate Zone',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'N',
    'North'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'S',
    'South'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'E',
    'East'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'W',
    'West'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'NW',
    'NW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'NE',
    'NE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'SW',
    'SW'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Anchor Direction',
    'SE',
    'SE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Type - UG',
    'DPX',
    'Duplex'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Type - UG',
    'QPX',
    'Quadraplex 600V'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Type - UG',
    'TPX',
    'Triplex'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Type - UG',
    'SW',
    'Single Wire'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Type - UG',
    'BusDuct',
    'BusDuct'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Type - UG',
    'Armour',
    'Armour Cable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Type - DC',
    '4',
    'OH Secondary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Type - DC',
    '5',
    'UG Secondary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Type - DC',
    '6',
    'OH Service'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Construction Type - DC',
    '7',
    'UG Service'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'P26',
    'Alameda - 26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XB',
    'Angels Camp - 95'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JC',
    'Antioch - 30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'HB',
    'Auburn - 20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TC',
    'Bakersfield - 66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JR',
    'Berkeley - 28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FF',
    'Burney - 87'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FB',
    'Chico - 16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'P72',
    'Chowchilla - 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TF',
    'Coalinga - 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'F20',
    'Colfax - 20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'PX',
    'Colusa - 9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JG',
    'Concord - 30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TG',
    'Corcoran - 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'VV',
    'Cupertino - 80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'RB',
    'Daly City - 56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'PY',
    'Davis - 50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TJ',
    'Dinuba - 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'F9',
    'Dixon - 9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JL',
    'East Oakland - 26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'LF',
    'Eureka - 36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'J48',
    'Fairfield - 48'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NF',
    'Fort Bragg - 46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'LJ',
    'Fortuna - 35'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JJ',
    'Fremont - 24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TK',
    'Fresno - 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'LM',
    'Garberville - 34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NC',
    'Geyserville - 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'VF',
    'Gilroy - 78'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'HM',
    'Grass Valley - 22'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NG',
    'Guerneville - 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'RH',
    'Half Moon Bay - 58'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JK',
    'Hayward - 24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BG',
    'Hollister - 8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XD',
    'Jackson - 95'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BJ',
    'King City - 4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NJ',
    'Lakeport - 46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TL',
    'Lemoore - 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'HD',
    'Lincoln - 20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JN',
    'Livermore - 24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'F93',
    'Lodi - 93'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TM',
    'Los Banos - 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'VJ',
    'Los Gatos - 80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TN',
    'Madera - 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XH',
    'Manteca - 93'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TS',
    'Mariposa - 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'HP',
    'Marysville - 11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TR',
    'Merced - 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BM',
    'Monterey - 7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NL',
    'Napa - 42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XL',
    'Newman - 91'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XN',
    'Oakdale - 91'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TQ',
    'Oakhurst - 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JQ',
    'Oakland - 26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FJ',
    'Orland - 14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FQ',
    'Oroville - 13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'P56',
    'Pacifica - 56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FM',
    'Paradise - 17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BR',
    'Paso Robles - 3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'RN',
    'Pen. Dist. Hq. - 58'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NN',
    'Petaluma - 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'HT',
    'Placerville - 21'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'P44',
    'Point Arena - 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FP',
    'Quincy - 18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FN',
    'Red Bluff - 85'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'FY',
    'Redding - 87'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'JT',
    'Richmond - 28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TD',
    'Ridgecrest - 66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'P48',
    'Rio Vista - 48'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'HS',
    'Roseville - 20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BT',
    'Salinas - 5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'D56',
    'San Bruno - 56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'RG',
    'San Francisco - 60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'VP',
    'San Jose - 78'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BV',
    'San Luis Obispo - 2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NQ',
    'San Rafael - 40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'VT',
    'Santa Cruz - 76'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'BX',
    'Santa Maria - 1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NR',
    'Santa Rosa - 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TT',
    'Selma - 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'W42',
    'Silverado - 42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NV',
    'Sonoma - 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XR',
    'Sonora - 91'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'J60',
    'South San Francisco - 60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XT',
    'Stockton - 93'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TV',
    'Taft - 66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XV',
    'Tracy - 93'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XX',
    'Turlock - 91'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NX',
    'Ukiah - 46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'PK',
    'Vacaville - 48'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'NY',
    'Vallejo - 42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'W30',
    'Walnut Creek - 30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'TX',
    'Wasco - 66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'X76',
    'Watsonville - 76'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'PS',
    'West Sacramento - 50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'V9',
    'Williams - 9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'Z46',
    'Willits - 46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'LW',
    'Willow Creek - 38'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'X50',
    'Winters - 50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'Z50',
    'Woodland - 50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'RZ',
    'Unknown -60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    'XJ',
    'Unknown -91'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '1ZZ',
    'unspecified dist 1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '11ZZ',
    'unspecified dist 11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '13ZZ',
    'unspecified dist 13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '14ZZ',
    'unspecified dist 14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '16ZZ',
    'unspecified dist 16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '17ZZ',
    'unspecified dist 17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '18ZZ',
    'unspecified dist 18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '2ZZ',
    'unspecified dist 2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '20ZZ',
    'unspecified dist 20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '21ZZ',
    'unspecified dist 21'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '22ZZ',
    'unspecified dist 22'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '24ZZ',
    'unspecified dist 24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '26ZZ',
    'unspecified dist 26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '28ZZ',
    'unspecified dist 28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '3ZZ',
    'unspecified dist 3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '30ZZ',
    'unspecified dist 30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '34ZZ',
    'unspecified dist 34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '35ZZ',
    'unspecified dist 35'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '36ZZ',
    'unspecified dist 36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '38ZZ',
    'unspecified dist 38'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '4ZZ',
    'unspecified dist 4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '40ZZ',
    'unspecified dist 40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '42ZZ',
    'unspecified dist 42'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '44ZZ',
    'unspecified dist 44'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '46ZZ',
    'unspecified dist 46'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '48ZZ',
    'unspecified dist 48'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '5ZZ',
    'unspecified dist 5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '50ZZ',
    'unspecified dist 50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '56ZZ',
    'unspecified dist 56'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '58ZZ',
    'unspecified dist 58'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '60ZZ',
    'unspecified dist 60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '66ZZ',
    'unspecified dist 66'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '7ZZ',
    'unspecified dist 7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '70ZZ',
    'unspecified dist 70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '72ZZ',
    'unspecified dist 72'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '76ZZ',
    'unspecified dist 76'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '78ZZ',
    'unspecified dist 78'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '8ZZ',
    'unspecified dist 8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '80ZZ',
    'unspecified dist 80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '85ZZ',
    'unspecified dist 85'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '87ZZ',
    'unspecified dist 87'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '88ZZ',
    'unspecified dist 88'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '9ZZ',
    'unspecified dist 9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '91ZZ',
    'unspecified dist 91'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '93ZZ',
    'unspecified dist 93'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Local Offices',
    '95ZZ',
    'unspecified dist 95'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Fill Type',
    'OIL',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Fill Type',
    'VAC',
    'Vacumm'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Soil Code',
    'COR',
    'Corrosive'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Soil Code',
    'DRY',
    'Dry'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Soil Code',
    'WET',
    'Wet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Soil Code',
    'XWT',
    'Extremely Wet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Soil Code',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Soil Code',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'PE',
    'PE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'PVC',
    'PVC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'WP',
    'Weatherproof'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'XLP',
    'Cable - Cross linked polyethylene 
insulated'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Insulation - OH',
    'TW',
    'HM-HDPE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Ownership',
    'CUST',
    'Customer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Ownership',
    'PGE',
    'Company'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '1',
    'Subsurface Transformer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '2',
    'Subsurface Single-Phase Banked for Open 
Delta/Delta WYE Operations'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '3',
    'Subsurface Duplex Transformers'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '4',
    'Subsurface Transformer with Internal 
Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '5',
    'Subsurface Transformer with Two Internal 
Switches'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '6',
    'Subsurface Duplex Transformer with 
Internal Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '7',
    'Subsurface Fused Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '8',
    'Subsurface Fused Switch Dual Well'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '9',
    'Subsurface Cutouts'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '10',
    'Subsurface Switch, Two Way, One Way 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '11',
    'Subsurface Switch, Two way, One Way 
Switched (Supervisory Controlled)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '12',
    'Subsurface Switch, Three way, Two Ways 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '13',
    'Subsurface Switch, Three Way, Three Ways 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '14',
    'Subsurface Switch, Three Way, Three Ways 
Switched (Two Switches Supervisory Controlled Only)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '15',
    'Subsurface Smart Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '16',
    'Subsurface Switch-Interrupter-Switch, 3
-Way, 3-Ways Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '17',
    'Subsurface Interrupter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '18',
    'Subsurface Interrupter (Supervisory 
Controlled)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '19',
    'Subsurface Sectionalizer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '20',
    'Vault Mounted Switch - 2-Way, 1-Way 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '21',
    'Vault Mounted Switch - 3-Way, 2-Way 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '22',
    'Vault Mounted Switch - 3-Way, 3-Way 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '23',
    'Vault Mounted Interrupter - 2-Way, 1-Way 
Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '24',
    'Vault Mounted Switch-Interrupter-Switch, 
3-Way, 3-Way Switched'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '25',
    'Subway Transformer in MH'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '26',
    'TGRAL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '27',
    'TGRAM (Three Way)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '28',
    'TGRAM (Four Way)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '29',
    'JBox - Subsurface'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '31',
    'Misc #2C - 24'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '32',
    'Misc #12C - 2 - 16 X 16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '33',
    'Network'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '30',
    '4kV Junction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '34',
    'TGRAM (Two Way)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Subsurface',
    '35',
    'Subsurface Triplex Transformer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Encasement Type',
    'BF',
    'Back Fill'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Encasement Type',
    'CCRT',
    'Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Encasement Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Encasement Type',
    'NA',
    'NA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Recloser CT Ratio',
    '5',
    '500:1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Recloser CT Ratio',
    '10',
    '1000:1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Recloser CT Ratio',
    '20',
    '2000:1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '12-15 FT',
    '12-15 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '16-19 FT',
    '16-19 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '20-24 FT',
    '20-24 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '25-29 FT',
    '25-29 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '30-34 FT',
    '30-34 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '35-39 FT',
    '35-39 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    '40 FT',
    '40 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    'OTHER',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Streetlight Pole Length',
    'OVER 40 FT',
    'Over 40 feet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Wind Speed',
    '2',
    '2 ft/sec'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Wind Speed',
    '3',
    '3 ft/sec'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Wind Speed',
    '4',
    '4 ft/sec'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '0',
    '0A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '5',
    '5A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '10',
    '10A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '15',
    '15A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '20',
    '20A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '25',
    '25A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '30',
    '30A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '33',
    '33A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '35',
    '35A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '50',
    '50A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '60',
    '60A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '70',
    '70A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '100',
    '100A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '125',
    '125A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '150',
    '150A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '170',
    '170A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '200',
    '200A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '220',
    '220A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '225',
    '225A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '250',
    '250A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '300',
    '300A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '400',
    '400A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '500',
    '500A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '560',
    '560A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '600',
    '600A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '720',
    '720A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '800',
    '800A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '900',
    '900A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '1200',
    '1200A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '1600',
    '1600A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '3000',
    '3000A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps',
    '4000',
    '4000A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps - DB Elbow',
    '200',
    '200 Amp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps - DB Elbow',
    '600',
    '600 Amp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Three Phase',
    '7',
    'ABC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'ALS',
    'ALS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'GE',
    'GE Harris'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'HW',
    'Hathaway'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'ITR',
    'ITRON'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'TI',
    'TI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'VAL',
    'Valmat'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RTU Type',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '5800',
    '5800L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '7500',
    '7500L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '9500',
    '9500L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '11000',
    '11000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '16000',
    '16000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '21000',
    '21000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '22000',
    '22000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '25500',
    '25500L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '37000',
    '37000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '46000',
    '46000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '2000',
    '2000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '4000',
    '4000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '6000',
    '6000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Lumens',
    '10000',
    '10000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AI',
    'ABB Instrumentation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AC',
    'Allis Chalmers'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AX',
    'American Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AM',
    'American/Singer/Pacific Mtr Wk'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AT',
    'AMETEK'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AP',
    'App. Tech'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AR',
    'Arkla'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AE',
    'Associated Engineering'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'BS',
    'Balteau Standard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'CN',
    'Cellnet (Formerly Dac)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'CO',
    'Connersville'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'DA',
    'Daniel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'DS',
    'Datastar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'DC',
    'DCSI'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'EM',
    'Electro Magnetic '
    || chr(38)
    ||' 
Square "D"'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'EL',
    'Elster'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'AB',
    'Elster/ABB/Westinghouse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'FP',
    'Fisher Price'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'GA',
    'Galvanic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'GD',
    'Gec Durham (Formerly Astra)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'GE',
    'General Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'HX',
    'Hexagram'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'HP',
    'Holophane'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'IN',
    'Instromet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'IT',
    'ITT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'JS',
    'Joslyn'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'KM',
    'Kimmon/Westinghouse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'LG',
    'Landis+Gyr/Siemens/Duncan'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'ME',
    'McGraw Edison'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'MR',
    'Mercury'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'MT',
    'Metretek'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'MM',
    'Metric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'MC',
    'Metricon'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'NN',
    'Nansen'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'NE',
    'Nertec'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'NS',
    'Non-Standard Streetlight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'NA',
    'Not Applicable Streetlight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'OB',
    'Obsolete Streetlight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'OT',
    'Other Streetlight/Pole'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'PR',
    'Peco-Robinson'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'PS',
    'Process Systems, Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'RB',
    'Robinton'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'RO',
    'Rockwell/Eqimtr/Invnsys/Sensus'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'RM',
    'Romet'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'DR',
    'Roots/Dresser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'SS',
    'Schlumberger/Sangamo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'SC',
    'Scientific Columbus'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'SP',
    'Sprague/Schlumberger'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'ST',
    'Stb Co'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'SU',
    'Superior'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'TE',
    'Tel-Data'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'TO',
    'Totalflow'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'TD',
    'Trans Data, Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'TT',
    'Transtel Group'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'U',
    'Unknown Manufacturer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'UK',
    'Unknown Streetlight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'WA',
    'Unknown Water Meter MFG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Fixture Manufacturer',
    'WH',
    'Westinghouse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '0',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '-400',
    '400X A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '-560',
    '560X A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '-1',
    '.5 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '5',
    '5 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '15',
    '15 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '16',
    '16 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '18',
    '18 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '20',
    '20 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '24',
    '24 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '25',
    '25 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '30',
    '30 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '35',
    '35 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '40',
    '40 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '50',
    '50 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '56',
    '56 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '60',
    '60 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '65',
    '65 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '70',
    '70 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '75',
    '75 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '80',
    '80 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '90',
    '90 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '100',
    '100 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '112',
    '112 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '125',
    '125 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '140',
    '140 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '150',
    '150 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '160',
    '160 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '175',
    '175 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '185',
    '185 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '200',
    '200 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '224',
    '224 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '225',
    '225 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '250',
    '250 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '256',
    '256 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '280',
    '280 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '296',
    '296 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '300',
    '300 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '320',
    '320 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '400',
    '400 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '448',
    '448 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '480',
    '480 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '560',
    '560 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '600',
    '600 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '640',
    '640 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '1',
    '1 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '2',
    '2 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '3',
    '3 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '4',
    '4 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '6',
    '6 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '7',
    '7 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '8',
    '8 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '10',
    '10 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'SUB Fuse Link Amps',
    '12',
    '12 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Auto Manual',
    'A',
    'Automatic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Auto Manual',
    'M',
    'Manual'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Auto Manual',
    'NA',
    'NA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'ABS',
    'ABS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'FIB',
    'Fiber'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'PVC',
    'PVC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'STL',
    'Steel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'WOD',
    'Wood'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'FLX',
    'Flex'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'TR',
    'Transite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'T',
    'Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'C',
    'Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'VC',
    'Vitrified Clay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'WI',
    'Wrought Iron'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'BI',
    'Bituminous'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ULS Material - Duct Bank',
    'SS',
    'Soapstones'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Framing Type',
    '1',
    'Horizontal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Framing Type',
    '2',
    'Vertical'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Framing Type',
    '3',
    'Triangular'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Framing Type',
    '4',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Joint Owner Space Type',
    'COM',
    'Common'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Joint Owner Space Type',
    'POWER',
    'Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Joint Owner Space Type',
    'NEUT',
    'Neutral'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Joint Owner Space Type',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Species',
    'DF',
    'Douglas Fir'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Species',
    'LP',
    'Lodgepole Pine'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Species',
    'WP',
    'Western (Ponderosa) Pine'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Species',
    'WC',
    'Western Red Cedar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Species',
    'OT',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Species',
    'NA',
    'Not Available'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RecloserControlType',
    '6',
    'Form 6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RecloserControlType',
    '3A',
    '3A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RecloserControlType',
    '4C',
    '4C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RecloserControlType',
    'H',
    'Hydraulic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'RecloserControlType',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '1',
    'N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '2',
    'T'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '3',
    'E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '4',
    'K'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '5',
    'CL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '6',
    'FT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '7',
    'ELF'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '8',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '9',
    'Not on List'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Type',
    '10',
    'Trip Saver'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Attachment Type',
    '1',
    'Tenant - PG'
    || chr(38)
    ||'E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Attachment Type',
    '2',
    'Tenant - OU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Attachment Type',
    '3',
    'Joint Owner'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'ADVE',
    'Advanced Energy, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'ALPH',
    'Alpha Technologies'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'BEAC',
    'Beacon'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'BERG',
    'Bergey Windpower Co.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'BPSY',
    'Ballard Power Systems'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'CEDI',
    'CFM Equipment Distributors Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'CNRE',
    'Open Energy Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'DTEC',
    'Diversified Technology'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'ENPE',
    'Enphase Energy'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'FRON',
    'Fronius'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'GENE',
    'General Electric'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'IPOW',
    'I-Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'JCWD',
    'Jacob''s Wind Turbine Inc'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'KACO',
    'Kaco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'MARI',
    'Mariah Power'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'MGNT',
    'Power-One'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'MOTI',
    'Motech Industries, Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'MSTR',
    'Mastermind'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'NGTC',
    'Ningbo Ginlong Technologies'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'OMNI',
    'Omnion'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'OTPS',
    'Outback Power Systems'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'PVPL',
    'PV Powered LLC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SCHU',
    'Schuco USA LP'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SDEG',
    'SolarEdge Technologies'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SHRP',
    'Sharp Corporation'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SMAA',
    'SMA America'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SORE',
    'Solectria Renewables, LLC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SPSC',
    'SatCon Power Systems Canada'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'SUNP',
    'SunPower Corp.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'VANR',
    'Vanner Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'XENT',
    'Xslent Energy Technologies'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'XNTX',
    'Xantrex Technology Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'YESI',
    'Yes! Solar Inc.'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Inverter Manufacturer',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Urban Rural Code',
    'U',
    'Urban'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Urban Rural Code',
    'R',
    'Rural'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trace Status',
    '1',
    'Non-Traceable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trace Status',
    '0',
    'Traceable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'AAC',
    'AAC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'A',
    'AL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'C',
    'CU'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'CUWD',
    'Copper Weld'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'HDCU',
    'Hard Drawn Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'SDCU',
    'Soft Drawn Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'STCU',
    'Stranded Copper'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'STL',
    'Steel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'BusDuct',
    'BusDuct'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'CL',
    'Copper covered Lead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'L',
    'Lead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'PL',
    'Paper covered Lead'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Material - UG',
    'AR',
    'ACSR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Bank Switch Type',
    'OIL',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Bank Switch Type',
    'VAC',
    'Vacuum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '1',
    'PMH 3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '2',
    'PMH 4 w/1 fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '3',
    'PMH 4 w/2 fuses'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '4',
    'PMH 4 w/3 fuses'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '5',
    'PMH 5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '6',
    'PMH 6 w/(1) Hot Leg'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '7',
    'PMH 9 w/(2) Hot Legs'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '8',
    'PMH 11 w/(1) Hot Leg'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '9',
    'PMH 11 w/(3) single phase Leg'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '10',
    'PMH 6 w/(3) 1-phase Leg'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '11',
    'PMH 9 w/(6)1-phase Leg'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '12',
    'PMH 9 w/ (3) 1-phase '
    || chr(38)
    ||' (1) 
3-phase Leg'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '13',
    'PMH 41,42,43 Single Bypass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '14',
    'PMH 42 Double Bypass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '15',
    'PMH 43 Triple Bypass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '16',
    'Padmounted Transformer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '17',
    'Padmounted Transformer with Load Break 
Elbows'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '18',
    'Padmounted Duplex Transformer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '19',
    'Padmounted Transformer with Internal 
Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '20',
    'Padmounted Transformer w/ Two Internal 
Switches'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '21',
    'Padmounted Auto Transformer 20780 GRD 
Y/21,00-12,000 GRD Y/6900'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '22',
    'Padmounted Transformer Auto-Bank with Two 
Reclosers'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '23',
    'Padmounted 3-phase, 12KV or 21KV to 4KV 
Transformer and Recloser'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '24',
    'Padmounted Capacitor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '25',
    'Padmounted Capacitor (Supervisory 
Controlled)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '26',
    'Padmounted Regulator'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '27',
    'Padmounted Regulator Bypass Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '28',
    'Padmounted Oil Switch'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '29',
    'Padmounted Oil Switch (Supervisory 
Controlled)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '30',
    'Padmounted Interrupter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '31',
    'Padmounted Interrupter (Supervisory 
Controlled)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '32',
    'Padmounted Transformer with Internal 
Interrupter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '33',
    'Padmounted Transformer with Internal 
Interrupter and Two Switches'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '34',
    'Padmounted Automatic Circuit Recloser 
(Supervisory Controlled)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '35',
    'Padmounted Sectionalizer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '36',
    'JBox - Padmount'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '37',
    'Misc #1 - 16'' X 16'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '38',
    'Misc #2 - 16'' X 32'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '39',
    'Misc #3 - 16'' X 40'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '40',
    'Misc #4 - 16'' X 24'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '41',
    'Misc #5 - 24'' X 32'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '42',
    'Misc #6 - 24'' X 40'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '43',
    'Misc #7 - 24'' X 60'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '44',
    'Misc #8 - 24'' X 50'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '45',
    'Misc #9 - 24'' X 80'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '46',
    'Misc #10 - 24'' X 56'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Device Group Type - Padmount',
    '47',
    'Misc #11 - 24'' X 24'''
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  ( 'Device Group Type - Padmount','48','Padmounted Triplex Transformer');
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'BION',
    'Bio-mass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'GSBP',
    'Gas By-Product'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'HYDR',
    'Hydro'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'NTLG',
    'Natural Gas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'OTHR',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'SOLR',
    'Solar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'SWMP',
    'Swamp Gas'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'WAVE',
    'Wave'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Power Source',
    'WIND',
    'Wind'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Status',
    'OFF',
    'offline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Status',
    'ON',
    'online'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Status',
    'UNSP',
    'unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Photocell Status',
    'CHGCELL',
    'Changed PCell - Yes'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Photocell Status',
    'NO',
    'Photocell Not Present - No'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Photocell Status',
    'YES',
    'Photcell Present - Yes'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '4',
    'A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '2',
    'B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '1',
    'C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '6',
    'AB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '5',
    'AC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '3',
    'BC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '7',
    'ABC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '10',
    'CN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '8',
    'N'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '9',
    'PN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '11',
    'RCN'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Phase Designation - Conductor Info',
    '12',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Structure Cover Type',
    '1',
    'Aluminum - Incidental'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Structure Cover Type',
    '2',
    'Concrete - Heavy Full Traffic'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Structure Cover Type',
    '3',
    'Quick Release - Incidental'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Structure Cover Type',
    '4',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Structure Cover Type',
    '5',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Type',
    'OIL',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Type',
    'VAC',
    'Vacuum'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Type',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Type',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Switch Type',
    'NA',
    'NA'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '9',
    '9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '10',
    '10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '12',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '13',
    '13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '14',
    '14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '15',
    '15'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '16',
    '16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '18',
    '18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '20',
    '20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '25',
    '25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '30',
    '30'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '32',
    '32'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '34',
    '34'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '35',
    '35'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '36',
    '36'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '38',
    '38'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '40',
    '40'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '45',
    '45'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '50',
    '50'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '55',
    '55'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '60',
    '60'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '65',
    '65'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '70',
    '70'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '75',
    '75'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '80',
    '80'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '85',
    '85'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '90',
    '90'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '95',
    '95'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '100',
    '100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '105',
    '105'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '110',
    '110'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '115',
    '115'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '120',
    '120'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '125',
    '125'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '130',
    '130'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '135',
    '135'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '140',
    '140'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '145',
    '145'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '150',
    '150'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '0',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '185',
    '185'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '200',
    '200'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Height',
    '222',
    '222'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'BOL',
    'Bollard'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'COB',
    'Cobra'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'LTN',
    'Lantern'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'ORL',
    'Ornamental light'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'PDT',
    'Pendant'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'SHB',
    'Shoe box'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'TD',
    'Tear drop'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Style',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Rated Amps - LB Elbow',
    '200',
    '200 Amp'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '7',
    'Current Limiting Fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '8',
    'Current Limiting Fuse w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '11',
    'Energy Limiting Fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '12',
    'Energy Limiting Fuse w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '9',
    'Fault Tamer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '10',
    'Fault Tamer w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '2',
    'Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '5',
    'Liquid Fuse'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '6',
    'Liquid Fuse w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '3',
    'Self Protected'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '4',
    'Self Protector w/Lightning Arrestor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'High Side Protection',
    '1',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Two Phase',
    '6',
    'AB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Two Phase',
    '5',
    'AC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Two Phase',
    '3',
    'BC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1',
    '1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '2',
    '2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '3',
    '3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '4',
    '4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '6',
    '6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '8',
    '8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '250',
    '250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '266',
    '266'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '267',
    '267'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '300',
    '300'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '336',
    '336'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '350',
    '350'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '366',
    '366'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '397',
    '397'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '400',
    '400'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '500',
    '500'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '600',
    '600'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '700',
    '700'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '715',
    '715'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '750',
    '750'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '795',
    '795'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '900',
    '900'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '954',
    '954'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1000',
    '1000'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1100',
    '1100'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1113',
    '1113'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1250',
    '1250'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1500',
    '1500'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1750',
    '1750'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1/0',
    '1/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '2/0',
    '2/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '3/0',
    '3/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '4/0',
    '4/0'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '2a',
    '2a'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '4a',
    '4a'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '6a',
    '6a'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '8a',
    '8a'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '1f',
    '1f'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    'NONE',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    'BusDuct',
    'BusDuct'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '10',
    '10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '12',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '477',
    '477'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '14',
    '14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conductor Size',
    '2000',
    '2000'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH',
    'Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,BRIC',
    'Asphalt and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,BRIC,CONC',
    'Asphalt, Brick or Tile, and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,BRIC,DIRT',
    'Asphalt, Brick or Tile, and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,CONC',
    'Asphalt, and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,CONC,DIRT',
    'Asphalt, Concrete, and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,CONC,GRAS',
    'Asphalt, Concrete, and Grass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,DIRT',
    'Asphalt and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,DIRT,BRIC',
    'Asphalt, Dirt, and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,DIRT,GRVL',
    'Asphalt, Grass, and Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,GRAS',
    'Asphalt and Grass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'ASPH,GRVL',
    'Asphalt and Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'BRIC',
    'Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'BRIC,ASPH',
    'Brick or Tile and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'BRIC,GRVL',
    'Brick or Tile and Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC',
    'Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,ASPH',
    'Concrete and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,ASPH,DIRT',
    'Concrete, Asphalt, and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC, BRIC',
    'Concrete and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC, BRIC, DIRT',
    'Concrete, Brick or TIle, and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,DIRT',
    'Concrete and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,DIRT,GRAS',
    'Concrete, Dirt, and Grass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,DIRT,GRVL',
    'Concrete, Dirt, and Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,GRAS',
    'Concrete and Grass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'CONC,GRVL',
    'Concrete adn Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT',
    'Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,ASPH',
    'Dirt and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT, ASPH,CONC',
    'Dirt, Asphalt, and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,BRIC',
    'Dirt and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT, BRIC, ASPH',
    'Dirt, Brick or Tile, and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,CONC',
    'Dirt and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,CONC,ASPH',
    'Dirt, Concrete, and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,CONC,BRIC',
    'Dirt, Concrete, and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT, GRAS',
    'Dirt and Grass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT, GRAS, GRVL',
    'Dirt, Grass, and Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,GRVL',
    'Dirt and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'DIRT,GRVL,ASPH',
    'Dirt, Gravel, and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS',
    'Grass'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,ASPH',
    'Grass and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,BRIC',
    'Grass and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,CONC',
    'Grass and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,DIRT',
    'Grass and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,DIRT,BRIC',
    'Grass, Dirt, and Brick or Tile'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,DIRT,CONC',
    'Grass, Dirt, and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRAS,DIRT,GRVL',
    'Grass, Dirt, and Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL',
    'Gravel'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL,ASPH',
    'Gravel and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL,CONC',
    'Gravel and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL,DIRT',
    'Gravel and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL,DIRT,ASPH',
    'Gravel, Dirt, and Asphalt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL,DIRT,CONC',
    'Gravel, Dirt, and Concrete'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'GRVL,GRAS,DIRT',
    'Gravel, Grass, and Dirt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Ground Condition',
    'NONE',
    'NONE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'ECLSD',
    'Default Rate - Elec - Retro Close 
ECLSD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'ECONV',
    'Default Rate - Elec - Close Acct ECONV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-A',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-B',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-C',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-D',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-D'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-E',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-E'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-F',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-F'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS1-F1',
    'PG'
    || chr(38)
    ||'E Owned St '
    || chr(38)
    ||' Hwy Light - LS1-F1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS2-A',
    'Cust Owned St '
    || chr(38)
    ||' Hwy Light - 
LS2-A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS2-B',
    'Cust Owned St '
    || chr(38)
    ||' Hwy Light - 
LS2-B'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS2-C',
    'Cust Owned St '
    || chr(38)
    ||' Hwy Light - 
LS2-C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'LS3',
    'Cust Owned Light - EM Rate - LS3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    'OL1',
    'Outdoor Lighting - OL1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '1',
    'S1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '2',
    'S2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '3',
    'S3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '4',
    'S4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '5',
    'S5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '6',
    'S6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '7',
    'S7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '7A',
    'S7A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '8',
    'S8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '9',
    'S9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '11',
    'S11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '12',
    'S12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Rate Schedule',
    '13',
    'S13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Switching Facility Manufacturer',
    'ABB',
    'ABB'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Switching Facility Manufacturer',
    'G'
    || chr(38)
    ||'W',
    'G '
    || chr(38)
    ||' W'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Switching Facility Manufacturer',
    'S'
    || chr(38)
    ||'C',
    'S and C'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Switching Facility Manufacturer',
    'SHNDR',
    'Shneider'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Switching Facility Manufacturer',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J01',
    'ALAMEDA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J04',
    'ALBANY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X20',
    'AMADOR CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N02',
    'AMERICAN CANYON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F47',
    'ANDERSON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X22',
    'ANGELS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J07',
    'ANTIOCH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L10',
    'ARCATA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H65',
    'YUBA CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T04',
    'ARVIN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B11',
    'ATASCADERO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R03',
    'ATHERTON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T05',
    'ATWATER INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H44',
    'AUBURN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T06',
    'AVENAL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y60',
    'BACKBONE TRANS. UEG'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T07',
    'BAKERSFIELD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T08',
    'BARSTOW INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R72',
    'BB TRANSMISSION'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R74',
    'BB TRANSMISSION ? OFF SYSTEM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R05',
    'BELMONT INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N01',
    'BELVEDERE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N04',
    'BENICIA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J10',
    'BERKELEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F49',
    'BIGGS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L20',
    'BLUE LAKE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J11',
    'BRENTWOOD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R09',
    'BRISBANE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B14',
    'BUELLTON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R07',
    'BURLINGAME INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N07',
    'CALISTOGA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V09',
    'CAMPBELL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V11',
    'CAPITOLA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B13',
    'CARMEL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X26',
    'CERES INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F51',
    'CHICO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T10',
    'CHOWCHILLA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P50',
    'CITRUS HEIGHTS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J12',
    'CLAYTON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N08',
    'CLEARLAKE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N10',
    'CLOVERDALE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T12',
    'CLOVIS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T14',
    'COALINGA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H47',
    'COLFAX INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R10',
    'COLMA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P02',
    'COLUSA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J13',
    'CONCORD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y15',
    'CONTRA COSTA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T16',
    'CORCORAN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y62',
    'CORE PROCUREMENT'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F50',
    'CORNING INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N13',
    'CORTE MADERA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N14',
    'COTATI INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V13',
    'CUPERTINO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R90',
    'DA NON ENERGY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R20',
    'DALY CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J14',
    'DANVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P01',
    'DAVIS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B16',
    'DEL REY OAKS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T18',
    'DINUBA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P04',
    'DIXON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T20',
    'DOS PALOS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J15',
    'DUBLIN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R21',
    'EAST PALO ALTO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J16',
    'EL CERRITO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P90',
    'ELK GROVE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J19',
    'EMERYVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X28',
    'ESCALON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L30',
    'EUREKA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N16',
    'FAIRFAX INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P07',
    'FAIRFIELD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L40',
    'FERNDALE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T22',
    'FIREBAUGH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P10',
    'FOLSOM INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N19',
    'FORT BRAGG INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L50',
    'FORTUNA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R23',
    'FOSTER CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T24',
    'FOWLER INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J22',
    'FREMONT INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T26',
    'FRESNO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P13',
    'GALT INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V15',
    'GILROY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B19',
    'GONZALES INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H50',
    'GRASS VALLEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B22',
    'GREENFIELD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F52',
    'GRIDELY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B20',
    'GROVER BEACH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R71',
    'GSTOR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R73',
    'GSTOR OFF ? SYSTEM'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B25',
    'GUADALUPE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X30',
    'GUSTINE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R27',
    'HALF MOON BAY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T29',
    'HANFORD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J25',
    'HAYWARD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N22',
    'HEALDSBURG INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J28',
    'HERCULES INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R30',
    'HETCH HETCHY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T27',
    'HETCH HETCHY'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J31',
    'HETCH HETCHY POWER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V17',
    'HETCH HETCHY POWER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X32',
    'HETCH HETCHY POWER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R36',
    'HILLSBOROUGH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B28',
    'HOLLISTER INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y35',
    'HUMBOLDT CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X33',
    'HUGHSON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T28',
    'HURON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X34',
    'IONE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P16',
    'ISLETON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X36',
    'JACKSON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T30',
    'KERMAN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B31',
    'KING CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T32',
    'KINGSBURG INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J32',
    'LAFAYETTE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N25',
    'LAKEPORT INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N28',
    'LARKSPUR INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X37',
    'LATHROP INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T34',
    'LEMOORE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H48',
    'LINCOLN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H49',
    'LIVE OAK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J34',
    'LIVERMORE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T36',
    'LIVINGSTON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X38',
    'LODI INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B34',
    'LOMPOC INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H51',
    'LOOMIS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V23',
    'LOS ALTOS HILLS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V21',
    'LOS ALTOS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T38',
    'LOS BANOS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V25',
    'LOS GATOS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T40',
    'MADERA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X40',
    'MANTECA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T42',
    'MARICOPA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B35',
    'MARINA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J37',
    'MARTINEZ INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H52',
    'MARYSVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T44',
    'MCFARLAND INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T46',
    'MENDOTA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R39',
    'MENLO PARK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T48',
    'MERCED INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N34',
    'MILL VALLEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R40',
    'MILLBRAE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V29',
    'MILPITAS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X42',
    'MODESTO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V31',
    'MONTE SERENO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y30',
    'MONTEREY CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B37',
    'MONTEREY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J38',
    'MORAGA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V33',
    'MORGAN HILL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B38',
    'MORRO BAY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V35',
    'MOUTAIN VIEW INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N37',
    'NAPA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H53',
    'NEVADA CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J40',
    'NEWARK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X44',
    'NEWMAN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N39',
    'NOVATO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X46',
    'OAKDALE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J43',
    'OAKLAND INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J42',
    'OAKLEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T50',
    'ORANGE COVE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J44',
    'ORINDA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F53',
    'OROVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B40',
    'PACIFIC GROVE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R45',
    'PACIFICA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R44',
    'PALO ALTO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V37',
    'PALO ALTO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F55',
    'PARADISE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T52',
    'PARLIER INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B43',
    'PASO ROBLES INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X48',
    'PATTERSON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R70',
    'PEP SHIPPER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N40',
    'PETALUMA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J46',
    'PIEDMONT INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J49',
    'PINOLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B46',
    'PISMO BEACH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J52',
    'PITTSBURG INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H21',
    'PLACER CO WATER'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P21',
    'PLACERVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H54',
    'PLACERVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J53',
    'PLEASANT HILL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J55',
    'PLEASANTON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X49',
    'PLYMOUTH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N43',
    'POINT ARENA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R46',
    'PORTOLA VALLEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P55',
    'RANCHO CORDOVA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F58',
    'RED BLUFF INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F56',
    'REDDING INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R48',
    'REDWOOD CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T54',
    'REEDLEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J58',
    'RICHMOND INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T55',
    'RIDGECREST INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L55',
    'RIO DELL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P24',
    'RIO VISTA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X51',
    'RIPON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X52',
    'RIVERBANK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H56',
    'ROCKLIN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N44',
    'ROHNERT PARK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H59',
    'ROSEVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N46',
    'ROSS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P27',
    'SACRAMENTO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N47',
    'SAINT HELENA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B49',
    'SALINAS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N48',
    'SAN ANSELMO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y47',
    'SAN BERNARDINO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R50',
    'SAN BRUNO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R52',
    'SAN CARLOS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y20',
    'SAN FRANCISCO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R60',
    'SAN FRANCISCO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T58',
    'SAN JOAQUIN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V43',
    'SAN JOSE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B52',
    'SAN JUAN BAUTISTA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J61',
    'SAN LEANDRO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y50',
    'SAN LUIS OBISPO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B55',
    'SAN LUIS OBISPO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R62',
    'SAN MATEO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J64',
    'SAN PABLO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N50',
    'SAN RAFAEL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J59',
    'SAN RAMON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B50',
    'SAND CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T56',
    'SANGER INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'Y53',
    'SANTA CLARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V47',
    'SANTA CLARA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V49',
    'SANTA CRUZ INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B57',
    'SANTA MARIA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N51',
    'SANTA ROSA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V51',
    'SARATOGA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N54',
    'SAUSALITO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V52',
    'SCOTTS VALLEY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B59',
    'SEASIDE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N56',
    'SEBASTOLPOL INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T60',
    'SELMA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T62',
    'SHAFTER INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F60',
    'SHASTA LAKE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R65',
    'SO SAN FRANCISCO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B62',
    'SOLEDAD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B63',
    'SOLVANG INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N59',
    'SONOMA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X54',
    'SONORA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X55',
    'STOCKTON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P30',
    'SUISUN INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V53',
    'SUNNYVALE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X57',
    'SUTTER CREEK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T64',
    'TAFT INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F59',
    'TEHAMA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N62',
    'TIBURON INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X59',
    'TRACY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L60',
    'TRINIDAD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X60',
    'TURLOCK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N63',
    'UKIAH INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P68',
    'UNIC BUTTE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L72',
    'UNIN MENDOCINO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J78',
    'UNINC ALAMEDA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V77',
    'UNINC ALAMEDA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X71',
    'UNINC ALAMEDA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X72',
    'UNINC ALPINE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P72',
    'UNINC AMADOR CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X73',
    'UNINC AMADOR CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F70',
    'UNINC BUTTE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X74',
    'UNINC CALAVERAS CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F72',
    'UNINC COLUSA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P70',
    'UNINC COLUSA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J80',
    'UNINC CONTRA COSTA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X76',
    'UNINC CONTRA COSTA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P74',
    'UNINC EL DORADO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H70',
    'UNINC EL DORADO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X78',
    'UNINC EL DORADO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B67',
    'UNINC FRESNO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T70',
    'UNINC FRESNO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F74',
    'UNINC GLENN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P75',
    'UNINC GLENN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L70',
    'UNINC HUMBOLDT CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B70',
    'UNINC KERN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T73',
    'UNINC KERN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T75',
    'UNINC KINGS CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N77',
    'UNINC LAKE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F78',
    'UNINC LASSEN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T76',
    'UNINC MADERA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N79',
    'UNINC MARIN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X80',
    'UNINC MARIPOSA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T77',
    'UNINC MARIPOSA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N81',
    'UNINC MENDOCINO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T79',
    'UNINC MERCED CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X82',
    'UNINC MERCED CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B73',
    'UNINC MONTEREY CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N83',
    'UNINC NAPA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P76',
    'UNINC NAPA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H72',
    'UNINC NEVADA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P88',
    'UNINC PLACER CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H74',
    'UNINC PLACER CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F81',
    'UNINC PLUMAS CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H77',
    'UNINC PLUMAS CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P78',
    'UNINC SACRAMENTO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H76',
    'UNINC SACRAMENTO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X83',
    'UNINC SACRAMENTO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B76',
    'UNINC SAN BENITO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T80',
    'UNINC SAN BERNARDINO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P80',
    'UNINC SAN JOAQUIN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J81',
    'UNINC SAN JOAQUIN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X85',
    'UNINC SAN JOAQUIN CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B79',
    'UNINC SAN LUIS OBISPO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T82',
    'UNINC SAN LUIS OBISPO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R80',
    'UNINC SAN MATEO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B83',
    'UNINC SANTA BARBARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T83',
    'UNINC SANTA BARBARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B85',
    'UNINC SANTA CLARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J82',
    'UNINC SANTA CLARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R81',
    'UNINC SANTA CLARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V85',
    'UNINC SANTA CLARA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B87',
    'UNINC SANTA CRUZ CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F82',
    'UNINC SHASTA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H78',
    'UNINC SIERRA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L73',
    'UNINC SISKIYOU CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F91',
    'UNINC SISKIYOU CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N85',
    'UNINC SOLANO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P82',
    'UNINC SOLANO CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N87',
    'UNINC SONOMA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X87',
    'UNINC STANISLAUS'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T84',
    'UNINC STANISLAUS CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H79',
    'UNINC SUTTER CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F83',
    'UNINC TEHAMA INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'L74',
    'UNINC TRINITY CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F86',
    'UNINC TRINITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T86',
    'UNINC TULARE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T88',
    'UNINC TUOLUMNE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X88',
    'UNINC TUOLUMNE CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P86',
    'UNINC YOLO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F89',
    'UNINC YUBA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H80',
    'UNINC YUBA CO'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J65',
    'UNION CITY INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P33',
    'VACAVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N66',
    'VALLEJO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T65',
    'VICTORVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'J67',
    'WALNUT CREEK INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'T66',
    'WASCO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'X61',
    'WATERFORD INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'V55',
    'WATSONVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P35',
    'WEST SACRAMENTO INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'H63',
    'WHEATLAND INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P36',
    'WILLIAMS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N68',
    'WILLITS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'F57',
    'WILLOWS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N70',
    'WINDSOR INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P37',
    'WINTERS INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'P40',
    'WOODLAND INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'R68',
    'WOODSIDE INCE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'N69',
    'YOUNTVILLE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Town Territory Code',
    'B10',
    'ARROYO GRANDE INC'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'GPS Source',
    'REC',
    'Recreational (2-10 meter)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'GPS Source',
    'RES',
    'Resource GPS (Sub-meter)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'GPS Source',
    'SVY',
    'Survey Grade GPS (cm)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'GPS Source',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'GPS Source',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '17',
    'Central Coast'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '16',
    'De Anza'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '9',
    'Diablo'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '6',
    'East Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '20',
    'Humboldt'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '12',
    'Fresno'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '13',
    'Kern'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '18',
    'Los Padres'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '14',
    'Mission'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '5',
    'North Bay'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '1',
    'Sonoma'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '2',
    'North Valley'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '8',
    'Peninsula'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '3',
    'Sacramento'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '7',
    'San Francisco'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '15',
    'San Jose'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '4',
    'Sierra'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '19',
    'Skyline'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '10',
    'Stockton'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Division Name',
    '11',
    'Yosemite'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '01',
    'Jan'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '02',
    'Feb'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '03',
    'Mar'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '04',
    'Apr'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '05',
    'May'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '06',
    'Jun'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '07',
    'Jul'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '08',
    'Aug'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '09',
    'Sep'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '10',
    'Oct'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '11',
    'Nov'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Months',
    '12',
    'Dec'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '50',
    '50 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '100',
    '100 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '200',
    '200 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '300',
    '300 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '600',
    '600 kVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '900',
    '900 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '1200',
    '1200 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '1800',
    '1800 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Capacitor Unit KVAR',
    '250',
    '250 KVAR'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Cable Test Result - Hot Oil',
    'WATER',
    'Water'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Cable Test Result - Hot Oil',
    'NOWAT',
    'No Water'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Motor Type',
    'S',
    'Synchronous'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Motor Type',
    'I',
    'Induction'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ConductorUse - SecUG',
    '5',
    'Secondary'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ConductorUse - SecUG',
    '6',
    'Service'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'ConductorUse - SecUG',
    '8',
    'Streetlight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '3',
    '3 Overhead Conventional'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '9',
    '9 Pad-Mounted Single Phase Self-Protected'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '10',
    '10 Pad-Mounted Single Phase Conventional'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '11',
    '11 Pad-Mounted Three Phase Conventional'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '12',
    '12 Constant Current Street Light'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '15',
    '15 Pad-Mounted Three Phase Self-Protected'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '32',
    '32 Pad-Mounted Duplex Self-Protected'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '33',
    '33 Pad-Mounted Three Phase in Vault'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '48',
    '48 Pad-Mounted three phase Style I '
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '49',
    '49 Pad-Mounted three phase Style II A (w/ 
secondary breaker)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '50',
    '50 Pad-Mounted three phase Style II B (w/ CL 
fuse)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '51',
    '51 Pad-Mounted three phase Style II C (w/ power 
fuse)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '52',
    '52 Pad-Mounted three phase Style II D (w/ CL 
Fuse)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '53',
    '53 Pad-Mounted three phase Style II E (w/ 
bayonet fuse)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '54',
    '54 Pad-Mounted three phase Style II F (w/ 
bayonet fuse)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '55',
    '55 Pad-Mounted three phase Style II G (w/ 
interrupter)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '56',
    '56 Pad-Mounted three phase Style II H (w/ 
interrupter)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '77',
    '77 Equipment Transformers'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '88',
    '88 Secondary Bus on Network System'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Trans Unit Type Surface',
    '99',
    '99 IDs Primary Customer Location'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '50',
    '50 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '100',
    '100 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '200',
    '200 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '280',
    '280 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '560',
    '560 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '600',
    '600 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '800',
    '800 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '900',
    '900 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '1200',
    '1200 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Max Continuous Current Protect',
    '2000',
    '2000 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Load Break',
    '0',
    'No'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Load Break',
    '1',
    'Yes'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Load Break',
    '9',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '1',
    'Closed Delta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '3',
    'Open Delta'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '4',
    'Open Wye'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '6',
    'Single Phase'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '5',
    'Wye, Neu (Gr)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '2',
    'Wye, No (Gr)'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Volt Reg Bank Connect Config',
    '99',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '1',
    '1 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '2',
    '2 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '3',
    '3 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '4',
    '4 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '5',
    '5 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '6',
    '6 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '7',
    '7 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '8',
    '8 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '9',
    '9 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '10',
    '10 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '12',
    '12 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '14',
    '14 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '15',
    '15 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '17',
    '17 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '18',
    '18 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '20',
    '20 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '24',
    '24 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '25',
    '25 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '29',
    '29 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '30',
    '30 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '35',
    '35 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '36',
    '36 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '40',
    '40 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '41',
    '41 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '44',
    '44 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '45',
    '45 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '50',
    '50 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '60',
    '60 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '63',
    '63 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '65',
    '65 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '67',
    '67 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '70',
    '70 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '75',
    '75 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '80',
    '80 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '85',
    '85 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '87',
    '87 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '89',
    '89 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '99',
    '99 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '100',
    '100 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '120',
    '120 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '123',
    '123 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '125',
    '125 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '130',
    '130 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '150',
    '150 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '175',
    '175 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '200',
    '200 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '300',
    '300 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '600',
    '600 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '999',
    '999 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '1000',
    '1000 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '1010',
    '1010 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '1015',
    '1015 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '1025',
    '1025 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Fuse Link Rating',
    '-99',
    '9999 A'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '1',
    'CATV'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '2',
    'Cell Tower'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '3',
    'DC Customer'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '4',
    'Irrigation Pump'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '5',
    'RR Crossing'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '6',
    'Siren'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '7',
    'Traffic Signal'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '8',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '9',
    'PGE'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '10',
    'StreetLight'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '11',
    'Bus Shelter'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Deliver Pt Customer Type',
    '12',
    'Fire Pump'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    'P',
    'Pilot'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '1',
    '1'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '2',
    '2'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '3',
    '3'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '4',
    '4'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '5',
    '5'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '6',
    '6'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '7',
    '7'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '8',
    '8'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '9',
    '9'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '10',
    '10'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '11',
    '11'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '12',
    '12'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '13',
    '13'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '14',
    '14'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '15',
    '15'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '16',
    '16'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '17',
    '17'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '18',
    '18'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '19',
    '19'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '20',
    '20'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '21',
    '21'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '22',
    '22'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '23',
    '23'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '24',
    '24'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '25',
    '25'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '26',
    '26'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '27',
    '27'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Conversion Work Package',
    '28',
    '28'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Export',
    'PLLO',
    'Parallel Only'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Export',
    'UNSP',
    'Unspecified'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Export',
    'RETL',
    'Retail'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Export',
    'WHSL',
    'Whole-sale'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Normal Status',
    '0',
    'Open'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Normal Status',
    '1',
    'Closed'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Normal Status',
    '2',
    'Not Applicable'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Material - Wood',
    'WOOD',
    'Wood'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Treatment Type - Non-Wood',
    'NON',
    'None'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Treatment Type - Non-Wood',
    'PNT',
    'Paint'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Pole Treatment Type - Non-Wood',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'LocalOperatingOffice',
    'TBD',
    'TBD'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Shunt Reactor Fill Type',
    'OIL',
    'Oil'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Shunt Reactor Fill Type',
    'OTH',
    'Other'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Network Relay Type',
    'EM',
    'Electric Mechanical'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Network Relay Type',
    'SS',
    'Solid State'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Network Relay Type',
    'MP',
    'Microprocessor'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Network Relay Type',
    'UNK',
    'Unknown'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '1',
    '240V - High Pressure - 50W - 3800L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '2',
    '240V - High Pressure - 70W - 5800L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '3',
    '240V - High Pressure - 100W - 9500L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '4',
    '240V - High Pressure - 150W - 16000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '5',
    '240V - High Pressure - 200W -  22000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '6',
    '240V - High Pressure - 250W - 25500L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '7',
    '240V - High Pressure - 310W - 37000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '8',
    '240V - High Pressure - 360W - 45000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '9',
    '240V - High Pressure - 400W - 46000L'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '10',
    'High Pressure - 35W - 2150L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '11',
    'High Pressure - 50W - 3800L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '12',
    'High Pressure - 70W - 5800L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '13',
    'High Pressure - 100W - 9500L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '14',
    'High Pressure - 150W - 16000L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '15',
    'High Pressure - 200W - 22000L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '16',
    'High Pressure - 250W - 25500L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '17',
    'High Pressure - 400W - 46000L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '18',
    'Incandescent - 58W - 600L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '19',
    'Incandescent - 92W - 1000L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '20',
    'Incandescent - 189W - 2500L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '21',
    'Incandescent - 295W - 4000L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
    DOMAIN_CODE,
    DOMAIN_DESCRIPTION
  )
  VALUES
  (
    'Street Light Item Type',
    '22',
    'Incandescent - 405W - 6000L SL'
  );
INSERT
INTO EDGIS.DOMAINLIST_TAB
  (
    DOMAIN_NAME,
  )
  (