$(document).ready(function () {
    $("#button1").click(function (evt) {

        var files = $("#file1").get(0).files;
        if (files.length > 0) {
            var data = new FormData();
            for (i = 0; i < files.length; i++) {
                data.append("file" + i, files[i]);
            }
            $.ajax({
                type: "POST",
                url: "/api/uploadfile",
                contentType: false,
                processData: false,
                data: data,
                success: function (messages) {
                    for (i = 0; i < messages.length; i++) {
                        alert(messages[i]);
                    }
                },
                error: function () {
                    alert("Error while invoking the Web API");
                }
            }); 
        }
    });
});

$(document).ready(function ($) {
    $(document).on("click", ".table_of_files_row", function () {
        var dataGet = { filePath: $(this).data("href") };
        $.ajax({
            type: "GET",
            url: "/Account/ListOfFiles",
            data: dataGet,
            success: function (result) {
                $("#list_of_files").html(result);
            },
            error: function () {
                alert("Error while invoking the Web API");
            }
        });
    });
});