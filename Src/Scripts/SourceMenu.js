
// create standard form for input or edit
function Add_form(url, judul_pop, w, h, tabs, tabclick, btn, fnopen = false, fnclose = false, cancel = true) {

    $().w2layout({
        name: 'layout',
        padding: 0,
        panels: [
            { type: 'top', size: 32, content: '<div style="padding: 7px;text-align:left">' + judul_pop + '</div>', style: 'border-bottom: 1px solid silver;' },
            { type: 'left', size: 250, resizable: true, minSize: 120 },
            { type: 'main', minSize: 350, overflow: 'scroll' }
        ]
    });

    $().w2sidebar({
        name: 'sidebar',
        nodes: [
            {
                id: 'general', text: 'General Data', group: true, expanded: true, nodes: tabs
            }
        ],
        onClick: function (event) {
            tabs.forEach(function (tab) {
                if (event.target == tab.id) {
                    switch_content1(tabs, tab.id);
                }
            });
            if (tabclick) tabclick(event);
        }
    });

    w2popup.open({
        title: judul_pop,
        width: w,
        height: h,
        modal: true,
        showMax: true,
        buttons: btn + (cancel ? '<button class="w2ui-btn" onclick="w2popup.close();">Cancel</button>' : ''),
        body: '<div id="main" style="position: absolute; left: 0px; top: 0px; right: 0px; bottom: 0px;overflow-y:scroll;"></div>',
        onOpen: function (event) {
            event.onComplete = function () {
                $('#w2ui-popup #main').w2render('layout');
                w2ui.layout.content('left', w2ui.sidebar);
                load_template('main', url);
                if (fnopen) fnopen();
            }
        },
        onToggle: function (event) {
            event.onComplete = function () {
                w2ui.layout.resize();
            }
        },
        onClose: function (event) {
            if (fnclose) fnclose();
            w2ui['layout'].destroy();
           
        }
    });

}
