spool  c:\temp\HighSide_counts_before_update.txt
set lines 1500
--select highsideconfiguration as CapacitorBank_HighSide, count(*) from EDGIS.zz_mv_capacitorbank group by highsideconfiguration;
select highsideconfiguration as VoltageRegulator_HighSide, count(*) from EDGIS.zz_mv_voltageregulator group by highsideconfiguration;
select highsideconfiguration as Transformer_HighSide, count(*) from EDGIS.zz_mv_transformer group by highsideconfiguration;
--select lowsideconfiguration as Transformer_LowSide, count(*) from EDGIS.zz_mv_transformer group by lowsideconfiguration;
spool off;
select Count(Distinct(PHASEDESIGNATION)) from EDGIS.zz_mv_transformerunit where transformerguid = (select GLOBALID from EDGIS.ZZ_MV_Transformer where OBJECTID = 11858087);
select PHASEDESIGNATION from EDGIS.zz_mv_transformerunit where transformerguid = (select GLOBALID from EDGIS.ZZ_MV_Transformer where OBJECTID = 4705381);