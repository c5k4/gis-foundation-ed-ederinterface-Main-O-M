delete from CircuitSource where DeviceGUID2 not in (select globalid from SUBElectricStitchPoint);
update CircuitSource set deviceguid = deviceguid2;
commit;
