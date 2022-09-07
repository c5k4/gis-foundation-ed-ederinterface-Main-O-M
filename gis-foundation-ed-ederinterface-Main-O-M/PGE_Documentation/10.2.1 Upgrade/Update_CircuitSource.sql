delete from EDGIS.CircuitSource where DeviceGUID not in (select globalid from EDGIS.ElectricStitchPoint);
commit;