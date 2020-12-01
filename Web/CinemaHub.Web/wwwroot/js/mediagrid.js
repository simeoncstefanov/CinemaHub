var submitBtn = $("#searchSubmit");



document.addEventListener("DOMContentLoaded", function () {
    requestJson(1);
});


$(submitBtn).on('click', function(e){
    e.preventDefault();

    requestJson(1);
    });

var requestJson = function(page){
    var searchQuery = document.getElementById("SearchQuery").value;
    var mediaType = document.getElementById("MediaType").value;
    var jsonRequestBody = JSON.stringify({
         "searchQuery": searchQuery,
         "page": page,
         "elementsPerPage": 20,
         "mediaType": mediaType,
        });

    $.ajax({
        type: "POST",
        url: "/api/media",
        contentType: "application/json",
        data: jsonRequestBody, // serializes the form's elements.
        success: function(data)
        { // show response from the php script.
            populateGrid(data);
        }
      });
}


var populateGrid = function(json) {
    pagesFound.innerHTML = json.resultCount + " results";

    var movieList = document.getElementsByClassName("flex-wrap-movielist")[0];
    var pagination = document.getElementById("paginationCount");
    var pages = document.getElementById("paginationNumbers");

    movieList.innerHTML = '';
    for(var result in json.results)
    {
        var media = json.results[result];
        movieList.innerHTML += `<div class="movie-item-style-2 movie-item-style-1">
                                    <img src="${media.imagePath}" alt="">
                                    <div class="hvr-inner">
                                        <a href="/Media/${media.mediaType + "s"}/${media.id}"> Read more <i class="ion-android-arrow-dropright"></i> </a>
                                    </div>
                                    <div class="mv-item-infor">
                                        <h6><a href="/Media/${media.mediaType + "s"}/${media.id}">${media.title}</a></h6>
                                        <p class="rate"><i class="ion-android-star"></i><span>${media.rating.toFixed(1)}</span> /10</p>
                                    </div>
                                </div>`
    };

    pagination.innerHTML = "Page "+json.currentPage+" of "+json.pages;
    var elements = document.querySelectorAll('.pageNumber');  
    for (var element of elements) {
            element.remove();
   // or 
   // element.parentNode.removeChild(element);
        }

            var pageButton = document.createElement("A");
        pageButton.innerHTML = 1;
        pageButton.classList.add("pageNumber");
        pages.appendChild(pageButton)
        if (json.currentPage == 1)
        {
            pageButton.classList.add("active");
        }

    for (i = json.currentPage - 3; i < json.currentPage + 4; i++) {
        if (i > 1 && i < json.pages)
        {
            var pageButton = document.createElement("A");
            pageButton.innerHTML = i;
            pageButton.classList.add("pageNumber");
            pages.appendChild(pageButton)
            if (i == json.currentPage)
            {
                pageButton.classList.add("active");
            }
        }
    }

    var pageButton = document.createElement("A");
    pageButton.innerHTML = json.pages;
    pageButton.classList.add("pageNumber");
    pages.appendChild(pageButton)
    if (json.currentPage == json.pages)
    {
        pageButton.classList.add("active");
    }


    var pageNumbers = document.getElementsByClassName("pageNumber");

    $(pageNumbers).on('click', function() {
        requestJson(parseInt(this.innerHTML))
    });
};

