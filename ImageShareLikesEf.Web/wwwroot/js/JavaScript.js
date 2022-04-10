$(() => {
    setInterval(() => {
        const id = $("#image-id").val();
        $.post('/home/getlikes', { id }, function (result) {
            $("#likes").text(result);
        });
    }, 1000);

    $("#like-button").on('click', function () {
        const id = $("#image-id").val();
        $.post('/home/AddLike', { id }, function () {
            $("#like-button").prop('disabled', true);
        });
    });
});