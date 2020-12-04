var input = document.querySelector('.keywords-filter'),
    tagify1 = new Tagify(input,
        {
            whitelist: [],
            originalInputValueFormat: item => item.join(', '),
            enforceWhitelist: true,
            dropdown: {
                maxItems: 20, // <- mixumum allowed rendered suggestions // <- custom classname for this dropdown, so it could be targeted
                enabled: 0, // <- show suggestions on focus
                closeOnSelect: false // <- do not hide the suggestions dropdown once an item has been selected
            }
        });

tagify1.on('input', onInput);

function onInput(e) {
    var val = e.detail.value;

    $.ajax({
        type: "GET",
        url: "/api/keywords?query=" + val + "&results=20",
        contentType: "application/json",
        async: false,
        success: function (data) {
            tagify1.settings.whitelist = data;
        }
    });

};