SPOOL C:\Temp\DeviceGroup_Bulk.txt;
set serveroutput on;

-- doule check first two lines have '' after SW
update EDGIS.DeviceGroup Set ADMSLabel = 'VLT SW''S W/INT' where SubTypeCD = 2 AND DeviceGroupType = 36;
update EDGIS.A145 Set ADMSLabel = 'VLT SW''S W/INT' where SubTypeCD = 2 AND DeviceGroupType = 36;

update EDGIS.DeviceGroup Set ADMSLabel = 'SS XFMR' where SubTypeCD = 2 AND DeviceGroupType = 1;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS 1P BANKED OD/DWYE' where SubTypeCD = 2 AND DeviceGroupType = 2;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS XFMR' where SubTypeCD = 2 AND DeviceGroupType = 3;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS TX W/ SW' where SubTypeCD = 2 AND DeviceGroupType = 4;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS TX W/ 2 SW' where SubTypeCD = 2 AND DeviceGroupType = 5;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS DPLX TX W/ SW' where SubTypeCD = 2 AND DeviceGroupType = 6;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS FU SW' where SubTypeCD = 2 AND DeviceGroupType = 7;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS FU SW' where SubTypeCD = 2 AND DeviceGroupType = 8;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS CUTOUTS' where SubTypeCD = 2 AND DeviceGroupType = 9;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS SW' where SubTypeCD = 2 AND DeviceGroupType = 10;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS SW' where SubTypeCD = 2 AND DeviceGroupType = 12;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS SW' where SubTypeCD = 2 AND DeviceGroupType = 13;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS SMART SW' where SubTypeCD = 2 AND DeviceGroupType = 15;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS SW-INT-SW' where SubTypeCD = 2 AND DeviceGroupType = 16;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS INT' where SubTypeCD = 2 AND DeviceGroupType = 17;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS SECT' where SubTypeCD = 2 AND DeviceGroupType = 19;
update EDGIS.DeviceGroup Set ADMSLabel = 'VLT SW' where SubTypeCD = 2 AND DeviceGroupType = 20;
update EDGIS.DeviceGroup Set ADMSLabel = 'VLT SW' where SubTypeCD = 2 AND DeviceGroupType = 21;
update EDGIS.DeviceGroup Set ADMSLabel = 'VLT SW' where SubTypeCD = 2 AND DeviceGroupType = 22;
update EDGIS.DeviceGroup Set ADMSLabel = 'VLT INT' where SubTypeCD = 2 AND DeviceGroupType = 23;
update EDGIS.DeviceGroup Set ADMSLabel = 'VLT SW-INT-SW' where SubTypeCD = 2 AND DeviceGroupType = 24;
update EDGIS.DeviceGroup Set ADMSLabel = 'SUBWAY TX IN MH' where SubTypeCD = 2 AND DeviceGroupType = 25;
update EDGIS.DeviceGroup Set ADMSLabel = 'TGRAL' where SubTypeCD = 2 AND DeviceGroupType = 26;
update EDGIS.DeviceGroup Set ADMSLabel = 'TGRAM' where SubTypeCD = 2 AND DeviceGroupType = 27;
update EDGIS.DeviceGroup Set ADMSLabel = 'TGRAM' where SubTypeCD = 2 AND DeviceGroupType = 28;
update EDGIS.DeviceGroup Set ADMSLabel = '4KV JUNCT' where SubTypeCD = 2 AND DeviceGroupType = 30;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 2 AND DeviceGroupType = 31;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 2 AND DeviceGroupType = 32;
update EDGIS.DeviceGroup Set ADMSLabel = 'NETWORK' where SubTypeCD = 2 AND DeviceGroupType = 33;
update EDGIS.DeviceGroup Set ADMSLabel = 'TGRAM' where SubTypeCD = 2 AND DeviceGroupType = 34;
update EDGIS.DeviceGroup Set ADMSLabel = 'SS XFMR' where SubTypeCD = 2 AND DeviceGroupType = 35;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 3' where SubTypeCD = 3 AND DeviceGroupType = 1;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 4' where SubTypeCD = 3 AND DeviceGroupType = 2;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 4' where SubTypeCD = 3 AND DeviceGroupType = 3;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 4' where SubTypeCD = 3 AND DeviceGroupType = 4;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 5' where SubTypeCD = 3 AND DeviceGroupType = 5;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 6' where SubTypeCD = 3 AND DeviceGroupType = 6;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 9' where SubTypeCD = 3 AND DeviceGroupType = 7;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 11' where SubTypeCD = 3 AND DeviceGroupType = 8;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 11' where SubTypeCD = 3 AND DeviceGroupType = 9;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 6' where SubTypeCD = 3 AND DeviceGroupType = 10;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 9' where SubTypeCD = 3 AND DeviceGroupType = 11;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 9' where SubTypeCD = 3 AND DeviceGroupType = 12;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 41' where SubTypeCD = 3 AND DeviceGroupType = 13;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 42' where SubTypeCD = 3 AND DeviceGroupType = 14;
update EDGIS.DeviceGroup Set ADMSLabel = 'PMH 43' where SubTypeCD = 3 AND DeviceGroupType = 15;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM XFMR' where SubTypeCD = 3 AND DeviceGroupType = 16;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM TX W/ LB' where SubTypeCD = 3 AND DeviceGroupType = 17;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM XFMR' where SubTypeCD = 3 AND DeviceGroupType = 18;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM TX W/ SW' where SubTypeCD = 3 AND DeviceGroupType = 19;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM TX W/ 2 SW' where SubTypeCD = 3 AND DeviceGroupType = 20;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM AUTO BK' where SubTypeCD = 3 AND DeviceGroupType = 21;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM AUTO BK W/ RCL' where SubTypeCD = 3 AND DeviceGroupType = 22;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM AUTO BK W/ RCL' where SubTypeCD = 3 AND DeviceGroupType = 23;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM CAP BK' where SubTypeCD = 3 AND DeviceGroupType = 24;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM REG' where SubTypeCD = 3 AND DeviceGroupType = 26;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM REG' where SubTypeCD = 3 AND DeviceGroupType = 27;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM SW' where SubTypeCD = 3 AND DeviceGroupType = 28;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM INT' where SubTypeCD = 3 AND DeviceGroupType = 30;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM TX W/ INT' where SubTypeCD = 3 AND DeviceGroupType = 32;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM TX W/ INT' where SubTypeCD = 3 AND DeviceGroupType = 33;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM RCL' where SubTypeCD = 3 AND DeviceGroupType = 34;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM SECT' where SubTypeCD = 3 AND DeviceGroupType = 35;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 37;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 38;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 39;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 40;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 41;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 42;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 43;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 44;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 45;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 46;
update EDGIS.DeviceGroup Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 47;
update EDGIS.DeviceGroup Set ADMSLabel = 'PM XFMR' where SubTypeCD = 3 AND DeviceGroupType = 48;

update EDGIS.A145 Set ADMSLabel = 'SS XFMR' where SubTypeCD = 2 AND DeviceGroupType = 1;
update EDGIS.A145 Set ADMSLabel = 'SS 1P BANKED OD/DWYE' where SubTypeCD = 2 AND DeviceGroupType = 2;
update EDGIS.A145 Set ADMSLabel = 'SS XFMR' where SubTypeCD = 2 AND DeviceGroupType = 3;
update EDGIS.A145 Set ADMSLabel = 'SS TX W/ SW' where SubTypeCD = 2 AND DeviceGroupType = 4;
update EDGIS.A145 Set ADMSLabel = 'SS TX W/ 2 SW' where SubTypeCD = 2 AND DeviceGroupType = 5;
update EDGIS.A145 Set ADMSLabel = 'SS DPLX TX W/ SW' where SubTypeCD = 2 AND DeviceGroupType = 6;
update EDGIS.A145 Set ADMSLabel = 'SS FU SW' where SubTypeCD = 2 AND DeviceGroupType = 7;
update EDGIS.A145 Set ADMSLabel = 'SS FU SW' where SubTypeCD = 2 AND DeviceGroupType = 8;
update EDGIS.A145 Set ADMSLabel = 'SS CUTOUTS' where SubTypeCD = 2 AND DeviceGroupType = 9;
update EDGIS.A145 Set ADMSLabel = 'SS SW' where SubTypeCD = 2 AND DeviceGroupType = 10;
update EDGIS.A145 Set ADMSLabel = 'SS SW' where SubTypeCD = 2 AND DeviceGroupType = 12;
update EDGIS.A145 Set ADMSLabel = 'SS SW' where SubTypeCD = 2 AND DeviceGroupType = 13;
update EDGIS.A145 Set ADMSLabel = 'SS SMART SW' where SubTypeCD = 2 AND DeviceGroupType = 15;
update EDGIS.A145 Set ADMSLabel = 'SS SW-INT-SW' where SubTypeCD = 2 AND DeviceGroupType = 16;
update EDGIS.A145 Set ADMSLabel = 'SS INT' where SubTypeCD = 2 AND DeviceGroupType = 17;
update EDGIS.A145 Set ADMSLabel = 'SS SECT' where SubTypeCD = 2 AND DeviceGroupType = 19;
update EDGIS.A145 Set ADMSLabel = 'VLT SW' where SubTypeCD = 2 AND DeviceGroupType = 20;
update EDGIS.A145 Set ADMSLabel = 'VLT SW' where SubTypeCD = 2 AND DeviceGroupType = 21;
update EDGIS.A145 Set ADMSLabel = 'VLT SW' where SubTypeCD = 2 AND DeviceGroupType = 22;
update EDGIS.A145 Set ADMSLabel = 'VLT INT' where SubTypeCD = 2 AND DeviceGroupType = 23;
update EDGIS.A145 Set ADMSLabel = 'VLT SW-INT-SW' where SubTypeCD = 2 AND DeviceGroupType = 24;
update EDGIS.A145 Set ADMSLabel = 'SUBWAY TX IN MH' where SubTypeCD = 2 AND DeviceGroupType = 25;
update EDGIS.A145 Set ADMSLabel = 'TGRAL' where SubTypeCD = 2 AND DeviceGroupType = 26;
update EDGIS.A145 Set ADMSLabel = 'TGRAM' where SubTypeCD = 2 AND DeviceGroupType = 27;
update EDGIS.A145 Set ADMSLabel = 'TGRAM' where SubTypeCD = 2 AND DeviceGroupType = 28;
update EDGIS.A145 Set ADMSLabel = '4KV JUNCT' where SubTypeCD = 2 AND DeviceGroupType = 30;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 2 AND DeviceGroupType = 31;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 2 AND DeviceGroupType = 32;
update EDGIS.A145 Set ADMSLabel = 'NETWORK' where SubTypeCD = 2 AND DeviceGroupType = 33;
update EDGIS.A145 Set ADMSLabel = 'TGRAM' where SubTypeCD = 2 AND DeviceGroupType = 34;
update EDGIS.A145 Set ADMSLabel = 'SS XFMR' where SubTypeCD = 2 AND DeviceGroupType = 35;
update EDGIS.A145 Set ADMSLabel = 'PMH 3' where SubTypeCD = 3 AND DeviceGroupType = 1;
update EDGIS.A145 Set ADMSLabel = 'PMH 4' where SubTypeCD = 3 AND DeviceGroupType = 2;
update EDGIS.A145 Set ADMSLabel = 'PMH 4' where SubTypeCD = 3 AND DeviceGroupType = 3;
update EDGIS.A145 Set ADMSLabel = 'PMH 4' where SubTypeCD = 3 AND DeviceGroupType = 4;
update EDGIS.A145 Set ADMSLabel = 'PMH 5' where SubTypeCD = 3 AND DeviceGroupType = 5;
update EDGIS.A145 Set ADMSLabel = 'PMH 6' where SubTypeCD = 3 AND DeviceGroupType = 6;
update EDGIS.A145 Set ADMSLabel = 'PMH 9' where SubTypeCD = 3 AND DeviceGroupType = 7;
update EDGIS.A145 Set ADMSLabel = 'PMH 11' where SubTypeCD = 3 AND DeviceGroupType = 8;
update EDGIS.A145 Set ADMSLabel = 'PMH 11' where SubTypeCD = 3 AND DeviceGroupType = 9;
update EDGIS.A145 Set ADMSLabel = 'PMH 6' where SubTypeCD = 3 AND DeviceGroupType = 10;
update EDGIS.A145 Set ADMSLabel = 'PMH 9' where SubTypeCD = 3 AND DeviceGroupType = 11;
update EDGIS.A145 Set ADMSLabel = 'PMH 9' where SubTypeCD = 3 AND DeviceGroupType = 12;
update EDGIS.A145 Set ADMSLabel = 'PMH 41' where SubTypeCD = 3 AND DeviceGroupType = 13;
update EDGIS.A145 Set ADMSLabel = 'PMH 42' where SubTypeCD = 3 AND DeviceGroupType = 14;
update EDGIS.A145 Set ADMSLabel = 'PMH 43' where SubTypeCD = 3 AND DeviceGroupType = 15;
update EDGIS.A145 Set ADMSLabel = 'PM XFMR' where SubTypeCD = 3 AND DeviceGroupType = 16;
update EDGIS.A145 Set ADMSLabel = 'PM TX W/ LB' where SubTypeCD = 3 AND DeviceGroupType = 17;
update EDGIS.A145 Set ADMSLabel = 'PM XFMR' where SubTypeCD = 3 AND DeviceGroupType = 18;
update EDGIS.A145 Set ADMSLabel = 'PM TX W/ SW' where SubTypeCD = 3 AND DeviceGroupType = 19;
update EDGIS.A145 Set ADMSLabel = 'PM TX W/ 2 SW' where SubTypeCD = 3 AND DeviceGroupType = 20;
update EDGIS.A145 Set ADMSLabel = 'PM AUTO BK' where SubTypeCD = 3 AND DeviceGroupType = 21;
update EDGIS.A145 Set ADMSLabel = 'PM AUTO BK W/ RCL' where SubTypeCD = 3 AND DeviceGroupType = 22;
update EDGIS.A145 Set ADMSLabel = 'PM AUTO BK W/ RCL' where SubTypeCD = 3 AND DeviceGroupType = 23;
update EDGIS.A145 Set ADMSLabel = 'PM CAP BK' where SubTypeCD = 3 AND DeviceGroupType = 24;
update EDGIS.A145 Set ADMSLabel = 'PM REG' where SubTypeCD = 3 AND DeviceGroupType = 26;
update EDGIS.A145 Set ADMSLabel = 'PM REG' where SubTypeCD = 3 AND DeviceGroupType = 27;
update EDGIS.A145 Set ADMSLabel = 'PM SW' where SubTypeCD = 3 AND DeviceGroupType = 28;
update EDGIS.A145 Set ADMSLabel = 'PM INT' where SubTypeCD = 3 AND DeviceGroupType = 30;
update EDGIS.A145 Set ADMSLabel = 'PM TX W/ INT' where SubTypeCD = 3 AND DeviceGroupType = 32;
update EDGIS.A145 Set ADMSLabel = 'PM TX W/ INT' where SubTypeCD = 3 AND DeviceGroupType = 33;
update EDGIS.A145 Set ADMSLabel = 'PM RCL' where SubTypeCD = 3 AND DeviceGroupType = 34;
update EDGIS.A145 Set ADMSLabel = 'PM SECT' where SubTypeCD = 3 AND DeviceGroupType = 35;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 37;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 38;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 39;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 40;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 41;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 42;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 43;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 44;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 45;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 46;
update EDGIS.A145 Set ADMSLabel = 'MISC' where SubTypeCD = 3 AND DeviceGroupType = 47;
update EDGIS.A145 Set ADMSLabel = 'PM XFMR' where SubTypeCD = 3 AND DeviceGroupType = 48;

-- update j-box records
update EDGIS.DeviceGroup Set ADMSLabel = DeviceGroupName where SubTypeCD = 2 AND DeviceGroupType = 29;
update EDGIS.DeviceGroup Set ADMSLabel = DeviceGroupName where SubTypeCD = 3 AND DeviceGroupType = 36;
update EDGIS.A145 Set ADMSLabel = DeviceGroupName where SubTypeCD = 2 AND DeviceGroupType = 29;
update EDGIS.A145 Set ADMSLabel = DeviceGroupName where SubTypeCD = 3 AND DeviceGroupType = 36;

commit;

SPOOL OFF;
set serveroutput off;