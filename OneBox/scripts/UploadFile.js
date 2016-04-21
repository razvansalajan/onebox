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




$(function () {
    $("#upload_button_blob").click(function () {
        // assert the browser support html5
        if (window.File && window.Blob && window.FormData) {
            alert("Your brwoser is awesome, let's rock!");
        }
        else {
            alert("Oh man plz update to a modern browser before try is cool stuff out.");
            return;
        }

        // start to upload each files in chunks
        var files = $("#upload_files")[0].files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var fileSize = file.size;
            var fileName = file.name;

            // calculate the start and end byte index for each blocks(chunks)
            // with the index, file name and index list for future using
            var blockSizeInKB = $("#block_size").val();
            var blockSize = blockSizeInKB * 1024;
            var blocks = [];
            var offset = 0;
            var index = 0;
            var list = "";
            while (offset < fileSize) {
                var start = offset;
                var end = Math.min(offset + blockSize, fileSize);

                blocks.push({
                    name: fileName,
                    index: index,
                    start: start,
                    end: end
                });
                list += index + ",";

                offset = end;
                index++;
            }

            // define the function array and push all chunk upload operation into this array
            var putBlocks = [];
            blocks.forEach(function (block) {
                putBlocks.push(function (callback) {
                    // load blob based on the start and end index for each chunks
                    var blob = file.slice(block.start, block.end);
                    // put the file name, index and blob into a temporary from
                    var fd = new FormData();
                    fd.append("name", block.name);
                    fd.append("index", block.index);
                    fd.append("file", blob);
                    // post the form to backend service (asp.net mvc controller action)
                    $.ajax({
                        url: "/Home/UploadInFormData",
                        data: fd,
                        processData: false,
                        contentType: "multipart/form-data",
                        type: "POST",
                        success: function (result) {
                            if (!result.success) {
                                alert(result.error);
                            }
                            callback(null, block.index);
                        }
                    });
                });
            });

            // invoke the functions one by one
            // then invoke the commit ajax call to put blocks into blob in azure storage
            async.series(putBlocks, function (error, result) {
                var data = {
                    name: fileName,
                    list: list
                };
                $.post("/Home/Commit", data, function (result) {
                    if (!result.success) {
                        alert(result.error);
                    }
                    else {
                        alert("done!");
                    }
                });
            });
        }
    });
});