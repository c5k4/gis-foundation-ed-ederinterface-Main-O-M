copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0401-25.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0401-50.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0401-100.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0401-200.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0401-250.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0401-500.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0402-25.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0402-50.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0402-100.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0402-200.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0402-250.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0402-500.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0403-25.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0403-50.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0403-100.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0403-200.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0403-250.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0403-500.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0404-25.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0404-50.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0404-100.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0404-200.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0404-250.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0404-500.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0405-25.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0405-50.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0405-100.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0405-200.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0405-250.mxd"
copy "Map Production Mxds\TIFFTemplate.mxd" "Map Production Mxds\TIFF-0405-500.mxd"

GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0401-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI -T -a 25-White.lyr,25-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0401-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI -T -a 50-White.lyr,50-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0401-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI -T -a 100-White.lyr,100-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0401-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI -T -a 200-White.lyr,200-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0401-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI -T -a 250-White.lyr,250-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0401-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAI -T -a 500-White.lyr,500-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0402-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII -T -a 25-White.lyr,25-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0402-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII -T -a 50-White.lyr,50-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0402-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII -T -a 100-White.lyr,100-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0402-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII -T -a 200-White.lyr,200-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0402-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII -T -a 250-White.lyr,250-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0402-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAII -T -a 500-White.lyr,500-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0403-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII -T -a 25-White.lyr,25-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0403-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII -T -a 50-White.lyr,50-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0403-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII -T -a 100-White.lyr,100-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0403-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII -T -a 200-White.lyr,200-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0403-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII -T -a 250-White.lyr,250-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0403-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIII -T -a 500-White.lyr,500-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0404-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV -T -a 25-White.lyr,25-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0404-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV -T -a 50-White.lyr,50-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0404-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV -T -a 100-White.lyr,100-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0404-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV -T -a 200-White.lyr,200-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0404-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV -T -a 250-White.lyr,250-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0404-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAIV -T -a 500-White.lyr,500-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0405-25.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV -T -a 25-White.lyr,25-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0405-50.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV -T -a 50-White.lyr,50-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0405-100.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV -T -a 100-White.lyr,100-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0405-200.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV -T -a 200-White.lyr,200-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0405-250.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV -T -a 250-White.lyr,250-Label.lyr
GenerateRequiredMxds.exe -i sde:oracle11g:/;local=edgisa1d -u edgis -p edgis!A1Di -s -M TIFF-0405-500.mxd -r EDGIS.plat_unified,EDGIS.MaintenancePlat -proj CAV -T -a 500-White.lyr,500-Label.lyr