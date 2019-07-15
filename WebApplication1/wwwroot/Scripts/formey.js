//--------------------------------------
// NOTIFICATIONS
//--------------------------------------
if (isLive) { //only run below on prod/live server

    var maxCount = 80; //finish iteration after 10 mins
    var i = 1;
    
    function notify() {
        var url = fURL + "f/NotifyTime";
        var formID = $("#Id").val();
        var logID = $("#logid").val();
        $.post(url, { FormId: formID, LogId: logID }, function (data) {
            //console.log(data);
        });
        if (i == maxCount) {
            clearInterval(refreshInterval);
            return;
        }
        i++;
    }
    var refreshInterval = setInterval(function () { notify() }, 15000);


    var interact = false;
    $('form :input').change(function () {
        if (interact == false) {
            interact = true;
            // $('#log').prepend('<p>Form changed</p>')
            var url = fURL + "/f/Interact";
            var formID = $("#Id").val();
            var logID = $("#logid").val();
            $.post(url, { FormId: formID, LogId: logID }, function (data) {
                //console.log(data);
            });
        }
    });
}



//--------------------------------------
// ON FILE UPLOAD
//--------------------------------------

function fileSelected(fileId, fieldItemName) {

    var file = document.getElementById('input_file_' + fileId).files[0];
    if (file) {
        var fileSize = 0;
        if (file.size > 1024 * 1024)
            fileSize = (Math.round(file.size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
        else
            fileSize = (Math.round(file.size * 100 / 1024) / 100).toString() + 'KB';

        $('#fileName_' + fileId).html('Name: ' + file.name);
        $('#fileSize_' + fileId).html('Size: ' + fileSize);
        $('#fileType_' + fileId).html('Type: ' + file.type);

        if (file.size > (mu * 1024 * 1024)) { //mu is in MB
            $('#error-msg-modal').html("File too big. Max file/s size is " + mu + "MB");
            $('[data-remodal-id=modalErr]').remodal().open();
        }

        supportedFormats = ['application/zip', 'application/x-zip-compressed', 'application/x-compressed-zip', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'text/plain', 'image/jpg', 'image/jpeg', 'image/gif', 'audio/mpeg', 'image/png', 'application/pdf', 'text/rtf', 'application/msword'];
        if (file) {
            if (supportedFormats.indexOf(file.type) < 0) {
                $('#error-msg-modal').html("[" + fieldItemName + "] - File type not allowed. Please upload only images or PDF/Docs");
                $('[data-remodal-id=modalErr]').remodal().open();
            }
        }
        else {
            $('#error-msg-modal').html("File type not allowed. Please upload only images or PDF/Docs");
            $('[data-remodal-id=modalErr]').remodal().open();
        }

        $(":submit").bind("click", function (e) { uploadFileProgress(); });

        // CLEAR FILE INPUT
        $("#clear_" + fileId).show(); //show clear
        //var control = $("#input_file_"+fileId);
        $("#clear_" + fileId).on("click", function () {
            //control.replaceWith(control = control.clone(true));


            $("#input_file_" + fileId).replaceWith('<input type="file" aria-required="false" aria-labelledby="label_' +
                fileId + '" name="input_file_' +
                fileId + '" id="input_file_' +
                fileId + '" onchange="fileSelected(\'' +
                fileId + '\',  \'resume\');" />');
            $("#clear_" + fileId).hide();

            $('#fileName_' + fileId).html('');
            $('#fileSize_' + fileId).html('');
            $('#fileType_' + fileId).html('');
        });
        //////////////////////

        //check TOTAL Upload Size in all file inputs
        if ($("input:file").length > 1) {
            var totalFileSize = 0;
            for (i = 0; i < $("input:file").length; i++) {
                totalFileSize += $("input:file")[0].files[0].size;
            }
            if (totalFileSize > (mu * 1024 * 1024)) {
                $('#error-msg-modal').html("Max combined file size is too big, allowance is" + mu + "MB in total.");
                $('[data-remodal-id=modalErr]').remodal().open();
            }
        }
    }
}
function uploadFileProgress() {
    if (instance.isValid()) {
        $('[data-remodal-id=modalProgress]').remodal().open();

        var pb = document.getElementById("progress-msg-modal"); //get animation working in IE
        pb.innerHTML = '<img src="' + fURL + '/Content/images/getdata.gif" width="128" height="15" />';
        pb.style.display = '';
    }
}