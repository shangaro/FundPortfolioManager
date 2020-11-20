
// Initiliaze table and get files row from s3
function getFiles() {
    var form = $("#pdfTable").serialize();
    $('#pdfTable').DataTable({
        columnDefs: [
            { "width": "", "targets": [0] },
            { "className": "text-center custom-middle-align", "targets": [0, 1, 2, 3] },


        ],
        language:
        {
            processing: "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },

        processing: true,
        serverSide: true,
        ajax: {

            url: "/File/GetFiles",
            type: "POST",
            dataType: "JSON",
            contentType: "application/x-www-form-urlencoded",
            data: form,
           
            //dataSrc: function (json) {
            //    //debugger;
            //    console.log(json);
            //    if (json.result == "Error") {
            //        alert(data.message);
            //    }
            //    //let jsonObj = $.parseJSON(JSON.stringify(json));
            //    //data

            //    return json;
            //}
        },
        columns: [
            { data: "guid" },
            { data: "name" },
            { data: "status" },
            { data: "eTag" }

        ]


    });
}


