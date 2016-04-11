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
    $(document).on("dblclick", ".table_of_files_row", function () {
        var dataGet = { filePath: $(this).data("href") };
        var typeFile = $(this).data('filetype');
        
        if (typeFile === "file") {
            return;
        }
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


$(document).ready(function ($) {
    $(document).on("click", ".current_path", function () {
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



$(document).ready(function ($) {
    $(document).on("click", ".dropdown_new", function () {
        var current_path = $("#current_path_info").data('current_path');
        console.log(current_path);
    });
});




$(document).ready(function ($) {
    $(document).on("click", "#create_new_folder", function () {
        var new_folder = $("#new_folder_input_field").val();
        var current_path = $("#current_path_info").data('current_path');
        var dataGet = { currentPath: current_path, newFolder: new_folder };
        $.ajax({
            type: "GET",
            url: "/Account/CreateNewFolder",
            data: dataGet,
            success: function (result) {
                if (result === "true") {
                    var dataGet = { filePath: current_path };
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
                }
            },
            error: function () {
                alert("Error while invoking the Web API");
            }
        });
    });
});