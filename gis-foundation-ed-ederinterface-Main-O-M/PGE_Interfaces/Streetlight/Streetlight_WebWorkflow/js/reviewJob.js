function reviewJob(evt) {
    var userComments = document.getElementById("txtReviewComments").value
    var approveRejectFlag;
    var jobDesc = userComments;
    switch(evt.id){
        case "btnProceed":
    approveRejectFlag = "Y";
    break;
case "btnApprove":
    approveRejectFlag = "Y";
    break;
case "btnReject":
    approveRejectFlag = "N";
    break;
    }
    dojo.xhrPost({
        url: "handler/ETViewerHandler.ashx",
        content: { module: "Review",
            jobID: jobID,
            comments: userComments,
            approveRejectFlag: approveRejectFlag,
            userLanId: userLanId,
            jobDesc: jobDesc,
            fromTaskID: fromTaskIDTransaction
        },
        sync: true,
        load: function (result) {
            var isJobReviewed = dojo.fromJson(result, true);
            var rectNode = document.getElementById(fromTaskIDTransaction);
            if (isJobReviewed == true) {
                switch (evt.id) {
                    case "btnProceed":
                        alert("Job: " + jobID + " has been reviewed.");
                        rectNode.setAttributeNS(null, "fill", "#34A853");
                        break;
                    case "btnApprove":
                        alert("Job: " + jobID + " has been approved.");
                        rectNode.setAttributeNS(null, "fill", "#34A853");
                        break;
                    case "btnReject":
                        alert("Job: " + jobID + " has been rejected.");
                        rectNode.setAttributeNS(null, "fill", "#ff1a1a");
                        break;
                }               
                rectNode.setAttributeNS(null, "stroke", "");
                disable("");
                getGridData();
            }
            else {
                alert("An error occurred. Please try after sometime.");
            }
        }
    });
}

function Clear() {
    document.getElementById("txtReviewComments").value = "";
    document.getElementById("txtJobInfoFiles").value = "";
    document.getElementById("txtJobDesc").value = "";
    document.getElementById("txtComments").value = "";
    document.getElementById("txtPgdbFiles").value = "";
    document.getElementById('fileInputPgdb').value = "";
    document.getElementById('fileInputJobInfo').value = "";
    previousSelMdbFiles = [];
}