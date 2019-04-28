
var ws;
$.MySuperSocket = {
    openSocket: function (openMethod,colseMethod,messageMethod,errorMethod) {
        if (!window.WebSocket) {
            console.log('您的浏览器不支持WebSocket，请选择其他的浏览器再尝试连接服务器');
        }
        var wsClient = new WebSocket('ws://localhost:6868');
        ws = wsClient;
        wsClient.onopen = openMethod;
        wsClient.onclose = colseMethod;
        wsClient.onmessage = messageMethod;
        wsClient.onerror = errorMethod;
    },
    sendMessage: function (socketEntity) {
        var str = JSON.stringify(socketEntity);
        ws.send(str);
    }


}

