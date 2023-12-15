function dateFormat(fmt, date) {
    let ret;
    const opt = {
        "Y+": date.getFullYear().toString(),        // 年
        "m+": (date.getMonth() + 1).toString(),     // 月
        "d+": date.getDate().toString(),            // 日
        "H+": date.getHours().toString(),           // 时
        "M+": date.getMinutes().toString(),         // 分
        "S+": date.getSeconds().toString()          // 秒
        // 有其他格式化字符需求可以继续添加，必须转化成字符串
    };
    for (let k in opt) {
        ret = new RegExp("(" + k + ")").exec(fmt);
        if (ret) {
            fmt = fmt.replace(ret[1], (ret[1].length == 1) ? (opt[k]) : (opt[k].padStart(ret[1].length, "0")))
        };
    };
    return fmt;
}

//$('.chatboxes .scrollable').each(function (index, ele) {
//    $(ele).on('DOMSubtreeModified', function () {
//        $(this).css('scrollTop', $(this).height());
//    });
//});

let conversations = {
    ConvErnieTurbo: {
        "temperature": 0.95,
        "top_p": 0.8,
        "penalty_score": 1.0,
        "stream": false,
        "conversation_id": "conv-turbo",
        "message": "",
        "model": 2,
        "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
    },
    ConvErnie3_5: {
        "temperature": 0.95,
        "top_p": 0.8,
        "penalty_score": 1.0,
        "stream": false,
        "conversation_id": "conv-3_5",
        "message": "",
        "model": 1,
        "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
    },
    ConvErnie4_0: {
        "temperature": 0.95,
        "top_p": 0.8,
        "penalty_score": 1.0,
        "stream": false,
        "conversation_id": "conv-4_0",
        "message": "",
        "model": 3,
        "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
    }
};


let sendBtnClickEventHandler = function (e) {
    let msg = $('#ipt-message').val();
    $('#ipt-message').val('');
    //console.log(msg);
    $(Object.keys(conversations)).each(function (index, key) {
        //console.log(key);
        let rndStr = Math.random().toString(36).slice(-8);
        let request = conversations[key];
        let wrapperId = "#wrapper_" + key;
        request.message = msg;
        request.conversation_id = `${request.conversation_id}_${rndStr}`;
        let wrapper = $(wrapperId);
        if (wrapper) {
            let author = '我';
            let time = dateFormat('HH:MM:SS', new Date());
            let replyId = `rpl_${rndStr}`;
            let botId;
            switch (key) {
                case 'ConvErnieTurbo': {
                    botId = 'ernie_turbo';
                    break;
                }
                case 'ConvErnie3_5': {
                    botId = 'ernie_3_5';
                    break;
                }
                case 'ConvErnie4_0': {
                    botId = 'ernie_4_0';
                    break;
                }
                default: break;
            }
            const me = `
                        <div class="chat-item">
                            <div class="row align-items-end justify-content-end">
                                <div class="col col-lg-8">
                                    <div class="chat-bubble chat-bubble-me">
                                        <div class="chat-bubble-title">
                                            <div class="row">
                                                <div class="col chat-bubble-author pb-2">${author}</div>
                                                <div class="col-auto chat-bubble-date">${time}</div>
                                            </div>
                                        </div>
                                        <div class="chat-bubble-body">
                                            <p>${msg}</p>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <span class="avatar">我</span>
                                </div>
                            </div>
                        </div>
                        `;
            const bot = `
                         <div class="chat-item">
                             <div class="row align-items-end">
                                 <div class="col-auto">
                                     <span class="avatar">百</span>
                                 </div>
                                 <div class="col col-lg-8">
                                     <div class="chat-bubble">
                                         <div class="chat-bubble-title">
                                             <div class="row">
                                                 <div class="col chat-bubble-author pb-2">${botId}</div>
                                                 <div class="col-auto chat-bubble-date" id='reply_time_${replyId}'></div>
                                             </div>
                                         </div>
                                         <div class="chat-bubble-body" id='reply_${replyId}'>
                                         </div>
                                     </div>
                                 </div>
                             </div>
                         </div>
                         `;

            $(me).appendTo($(wrapper));
            $(bot).appendTo($(wrapper));

            let _scrollItem = $(wrapper).closest('.scrollable');
            //console.log(_scrollItem);
            let scrollTopVal = $(_scrollItem).height();
            //console.log(scrollTopVal);
            $(_scrollItem).scrollTop(scrollTopVal);


            $.ajax(apiEndpoint, {
                method: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(request)
            }).done(function (data, status, xhr) {
                //console.log(data);
                if (data) {
                    $(`#reply_time_${replyId}`).html(dateFormat('HH:MM:SS', new Date()));
                    var markdeownResult = data.result;
                    const typing = new EasyTyper({
                        output: '',
                        isEnd: false,
                        speed: 20,
                        singleBack: false,
                        sleep: 0,
                        type: 'normal',
                        backSpeed: 40,
                        sentencePause: true,
                    },
                        marked.parse(markdeownResult),
                        instance => {
                            // 回调函数
                            // 此回调一般用于获取新的数据然后循环输出
                            // instance { 实例EasyTyper }
                            //console.log(instance); // 打印出实例对象

                            //var result = marked.parse(markdeownResult);
                            //$('#reply_' + replyId).html(result);

                        }, (output, instance) => {
                            // 钩子函数
                            // output { 当前帧的输出内容 }
                            // instance { 实例EasyTyper }
                            // 通过钩子函数动态更新dom元素
                            $('#reply_' + replyId).html(`${output}`);

                            let _scrollItem = $(wrapper).closest('.scrollable');
                            //console.log(_scrollItem);
                            let scrollTopVal = $(_scrollItem).prop('scrollTop') + $('#reply_' + replyId).height();
                            //console.log(scrollTopVal);
                            $(_scrollItem).scrollTop(scrollTopVal);
                        });
                }
            })
                .fail(function (data, status, xhr) {
                    console.log(data);
                    $('#reply_' + replyId).html('发生错误');
                });

        }
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

// 导出
function exportBlob(text, fileName) {
    const blob = new Blob([text], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}

$('.btn-export').on('click', function () {
    let exptime = dateFormat('YYYYmmddHHMMSS', new Date());
    let wrapper = $(this).closest('.card');
    let filename = $(this).data('key') + '-' + (exptime) + '.txt';
    let dialogs = $(wrapper).find('.chat-bubble');
    let sections = [];

    sections.push(`--------------${exptime}-------------\r\n`);
    $(dialogs).each(function (index, item) {
        let author = $(item).find('.chat-bubble-author').text();
        let time = $(item).find('.chat-bubble-date').text();
        let content = $(item).find('.chat-bubble-body').text();

        sections.push(`[${author}]  ${time}\r\n\r\n`);
        sections.push(content.trim());
        sections.push('\r\n-------------------------------\r\n');
    });

    let blob = sections.join('');
    console.log(blob);

    exportBlob(blob, filename);

})