function getParam(paramName) {
    paramValue = "", isFound = !1;
    if (this.location.search.indexOf("?") == 0 && this.location.search.indexOf("=") > 1) {
        arrSource = unescape(this.location.search).substring(1, this.location.search.length).split("&"), i = 0;
        while (i < arrSource.length && !isFound) arrSource[i].indexOf("=") > 0 && arrSource[i].split("=")[0].toLowerCase() == paramName.toLowerCase() && (paramValue = arrSource[i].split("=")[1], isFound = !0), i++
    }
    return paramValue == "" && (paramValue = null), paramValue
}

var s = $.MySuperSocket;
s.openSocket(open, close, message, erro);
function open(e) {
    var entity = {
        "FromUser": getParam("name"),
        "Tag": "c"
    }
    s.sendMessage(entity);
    alert(getParam("name"));
}
function close(e) {
}
function message(e) {
    data = JSON.parse(e.data);
    if (data.Tag == "b") {

        window.location.href = "www.baidu.com";
    } else if (data.Tag == "i") {
        $("#id").val(data.RoomID);
    }
    if (data.Tag == "q") {
        alert("q");
        window.location.href = "/Login/Home?name=" + getParam("name");
    }
    console.log("m:");
    console.log(data);

}
function erro(e) {

}


$("#c").click(function () {
    window.location.href = "/Login/room?name=" + getParam("name") + "&roomID=" + getParam("roomID");
});

$("#b").click(function () {
    alert("click");
    var c = {
        "FromUser": getParam("name"),
        "Tag": "q",
        "RoomID": getParam("roomID")
    }
    s.sendMessage(c)

});
