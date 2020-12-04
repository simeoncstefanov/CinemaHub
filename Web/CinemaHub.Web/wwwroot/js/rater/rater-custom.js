var options = {
    max_value: 10,
    step_size: 1,
    selected_symbol_type: 'utf8_star',
    cursor: 'pointer',
    readonly: false,
    change_once: false,
}

var options2 = {
    max_value: 10,
    step_size: 1,
    selected_symbol_type: 'utf8_star',
    cursor: 'default',
    readonly: true,
    change_once: false,
}

$(".rate2").rate(options);

$(".rate-readonly").rate(options2);

$(".rate2").on("change", function (ev, data) {

    var score = data.to;
    var antiForgeryToken = $('#antiForgeryForm input[name=__RequestVerificationToken]').val();
    var modelId = $('#MediaId').val();

    $.ajax({
        type: "POST",
        url: "/api/reviews",
        async: false,
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        data: JSON.stringify({
            mediaId: modelId,
            value: score
        }),
        contentType: "application/json",
        success: function (resultData) {
            $('#rating-count').text(resultData.totalVotes + " Ratings");
            $('#rating-average').text(resultData.averageVote.toFixed(1));
        }
    });
});

$(".rate2").on("updateSuccess", function (ev, data) {
    console.log("This is a custom success event");
});
