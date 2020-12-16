var input = $(".Keywords");

var tagifyList = [];
for (var i in input) {
    tagifyList.push(new Tagify(input[i], {
        whitelist: [],
        dropdown: {
            maxItems: 20,           // <- mixumum allowed rendered suggestions
            classname: "tags-look", // <- custom classname for this dropdown, so it could be targeted
            enabled: 0,             // <- show suggestions on focus
            closeOnSelect: false    // <- do not hide the suggestions dropdown once an item has been selected
        },
        callbacks: {
            "input": (e) => onInput(e)
        }
    }))
}

function onInput(e) {
            var val = e.detail.value;

            $.ajax({
                type: "GET",
                url: "/api/keywords?query=" + val + "&results=20",
                contentType: "application/json",
                async: true,
                success: function (data) {
                    e.detail.tagify.settings.whitelist = data;
                }
            });
        };