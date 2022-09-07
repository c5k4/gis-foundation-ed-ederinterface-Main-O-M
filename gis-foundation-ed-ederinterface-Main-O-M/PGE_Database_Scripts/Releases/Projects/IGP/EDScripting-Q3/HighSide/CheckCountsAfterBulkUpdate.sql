SPOOL  'c:\temp\highside_counts_after_update.txt';
set lines 1500  
SET sqlformat ansiconsole;
select highsideconfiguration as CapacitorBank_HighSide, count(*) from EDGIS.zz_mv_capacitorbank group by highsideconfiguration;
select highsideconfiguration as VoltageRegulator_HighSide, count(*) from EDGIS.zz_mv_voltageregulator group by highsideconfiguration;
select highsideconfiguration_new as VoltageRegulator_HighSide_New, count(*) from EDGIS.zz_mv_voltageregulator group by highsideconfiguration_new;
select highsideconfiguration as Transformer_HighSide, count(*) from EDGIS.zz_mv_transformer group by highsideconfiguration;
select lowsideconfiguration as Transformer_LowSide, count(*) from EDGIS.zz_mv_transformer group by lowsideconfiguration;

