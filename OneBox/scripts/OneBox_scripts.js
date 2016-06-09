
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



/*
    DOUBLE click on a file item. Update the file list.
    The full path of the desired folder is required.
*/
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

/*
    When a folder from the path (Onebox->folder1->folder2) is clicked.
    Update the list of files.
    Creates a ajax request for retrieving the files from the current folder.
    The full path for the desired folder is required.
*/
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

/*
    Change colour of the selected row.
    TODO : change the background color for the selected item if it is clicked outside of the table.
*/
$(document).ready(function ($) {
    $(document).on("click", ".table_of_files_row", function () {
        $('.table_of_files_row').removeClass('highlighted');
        $(this).addClass('highlighted');
    });

});


/*
    Create new folder button.
    Creates new ajax request for creating new folder giving the full path for it.
*/
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


/*
    Delete item.
*/

$(document).ready(function ($) {
    $(document).on("click", "#delete-button", function () {
        console.log("delete");
        var current_folder_path = $("#current_path_info").data('current_path');
        var current_path_selected_item = $("#current_path_info_selected_file").data("current_path_file");;
        var dataGet = {currentPathSelectedItem: current_path_selected_item};
        $.ajax({
            type: "GET",
            url: "/Account/DeleteItem",
            data: dataGet,
            success: function (result) {
                if (result === "true") {
                    var dataGet = { filePath: current_folder_path };
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
                }else{
                    alert("error within the deleting the file.");
                }
            },
            error: function () {
                alert("Error while invoking the Web API");
            }
        });
    });
});


/*
    Click on move item.Load tree view
*/

$(document).ready(function ($) {
    $(document).on("click", "#move-menu-button", function () {

        $('#container').jstree("destroy");
        $('#container').jstree({
            'core': {
                'data': {
                    "url": "/Account/MoveItem",
                    "data": function (node) {
                        if (node.id === "#") {
                            return { "id": $("#root_name").data('root_name'), "root": "1" };
                        } else {
                            return { "id": node.data.pathOfTheFile, "root": "0" };
                        }
                    }
                }


            }
        });
        $("#container").on("changed.jstree", function (evt, data) {
           var path = data.instance.get_path(data.node,'/');
           console.log('Selected: ' + path);
           $("#current_path_folder_to_move_from_treeview").data('current_path', path);
       }
);
       // $('#container').jstree("refresh");
    });
});


/*
    Rename item.
    Creates new ajax request for creating new folder giving the full path for it.
*/

$(document).ready(function ($) {
    $(document).on("click", "#rename_item", function () {
        console.log("ceva");
        var new_name = $("#rename_item_input_field").val();
        var current_folder_path = $("#current_path_info").data('current_path');
        var current_path_selected_item = $("#current_path_info_selected_file").data("current_path_file");
        var dataGet = { currentFolderPath: current_folder_path, currentPathSelectedItem: current_path_selected_item, newSelectedItemName: new_name };
        $.ajax({
            type: "GET",
            url: "/Account/RenameItem",
            data: dataGet,
            success: function (result) {
                if (result === "true") {
                    var dataGet = { filePath: current_folder_path };
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


/*
    move button pressed.
    Creates new ajax request for moving the selected folder into the folder from tree view.
*/

$(document).ready(function ($) {
    $(document).on("click", "#move_folder_button", function () {

        console.log($("#current_path_folder_to_move_from_treeview").data('current_path'));
        var old = $("#current_path_folder_to_move_from_treeview").data('current_path');
        var res = old.split("/");
        var ans = $("#root_name").data("root_name");

        for (var i = 1; i < res.length; ++i) {
            ans += "/" + res[i];
        }
        console.log(ans);
        console.log($("#current_path_info_selected_file").data("current_path_file"));
        var current_folder_path = $("#current_path_info").data('current_path');

        var dataGet = { selectedFolderToMove: $("#current_path_info_selected_file").data("current_path_file"), currentFolderDestination: ans};
        $.ajax({
        type: "GET",
        url: "/Account/MoveItemToFolder",
        data: dataGet,
        success: function (result) {
            if (result === "true") {
                var dataGet = { filePath: current_folder_path };
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




/*
    Dropzone..
*/
$(document).ready(function ($) {
    Dropzone.options.dropzoneJsForm = {
        //prevents Dropzone from uploading dropped files immediately
        autoProcessQueue: false,
        maxFilesize : 5000,
        init: function () {
            var submitButton = document.querySelector("#submit-all");
            var myDropzone = this; //closure

            submitButton.addEventListener("click", function () {

                //tell Dropzone to process all queued files
                //console.log("ceva");
                myDropzone.processQueue();
            });

            this.on('sending', function (file, xhr, formData) {
                console.log($("#current_path_info").data('current_path'));
                formData.append("currentPath", $("#current_path_info").data('current_path'));
            });

            this.on("complete", function (data) {
                //var res = eval('(' + data.xhr.responseText + ')');
                console.log("ceva2");
                console.log("s-a gatat2");
                // make a call ajax to refresh the list of files
                var current_folder_path = $("#current_path_info").data('current_path');
                var dataGet = { filePath: current_folder_path };
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
                //console.log(data.xhr.responseText);
                //var res = JSON.parse(data.xhr.responseText);
            });
        }
    };

    Dropzone.prototype.uploadFiles = function (files) {
        console.log("ceva");
        var resumable = new Resumable({
            target: '/api/UploadFile',
            maxFiles: 10,
            simultaneousUploads: 1,
            testChunks: true,
            query : {currentPath: $("#current_path_info").data('current_path')}
        });

        if (resumable.support) {
            for (var j = 0; j < files.length; j++) {
                var fileLocal = files[j];
                resumable.addFile(fileLocal);
            }

            resumable.on('fileAdded', function (file) {
                console.log("din reusable upload starts or something");
                
                resumable.upload();
            });

            resumable.on('fileProgress', function (file) {
                var progressValue = Math.floor(resumable.progress() * 100);
                Dropzone.prototype.defaultOptions.uploadprogress(file.file, progressValue, null);
            });

            resumable.on('fileSuccess', (function (_this) {
                return function (file) {
                    // aici e interesant
                    return _this._finished([file.file], "success", null);
                }
            })(this));

            resumable.on('error', (function (_this) {
                return function (message, file) {
                    return _this._errorProcessing([file.file], message, null);
                }
            })(this));
        } else {
            // Otherwise use the old upload function
        }
    }
});

/*
    SINGLE click on a file from the current list. 
    It changes the current_path_info_selected_file div.
*/
$(document).ready(function ($) {
    $(document).on("click", ".table_of_files_row", function () {
        var filename = $(this).data("filename");
        var current_path = $("#current_path_info").data('current_path');     
        var filepathinfo = current_path + "/" + filename;
        $("#current_path_info_selected_file").data("current_path_file", filepathinfo);
    });
});

/*
    current_path_info_selected_file. This div helpes me for knowing which is the last item that I clicked in order to download it.
    Click the download button. It creates an ajax call with the current path as a request for download.
*/

$(document).ready(function ($) {
    $(document).on("click", "#download-button", function (e) {
        
        var current_path_info = $("#current_path_info_selected_file").data("current_path_file");
        console.log(current_path_info);
        e.preventDefault();
        window.location.href = "Account/DownloadFile?filePath=" + current_path_info; 
        /*
        var dataGet = { filePath: current_path_info };
        $.ajax({
            type: "GET",
            url: "/Account/DownloadFile",
            data: dataGet,
            success: function (result) {
                window.loca
            },
            error: function () {
                alert("Error while invoking the Web API");
            }
        });
        */
    });
});
