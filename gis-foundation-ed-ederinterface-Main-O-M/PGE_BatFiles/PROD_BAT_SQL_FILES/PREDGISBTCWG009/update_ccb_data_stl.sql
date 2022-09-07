set pagesize 0
set linesize 200
set echo on term on
set serveroutput on size unlimited
set time on
set timing on

-- rebuild indexes
alter index PGEDATA.SLCDX_SPITEMIDX rebuild;
alter index PGEDATA.SLCDX_DSADDRIDX rebuild;
alter index PGEDATA.SLCDX_MAPNIDX rebuild;
alter index PGEDATA.SLCDX_INSDATEIDX rebuild;
alter index PGEDATA.SLCDX_SPIDIDX rebuild;
alter index PGEDATA.SLCDX_FXIDX rebuild;
alter index PGEDATA.SLCDX_OFC_IDX rebuild;
alter index PGEDATA.SLCDX_BADGEIDX rebuild;
analyze table PGEDATA.slcdx_data compute statistics for all indexed columns for all indexes for table;

-- create unique set of lamp records
exec PGEDATA.unique_slcdx;
exec PGEDATA.unique_slcdx_gisid;
analyze table PGEDATA.slcdx_uniquedata compute statistics for all indexed columns for all indexes for table;

-- update stl table with unique data
exec PGEDATA.update_stl_ccb;
exec PGEDATA.update_stl_ccb_gisid;

set wrap off
set echo off term off
set serveroutput off
set time off
set timing off

exit;
