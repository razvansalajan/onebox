
Dropzone.options.dropzoneJsForm = {

    //prevents Dropzone from uploading dropped files immediately
    autoProcessQueue: false,

    init: function () {
        var submitButton = document.querySelector("#submit-all");
        var myDropzone = this; //closure

        submitButton.addEventListener("click", function () {

            //tell Dropzone to process all queued files
            myDropzone.processQueue();
        });

    }
};
