copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0401-25.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0401-50.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0401-100.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0401-200.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0401-250.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0401-500.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0402-25.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0402-50.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0402-100.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0402-200.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0402-250.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0402-500.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0403-25.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0403-50.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0403-100.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0403-200.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0403-250.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0403-500.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0404-25.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0404-50.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0404-100.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0404-200.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0404-250.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0404-500.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0405-25.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0405-50.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0405-100.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0405-200.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0405-250.mxd"
copy "Map Production Mxds\PDFTemplate.mxd" "Map Production Mxds\PDF-0405-500.mxd"

GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0401-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI  -a 25.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0401-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI  -a 50.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0401-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI  -a 100.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0401-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI  -a 200.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0401-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI  -a 250.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0401-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI  -a 500.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0402-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII  -a 25.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0402-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII  -a 50.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0402-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII  -a 100.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0402-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII  -a 200.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0402-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII  -a 250.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0402-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII  -a 500.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0403-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII  -a 25.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0403-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII  -a 50.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0403-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII  -a 100.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0403-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII  -a 200.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0403-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII  -a 250.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0403-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII  -a 500.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0404-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV  -a 25.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0404-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV  -a 50.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0404-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV  -a 100.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0404-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV  -a 200.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0404-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV  -a 250.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0404-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV  -a 500.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0405-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV  -a 25.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0405-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV  -a 50.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0405-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV  -a 100.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0405-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV  -a 200.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0405-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV  -a 250.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M PDF-0405-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV  -a 500.lyr