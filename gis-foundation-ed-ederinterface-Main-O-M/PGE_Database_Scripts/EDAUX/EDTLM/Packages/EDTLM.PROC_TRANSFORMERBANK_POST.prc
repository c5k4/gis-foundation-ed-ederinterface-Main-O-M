Prompt drop Procedure PROC_TRANSFORMERBANK_POST;
DROP PROCEDURE EDTLM.PROC_TRANSFORMERBANK_POST
/

Prompt Procedure PROC_TRANSFORMERBANK_POST;
--
-- PROC_TRANSFORMERBANK_POST  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDTLM.Proc_TransformerBank_Post
AS
  v_TRF_ID     VARCHAR2(100) ;
  v_BANK_CD    NUMBER(1) ;
  v_NP_KVA     NUMBER(7,1) ;
  v_GLOBAL_ID  VARCHAR2(38) ;
  v_PHASE_CD   NUMBER(1) ;
  v_TRF_TYP    VARCHAR2(4);
  v_REC_STATUS VARCHAR2(1) ;
  v_count      NUMBER(15);
  v_Max_Rec    NUMBER(15);
  v_CaseType   VARCHAR2(100);
  v_Param      NUMBER(1);
  CURSOR c1
  IS
    SELECT trim(a.TRF_ID) TRF_ID,
      NVL(to_number(trim(a.BANK_CD)),0) BANK_CD,
      NVL(TO_NUMBER(trim(A.NP_KVA)), 0) NP_KVA,
      A.GLOBAL_ID,
      NVL(TO_NUMBER(trim(A.PHASE_CD)), 0) PHASE_CD,
      NVL(DECODE(trim(A.TRF_TYP),'01','1','02','2','03','3', '04','4','05','5','06','6','07','7','08','8','09','9',trim(A.TRF_TYP)),'') TRF_TYP,
      'I' REC_STATUS
    FROM UC4ADMIN.TRANSFORMER_BANK a
    WHERE trim(a.trf_id) IS NOT NULL
    AND a.Global_id NOT  IN
      (SELECT tb.global_id FROM EDTLM.transformer_bank tb
      )
  AND trim(a.trf_id) IN
    (SELECT t.global_id FROM transformer t
    )
  AND LENGTH(TRIM(TRANSLATE(a.BANK_CD, ' +-.0123456789', ' '))) IS NULL
  AND NVL (To_Number(trim(a.BANK_CD)),0)                        <= 9
  ORDER BY trim(a.TRF_ID),
    NVL(to_number(trim(a.BANK_CD)),0);
BEGIN
  --###################### POST UPDATE SCRIPTS - Adjust 0 BANK_CD in TLM ###########################
  --All with 1 counts:All 0 bank cd to 1
  UPDATE EDTLM.transformer_bank
  SET BANK_CD   =1
  WHERE trf_id IN
    (SELECT trf_id
    FROM EDTLM.transformer_bank
    WHERE trf_id IN
      ( SELECT trf_id FROM EDTLM.transformer_bank WHERE bank_cd=0
      )
    GROUP BY trf_id
    HAVING COUNT(1)=1
    );
  INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.transformer_bank',
      'All with 1 counts:All 0 bank cd to 1',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
  --All with 3 counts: bank_cd 0 to 1
  UPDATE EDTLM.transformer_bank
  SET bank_cd   =1
  WHERE trf_id IN
    ( SELECT trf_id FROM EDTLM.transformer_bank WHERE bank_cd=0
    )
  AND trf_id NOT IN
    (SELECT trf_id
    FROM EDTLM.transformer_bank
    WHERE trf_id
      ||'-'
      || bank_cd= trf_id
      || '-'
      || 1
    )
  AND bank_cd=0;
  INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.transformer_bank',
      'All with 2 and 3 counts: bank_cd 0 to 1',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
  --All with 3 counts: bank_cd 0 to 2
  UPDATE EDTLM.transformer_bank
  SET bank_cd   =2
  WHERE trf_id IN
    ( SELECT trf_id FROM EDTLM.transformer_bank WHERE bank_cd=0
    )
  AND trf_id NOT IN
    (SELECT trf_id
    FROM EDTLM.transformer_bank
    WHERE trf_id
      ||'-'
      || bank_cd= trf_id
      || '-'
      || 2
    )
  AND bank_cd=0;
  INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.transformer_bank',
      'All with 2 and 3 counts: bank_cd 0 to 2',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
  --All with 3 counts: bank_cd 0 to 3
  UPDATE EDTLM.transformer_bank
  SET bank_cd   =3
  WHERE trf_id IN
    ( SELECT trf_id FROM EDTLM.transformer_bank WHERE bank_cd=0
    )
  AND trf_id NOT IN
    (SELECT trf_id
    FROM EDTLM.transformer_bank
    WHERE trf_id
      ||'-'
      || bank_cd= trf_id
      || '-'
      || 3
    )
  AND bank_cd=0;
  INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.transformer_bank',
      'All with 2 and 3 counts: bank_cd 0 to 3',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
  --#####################################################################
  v_count  :=0;
  v_Max_Rec:=0;
  OPEN c1;
  LOOP
    FETCH c1
    INTO v_TRF_ID,
      v_BANK_CD,
      v_NP_KVA,
      v_GLOBAL_ID,
      v_PHASE_CD,
      v_TRF_TYP,
      v_REC_STATUS;
    --dbms_output.put_line('Record No. ' || c1%rowcount || '=>' || v_TRF_ID || '-' || v_BANK_CD || '-' || v_NP_KVA || '-' || v_GLOBAL_ID || '-' || v_PHASE_CD || '-' || v_TRF_TYP || '-' || v_REC_STATUS);
    v_count  :=0;
    v_Max_Rec:=0;
    SELECT COUNT(1)
    INTO v_count
    FROM transformer_bank
    WHERE TRF_ID =
      (SELECT id FROM transformer WHERE global_id=v_TRF_ID
      );
    IF (v_count   IS NULL OR v_count=0 ) THEN --No record
      v_CaseType  :='No Record. Orig BankCD='|| v_BANK_CD;
      IF v_BANK_CD =0 THEN
        v_BANK_CD :=1;
      ELSE
        v_BANK_CD:=v_BANK_CD;
      END IF;
      v_CaseType:=v_CaseType ||' New BankCD='|| v_BANK_CD;
    ELSE --Records found in TLM
      IF v_BANK_CD =0 THEN
        v_CaseType:='Exist. BankCD Zero=> Orig BCD=' || v_BANK_CD;
        SELECT COUNT(1)
        INTO v_count
        FROM EDTLM.transformer_bank
        WHERE TRF_ID =
          (SELECT id FROM transformer WHERE global_id=v_TRF_ID
          )
        AND bank_cd  =1;
        IF (v_count IS NULL) OR v_count=0 THEN --Insert Bank_CD 0 as 1
          v_BANK_CD :=1;
        ELSE --Insert Bank_CD 0 as max+1
          SELECT MAX(bank_cd)+1
          INTO v_Max_Rec
          FROM EDTLM.transformer_bank
          WHERE TRF_ID =
            (SELECT id FROM transformer WHERE global_id=v_TRF_ID
            );
          v_BANK_CD:=v_Max_Rec;
        END IF;
        v_CaseType:=v_CaseType || '=> New BCD=' || v_BANK_CD;
      ELSE --Non Zero bank_CD Cases
        v_CaseType:='Exist. NonZero BankCD=> Orig BCD=' || v_BANK_CD;
        SELECT COUNT(1)
        INTO v_count
        FROM EDTLM.transformer_bank
        WHERE TRF_ID =
          (SELECT id FROM transformer WHERE global_id=v_TRF_ID
          )
        AND bank_cd  =v_BANK_CD;
        IF (v_count IS NULL OR v_count=0 ) THEN --No Pair Exist in TLM. Go ahead and Insert
          v_BANK_CD :=v_BANK_CD;
        ELSE --Pair Exist in TLM. Insert Bank_CD as max+1
          SELECT MAX(bank_cd)+1
          INTO v_Max_Rec
          FROM EDTLM.transformer_bank
          WHERE TRF_ID =
            (SELECT id FROM transformer WHERE global_id=v_TRF_ID
            );
          v_BANK_CD:=v_Max_Rec;
        END IF;
        v_CaseType:=v_CaseType || '=> New BCD=' || v_BANK_CD;
      END IF;
    END IF;
    --dbms_output.put_line(v_CaseType);
    --Final Check Before Insert
    SELECT COUNT(1)
    INTO v_count
    FROM EDTLM.transformer_bank
    WHERE (TRF_ID =
      (SELECT id FROM transformer WHERE global_id=v_TRF_ID
      )
    AND bank_cd  =v_BANK_CD)
    OR GLOBAL_ID =v_GLOBAL_ID;
    IF (v_count IS NULL OR v_count=0 ) THEN --No Any Record  Exist in TLM. Go ahead and Insert
      INSERT
      INTO EDTLM.transformer_bank
        (
          TRF_ID,
          BANK_CD,
          NP_KVA,
          GLOBAL_ID,
          PHASE_CD,
          TRF_TYP,
          REC_STATUS
        )
        VALUES
        (
          (SELECT id FROM EDTLM.TRANSFORMER WHERE global_id=v_TRF_ID
          )
          ,
          v_BANK_CD,
          NVL(TO_NUMBER(trim(v_NP_KVA)), 0),
          v_GLOBAL_ID,
          NVL(TO_NUMBER(trim(v_PHASE_CD)), 0),
          NVL(DECODE(trim(v_TRF_TYP),'01','1','02','2','03','3', '04','4','05','5','06','6','07','7','08','8','09','9', trim(v_TRF_TYP)),'') ,
          'I'
        );
      INSERT
      INTO CD_LOG_STATUS VALUES
        (
          '',
          'Inserted TRF_ID ='
          || v_TRF_ID
          || ' And BANK_CD='
          || v_BANK_CD,
          v_CaseType,
          'PASS',
          sysdate
        );
      COMMIT;
    END IF;
    dbms_output.put_line(c1%rowcount || '- Inserted TRF_ID =' || v_TRF_ID || ' And BANK_CD=' || v_BANK_CD);
    -- rollback;
    EXIT
  WHEN c1%notfound;
  END LOOP;
  CLOSE c1;
  --Update Flag in GIS
  MERGE INTO UC4ADMIN.TRANSFORMER_BANK A1 USING
  (SELECT a.Global_id
    FROM UC4ADMIN.TRANSFORMER_BANK a
    JOIN EDTLM.TRANSFORMER_BANK b
    ON a.global_id     =b.global_id
    WHERE b.rec_status ='I'
  )
  B1 ON (A1.GLOBAL_ID=B1.Global_id)
WHEN Matched THEN
  UPDATE SET A1.TRANS_TYPE='I',A1.TRANS_DATE=sysdate,A1.PROC_FLG='P';
  
  INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.TRANSFORMER_BANK',
      'PROC_FLAG UPDATE POST INSERT - Transformer Bank',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;



--####################### TRANSFORMER ################################

Insert into TRANSFORMER_MISSING Select distinct cgc_id from EDTLM.Transformer where REC_STATUS in ('I','R')
and cgc_id not in (select cgc_id from EDTLM.TRANSFORMER_MISSING);

Insert into TRANSFORMER_MISSING 
Select distinct cgc_id from EDTLM.Transformer where REC_STATUS in ('U')
and cgc_id not in (select cgc_id from EDTLM.TRANSFORMER_MISSING) and cgc_id in 
(
Select a.cgc_id from EDTLM.Transformer  a join UC4ADMIN.TRANSFORMER b on a.GLOBAL_ID=b.GLOBAL_ID
where a.CGC_ID<>b.CGC_ID and a.REC_STATUS in ('U')
);

INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.TRANSFORMER_MISSING',
      'Inserted New CGC_ID from Transformer',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
  
  
--########################## TRANSFORMER_BANK #############################
Insert into TRANSFORMER_MISSING 
Select distinct b.cgc_id from EDTLM.TRANSFORMER_BANK a join EDTLM.Transformer b on a.trf_id=b.id
where a.REC_STATUS in ('I','R')
and b.cgc_id not in (select cgc_id from EDTLM.TRANSFORMER_MISSING);

Insert into TRANSFORMER_MISSING 
Select distinct b.cgc_id from EDTLM.TRANSFORMER_BANK a join EDTLM.Transformer b on a.trf_id=b.id
where a.REC_STATUS in ('U')
and b.cgc_id not in (select cgc_id from EDTLM.TRANSFORMER_MISSING)
and b.cgc_id in 
(Select distinct e.cgc_id from EDTLM.TRANSFORMER_BANK c join 
UC4ADMIN.TRANSFORMER_BANK d on c.global_id=d.global_id
join EDTLM.Transformer e on c.trf_id=e.id
where c.REC_STATUS in ('U') and d.trf_id<>e.GLOBAL_ID
);

INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.TRANSFORMER_MISSING',
      'Inserted New CGC_ID from Transformer Bank',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
--################################ METER #######################

Insert into TRANSFORMER_MISSING 
Select distinct b.cgc_id from EDTLM.METER a join EDTLM.Transformer b on a.trf_id=b.id
where a.REC_STATUS in ('I','R')
and b.cgc_id not in (select cgc_id from EDTLM.TRANSFORMER_MISSING);

Insert into TRANSFORMER_MISSING 
Select distinct b.cgc_id from EDTLM.TRANSFORMER_BANK a join EDTLM.Transformer b on a.trf_id=b.id
where a.REC_STATUS in ('U')
and b.cgc_id not in (select cgc_id from EDTLM.TRANSFORMER_MISSING)
and b.cgc_id  in 
(Select distinct e.cgc_id from EDTLM.METER c join 
UC4ADMIN.SERVICEPOINT d on c.global_id=d.global_id
join EDTLM.Transformer e on c.trf_id=e.id
where c.REC_STATUS in ('U') and d.trf_global_id<>e.GLOBAL_ID
);


INSERT
  INTO CD_LOG_STATUS VALUES
    (
      'EDTLM.TRANSFORMER_MISSING',
      'Inserted New CGC_ID from Meter',
      'Successfully Updated',
      'PASS',
      sysdate
    );
  COMMIT;
  
  
EXCEPTION
WHEN OTHERS THEN
  RAISE_APPLICATION_ERROR(-20001,'An error was encountered - ' ||SQLCODE||' -ERROR'||SQLERRM);
END;
/
