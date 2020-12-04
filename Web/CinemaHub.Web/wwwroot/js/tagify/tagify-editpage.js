var input = document.querySelector('input[id="Genres"]'),
    // init Tagify script on the above inputs
    tagify = new Tagify(input, {
        whitelist: ["Action", "Adventure", "Animation", "Comedy", "Crime", "Documentary", "Drama", "Family", "Fantasy", "History", "Horror", "Music", "Mystery", "Romance", "Science Fiction", "TV Movie", "Thriller", "War", "Western"],
        maxTags: 10,
        enforceWhitelist: true,
        originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(', '),
        dropdown: {
            maxItems: 20,           // <- mixumum allowed rendered suggestions
            classname: "tags-look", // <- custom classname for this dropdown, so it could be targeted
            enabled: 0,             // <- show suggestions on focus
            closeOnSelect: false   // <- do not hide the suggestions dropdown once an item has been selected
        }
    })



var input = document.querySelector('textarea[id="Keywords"]'),
    tagify1 = new Tagify(input, {
        whitelist: [],
        dropdown: {
            maxItems: 20,           // <- mixumum allowed rendered suggestions
            classname: "tags-look", // <- custom classname for this dropdown, so it could be targeted
            enabled: 0,             // <- show suggestions on focus
            closeOnSelect: false    // <- do not hide the suggestions dropdown once an item has been selected
        }
    })

tagify1.on('input', onInput)

function onInput(e) {
    var val = e.detail.value;

    $.ajax({
        type: "GET",
        url: "/api/keywords?query=" + val + "&results=20",
        contentType: "application/json",
        async: true,
        success: function (data) {
            tagify1.settings.whitelist = data;
        }
    });

};

