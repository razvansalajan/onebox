jQuery(document).ready(function ($) {
    $(".clickable-row").click(function () {
        var dataGet = { filePath: $(this).data("href") };
        $.ajax({
            type: "GET",
            url: "/Account/GetFile2s",
            data: dataGet,
            success: function (result) {
                $("#div1").html(result);
            },
            error: function () {
                alert("Error while invoking the Web API");
            }
        });
    });
});