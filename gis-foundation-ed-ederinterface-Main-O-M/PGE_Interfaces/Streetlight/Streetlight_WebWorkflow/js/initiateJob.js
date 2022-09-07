var jobData = null;
var previousSelMdbFiles = [];  
document.getElementById('btnInitiate').addEventListener("click", initiateJob);
document.getElementById('fileInputPgdb').addEventListener("click", fileInputClick);
document.getElementById('fileInputJobInfo').addEventListener("click", fileInputClick);

function fileInputClick(evt) {
    var textareaId;
    switch (evt.target.id) {
        case "fileInputPgdb":
            textareaId = "txtPgdbFiles";
            break;
        case "fileInputJobInfo":
            textareaId = "txtJobInfoFiles";
            break;
    }
    if (document.getElementById(textareaId).value == "") {
        var target = evt.target || evt.srcElement;
        target.value = "";
    }
}
function loadFile(evt) {
    var fileInput;
    var textareaID; 
    var isFileNumExceedLimit = false;   
    switch (evt.id) {
        case "btnPgdbBrws":
            fileInput = document.getElementById('fileInputPgdb');
            textareaID = "txtPgdbFiles";
            break;
        case "btnJobInfoBrws":
            fileInput = document.getElementById('fileInputJobInfo');
            textareaID = "txtJobInfoFiles";
            break;
    }
    fileInput.click();
           
          for (var i = 0; i <= fileInput.files.length - 1; i++) {
              var fname = fileInput.files.item(i).name; // THE NAME OF THE FILE. 
              var fileExtension =  fname.split(".")[1];                    
              // SHOW THE EXTRACTED DETAILS OF THE FILE.
              if (evt.id == "btnJobInfoBrws") {
                  if (fileExtension == "doc" || fileExtension == "docx" || fileExtension == "pdf" || fileExtension == "xls" || fileExtension == "xlsx") {
                      if (document.getElementById(textareaID).value == "") {
                          document.getElementById(textareaID).value = fname;
                      }
                     
                  } else {
                      alert("Please select either word document or pdf file.")
                      return;
                  }
              } else if (evt.id == "btnPgdbBrws") {
                  
                  if (fname.split(".")[1] == "mdb") {
                      if (document.getElementById(textareaID).value == "") {
                          document.getElementById(textareaID).value = fname;
                          previousSelMdbFiles.push(fname);
                      } else {
                          if (previousSelMdbFiles.length < 10) {
                              if (previousSelMdbFiles.indexOf(fname) < 0) {
                                  document.getElementById(textareaID).value = document.getElementById(textareaID).value + '\n' + fname;
                                  previousSelMdbFiles.push(fname);
                              }
                          } else {
                            alert("Maximum 10 files can be selected.");
                            return;
                          }
                      }                                    
                     
                      
                  } else {
                      alert("Please select only .mdb file.")
                      return;
                  }
              }
          }         
}

function initiateJob() {
    // GET THE FILE INPUT.
    var fileInput = document.getElementById('fileInputPgdb');
    var jonInfoFileIP = document.getElementById('fileInputJobInfo');
    if (fileInput.files.length > 0) {
        if (jonInfoFileIP.files.length > 0) {
            _value = fileInput.value;
            var txData = "";
            var fullPath = _value;
            if (fullPath) {
                for (var i = 0; i < fullPath.split(',').length; i++) {
                    var fileFullPath = fullPath.split(',')[i];
                    var startIndex = (fileFullPath.indexOf('\\') >= 0 ? fileFullPath.lastIndexOf('\\') : fileFullPath.lastIndexOf('/'));
                    var filename = fileFullPath.substring(startIndex);
                    if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                        filename = filename.substring(1);
                    }
                    if (i == (fullPath.split(',').length - 1)) {
                        txData = txData + filename + ";";
                    } else {
                        txData = txData + filename + ",";
                    }
                }
            }


            // VALIDATE OR CHECK IF ANY FILE IS SELECTED.
            if (jonInfoFileIP.files.length > 0) {
                _value = _value + "," + jonInfoFileIP.value;
                var docPath = jonInfoFileIP.value;
                var startIndex = (docPath.indexOf('\\') >= 0 ? docPath.lastIndexOf('\\') : docPath.lastIndexOf('/'));
                var filename = docPath.substring(startIndex);
                if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                    filename = filename.substring(1);
                }
                txData = txData + "" + filename;
            }

            try {
                var jobDesc = document.getElementById("txtJobDesc").value;
                var comments = document.getElementById("txtComments").value;
                dojo.xhrPost({
                    url: "handler/ETViewerHandler.ashx",
                    content: {
                        module: "NewJob",
                        postData: _value,
                        jobDesc: jobDesc,
                        comments: comments,
                        txData: txData

                    },
                    sync: true,
                    load: function (result) {
                        var fileSuccessCount = null;
                        var alertString;
                        var strFileStatus = dojo.fromJson(result, true);

                        if (strFileStatus.length > 0) {
                            if (strFileStatus.length == 1) {
                                if (strFileStatus[0].split(',')[0] == "FILELOCKED") {
                                    var answer = confirm("Files are(is) being used by another process. Please close all files. \n If you want to Retry click on 'OK' button.");
                                    if (answer == true) {
                                        initiateJob();
                                    }
                                    else {
                                        return;
                                    }
                                }
                                else if (strFileStatus[0].split(',')[0] == "FILEEXIST") {
                                    alert("Cannot move a file when that file already exists.");
                                    return;
                                }
                                else if (strFileStatus[0].split(',')[0] == "ERROR") {
                                    alert("An error occurred. Please contact your admin.");
                                    return;
                                }
                                else if (strFileStatus[0].split(',')[0] == "NOFILE") {
                                    alert("Access denied on source or target location. Please contact to system administrator.");
                                    return;
                                }
                            }


                            for (var i = 0; i < (strFileStatus.length - 1); i++) {
                                if (strFileStatus[i].split(',')[1] == "1") {
                                    fileSuccessCount++;
                                }
                            }
                            if (fileSuccessCount == (strFileStatus.length - 1)) {
                                getGridData();
                                //Get the Job ID from last index of LIst and display in alert message
                                
                                var strJobID = strFileStatus[strFileStatus.length - 1];
                                alertString = "Job : " + strJobID + " has been initiated successfully.";
                                document.getElementById('txtPgdbFiles').value = "";
                                document.getElementById('txtJobInfoFiles').value = "";
                                document.getElementById('txtJobDesc').value = "";
                                document.getElementById('txtComments').value = "";

                            } else {
                                alertString = "Error occurred while moving file(s). Job cannnot be created.";
                                document.getElementById('txtPgdbFiles').value = "";
                                document.getElementById('txtJobInfoFiles').value = "";
                                document.getElementById('txtJobDesc').value = "";
                                document.getElementById('txtComments').value = "";
                            }
                            alert(alertString);

                        }
                    }
                });
            } catch (e) {
                
            }
        } else {
            alert('Please Job Information document.')
         }

    } else {
    alert('Please select mdb files.');
    }
}



function getGridData() {
    userData = {};
    jobData = null;
    configMsgJson = null;
    try {
        dojo.xhrPost({
            url: "handler/ETViewerHandler.ashx",
            content: { module: "User" },
            sync: true,
            load: function (result) {
                jobData = dojo.fromJson(result, true);
                if (jobData.Dashboard[0].ErrorMsg == undefined) {                
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
}
	


