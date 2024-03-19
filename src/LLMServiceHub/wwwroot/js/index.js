import { SSE } from '../lib/sse.js/sse.js';

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


//模型设置
let useSSE = true;
let m_t = 0.95;
let m_tp = 0.8;
let m_ps = 1.0;

let _con_id = {
    ConvErnie3_5: '',
    ConvErnieTurbo: '',
    ConvErnie4_0: ''
};


//let rndStr = Math.random().toString(36).slice(-8);

let conversations = {
    ConvErnieTurbo: {
        "temperature": m_t || 0.95,
        "top_p": m_tp || 0.8,
        "penalty_score": m_ps || 1.0,
        "stream": useSSE,
        "conversation_id": "conv-turbo",
        "message": "",
        "model": 2,
        "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
    },
    ConvErnie3_5: {
        "temperature": m_t || 0.95,
        "top_p": m_tp || 0.8,
        "penalty_score": m_ps || 1.0,
        "conversation_id": "conv-3_5",
        "message": "",
        "model": 1,
        "stream": useSSE,
        "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
    },
    ConvErnie4_0: {
        "temperature": m_t || 0.95,
        "top_p": m_tp || 0.8,
        "penalty_score": m_ps || 1.0,
        "stream": useSSE,
        "conversation_id": "conv-4_0",
        "message": "",
        "model": 3,
        "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
    }
};

$('#r-temperature').val(m_t);
$('#temperature-val').val(m_t);
$('#r-top_p').val(m_tp);
$('#top_p-val').val(m_tp);
$('#r-penalty_score').val(m_ps);
$('#penalty_score-val').val(m_ps);

$('.llm-model-config').html(`温度:${m_t};多样性:${m_tp};惩罚系数:${m_ps};`);


function syncCurrentVal(dataKey, currentVal) {
    $(`#${dataKey}-val`).val(currentVal);
    conversations.ConvErnieTurbo[dataKey] = parseFloat(currentVal);
    conversations.ConvErnie3_5[dataKey] = parseFloat(currentVal);
    conversations.ConvErnie4_0[dataKey] = parseFloat(currentVal);

    //data-llm-name
    $('.llm-model-config').each(function (index, tagEl) {
        let llmName = $(tagEl).data('llm-name');
        let bindingData = conversations[llmName];
        $(tagEl).html(`温度:${bindingData.temperature};多样性:${bindingData.top_p};惩罚系数:${bindingData.penalty_score};`);
    });
}

$('.llm-range-picker').on('change', function (e) {
    let _self = $(this);
    let currentVal = $(_self).val();
    let dataKey = $(_self).data('sync-key');

    syncCurrentVal(dataKey, currentVal);
});

$('.btn-discard-llm-setting').on('click', function (e) {
    let _self = $(this);
    let defaultVal = $(_self).data('val');
    let dataKey = $(_self).data('rel-key');
    console.log(defaultVal);
    console.log(dataKey);

    $(`#r-${dataKey}`).val(defaultVal);

    syncCurrentVal(dataKey, defaultVal);
});



let sendBtnClickEventHandler = async function (e) {
    let msg = $('#ipt-message').val();
    $('#ipt-message').val('');
    //console.log(msg);
    $(Object.keys(conversations)).each(async function (index, key) {
        console.log(`key->${key}`);
        var availableLLM = $('#chk-' + key).is(':checked');
        console.log(`MODEL: ${key} -> ${availableLLM}`);

        if (!availableLLM){
            return;
        }
        let rndStr = Math.random().toString(36).slice(-8);
        let request = conversations[key];
        let wrapperId = "#wrapper_" + key;
        request.message = msg;
        if (_con_id[key] != '') {
            console.log(`conversation_id found: ${_con_id[key]}`)
            request.conversation_id = _con_id[key];
        }
        else {
            request.conversation_id = `${request.conversation_id}_${rndStr}`;
            _con_id[key] = request.conversation_id;
        }
        
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
                                         <input type='hidden' id='reply_hid_${replyId}' value='' />
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

            console.log('sending request with:');
            console.log(request);

            /*
            if (!useSSE) {
                // normal ajax request
                $.ajax(apiEndpoint, {
                    method: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(request)
                }).done(function (data, status, xhr) {
                    console.log(data);
                    if (data) {
                        $(`#reply_time_${replyId}`).html(dateFormat('HH:MM:SS', new Date()));
                        var markdeownResult = data.aigc_message;
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
            else {
                
            }
            */

            // sse request
            var firstLineRecieved = false;
            var chunktimeout = 120000;
            var chunkQueue = [];
            var curChunkIndex = 0;
            var curCharIndex = 0;
            var lastSentence = -1;
            var chunkTimer = null;
            var itv = 50;
            var timeelapsed = 0;

            var typeChunk = function () {
                if (timeelapsed > chunktimeout) {
                    //console.log(`timeout: ${timeelapsed}`);
                    clearInterval(chunkTimer);
                }
                var curChunk = chunkQueue[curChunkIndex];
                if (!curChunk) return;
                var cur_sentence = curChunk.chunk || '';

                if (lastSentence > 0 && ((curChunkIndex == lastSentence) || !cur_sentence)) {
                    //console.log('>>>>>>>> end');
                    clearInterval(chunkTimer);
                    curChunkIndex = 0;
                    return;
                }
                var sl = 0;
                if (cur_sentence) {
                    sl = cur_sentence.length || 0;
                    var ch = cur_sentence[curCharIndex] || '';
                    if (ch) {
                        var reply = ($(`#reply_hid_${replyId}`).val() || '') + ch;
                        $(`#reply_hid_${replyId}`).val(reply);
                        $(`#reply_${replyId}`).html(marked.parse(reply));
                        //$(`#reply_${replyId}`).html(reply);

                        // set scroll position
                        var _scrollItem = $(wrapper).closest('.scrollable');
                        //console.log(_scrollItem);
                        var scrollTopVal = $(_scrollItem).prop('scrollTop') + $('#reply_' + replyId).height();
                        //console.log(scrollTopVal);
                        $(_scrollItem).scrollTop(scrollTopVal);
                    }
                }
                curCharIndex++;
                if (curCharIndex == sl) {
                    //console.log(cur_sentence);
                    //console.log(`[${curChunkIndex}][${curCharIndex}] | lastsentence: ${lastSentence}`);
                    curCharIndex = 0;
                    curChunkIndex++;
                }
                timeelapsed += itv;
            };

            EventSource = SSE;
            var source = new SSE(apiEndpoint, {
                headers: { 'Content-Type': 'application/json' },
                payload: JSON.stringify(request),
                withCredentials: true,
                debug: false,
                start: false,
                method: 'POST'
            });
            //source.addEventListener('status', function (e) {
            //    console.log('\n>>>>>>>>>>>>>>>[status]>>>>>>>>>>>>>>>>>>\n' + e.data);
            //});
            source.addEventListener('message', function (e) {
                // Assuming we receive JSON-encoded data payloads:
                var ret = useSSE ? e.data : e.source.chunk;
                var data = JSON.parse(ret);
                //console.log(`-------------${botId} message in ↓-------------`);
                //console.log(data);
                if (data) {
                    var chunk = data.aigc_message;
                    //chunkQueue.push(Array.from(chunk));
                    chunkQueue.push({
                        row: data.llm_response_data.sentence_id,
                        chunk: chunk.split('')
                    });
                    //markdownResult += chunk;

                    if (!firstLineRecieved) {
                        $(`#reply_time_${replyId}`).html(dateFormat('HH:MM:SS', new Date()));
                        //typeChunk();
                        chunkTimer = setInterval(typeChunk, itv);
                        firstLineRecieved = true;
                    }

                    if (data.llm_response_data.is_end) {
                        lastSentence = data.llm_response_data.sentence_id;
                    }
                }
            });

            source.stream();            
            
        }
    });
    e.preventDefault();
};

$(document).on('keyup', function (e) {
    var event = e || window.event;
    var key = event.which || event.keyCode || event.charCode;
    if (key == 13 && event.ctrlKey) {
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