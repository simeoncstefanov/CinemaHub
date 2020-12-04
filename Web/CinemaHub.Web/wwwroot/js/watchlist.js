var button = $(".watchlist-btn");
var watchlistType = $(".hvr-grow");
var parentBtn = $("#watch-text");
var mediaId = $("#MediaId");
var antiForgeryToken = $('#antiForgeryForm input[name=__RequestVerificationToken]').val();


watchlistType.on("click",
    function(e) {
        postWatchlist(this.dataset.value);
    });

var postWatchlist = function(watchtype) {
    $.ajax({
        type: "POST",
        url: `/api/watchlist?mediaId=${mediaId.val()}&watchType=${watchtype}`,
        contentType: "application/json",
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        success: function (data) {
            updateButton(data.watchType, data.isAdded);
        }
    });
}

var deleteWatchlist = function() {
    $.ajax({
        type: "POST",
        url: `/api/watchlist?mediaId=${mediaId.val()}&delete=true`,
        contentType: "application/json",
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        success: function (data) {
            updateButton("", false);
        }
    });
}

var updateButton = function(watchType, isWatched) {
    watchlistType.removeClass("active-watch");
    var color = "";

    if (isWatched == false) {
        parentBtn.css("color", "#dd003f");
        parentBtn.html(`<i class="ion-heart watchlist-btn"></i> Add to Watchlist`);
        return;
    }
    if (watchType == "onwatchlist") {
        color = "whitesmoke";
        parentBtn.css("color", color);
        parentBtn.html(`<i class="fa fa-clock-o watchlist-btn" style="color: whitesmoke; padding: 12px 14px; border: 1px solid ${color};"></i> Want to watch`);
        $("#watch1").addClass("active-watch");
        button = $(".watchlist-btn");
    } else if (watchType == "completed") {
        color = "green";
        parentBtn.css("color", color);
        parentBtn.html(`<i class= "fa fa-check-circle watchlist-btn" style="color: green; padding: 12px 14px; border: 1px solid ${color};"></i > Watched`);
        $("#watch2").addClass("active-watch");
        button = $(".watchlist-btn");
    } else if (watchType == "dropped") {
        color = "red";
        parentBtn.css("color", color);
        parentBtn.html(`<i class="fa fa-times-circle watchlist-btn" style="color: red; padding: 12px 14px; border: 1px solid ${color};"></i> Dropped`);
        $("#watch3").addClass("active-watch");
        button = $(".watchlist-btn");
    } else if (watchType == "currentlywatched") {
        color = "orange";
        parentBtn.css("color", color);
        parentBtn.html(
            `<i class="fa fa-eye watchlist-btn" style="color: orange; padding: 12px 14px; border: 1px solid ${color}; "></i> Currently watched`);
        $("#watch4").addClass("active-watch");
        button = $(".watchlist-btn");
    } else {
        button = $(".watchlist-btn");
    }

    button.on("click", deleteWatchlist);
}

$(document).ready(function () {
    var watchType = $('#watchType').val().toLowerCase();
    if (watchType == "") {
        updateButton("", false);
    } else {
        updateButton(watchType, true);
    }
});

