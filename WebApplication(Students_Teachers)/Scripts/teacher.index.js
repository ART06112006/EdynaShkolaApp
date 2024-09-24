$(document).ready(function () {
    $(".add-lesson-btn").click(function () {
        let timeTableId = $(this).data("timeTableId");

        $("#timeTableId").val(timeTableId);
    });
});