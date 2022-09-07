Prompt drop Function PGE_CAL_GEN_TYPE;
DROP FUNCTION EDSETT.PGE_CAL_GEN_TYPE
/

Prompt Function PGE_CAL_GEN_TYPE;
--
-- PGE_CAL_GEN_TYPE  (Function) 
--
CREATE OR REPLACE FUNCTION EDSETT."PGE_CAL_GEN_TYPE" ( GID NVARCHAR2)  RETURN VARCHAR2    IS
  h_cnt NUMBER (10)        := 0;
  h_gen_type NVARCHAR2 (50) := NULL;
BEGIN
   SELECT COUNT (DISTINCT r.gen_tech_cd)
   INTO h_cnt
   FROM edsett.sm_generation g
     JOIN edsett.sm_protection p
        ON g.id = p.parent_id
     JOIN edsett.sm_generator r
        ON p.id           = r.protection_id
   WHERE p.parent_type  = 'GENERATION'
        AND g.global_id = gid;
  IF h_cnt  = 1 THEN
      SELECT DISTINCT Upper (r.gen_tech_cd)
      INTO h_gen_type
      FROM edsett.sm_generation g
        JOIN edsett.sm_protection p
            ON g.id = p.parent_id
        JOIN edsett.sm_generator r
          ON p.id           = r.protection_id
      WHERE p.parent_type  = 'GENERATION'
           AND g.global_id = gid;
  ELSIF (h_cnt  > 1) THEN
     		 h_gen_type  := 'MIXD';
  END IF;
 RETURN h_gen_type;
END;
/
