Update EDGIS.edschem_postedsession SET STATUS = 1 , PROCESSID = null 
where STATUS = 1 and SESSIONID not in 
(
Select SESSIONID from EDGIS.edschem_updatepolygon
);
COMMIT;
exit;