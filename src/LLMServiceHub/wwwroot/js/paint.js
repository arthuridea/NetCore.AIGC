let sendBtnClickEventHandler = function (e) {
    let msg = $('#ipt-message').val();
    $('#ipt-message').val('');

    let request = {
        "conversation_id": "",
        "prompt": msg,
        "image_size": 2,
        "image_num": 1,
        "image": null,
        "url": null,
        "pdf_file": null,
        "pdf_file_num": null,
        "change_degree": 1
    };

    $.ajax(apiEndpoint, {
        method: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(request)
    }).done(function (data, status, xhr) {
        console.log(data);
        if (data) {

        }
    })
    .fail(function (data, status, xhr) {
        console.log(data);
    });
    e.preventDefault();
};

$(document).on('keyup', function (e) {
    var event = e || window.event;
    var key = event.which || event.keyCode || event.charCode;
    if (key == 13) {
        //这里填写你要做的事件
        sendBtnClickEventHandler(e);
    }
});
$('.btn-send-message').on('click', sendBtnClickEventHandler);