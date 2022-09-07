#!/bin/sh
#============================================================================
# USAGE:        get_oracle_sid.sh dbtype machine
# Returns:      oracle_sid
# (machine is optional) (machine default is current host)
#============================================================================

#=============================================================================
# Set script variables and environment
#-----------------------------------------------------------------------------
dbtype_default=foo
#=============================================================================

#============================================================================
# Set dbtype
#----------------------------------------------------------------------------
if [ -z "$1" ] ; then
  dbtype=$dbtype_default
else
  dbtype=$1
fi
#echo "dbtype           = $dbtype"
dbtype=`echo $dbtype | tr '[:upper:]' '[:lower:]'`
#echo "dbtype           = $dbtype"
#============================================================================

#============================================================================
# Set machine
#----------------------------------------------------------------------------
if [ -z "$2" ] ; then
  machine=`uname -n`
else
  machine=$2
fi
machine=`echo $machine | tr '[:upper:]' '[:lower:]'`
#echo "machine          = $machine"
#============================================================================

#============================================================================
# Set oracle_sid
#----------------------------------------------------------------------------
oracle_sid=$dbtype

if [ "$dbtype" == 'edmaint' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgis1p    ;;
    "edgisdboraprd02") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd11") oracle_sid=edgis1p    ;;
    "edgisdboraprd12") oracle_sid=edgis1p    ;;
    "edgisdboraprd21") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd22") oracle_sid=edgis1pdg  ;;
    "edgisdboraqa11")  oracle_sid=edgis1q    ;;
    "edgisdboraqa12")  oracle_sid=edgis1q    ;;
    "edgisdbqa01")     oracle_sid=edgist2q   ;;
	"edgisdboraprd05") oracle_sid=edgis1p    ;;
  esac
fi

if [ "$dbtype" == 'edmaintdc1' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgis1p    ;;
    "edgisdboraprd02") oracle_sid=edgis1p    ;;
    "edgisdboraprd11") oracle_sid=edgis1p    ;;
    "edgisdboraprd12") oracle_sid=edgis1p    ;;
    "edgisdboraprd21") oracle_sid=edgis1p    ;;
    "edgisdboraprd22") oracle_sid=edgis1p    ;;
    "edgisdboraqa11")  oracle_sid=edgis1q    ;;
    "edgisdboraqa12")  oracle_sid=edgis1q    ;;
    "edgisdbqa01")     oracle_sid=edgist2q   ;;
  esac
fi

if [ "$dbtype" == 'edmaintdc2' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd02") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd11") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd12") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd21") oracle_sid=edgis1pdg  ;;
    "edgisdboraprd22") oracle_sid=edgis1pdg  ;;
    "edgisdboraqa11")  oracle_sid=edgis1q    ;;
    "edgisdboraqa12")  oracle_sid=edgis1q    ;;
    "edgisdbqa01")     oracle_sid=edgist2q   ;;
  esac
fi

if [ "$dbtype" == 'edpub' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgisp1p ;;
    "edgisdboraprd02") oracle_sid=edgisp3p ;;
    "edgisdboraprd11") oracle_sid=edgisp1p ;;
    "edgisdboraprd12") oracle_sid=edgisp3p ;;
    "edgisdboraprd21") oracle_sid=edgisp1p ;;
    "edgisdboraprd22") oracle_sid=edgisp3p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
    "edgisdbqa01")     oracle_sid=edgist2q ;;
  esac
fi

if [ "$dbtype" == 'edpubdc1' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgisp1p ;;
    "edgisdboraprd02") oracle_sid=edgisp1p ;;
    "edgisdboraprd11") oracle_sid=edgisp1p ;;
    "edgisdboraprd12") oracle_sid=edgisp1p ;;
    "edgisdboraprd21") oracle_sid=edgisp1p ;;
    "edgisdboraprd22") oracle_sid=edgisp1p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
    "edgisdbqa01")     oracle_sid=edgist2q ;;
  esac
fi

if [ "$dbtype" == 'edpubdc2' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgisp3p ;;
    "edgisdboraprd02") oracle_sid=edgisp3p ;;
    "edgisdboraprd11") oracle_sid=edgisp3p ;;
    "edgisdboraprd12") oracle_sid=edgisp3p ;;
    "edgisdboraprd21") oracle_sid=edgisp3p ;;
    "edgisdboraprd22") oracle_sid=edgisp3p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
    "edgisdbqa01")     oracle_sid=edgist2q ;;
  esac
fi

if [ "$dbtype" == 'edbatch' ] ; then
  case $machine in
    "edgisdboraprd05") oracle_sid=edgisp2p ;;
    "edgisdboraprd06") oracle_sid=edgisp4p ;;
    "edgisdboraprd11") oracle_sid=edgisp2p ;;
    "edgisdboraprd12") oracle_sid=edgisp2p ;;
    "edgisdboraprd21") oracle_sid=edgisp4p ;;
    "edgisdboraprd22") oracle_sid=edgisp4p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
    "edgisdbqa01")     oracle_sid=edgist2q ;;
  esac
fi

if [ "$dbtype" == 'edbatchdc1' ] ; then
  case $machine in
    "edgisdboraprd05") oracle_sid=edgisp2p ;;
    "edgisdboraprd02") oracle_sid=edgisp2p ;;
    "edgisdboraprd11") oracle_sid=edgisp2p ;;
    "edgisdboraprd12") oracle_sid=edgisp2p ;;
    "edgisdboraprd21") oracle_sid=edgisp2p ;;
    "edgisdboraprd22") oracle_sid=edgisp2p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
    "edgisdbqa01")     oracle_sid=edgist2q ;;
  esac
fi

if [ "$dbtype" == 'edbatchdc2' ] ; then
  case $machine in
    "edgisdboraprd06") oracle_sid=edgisp2p ;;
    "edgisdboraprd02") oracle_sid=edgisp4p ;;
    "edgisdboraprd11") oracle_sid=edgisp4p ;;
    "edgisdboraprd12") oracle_sid=edgisp4p ;;
    "edgisdboraprd21") oracle_sid=edgisp4p ;;
    "edgisdboraprd22") oracle_sid=edgisp4p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
    "edgisdbqa01")     oracle_sid=edgist2q ;;
  esac
fi

#============================================================================
# oracle_sid for Substation
#----------------------------------------------------------------------------
if [ "$dbtype" == 'edsub' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edsub1p    ;;
    "edgisdboraprd02") oracle_sid=edsub1p    ;;
    "edgisdboraprd11") oracle_sid=edsub1p    ;;
    "edgisdboraprd12") oracle_sid=edsub1p    ;;
    "edgisdboraprd21") oracle_sid=edsub1pdg  ;;
    "edgisdboraprd22") oracle_sid=edsub1pdg  ;;
    "edgisdboraqa11")  oracle_sid=edsub1q    ;;
	"edgisdboraqa01")  oracle_sid=edsub1q    ;;
    "edgisdboraqa12")  oracle_sid=edsub1q    ;;
	"edgisdboraprd05") oracle_sid=edsub1p    ;;
  esac
fi

if [ "$dbtype" == 'edsubdc1' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edsub1p    ;;
    "edgisdboraprd02") oracle_sid=edsub1p    ;;
    "edgisdboraprd11") oracle_sid=edsub1p    ;;
    "edgisdboraprd12") oracle_sid=edsub1p    ;;
    "edgisdboraprd21") oracle_sid=edsub1p    ;;
    "edgisdboraprd22") oracle_sid=edsub1p    ;;
    "edgisdboraqa11")  oracle_sid=edsub1q    ;;
    "edgisdboraqa12")  oracle_sid=edsub1q    ;;
  esac
fi

if [ "$dbtype" == 'edsubdc2' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edsub1pdg  ;;
    "edgisdboraprd02") oracle_sid=edsub1pdg  ;;
    "edgisdboraprd11") oracle_sid=edsub1pdg  ;;
    "edgisdboraprd12") oracle_sid=edsub1pdg  ;;
    "edgisdboraprd21") oracle_sid=edsub1pdg  ;;
    "edgisdboraprd22") oracle_sid=edsub1pdg  ;;
    "edgisdboraqa11")  oracle_sid=edsub1q    ;;
    "edgisdboraqa12")  oracle_sid=edsub1q    ;;
  esac
fi

if [ "$dbtype" == 'edsubpub' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edsubp1p ;;
    "edgisdboraprd02") oracle_sid=edsubp3p ;;
    "edgisdboraprd11") oracle_sid=edsubp1p ;;
    "edgisdboraprd12") oracle_sid=edsubp3p ;;
    "edgisdboraprd21") oracle_sid=edsubp1p ;;
    "edgisdboraprd22") oracle_sid=edsubp3p ;;
    "edgisdboraqa01")  oracle_sid=edsubp1q ;;
  esac
fi

if [ "$dbtype" == 'edsubpubdc1' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edsubp1p ;;
    "edgisdboraprd02") oracle_sid=edsubp1p ;;
    "edgisdboraprd11") oracle_sid=edsubp1p ;;
    "edgisdboraprd12") oracle_sid=edsubp1p ;;
    "edgisdboraprd21") oracle_sid=edsubp1p ;;
    "edgisdboraprd22") oracle_sid=edsubp1p ;;
    "edgisdboraqa01")  oracle_sid=edsubp1q ;;
  esac
fi

if [ "$dbtype" == 'edsubpubdc2' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edsubp3p ;;
    "edgisdboraprd02") oracle_sid=edsubp3p ;;
    "edgisdboraprd11") oracle_sid=edsubp3p ;;
    "edgisdboraprd12") oracle_sid=edsubp3p ;;
    "edgisdboraprd21") oracle_sid=edsubp3p ;;
    "edgisdboraprd22") oracle_sid=edsubp3p ;;
    "edgisdboraqa01")  oracle_sid=edsubp1q ;;
  esac
fi

if [ "$dbtype" == 'edsubbatch' ] ; then
  case $machine in
    "edgisdboraprd05") oracle_sid=edsubp2p ;;
    "edgisdboraprd06") oracle_sid=edsubp4p ;;
    "edgisdboraprd11") oracle_sid=edsubp2p ;;
    "edgisdboraprd12") oracle_sid=edsubp2p ;;
    "edgisdboraprd21") oracle_sid=edsubp4p ;;
    "edgisdboraprd22") oracle_sid=edsubp4p ;;
    "edgisdboraqa01")  oracle_sid=edsubp1q ;;
	"edgisdboraqa04")  oracle_sid=edsubp1q ;;
  esac
fi

if [ "$dbtype" == 'edsubbatchdc1' ] ; then
  case $machine in
    "edgisdboraprd05") oracle_sid=edsubp2p ;;
    "edgisdboraprd02") oracle_sid=edsubp2p ;;
    "edgisdboraprd11") oracle_sid=edsubp2p ;;
    "edgisdboraprd12") oracle_sid=edsubp2p ;;
    "edgisdboraprd21") oracle_sid=edsubp2p ;;
    "edgisdboraprd22") oracle_sid=edsubp2p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
  esac
fi

if [ "$dbtype" == 'edsubbatchdc2' ] ; then
  case $machine in
    "edgisdboraprd06") oracle_sid=edsubp2p ;;
    "edgisdboraprd02") oracle_sid=edsubp4p ;;
    "edgisdboraprd11") oracle_sid=edsubp4p ;;
    "edgisdboraprd12") oracle_sid=edsubp4p ;;
    "edgisdboraprd21") oracle_sid=edsubp4p ;;
    "edgisdboraprd22") oracle_sid=edsubp4p ;;
    "edgisdboraqa01")  oracle_sid=edgisp1q ;;
  esac
fi

if [ "$dbtype" == 'redline' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgisw1p    ;;
    "edgisdboraprd02") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd11") oracle_sid=edgisw1p    ;;
    "edgisdboraprd12") oracle_sid=edgisw1p    ;;
    "edgisdboraprd21") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd22") oracle_sid=edgisw1pdg  ;;
    "edgisdboraqa01")  oracle_sid=edgisw2q    ;;
    "edgisdbqa01")     oracle_sid=edgist2q    ;;
  esac
fi

if [ "$dbtype" == 'redlinedc1' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgisw1p    ;;
    "edgisdboraprd02") oracle_sid=edgisw1p    ;;
    "edgisdboraprd11") oracle_sid=edgisw1p    ;;
    "edgisdboraprd12") oracle_sid=edgisw1p    ;;
    "edgisdboraprd21") oracle_sid=edgisw1p    ;;
    "edgisdboraprd22") oracle_sid=edgisw1p    ;;
    "edgisdboraqa01")  oracle_sid=edgisw2q    ;;
    "edgisdbqa01")     oracle_sid=edgist2q    ;;
  esac
fi

if [ "$dbtype" == 'redlinedc2' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd02") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd11") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd12") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd21") oracle_sid=edgisw1pdg  ;;
    "edgisdboraprd22") oracle_sid=edgisw1pdg  ;;
    "edgisdboraqa01")  oracle_sid=edgisw2q    ;;
    "edgisdbqa01")     oracle_sid=edgist2q    ;;
  esac
fi

if [ "$dbtype" == 'lbmaint' ] ; then
  case $machine in
    "lbgisdboraprd01") oracle_sid=lbgism1p   ;;
    "lbgisdboraprd02") oracle_sid=lbgism1pdg ;;
    "edgisdboraqa01")  oracle_sid=lbgism1p   ;;
    "edgisdbqa01")     oracle_sid=lbgism1p   ;;
  esac
fi

if [ "$dbtype" == 'lbmaintdc1' ] ; then
  case $machine in
    "lbgisdboraprd01") oracle_sid=lbgism1p   ;;
    "lbgisdboraprd02") oracle_sid=lbgism1p   ;;
    "edgisdboraqa01")  oracle_sid=lbgism1p   ;;
    "edgisdbqa01")     oracle_sid=lbgism1p   ;;
  esac
fi

if [ "$dbtype" == 'lbmaintdc2' ] ; then
  case $machine in
    "lbgisdboraprd01") oracle_sid=lbgism1pdg ;;
    "lbgisdboraprd02") oracle_sid=lbgism1pdg ;;
    "edgisdboraqa01")  oracle_sid=lbgism1pdg ;;
    "edgisdbqa01")     oracle_sid=lbgism1pdg ;;
  esac
fi

if [ "$dbtype" == 'lbpub' ] ; then
  case $machine in
    "lbgisdboraprd01") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd02") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd11") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd12") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd21") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd22") oracle_sid=lbgis2p  ;;
    "edgisdboraqa01")  oracle_sid=lbgis1p  ;;
    "edgisdbqa01")     oracle_sid=lbgis1p  ;;
  esac
fi

if [ "$dbtype" == 'lbpubdc1' ] ; then
  case $machine in
    "lbgisdboraprd01") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd02") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd11") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd12") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd21") oracle_sid=lbgis1p  ;;
    "lbgisdboraprd22") oracle_sid=lbgis1p  ;;
    "edgisdboraqa01")  oracle_sid=lbgis1p  ;;
    "edgisdbqa01")     oracle_sid=lbgis1p  ;;
  esac
fi

if [ "$dbtype" == 'lbpubdc2' ] ; then
  case $machine in
    "lbgisdboraprd01") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd02") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd11") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd12") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd21") oracle_sid=lbgis2p  ;;
    "lbgisdboraprd22") oracle_sid=lbgis2p  ;;
    "edgisdboraqa01")  oracle_sid=lbgis2p  ;;
    "edgisdbqa01")     oracle_sid=lbgis2p  ;;
  esac
fi

if [ "$dbtype" == 'edschm' ] ; then
  case $machine in
	"edgisdboraprd01") oracle_sid=edscmm1p  ;;
    "edgisdboraprd02") oracle_sid=edscmm1p  ;;
    "edgisdboraprd03") oracle_sid=edscmm1p  ;;
    "edgisdboraprd04") oracle_sid=edscmm1p  ;;
  esac
fi

if [ "$dbtype" == 'edschmpub' ] ; then
  case $machine in
    "edgisdboraprd01") oracle_sid=edscmp1p  ;;
    "edgisdboraprd02") oracle_sid=edscmp2p  ;;
  esac
fi

if [ "$dbtype" == 'edaux' ] ; then
  case $machine in
    "edgisdboraqa04") oracle_sid=edgisspt  ;;
    "edgisdboraprd03") oracle_sid=edaux1p  ;;
    "edgisdboraprd04") oracle_sid=edaux1p  ;;
     "edgisdboraprd05")  oracle_sid=edaux1p  ;;  
    "edgisdboraprd06")  oracle_sid=edaux1p  ;; 
  esac
fi

if [ "$dbtype" == 'datamart' ] ; then
  case $machine in
    "edgisdboraprd05")  oracle_sid=eddatm1p ;;  
    "edgisdboraprd06")  oracle_sid=eddatm2p ;;  
    "edgisdboraqa04")  oracle_sid=eddatamq  ;;  
  esac
fi

if [ "$dbtype" == 'datamartsub' ] ; then
  case $machine in
    "edgisdboraprd05")  oracle_sid=eddats1p ;;  
    "edgisdboraprd06")  oracle_sid=eddats2p ;;  
    "edgisdboraqa04")  oracle_sid=eddatasq  ;;  
  esac
fi

#oracle_sid=`echo $oracle_sid | tr '[:upper:]' '[:lower:]'`
#echo "oracle_sid       = $oracle_sid"
#============================================================================

#============================================================================
# Exit and return password
#----------------------------------------------------------------------------
#echo "Exiting  $0."
#echo
echo $oracle_sid
exit $oracle_sid
#============================================================================
