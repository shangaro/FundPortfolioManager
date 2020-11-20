// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


// Write your JavaScript code.
$(document).ready(function () {

    getFiles();
    var progressbarContainer = $(".progress-bar-container");
    progressbarContainer.hide();
    var progressbarStick = $("#progress-bar-stick"),
        progressLabel = $(".progress-bar-label");
    progressbarStick.progressbar({
        value: false,
        change: function () {
            progressLabel.text(progressbarStick.progressbar("value") + "%");
        },
        complete: function () {
            progressLabel.text("Complete!");
        }
    });

    //function progress() {

    //    var val = progressbarStick.progressbar("value") || 0;
    //    progressbarStick.progressbar("value", val + 2);

    //    if (val < 99) {
    //        setTimeout(progress, 80);
    //    }
        
    //}
    //setTimeout(progress, 2000);

    var fileCount = 0;
    $("#upload-file").on('change', function (e) {

        console.log("* file", e.target.files);
        fileCount = parseInt(e.target.files.length);
        
    });
    var uploadForm = $("#upload-form");
    var uploadBtn = $("#upload-button");
    uploadForm.on('submit', function (event) {
        event.preventDefault();
        if (fileCount > 0) {
            console.log("button clicked");
            console.log("**form data",$(this)[0])
            uploadBtn.attr("disabled", "disabled");
            progressbarContainer.show();
            $.ajax({
                type: 'POST',
                data: new FormData($(this)[0]),
                url: "/File/FileUploads",
                contentType: false,
                cache: false,
                processData:false,
                error: function (err) {
                    console.log(err);

                },
                success: function (json) {
                    //var data = JSON.parse(JSON.stringify(json));
                    console.log("**data is", json);
                    progressbarStick.progressbar("value", 100);
                    var pdfTable = $("#pdfTable").DataTable();
                    $.ajax(
                        {
                            type: 'POST',
                            data: $("#pdfTable").serialize(),
                            contentType: "application/x-www-form-urlencoded",
                            cache: false,
                            url: "/File/GetFiles",
                            success: function (msg) {
                                console.log("**returned from server", msg);
                                pdfTable.clear();
                                pdfTable.rows.add(msg);
                            },
                            error: function (err) {
                                return err.statusText;
                            }

                        });
                    
                   
                }
                

            });
            var intervalId = setInterval(_ =>
            {
                let val = progressbarStick.progressbar("value") || 0;
                progressbarStick.progressbar("value", val + 2);
                console.log("*val", val);
                if (val === 100) {
                    
                    clearInterval(intervalId);
                    setTimeout(_ => {
                        progressbarStick.progressbar("value", 0);
                        uploadBtn.removeAttr("disabled");  
                        progressbarContainer.hide();
                    }, 2000);
                } 
            }, 250);
            
           
        }
        
        
        
    });

    

    
});


