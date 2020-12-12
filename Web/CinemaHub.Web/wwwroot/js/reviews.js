var reviewsList = $('#reviews .row');
var modelId = $('#MediaId').val();
var totalReviews = parseInt($('#totalReviews').val());
var loadButton = $('#loadBtn');
var currentPage = 1;

var options2 = {
    max_value: 10,
    step_size: 1,
    selected_symbol_type: 'utf8_star',
    cursor: 'default',
    readonly: true,
    change_once: false,
}

loadButton.on("click", function () {
    currentPage += 1;
    loadReviews(currentPage);
})

var loadReviews = function(page) {
    $.ajax({
    type: "GET",
    url: `/api/reviews?mediaId=${modelId}&page=${page}`,
    contentType: "application/json",
    success: function(data)
    { 
        populateReviews(data);
    }
  });
};

var populateReviews = function(data) {
    for(var item in data) {
        var review = data[item];
        var date = new Date(review.createdOn);
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();

        var html = `<div class="mv-user-review-item">
        <div class="user-infor">
        <img class="avatar-image" src="${review.avatarImage}" alt="">
        <div>
        <h3>${review.title}</h3>
        <div class="no-star">
            <div class="rate-readonly" data-rate-value=${review.rating}></div>
        </div>
        <p class="time">
            ${day}/${month}/${year} by <a href="#"> ${review.creator}</a>
        </p>
        </div>
        </div>
        <p>${review.reviewText}</p>
        </div>`;
         reviewsList.append(html);
    }
    $(".rate-readonly").rate(options2);
};


$( document ).ready(loadReviews(currentPage));






