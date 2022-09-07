var jobData = null;
document.getElementById("txtPgdbFiles").value = "";
document.getElementById("txtJobInfoFiles").value = "";
require([
            'dojo/_base/declare',
            'dstore/Memory',
            'dgrid/Grid',
            'dgrid/extensions/Pagination',
            'dojo/domReady!',

		], function (declare,Memory, Grid, Pagination) {
		    userData = {};
		    configMsgJson = null;
		    try {
		        dojo.xhrPost({
		            url: "handler/ETViewerHandler.ashx",
		            content: { module: "User" },
		            sync: true,
		            load: function (result) {
		                jobData = dojo.fromJson(result, true);
		                if (jobData.Dashboard[0].ErrorMsg == undefined) {
		                    var strRoleName = "";
		                    if (jobData.UserRole != undefined) {
		                        for (var i = 0; i < jobData.UserRole.length; i++) {
		                            strRoleName = strRoleName + jobData.UserRole[i].ROLENAME + ",";
		                        }
		                        strRoleName = strRoleName.substring(0, strRoleName.length - 1);

		                    } else {
		                        for (var i = 0; i < jobData.Dashboard.length; i++) {
		                            strRoleName = strRoleName + jobData.Dashboard[i].ROLENAME + ",";
		                        }
		                        strRoleName = strRoleName.substring(0, strRoleName.length - 1);
		                    }
		                    userData = {		                        
		                        "userLanId": jobData.Dashboard[0].LANID
		                    }
		                    userLanId = jobData.Dashboard[0].LANID;
		                    document.getElementById("pgeUser").innerText = "Name: " + userData.userLanId + "|" + "Role: " + strRoleName;
		                    bindGrid(jobData);
		                } else {
		                    var errMsg = jobData.Dashboard[0].ErrorMsg;
		                    var displayMessage;
		                    switch (errMsg) {
		                        case "INVALIDUSER":
		                            displayMessage = "You are not authorised to access Web Workflow Manager.";
		                            var uri = "displayMsg=" + displayMessage;
		                            var uri_enc = encodeURIComponent(uri);
		                            window.location.href = "invalidUser.htm?" + uri_enc;
		                            break;
		                        case "AUTHENTICATIONFAILED":
		                            displayMessage = "Authentication Failed.";
		                            var uri = "displayMsg=" + displayMessage;
		                            var uri_enc = encodeURIComponent(uri);
		                            window.location.href = "invalidUser.htm?" + uri_enc;
		                            break;
		                    }
		                }
		            }
		        });

		    } catch (ex) {

		    }
		});


		function bindGrid(jobData) {
		    require([
            'dojo/_base/declare',
            'dstore/Memory',
            'dgrid/Grid',
            'dgrid/extensions/Pagination',
            'dgrid/Keyboard',
            'dgrid/Selection',      
            'dojo/domReady!',

		], function (declare, Memory, Grid, Pagination, Keyboard, Selection) {
		    var userJobData
		    if (jobData.Dashboard[0].JOBCOUNT == "0") {
		        userJobData = [];
		    } else {
		        userJobData = jobData.Dashboard;
		    }
		 
		    var dashBoardNode = document.getElementById('DashBoard');
		    while (dashBoardNode.hasChildNodes()) {
		        dashBoardNode.removeChild(dashBoardNode.lastChild);
		    }
		    var divDbGrid = document.createElement("div")
		    divDbGrid.setAttribute("id", "dbGrid");
		    dashBoardNode.appendChild(divDbGrid);

		    var dbGridStyle = document.getElementById("DashBoard").style.display;
		    document.getElementById("DashBoard").style.display = "block";
		    var dbGrid = new (declare([Grid, Pagination, Keyboard, Selection]))({
		        collection: new Memory({ data: userJobData, idProperty: "ID" }), // added by Sarwjit.		          
		        className: 'dgrid-autoheight',
		        pagingLinks: 1,
		        rowsPerPage: 15,
		        pagingTextBox: true,
		        firstLastArrows: true,
		        pageSizeOptions: [15, 20, 30],
		        height: 380,
		        noDataMessage: "No Jobs Available.",
		        columns: {
		            //ID:  'Id',                
		            ID: 'S.No', //show
		            REL_JOBID: 'Job ID', // show
		            JOBDESC: "Job Description",  //show
		            REL_TASKID: 'Task ID', //show
		            TASKNAME: { label: "Task Name", formatter: makeHyperlink }, // show
		            ROLENAME: "Role Name", //hide
		            TASKDESC: "Task Desc", //hide
		            ROLEDESC: "Role", //hide
		            TXDATE: "Date", // show		                
		            ROLEID: "Role ID", //hide	               		               
		            SUBMITTER: "Lan ID"  //hide
		        }
		    }, 'dbGrid');
		    dbGrid.styleColumn("ROLEID", "display: none;");		  
		    dbGrid.styleColumn("ROLEDESC", "display: none;");
		    dbGrid.styleColumn("TASKDESC", "display: none;");
		    dbGrid.styleColumn("SUBMITTER", "display: none;");
		   
		    dbGrid.startup();
		    document.getElementById("DashBoard").style.display = dbGridStyle;

		    dbGrid.on('a.hyperlinkClass:click', function (evt) {
		        var cell = dbGrid.row(evt);
		        var data = cell.data;
		        jobID = data.REL_JOBID;
		        TabClick = "Dashboard";
		        fromTaskIDTransaction = data.REL_TASKID;
		        document.getElementById('txtReviewComments').value = "";
		        ClearLastSelColronWF();
		        getColorData();
		        displaySelWorkflowData(data.REL_TASKID, "WFStep");
		        DisplayMainJob(data);
		        document.getElementById('divMJWorkflow').appendChild(document.getElementById("divWorkflow"));
		        document.getElementById("mainJobTab").style.display = "block";
		        openTab(null, "MainJob");
		    });

		  
		    var historyNode = document.getElementById('History');
		    while (historyNode.hasChildNodes()) {
		        historyNode.removeChild(historyNode.lastChild);
		    }

		    var divHistGrid = document.createElement("div")
		    divHistGrid.setAttribute("id", "histGrid");
		    historyNode.appendChild(divHistGrid);

		    document.getElementById("History").style.display = "block";
		    var histGrid = new (declare([Grid, Pagination]))({
		        collection: new Memory({ data: jobData.JobHistory, idProperty: "ID" }), // added by Sarwjit
		        className: 'dgrid-autoheight',
		        pagingLinks: 1,
		        rowsPerPage: 15,
		        pagingTextBox: true,
		        firstLastArrows: true,
		        pageSizeOptions: [15, 20, 30],
		        height: 380,
		        noDataMessage: "No Jobs Available.",
		        columns: {
		            ID: 'S.No', //show
		            REL_JOBID: { label: "Job ID", formatter: makeHyperlink }, //show
		            JOBDESC: "Job Description", //show		            
		            JOBSTATUS: "Job Status", //show		           
		            STARTDATE: "Start Date",
		            ENDDATE: "End Date",
		            SUBMITTER: "Lan ID" //hide
		        }
		    }, 'histGrid');

		  
		    histGrid.styleColumn("SUBMITTER", "display: none;");
		   
		    histGrid.startup();
		    document.getElementById("History").style.display = "none";

		    histGrid.on('a.hyperlinkClass:click', function (evt) {
		        var cell = histGrid.row(evt);
		        var data = cell.data;
		        jobID = data.REL_JOBID;
		        TabClick = "History";
		        document.getElementById('txtReviewComments').value = "";
		        ClearLastSelColronWF();
		        var rectNode = document.getElementById("1");
		        rectNode.setAttributeNS(null, "stroke", "#Fbbc05");
		        rectNode.setAttributeNS(null, "stroke-width", "4");
		        lastTaskId = "1";
		        getColorData();
		        displaySelWorkflowData("1", "HistGrid");
		        DisplayMainJob(data);		      
		        document.getElementById('divMJWorkflow').appendChild(document.getElementById("divWorkflow"));
		        document.getElementById("mainJobTab").style.display = "block";
		        openTab(null, "MainJob");

		    });

		    function makeHyperlink(data) {
		        return '<a class="hyperlinkClass" href="#">' + data + '</a>'
		    }
		});
		}


		function openTab(evt, tab) {
		    if (tab == "NewJob") {
		        TabClick = null;
		        var isUserAuthorised = false;
		        if (jobData.UserRole != undefined) {
		            for (var i = 0; i < jobData.UserRole.length; i++) {
		                if (jobData.UserRole[i].ROLE == "1" || jobData.UserRole[i].ROLE == "7") {
		                    isUserAuthorised = true;
		                    break;
		                }
		            }
		        } else {
		            for (var i = 0; i < jobData.Dashboard.length; i++) {
		                if (jobData.Dashboard[i].ROLEID == "1" || jobData.Dashboard[i].ROLE == "7") {
		                    isUserAuthorised = true;
		                    break;
		                }
		            }
		        }

		        if (isUserAuthorised) {
		            $("#newJobInitiatediv").append($("#divInitiateJob"));
		            ClearLastSelColronWF();
		            setColor("GenericFlow", "1");
		            Enable("1");
		            $("#divNJWorkflow").append($("#divWorkflow"));
		            document.getElementById("divInitiateJob").style.display = "block";
		        } else {
		            alert("You are not authorised to create New Job");
		            return;
		        }
		    }
		    var i, x, tablinks;
		    x = document.getElementsByClassName("initTab");
		    for (i = 0; i < x.length; i++) {
		        x[i].style.display = "none";
		    }
		    tablinks = document.getElementsByClassName("tablink");
		    for (i = 0; i < x.length; i++) {
		        tablinks[i].className = tablinks[i].className.replace(" w3-orange", "");
		    }
		    document.getElementById(tab).style.display = "block";
		   
		    switch (tab) {
		        case "Dashboard":
		            TabClick = tab;
		            break;
		        case "History":
		            TabClick = tab;
		            break;

		    }
		    if (evt == null) {
		        tablinks[3].className += " w3-orange";
		    }
		    else {
		        evt.currentTarget.className += " w3-orange";
		        document.getElementById("mainJobTab").style.display = "none";
		        AppendInitDivinNJDiv();
		    }
		}	    
	

		function AppendInitDivinNJDiv() {
		    $("#newJobInitiatediv").append($("#divInitiateJob"));
		    $('#mainJobInitiatediv').find('#divInitiateJob').remove();
		}


		function setColor(calledFrom,data) {
		    var rectNode; 
		    var pendingTaskArray = [];
		    switch (calledFrom) {
		        case "Grid":
		            if (taskColorData.COMPLETEDTASK.length > 0) {
		                for (var i = 0; i < taskColorData.COMPLETEDTASK.length; i++) {
		                    rectNode = document.getElementById(taskColorData.COMPLETEDTASK[i].TASKID);
		                    rectNode.setAttributeNS(null, "fill", "#34A853");    // green Color
		                }
		            }
		            if (taskColorData.ACTIVETASK.length > 0) {
		                for (var i = 0; i < taskColorData.ACTIVETASK.length; i++) {
		                    rectNode = document.getElementById(taskColorData.ACTIVETASK[i].REL_TASKID);
		                    rectNode.setAttributeNS(null, "fill", "#Fbbc05");   // Yellow COde 
		                    pendingTaskArray.push(taskColorData.ACTIVETASK[i].REL_TASKID);
		                }
		                var maxTaskID = getMaxOfArray(pendingTaskArray);

		                function getMaxOfArray(numArray) {
		                    return Math.max.apply(null, numArray);
		                }
		                for (var count = 0; count < config.Divdisplay.length; count++) {
		                   
		                    if (parseInt(config.Divdisplay[count].TaskID) > parseInt(maxTaskID)) {	                   
		                        rectNode = document.getElementById(config.Divdisplay[count].TaskID);
		                        rectNode.setAttributeNS(null, "fill", "gray");  // gray
		                    }

		                }

		            }
		            break;
		        case "GenericFlow":

		            for (var count = 0; count < config.Divdisplay.length; count++) {
		                rectNode = document.getElementById(config.Divdisplay[count].TaskID);

		                if (config.Divdisplay[count].TaskID == data) {
		                    rectNode.setAttributeNS(null, "fill", "#Fbbc05");   // Yellow COde 

		                }
		                else if (parseInt(config.Divdisplay[count].TaskID) < parseInt(data)) {
		                    rectNode.setAttributeNS(null, "fill", "#34A853");    // green Color
		                }
		                else {

		                    rectNode.setAttributeNS(null, "fill", "gray");  // gray
		                }
		            }
		            break;                
		    }		   
		}

		function setDiv(data,calledFrom) {
		    var getCounterval;
		    var isUserAuthorised = false;
		    document.getElementById('divReview').style.display = 'none';
		    document.getElementById('divGISProcess').style.display = 'none';
		    for (var divcounter = 0; divcounter < config.Divdisplay.length; divcounter++) {
		        if (config.Divdisplay[divcounter].TaskID == data) {
		            switch (calledFrom) {
		                case "WFStep":
		                    var item = document.getElementById("divInitiateJob");
		                    if (item.parentNode.id == "mainJobInitiatediv") {
		                        AppendInitDivinNJDiv();
		                    }
		                    if (data == "1") {
		                        $("#mainJobInitiatediv").append($("#divInitiateJob"));
		                    }
		                    break;
		                case "HistGrid":
		                    $("#mainJobInitiatediv").append($("#divInitiateJob"));
		                   
		                    break;
		            }

		            if (config.Divdisplay[divcounter].TaskID == "2" || config.Divdisplay[divcounter].TaskID == "4.2" || config.Divdisplay[divcounter].TaskID == "5.2") {
		                if (config.Divdisplay[divcounter].TaskID == AssignedTaskIDfromHist) {
		                    document.getElementById('lblGISProcess').innerHTML = 'GIS process in progress....';
		                    document.getElementById(config.Divdisplay[divcounter].divname[0]).style.display = 'block';
		                } else {
		                    document.getElementById(config.Divdisplay[divcounter].divname[1]).style.display = 'block';
		                }

		            } else {
		                document.getElementById(config.Divdisplay[divcounter].divname).style.display = 'block';
		            }
		            getCounterval = divcounter;
		        }
       
		    }

		    if (data == AssignedTaskIDfromHist) {
		        if (jobData.UserRole != undefined) {
		            for (var i = 0; i < jobData.UserRole.length; i++) {
		                if (jobData.UserRole[i].ROLE == assignTaskOwnerRole || jobData.UserRole[i].ROLE =="7") {
		                    isUserAuthorised = true;
		                    break;
		                }
		            }
		        } else {
		            for (var i = 0; i < jobData.Dashboard.length; i++) {
		                if (jobData.Dashboard[i].ROLE == assignTaskOwnerRole || jobData.Dashboard[i].ROLE == "7") {
		                    isUserAuthorised = true;
		                    break;
		                }
		            }
		        }

		        if (isUserAuthorised) {
		            Enable("");
		        } else {
		            disable("");
		        }

		    } else {
		        if (data == "1") {
		            disable("1");
		        } else {
		            disable("");
		        }
		    }       
		    		   
		   
		    switch (config.Divdisplay[getCounterval].btnname) {
		        case "P":
		            document.getElementById('btnProceed').style.display = 'inline-block';
		            document.getElementById('btnApprove').style.display = 'none';
		            document.getElementById('btnReject').style.display = 'none';
		            document.getElementById('btnCancel').style.display = 'inline-block';
		            //code block
		            break;
		        case "A":
		            document.getElementById('btnProceed').style.display = 'none';
		            document.getElementById('btnApprove').style.display = 'inline-block';
		            document.getElementById('btnReject').style.display = 'inline-block';
		            document.getElementById('btnCancel').style.display = 'inline-block';
		            break;
		        case "":
		            document.getElementById('btnProceed').style.display = 'none';
		            document.getElementById('btnApprove').style.display = 'none';
		            document.getElementById('btnReject').style.display = 'none';
		            document.getElementById('btnCancel').style.display = 'none';
		    } 
		}

		function disable(TaskID) {
		    switch (TaskID) {
		        case "":
		            document.getElementById('jobid').disabled = true;
		            document.getElementById('desc').disabled = true;
		            document.getElementById('lanid').disabled = true;
		            document.getElementById('txtReviewComments').disabled = true;
		            document.getElementById('btnProceed').disabled = true;
		            document.getElementById('btnApprove').disabled = true;
		            document.getElementById('btnReject').disabled = true;
		            document.getElementById('btnCancel').disabled = true;
		            break;
		        case "1":
		            document.getElementById('txtJobInfoFiles').disabled = true;
		            document.getElementById('txtJobDesc').disabled = true;
		            document.getElementById('txtComments').disabled = true;
		            document.getElementById('txtPgdbFiles').disabled = true;
		            document.getElementById('btnInitiate').disabled = true;
		            document.getElementById('btnInitiateCancel').disabled = true;
		            document.getElementById('btnJobInfoBrws').disabled = true;
		            document.getElementById('btnPgdbBrws').disabled = true;
		            break;
		       
		    } 	   
		}

		function Enable(TaskID) {
		    document.getElementById('txtReviewComments').value = "";
		    document.getElementById('txtJobInfoFiles').value = "";
		    document.getElementById('txtJobDesc').value = "";
		    document.getElementById('txtComments').value = "";
		    document.getElementById('txtPgdbFiles').value = "";
		    switch (TaskID) {
		        case "":
		            document.getElementById('txtReviewComments').disabled = false;
		            document.getElementById('jobid').enabled = false;
		            document.getElementById('desc').disabled = false;
		            document.getElementById('lanid').disabled = false;
		            document.getElementById('btnProceed').disabled = false;
		            document.getElementById('btnApprove').disabled = false;
		            document.getElementById('btnReject').disabled = false;
		            document.getElementById('btnCancel').disabled = false;
		            break;
		        case "1":
		            document.getElementById('txtJobInfoFiles').disabled = false;
		            document.getElementById('txtJobDesc').disabled = false;
		            document.getElementById('txtComments').disabled = false;
		            document.getElementById('txtPgdbFiles').disabled = false;
		            document.getElementById('btnInitiate').disabled = false;
		            document.getElementById('btnInitiateCancel').disabled = false;
		            document.getElementById('btnJobInfoBrws').disabled = false;
		            document.getElementById('btnPgdbBrws').disabled = false;
		            break;
		    }
		}
		   
	
		function DisplayMainJob(data) {		   
		    document.getElementById('jobid').innerHTML = "";
		    document.getElementById('desc').innerHTML = "";
		    document.getElementById('lanid').innerHTML = "";		    
		    document.getElementById('jobid').innerHTML = "JobID:  "+ data.REL_JOBID;		   
		    document.getElementById('lanid').innerHTML = "Submitter:  " + data.SUBMITTER;
		    if (data.JOBDESC == null || data.JOBDESC == "null") {
		        document.getElementById('txtJobDesc').value = "";
		        document.getElementById('desc').innerHTML = "Job Description:  ";
             }
		    else {
		        document.getElementById('txtJobDesc').value = data.JOBDESC;
		        document.getElementById('desc').innerHTML = "Job Description:  " + data.JOBDESC;
            }
		  
		}

		function displaySelWorkflowData(taskID,calledFrom) {
		    dojo.xhrPost({
		        url: "handler/ETViewerHandler.ashx",
		        content: { module: "MainJob",
		            jobID: jobID,
		            taskID: taskID,
		            callingFlag: "WFSteps"
		        },
		        sync: true,
		        load: function (result) {
		            jobCommentsData = dojo.fromJson(result, true);
		            AssignedTaskIDfromHist = null;
		            document.getElementById('txtReviewComments').value = "";
		            document.getElementById('txtComments').value = "";
		            if (jobCommentsData.COMPLETEDTASK.length > 0) {
		                if (jobCommentsData.COMPLETEDTASK[0].COMMENTS == null) {
		                    document.getElementById('txtReviewComments').value = "";
		                }
		                else {

		                    document.getElementById('txtReviewComments').value = jobCommentsData.COMPLETEDTASK[0].COMMENTS;
		                }
		                if (taskID == "1") {
		                    if (jobCommentsData.COMPLETEDTASK[0].COMMENTS == null) {
		                        document.getElementById('txtComments').value = "";
		                    } else {
		                        document.getElementById('txtComments').value = jobCommentsData.COMPLETEDTASK[0].COMMENTS;
		                    }
		                    var txDataArray = jobCommentsData.COMPLETEDTASK[0].TXDATA.split(';');
		                    var mdbFiles = "";
		                    var mdbFilesArray = txDataArray[0].split(',');
		                    for (var i = 0; i < mdbFilesArray.length; i++) {
		                        mdbFiles = mdbFiles + mdbFilesArray[i] + "\n";
		                    }

		                    document.getElementById('txtPgdbFiles').value = mdbFiles;
		                    if (txDataArray.length == 1) {
		                        document.getElementById('txtJobInfoFiles').value = "";
		                    } else {
		                        document.getElementById('txtJobInfoFiles').value = txDataArray[1];
		                    }
		                    	                
		                }

		                approveRejectFlag = jobCommentsData.COMPLETEDTASK[0].RESULT;
		            }
		            if (jobCommentsData.ACTIVETASK.length > 0) {
		                AssignedTaskIDfromHist = jobCommentsData.ACTIVETASK[0].ASSIGNEDTASK;
		                fromTaskIDTransaction = AssignedTaskIDfromHist;
		                assignTaskOwnerRole = jobCommentsData.ACTIVETASK[0].TASKOWNERROLE;
		            }

		            if (jobCommentsData.MAXASSIGNEDTASK.length > 0) {
		                CurrentTaskIDfrmDB = jobCommentsData.MAXASSIGNEDTASK[0].MAXTASKID;
		            }
		            setDiv(taskID, calledFrom);
		        }
		    });
		}

		function getColorData(data) {
		    dojo.xhrPost({
		        url: "handler/ETViewerHandler.ashx",
		        content: { module: "MainJob",
		            jobID: jobID,
		            callingFlag: "histGrid"
		        },
		        sync: true,
		        load: function (result) {
		            jobCommentsData = dojo.fromJson(result, true);
		            taskColorData = jobCommentsData;
		            if (jobCommentsData.COMPLETEDTASK.length > 0) {
		                for (var i =0; i < jobCommentsData.COMPLETEDTASK.length; i++) {
		                    if (jobCommentsData.COMPLETEDTASK[i].TASKID == "1") {
		                        document.getElementById('txtComments').value = jobCommentsData.COMPLETEDTASK[i].COMMENTS;
		                    }
		                }
		            }
		            else {
		                document.getElementById('txtReviewComments').value = "";
		                document.getElementById('txtComments').value = "";
		            }
		            setColor("Grid");	           
		        }
		    });
		}

		function ClearLastSelColronWF() {
		    if (lastTaskId != '') {
		        var prevSelectedNode = document.getElementById(lastTaskId);		        
		        prevSelectedNode.setAttributeNS(null, "stroke", "");
		    }
		}
        var taskColorData
		var config = config;
		var jobID;
		var CurrentTaskIDfrmDB;
		var AssignedTaskIDfromHist;
		var assignTaskOwnerRole;
		var lastTaskId = '';
		var TabClick = 'Dashboard';
		var fromTaskIDTransaction;
		var taskResult;
		var userLanId;