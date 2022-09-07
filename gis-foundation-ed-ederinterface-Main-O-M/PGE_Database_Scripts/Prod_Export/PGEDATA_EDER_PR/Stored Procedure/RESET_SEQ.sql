--------------------------------------------------------
--  DDL for Procedure RESET_SEQ
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."RESET_SEQ" ( p_owner  in varchar2  ,p_seq_name in varchar2, p_table_name in varchar2,p_col_name in varchar2 ) is
    h_val number := 0;
    h_max_id number := 0;
    h_diff number := 0;

 

begin

 

    execute immediate   'select ' ||p_owner||'.'|| p_seq_name || '.nextval from dual' INTO h_val;

 

    execute immediate 'select nvl(max('||p_col_name||'),0) from ' ||p_owner||'.'|| p_table_name INTO h_max_id;

 

       h_diff := h_max_id - h_val ;

 

    execute immediate    'alter sequence ' || p_seq_name || ' increment by ' || h_diff ;

 

    execute immediate   'select ' ||p_owner||'.'|| p_seq_name || '.nextval from dual' INTO h_val;

 

     execute immediate    'alter sequence ' ||p_owner||'.'|| p_seq_name || ' increment by 1 minvalue 0';

 

end;
