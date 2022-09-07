Prompt drop Procedure UPDATESCHEMFCIDS;
DROP PROCEDURE EDGIS.UPDATESCHEMFCIDS
/

Prompt Procedure UPDATESCHEMFCIDS;
--
-- UPDATESCHEMFCIDS  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.UpdateSchemFCIDs AS

BEGIN

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4510' where to_feature_fcid = '997';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4513' where to_feature_fcid = '998';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4518' where to_feature_fcid = '1000';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4526' where to_feature_fcid = '1001';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4528' where to_feature_fcid = '1002';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4499' where to_feature_fcid = '1003';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4523' where to_feature_fcid = '1004';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4505' where to_feature_fcid = '1005';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4519' where to_feature_fcid = '1006';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4525' where to_feature_fcid = '1008';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4502' where to_feature_fcid = '1009';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4514' where to_feature_fcid = '1010';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4516' where to_feature_fcid = '1011';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4507' where to_feature_fcid = '1012';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4500' where to_feature_fcid = '1013';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4508' where to_feature_fcid = '1014';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4524' where to_feature_fcid = '1015';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4498' where to_feature_fcid = '1016';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4512' where to_feature_fcid = '1019';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4529' where to_feature_fcid = '1021';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '-1' where to_feature_fcid = '1022';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4504' where to_feature_fcid = '1023';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '-1' where to_feature_fcid = '1024';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4511' where to_feature_fcid = '1025';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4509' where to_feature_fcid = '3849';

update edgis.pge_elecdistnetwork_trace set to_feature_schem_fcid = '4521' where to_feature_fcid = '19090';

END UpdateSchemFCIDs ;
/


Prompt Grants on PROCEDURE UPDATESCHEMFCIDS TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.UPDATESCHEMFCIDS TO GISINTERFACE
/
