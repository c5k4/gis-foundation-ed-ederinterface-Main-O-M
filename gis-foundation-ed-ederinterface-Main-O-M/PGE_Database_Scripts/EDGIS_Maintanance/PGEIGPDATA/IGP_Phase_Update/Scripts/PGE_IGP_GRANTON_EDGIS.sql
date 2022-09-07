spool D:\TEMP\Grant_edgis.txt

grant all on EDGIS.ZZ_MV_PRIUGCONDUCTORINFO to pgeigpdata;
grant all on EDGIS.ZZ_MV_PRIOHCONDUCTORINFO to pgeigpdata;
grant all on EDGIS.ZZ_MV_SECUGCONDUCTORINFO to pgeigpdata;
grant all on EDGIS.ZZ_MV_SECOHCONDUCTORINFO to pgeigpdata;
grant all on EDGIS.ZZ_MV_PRIUGCONDUCTOR to pgeigpdata;
grant all on EDGIS.ZZ_MV_PRIOHCONDUCTOR to pgeigpdata;
grant all on EDGIS.ZZ_MV_SECUGCONDUCTOR to pgeigpdata;
grant all on EDGIS.ZZ_MV_SECOHCONDUCTOR to pgeigpdata;
grant all on EDGIS.ZZ_MV_SECOHCONDUCTOR to pgeigpdata;
grant all on EDGIS.ZZ_MV_DISTBUSBAR to pgeigpdata;
grant all on EDGIS.ZZ_MV_TRANSFORMERUNIT to pgeigpdata;
grant all on EDGIS.ZZ_MV_SERVICELOCATION to pgeigpdata;
grant all on EDGIS.ZZ_MV_TRANSFORMER to pgeigpdata;

grant all on EDGIS.zz_mv_openpoint to pgeigpdata;
grant all on EDGIS.zz_mv_switch to pgeigpdata;
grant all on EDGIS.zz_mv_fuse to pgeigpdata;
grant all on EDGIS.zz_mv_dynamicprotectivedevice to pgeigpdata;
grant all on EDGIS.zz_mv_faultindicator to pgeigpdata;
grant all on EDGIS.zz_mv_stepdown to pgeigpdata;
grant all on EDGIS.zz_mv_voltageregulator to pgeigpdata;


commit;
spool off