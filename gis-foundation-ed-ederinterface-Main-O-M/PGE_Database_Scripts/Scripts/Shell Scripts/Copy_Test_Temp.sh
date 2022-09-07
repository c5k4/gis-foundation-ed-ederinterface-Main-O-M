#!/bin/sh
        spawn scp  /u02/uc4/edgisp2p/export/edgisp2p_2015_09_13_03_49_40.log /u02/uc4/edgisp2p/import
        set pass "itgis!1234"
        expect {
        password: {send "$pass\r"; exp_continue}
                  }

