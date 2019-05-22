
using Newtonsoft.Json;
using SuperWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Timers;
using System.Web.Script.Serialization;
using XMGAME.Model;

namespace XMGAME.Comm
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-4
    /// 修改时间：
    /// 功能：Socket通讯处理类
    /// </summary>
    public class SocketHandler
    {

        #region 变量
        /// <summary>
        ///房间容量
        /// </summary>
        public static int gintRoomSize = Convert.ToInt32(ResourceHelp.GetResourceString("roomSize"));

        /// <summary>
        /// 用户连接SocketSession 集合 键：用户令牌 值：session 
        /// </summary>
        private static Dictionary<string, WebSocketSession> gdicSessiomMap = new Dictionary<string, WebSocketSession>();

        /// <summary>
        /// 游戏房间 key：房间ID value：房间内用户令牌集合
        /// </summary>
        private static Dictionary<string, List<string>> gdicSessionRoom = new Dictionary<string, List<string>>();

        /// <summary>
        /// 房间准备人数 key: 房间ID value :
        /// </summary>
        private static Dictionary<string, int> gdicSessionReady = new Dictionary<string, int>();

        /// <summary>
        /// 在线人数
        /// </summary>
        private static Dictionary<string, bool> gdicLive = new Dictionary<string, bool>();

        /// <summary>
        /// 保存要关闭的Session
        /// </summary>
        private static Dictionary<string, WebSocketSession> gdicSessionClose = new Dictionary<string, WebSocketSession>();

        /// <summary>
        /// 记录房间内谁准备了
        /// </summary>
        private static Dictionary<string, List<string>> gdicRoomReadyPerson = new Dictionary<string, List<string>>();

        /// <summary>
        /// 记录房间谁已经结束游戏
        /// </summary>
        private static Dictionary<string, List<string>> gdicGameOver = new Dictionary<string, List<string>>();

        /// <summary>
        /// 保存登录人员
        /// </summary>
        public static Dictionary<string, string> gdicLoginUser = new Dictionary<string, string>();

        /// <summary>
        /// 记录每个游戏的房间
        /// </summary>
        public static Dictionary<int, Dictionary<string, List<string>>> gdicGameRoom = new Dictionary<int, Dictionary<string, List<string>>>();



        /// <summary>
        /// 执行方法的命名空间
        /// </summary>
        private static string gstrClassPath = ResourceHelp.GetResourceString("mbns");
        #endregion


        #region 启动WebSocket
        public void SetUp() {

            WebSocketServer webSocket = new WebSocketServer();
            webSocket.NewSessionConnected += HandlerNewSessionConnected;
            webSocket.NewMessageReceived += HandlerNewMessageReceived;
            webSocket.SessionClosed += HanderSessionClosed;
            webSocket.Setup(ResourceHelp.GetResourceString("ip"), Convert.ToInt32(ResourceHelp.GetResourceString("port")));
            webSocket.Start();


        }
        #endregion


        #region Socket连接关闭时触发
        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-4
        /// 修改时间：2019-4-28 14:47
        /// 功能：（有用户关闭socket连接触发的方法）
        /// 如果用户关闭连接时在游戏间内通知房间的其他人
        /// </summary>
        /// <param name="aSession">用户session</param>
        /// <param name="aValue">关闭状态码</param>

        private void HanderSessionClosed(WebSocketSession aSession, SuperSocket.SocketBase.CloseReason aValue)
        {
            Dictionary<string, List<string>> objRoom = gdicSessionRoom;
            if (gdicSessiomMap.ContainsKey(aSession.SessionID))
            {
                gdicSessiomMap.Remove(aSession.SessionID);
            }
               string strUserKey = null;
                strUserKey  = gdicSessiomMap.Where(u => u.Value == aSession).FirstOrDefault().Key;
                if (strUserKey == null)
                {
                    strUserKey = gdicSessionClose.Where(u => u.Value == aSession).FirstOrDefault().Key;
                    if (strUserKey == null)
                        return;
                }
       
            gdicSessiomMap.Remove(strUserKey);
            string strRoomId = objRoom.Where(u => u.Value.Contains(strUserKey)).FirstOrDefault().Key;
            if (strRoomId == null) {
                return;
            }
        
            List<string> Room = objRoom.Where(u => u.Value.Contains(strUserKey)).FirstOrDefault().Value;
            if (Room != null) {
                if (gdicGameOver.ContainsKey(strRoomId)) {
                    List<string> perpons = gdicGameOver[strRoomId];
                    if (!perpons.Contains(strUserKey)) {
                        perpons.Add(strUserKey);
                    }
                    gdicGameOver[strRoomId] = perpons;
                }
                Room.Remove(strUserKey);
            
                if (gdicRoomReadyPerson[strRoomId].Contains(strUserKey)) {
                       List<string> perpons= gdicRoomReadyPerson[strRoomId];
                       perpons.Remove(strUserKey);
                       gdicRoomReadyPerson[strRoomId] = perpons;
                    gdicSessionReady[strRoomId] = perpons.Count();
                }
                if (Room.Count() == 0) {
                    objRoom.Remove(strRoomId);
                    if (gdicSessionReady.ContainsKey(strRoomId))
                        gdicSessionReady.Remove(strRoomId);
                         gdicRoomReadyPerson.Remove(strRoomId);
                }
                SocketEntity socketEntity = new SocketEntity()
                {
                    FromUser=strUserKey,
                    Tag = SocketEnum.lost.ToString(),
                    Message = ResourceHelp.GetResourceString("lostConn"),
                    ToUser = Room
                };
                handlerSendMessage(socketEntity);
            }

        }

        #endregion


        #region 有新socket连接时触发
        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-4
        /// 修改时间：
        /// 功能：前台给服务端发送信息时触发 处理把信息传给 handlerMessage 方法处理
        /// </summary>
        /// <param name="session">用户session</param>
        /// <param name="value">前端传来的信息</param>
        private void HandlerNewMessageReceived(WebSocketSession session, string value)
        {

            handlerMessage(value, session);
        }

        #endregion


        #region 有新信息接收时触发
        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-
        /// 修改时间：
        /// 功能：处理新连接
        /// </summary>
        /// <param name="session">用户session</param>
        private void HandlerNewSessionConnected(WebSocketSession session)
        {
            if (!gdicSessiomMap.ContainsKey(session.SessionID))
                gdicSessiomMap.Add(session.SessionID, session);
        }

        #endregion


        #region 解析传入的信息并执行相对应的方法
        /// <summary>
        /// 处理前端传来的信息判断要执行什么操作
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="session">websocket连接Session</param>
        public void handlerMessage(string message, WebSocketSession session) {

            SocketEntity socketEntity = JsonHelper.JsonToObject<SocketEntity>(message);
            SocketEnum socketEnum = (SocketEnum)Enum.Parse(typeof(SocketEnum), socketEntity.Tag);
            switch (socketEnum)
            {
                case SocketEnum.i:
                    handlerInRoom(socketEntity);
                    break;
                case SocketEnum.m:
                    handlerSendMessage(socketEntity);
                    break;
                case SocketEnum.ac:
                    handlerControllerAction(socketEntity, session);
                    break;
                case SocketEnum.c:
                    updateSessionKey(socketEntity, session);
                    break;
                case SocketEnum.q:
                    quit(socketEntity);
                    break;
                case SocketEnum.r:
                    handlerReady(socketEntity);
                    break;
                case SocketEnum.live:
                    handlerLiveBag(socketEntity);
                    break;
                case SocketEnum.gv:
                    handlerGameOver(socketEntity);
                    break;
            }



        }
        #endregion


        #region 处理方法
        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：
        /// 功能：接收心跳信息
        /// </summary>
        public void handlerLiveBag(SocketEntity aSocketMessage) {
            if (gdicLive.ContainsKey(aSocketMessage.FromUser))
                gdicLive[aSocketMessage.FromUser] = true;
        }


        /// <summary>
        /// 处理发送信息
        /// </summary>
        /// <param name="aSocketMessage">信息实体对象</param>
        /// <param name="aUserSession">用户 websocket session 可以为空</param>
        public void handlerSendMessage(SocketEntity aSocketMessage, WebSocketSession aUserSession = null) {

            if (aUserSession != null) {

                if (!aUserSession.InClosing)
                    aUserSession.Send(JsonConvert.SerializeObject(aSocketMessage));
                return;
            }
            //Task.Factory.StartNew(() => {
            //    SendDataBySessionId(sessionObj, data, objRoom.User1Session);
            //});


            foreach (var item in aSocketMessage.ToUser)
            {
                if (gdicSessiomMap.ContainsKey(item)) {
                    WebSocketSession toUser = gdicSessiomMap[item];
                    if (!toUser.InClosing)
                        toUser.Send(JsonConvert.SerializeObject(aSocketMessage));
                }

            }

        }

        //private static void SendDataBySessionId(SocketSession session, string data, string sessionId)
        //{
        //    session.AppServer.GetSessionByID(sessionId).Send(data);
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aobjSocket"></param>
        private void handlerGameOver(SocketEntity aobjSocket) {
            if (!gdicGameOver.ContainsKey(aobjSocket.RoomID)) {
                return;
            }         
             List<string> per= gdicGameOver[aobjSocket.RoomID];
            if (per.Contains(aobjSocket.FromUser)) {
                return;
            }
            per.Add(aobjSocket.FromUser);
            gdicGameOver[aobjSocket.RoomID] = per;
            if (per.Count() == gintRoomSize) {            
                gdicSessionRoom.Remove(aobjSocket.RoomID);
                gdicSessionReady.Remove(aobjSocket.RoomID);
                gdicRoomReadyPerson.Remove(aobjSocket.RoomID);
                aobjSocket.FromUser = "System";
                aobjSocket.Tag = SocketEnum.vg.ToString();
                aobjSocket.ToUser = per;
                handlerSendMessage(aobjSocket);
            }


        }

        /// <summary>
        /// 处理用户进入游戏房间房间（单游戏）
        /// </summary>
        /// <param name="socket">信息对象</param>
        private void handlerInRoom(SocketEntity socket) {

            CloseArea(socket);
            List<string> room = null;
            //有房间ID并且房没满就进入当前房间
            if (socket.RoomID != ""&&gdicSessionRoom.ContainsKey(socket.RoomID))
            {
                  room= gdicSessionRoom[socket.RoomID].Count()<gintRoomSize?gdicSessionRoom[socket.RoomID]:null;             
            }
            else {
                //随机进房
                RandomInRoom(ref room,gdicSessionRoom);
            }
               //把用户放到房间
            if (room != null&&!room.Contains(socket.FromUser)&&room.Count()<gintRoomSize)
            {        
                room.Add(socket.FromUser);                              
                socket.RoomID = gdicSessionRoom.Where(u => u.Value == room).FirstOrDefault().Key;
            }
            else {
                //新建房间
                InNewRoom(ref room,ref socket);
                
            }
            socket.ToUser =room;
            socket.Message= ResourceHelp.GetResourceString("InRoom");         
            handlerSendMessage(socket);

        }

        /// <summary>
        /// 保存要关闭的Session
        /// </summary>
        /// <param name="socket"></param>
        private void CloseArea(SocketEntity socket) {
            if (gdicSessionClose.ContainsKey(socket.FromUser))
            {
                gdicSessionClose[socket.FromUser] = gdicSessiomMap[socket.FromUser];

            }
            else
            {
                gdicSessionClose.Add(socket.FromUser, gdicSessiomMap[socket.FromUser]);
            }
        }

        /// <summary>
        /// 随机进房
        /// </summary>
        /// <param name="aRoom">返回出去的房间</param>
        /// <param name="aobjRooms">房间集合</param>
        private void RandomInRoom(ref List<string> aRoom,Dictionary<string,List<string>> aobjRooms) {
            foreach (List<string> item in aobjRooms.Values)
            {
                if (item.Count() < gintRoomSize)
                {
                    aRoom = item;
                    break;
                }
            }
        }

        /// <summary>
        /// 新建房间
        /// </summary>
        /// <param name="room">返回出去的房间</param>
        /// <param name="socket">socket信息</param>
        private void InNewRoom(ref List<string> room,ref SocketEntity socket) {
            room = new List<string>();
            room.Add(socket.FromUser);
            string roomID = "";
            if (socket.RoomID.Trim() != "")
            {
                roomID = socket.RoomID;
            }
            else
            {
                roomID = Guid.NewGuid().ToString();
            }
            gdicSessionRoom.Add(roomID, room);
            gdicSessionReady.Add(roomID, 0);
            gdicRoomReadyPerson.Add(roomID,new List<string>());
            socket.RoomID = roomID;
        }

        /// <summary>
        /// 处理有（多个\单个）游戏时进房间
        /// </summary>
        /// <param name="socket">信息对象</param>
        private void handlerInRoomMoreGame(SocketEntity socket)
        {

            if (gdicSessionClose.ContainsKey(socket.FromUser))
            {
                gdicSessionClose[socket.FromUser] = gdicSessiomMap[socket.FromUser];

            }
            else
            {
                gdicSessionClose.Add(socket.FromUser, gdicSessiomMap[socket.FromUser]);
            }

            if (!gdicGameRoom.ContainsKey(socket.GameID)) {

                List<string> objUser = new List<string>();
                objUser.Add(socket.FromUser);
                socket.FromUser = "";
                socket.Message = ResourceHelp.GetResourceString("inRoomErro") ;
                handlerSendMessage(socket);
                return;
            }
            Dictionary<string,List<string>> objRooms=gdicGameRoom[socket.GameID];
            List<string> room = null;
            //有房间ID并且房没满就进入当前房间
            if (socket.RoomID != "" && objRooms.ContainsKey(socket.RoomID))
            {
                room = objRooms[socket.RoomID].Count() < gintRoomSize ? objRooms[socket.RoomID] : null;
            }
            else
            {
                //随机进房
                foreach (List<string> item in objRooms.Values)
                {
                    if (item.Count() < gintRoomSize)
                    {
                        room = item;
                        break;
                    }
                }
            }
            //把用户放到房间
            if (room != null && !room.Contains(socket.FromUser) && room.Count() < gintRoomSize)
            {

                room.Add(socket.FromUser);
                socket.RoomID = objRooms.Where(u => u.Value == room).FirstOrDefault().Key;
            }
            else
            {
                //新建房间
                room = new List<string>();
                room.Add(socket.FromUser);
                string roomID = "";
                if (socket.RoomID.Trim() != "")
                {
                    roomID = socket.RoomID;
                }
                else
                {
                    roomID = Guid.NewGuid().ToString();
                }
                objRooms.Add(roomID, room);
                gdicSessionReady.Add(roomID, 0);
                socket.RoomID = roomID;

            }
            socket.ToUser = room;
            socket.Message = ResourceHelp.GetResourceString("InRoom");
            handlerSendMessage(socket);

        }

        /// <summary>
        /// 处理用户准备
        /// </summary>
        /// <param name="socket">信息对象</param>
        private void handlerReady(SocketEntity socket) {       
            int count=  gdicSessionReady[socket.RoomID];
            count++;
            List<string> objRoomPerson = gdicRoomReadyPerson[socket.RoomID];
            if (!objRoomPerson.Contains(socket.FromUser)) {
                objRoomPerson.Add(socket.FromUser);
            }
      
            gdicRoomReadyPerson[socket.RoomID] = objRoomPerson;
            //所有人准备了就返回标识码 b 代表开始 
            if (count == gintRoomSize) {
                if (gdicSessionRoom[socket.RoomID].Count() == gintRoomSize)
                {
                    gdicGameOver.Add(socket.RoomID,new List<string>());
                    socket.Tag = SocketEnum.b.ToString();
                    socket.ToUser = gdicSessionRoom[socket.RoomID];
                    gdicSessionReady[socket.RoomID] = count;
                    handlerSendMessage(socket);
                }
                else {
                    count--;
                    gdicSessionReady[socket.RoomID] = count;
                }                
            }
            else
            {              
                gdicSessionReady[socket.RoomID] = count;            
                socket.Message =ResourceHelp.GetResourceString("ready");
                socket.ToUser = gdicSessionRoom[socket.RoomID];
                handlerSendMessage(socket);                
            }
        }


        /// <summary>
        /// 修改SessionMap 集合Key 保持与用户通讯
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="session"></param>
        private void updateSessionKey(SocketEntity socket,WebSocketSession session) {
            gdicSessiomMap.Remove(session.SessionID);
            if (gdicSessiomMap.ContainsKey(socket.FromUser))
            {
                gdicSessiomMap[socket.FromUser] = session;
            }
            else {
                gdicSessiomMap.Add(socket.FromUser, session);
            }
           
        }

        /// <summary>
        /// 游戏结束退出房间
        /// Tag:q
        /// </summary>
        /// <param name="socket">信息实体</param>
        private void quit(SocketEntity socket)
        {
            List<string> toUser = new List<string>();
            toUser.Add(socket.FromUser);
            gdicSessionReady[socket.RoomID] = 0;
            socket.ToUser = toUser;
            handlerSendMessage(socket);
        }

        /// <summary>
        /// 处理前端要的执行方法
        /// Tag:ac
        /// </summary>
        /// <param name="socketEntity">信息实体</param>
        /// <param name="webSocketSession">用户session</param>
        private void handlerControllerAction(SocketEntity socketEntity,WebSocketSession webSocketSession) {
            JavaScriptSerializer js = new JavaScriptSerializer();   
            //得到要执行的方法名称和类名
            string message=socketEntity.ActionMethod;
            string [] am=message.Split('.');
            //得到需要传入的参数
            Dictionary<string, object> param = js.Deserialize<Dictionary<string, object>>(socketEntity.Message);
            
            object backObj = null;
            //得到要执行的方法对象和类实例对象
        
            ResponseVo responseVo = null;
         try
            {
                MethodInfo method = GetActionMethod(out backObj, className: am[0], method: am[1]);
                //得到方法执行数据
                object result = takeData(method, param, backObj);
                 responseVo = GetErroResult(method, result);
          }
            catch (Exception ex)
          {
    
              responseVo = getResponseVo(500, ex.Message);
           }


            //处理ResponseVo对象并发送数据
            socketEntity.Message =JsonHelper.ReplaceDateTime(js.Serialize(responseVo));
            if (socketEntity.FromUser != "") {
                List<string> vs = new List<string>();
                vs.Add(socketEntity.FromUser);
                socketEntity.ToUser = vs;
                webSocketSession = null;
            }
          
            handlerSendMessage(socketEntity,webSocketSession);
        }

        #endregion

        #region 自定义辅助方法
        /// <summary>
        /// 得到错误信息和错误的状态码
        /// </summary>
        /// <param name="method">执行的方法</param>
        /// <param name="aObjResult">执行方法的结果</param>
        /// <returns></returns>
        private ResponseVo GetErroResult ( MethodInfo method ,object aObjResult=null) {

            Attribute attribute = method.GetCustomAttribute(typeof(ErroAttribute));
            if (attribute == null) {
                return getResponseVo(obj:aObjResult);
            }
            Type type = attribute.GetType();
            object [] objRelus= (object[])type.GetProperty("Rule").GetValue(attribute);
            string strCode = null;
            for (int i = 1; i < objRelus.Length; i+=2)
            {
                if (aObjResult == objRelus[i]) {
                    strCode = objRelus[i - 1].ToString();
                    break;
                }

            }
            if (strCode == null) {
                strCode = "200";
            }
       
            string message = ResourceHelp.GetResourceString(strCode);
            ResponseVo responseVo = new ResponseVo()
            {
                Code = Convert.ToInt32(strCode),
                Message = message,
                Data=aObjResult
            };
            return responseVo;
        }

        /// <summary>
        /// 根据类名和方法名得到方法对象
        /// </summary>
        /// <param name="backObj">返回出去的类实例</param>
        /// <param name="className">类名</param>
        /// <param name="method">方法名</param>
        /// <returns></returns>
        private MethodInfo GetActionMethod( out object backObj,string className,string method) {
            Assembly assembly = Assembly.Load(gstrClassPath);
            Type type = assembly.GetType(gstrClassPath + "." + className);
            backObj = assembly.CreateInstance(gstrClassPath + "." + className, false);
            MethodInfo methodEx = type.GetMethod(method);
            return methodEx;
        }

        /// <summary>
        /// 从数据库中拿数据或从Redis拿数据
        /// </summary>
        /// <param name="method">方法对象</param>
        /// <param name="param">方法需要的参数</param>
        /// <param name="obj">类实例对象</param>
        /// <returns></returns>
        private object takeData(MethodInfo method, Dictionary<string, object> param, object obj) {

            if (method == null) {
                return null;
            }
            
            Dictionary<string, object> paramMethod = new Dictionary<string, object>();
            if (param != null) {
                keyToUpper(ref param);
                //如果参数为实体的话就把参数封装为实体类
                ParameterInfo[] parameterInfo = method.GetParameters();
                entityMapping(ref paramMethod, ParameterInfoArrayToDic(parameterInfo), ref param);

                if (param == null)
                {
                    param = new Dictionary<string, object>();
                }
            }
          
            //获取方法上的自定义特性 
            Attribute attribute = method.GetCustomAttribute(typeof(RedisAttribute));
            //如果有就从Redis中拿数据
            if (attribute != null)
            {
                Type at = attribute.GetType();
                string key = at.GetProperty("Key").GetValue(attribute).ToString();
                bool isDelete = Convert.ToBoolean(at.GetProperty("IsDelete").GetValue(attribute));
                string ArgumentName = at.GetProperty("ArgumentName").GetValue(attribute).ToString().ToUpper();
                string redisKey = key + "." + ArgumentName;
                if (param != null && param.ContainsKey(ArgumentName))
                {
                    redisKey += "::" + param[ArgumentName];
                }
                if (isDelete)
                {
                    RedisHelper.DeleteKey(redisKey);
                   
                }
                else
                {
    
                    if (RedisHelper.ContainsKey(redisKey))
                    {
                        return RedisHelper.GetData<object>(redisKey);
                    }
                
                        object resultM = method.Invoke(obj, paramMethod.Values.ToArray());
                        RedisHelper.SetData(redisKey, resultM);
                        return resultM;
                   
                }

            }                    
            return  method.Invoke(obj, paramMethod.Values.ToArray());
                    
        }


        private Dictionary<string, Type> ParameterInfoArrayToDic(ParameterInfo[] parameterInfo)
        {
            Dictionary<string, Type> pairs = new Dictionary<string, Type>();
            foreach (var item in parameterInfo)
            {
                pairs.Add(item.Name,item.ParameterType);
            }
            return pairs;
        }

        private Dictionary<string, Type> ParameterInfoArrayToDic(PropertyInfo[] parameterInfo)
        {
            Dictionary<string, Type> pairs = new Dictionary<string, Type>();
            foreach (var item in parameterInfo)
            {
                pairs.Add(item.Name, item.PropertyType);
            }
            return pairs;
        }

        /// <summary>
        /// 实体映射 把传来的参数
        /// </summary>
        /// <param name="paramMethod"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        private void entityMapping(ref Dictionary<string, object> paramMethod, Dictionary<string, Type> parameter, ref Dictionary<string, object> param,bool takeKey=false) {
         
           
            foreach (var item in parameter)
            {
                Type type = item.Value;

                if (type.IsClass&&type!=typeof(string))
                {
                    Dictionary<string, PropertyInfo> pairs = type.GetProperties().ToDictionary(t => t.Name);
                    object newObj = type.Assembly.CreateInstance(type.FullName);
                    if (parameter.Count > 1) {
                        takeKey = true;
                    }
                    if (typeof(IList).IsAssignableFrom(type))
                    {
                        Type[] types= type.GetGenericArguments();
                        if (!param.ContainsKey(item.Key.ToUpper())) {
                            continue;
                        }
                        AssembleList(ref newObj,param[item.Key.ToUpper()],types[0]);
                    }
                    else {
                        if (takeKey)
                        {
                            Dictionary<string,object> objValue =(Dictionary<string,object>)param[item.Key.ToUpper()];
                            keyToUpper(ref objValue);
                            MapperEntity(pairs,objValue, ref newObj);
                        }
                        else {
                            MapperEntity(pairs, param, ref newObj);
                        }
                       
                    }                  
                    paramMethod.Add(item.Key, newObj);
                }
                else
                {
                    if(param.ContainsKey(item.Key.ToUpper()))
                    paramMethod.Add(item.Key, param[item.Key.ToUpper()]);
                }

            }
        }

        /// <summary>
        /// 映射集合类型数据
        /// </summary>
        /// <param name="obj">方法参数对象</param>
        /// <param name="param">参数</param>
        /// <param name="valueType">值类型</param>
        private void AssembleList(ref object obj,object param,Type valueType) {               
                Type type = param.GetType();     
            
                foreach (var value in (ArrayList)param)
                {                                     
                        Dictionary<string,object> valueV = (Dictionary<string,object>)value;
                        object objValue = valueType.Assembly.CreateInstance(valueType.FullName);
                        keyToUpper(ref valueV);
                        MapperEntity(valueType.GetProperties().ToDictionary(t => t.Name),  valueV, ref objValue);
                        Type objType = obj.GetType();
                       
                        objType.GetMethod("Add").Invoke(obj, new object[] { objValue });                   
                }                         
        }



        /// <summary>
        /// 得到实体响应对象
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">信息</param>
        /// <param name="obj">返回值</param>
        /// <returns></returns>
        private ResponseVo getResponseVo(int code=200 ,string message="成功",object obj=null) {
            ResponseVo responseVo = new ResponseVo() {
                    Code=code,
                    Message=message,
                    Data=obj
            };
 
            return responseVo;
        }


        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：2019-
        /// 功能：把传来的参数映射到调用方法需要的实体类
        /// </summary>
        /// <param name="fields">实体类属性</param>
        /// <param name="param">前端的参数</param>
        /// <param name="obj">实体类对象</param>
        private void MapperEntity(Dictionary<string, PropertyInfo> fields, Dictionary<string, object> param, ref object obj) {

            foreach (var item in fields)
            {
                if (item.Value.PropertyType.IsClass && item.Value.PropertyType != typeof(string))
                {
                    object newObj = null;
                    Type objValueType = item.Value.PropertyType;
                    if (typeof(IList).IsAssignableFrom(objValueType))
                    {
                        Type[] types = objValueType.GetGenericArguments();
                        if (!param.ContainsKey(item.Key.ToUpper()))
                        {
                            continue;
                        }
                        newObj= objValueType.Assembly.CreateInstance(objValueType.FullName);
                     
                        AssembleList(ref newObj, param[item.Key.ToUpper()], types[0]);
                    }
                    else {
                        Dictionary<string, object> pairs = new Dictionary<string, object>();
                        PropertyInfo[] propertyInfos = objValueType.GetProperties();
                        Dictionary<string, Type> keys = new Dictionary<string, Type>();
                        keys.Add(objValueType.Name,objValueType);
                        entityMapping(ref pairs,keys, ref param,true);
                        newObj= pairs.Values.ToArray()[0];
                    }
                  
                    item.Value.SetValue(obj, newObj);
                    param.Remove(item.Key);
                }
                else {

                    if (param.ContainsKey(item.Key.ToUpper()))
                    {

                        object value = null;
                        if (item.Value.GetCustomAttribute(typeof(DateTimeAttribute)) != null)
                        {
                            value = Convert.ToDateTime(param[item.Key.ToUpper()]);
                        }
                        else
                        {
                            value = param[item.Key.ToUpper()];
                        }
                        item.Value.SetValue(obj, value);
                        param.Remove(item.Key);
                    }

                }
                                                    
            }
        }

      


        /// <summary>
        /// 把字典的键变成大写
        /// </summary>
        /// <param name="param">字典对象</param>
        private void keyToUpper(ref Dictionary<string, object> param) {
            Dictionary<string, object> paramUpper = new Dictionary<string, object>();
            foreach (var item in param)
            {
                paramUpper.Add(item.Key.ToUpper(),item.Value);
            }
            param = paramUpper;
        }

        /// <summary>
        /// 通知被逼掉线用户
        /// </summary>
        /// <param name="astrToken">用户令牌</param>
        public  void  InformLostLogin(string  astrToken) {
            List<string> objUser = new List<string>();
            objUser.Add(astrToken);
            SocketEntity objMessage = new SocketEntity();
            objMessage.Tag = SocketEnum.s.ToString();
            objMessage.Message = ResourceHelp.GetResourceString("loginErro") ;
            objMessage.ToUser = objUser;
            handlerSendMessage(objMessage);
            gdicSessiomMap[astrToken].Close();
        }

        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：2019-
        /// 功能：用户掉线的处理
        /// </summary>
        /// <param name="key"></param>
        /// <param name="roomID"></param>
        private void lostConnection(string key,string roomID) {

            object obj = null;
            MethodInfo method = GetActionMethod(out obj,"UserBLL", "GetUserByToken");
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            pairs.Add("token",key);
            User ret = (User)takeData(method,pairs,obj);
         
            MethodInfo objRecordMethod = GetActionMethod(out obj,"RecordBLL", "UpdateRecord");
            pairs = new Dictionary<string, object>();
            pairs.Add("AccountName",ret.AccountName);
            pairs.Add("Integral",-20);
            pairs.Add("EndTime",DateTime.Now);
            pairs.Add("RoomID",roomID);

            takeData(objRecordMethod,pairs,obj);
        }



        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：
        /// 功能：发送心跳包
        /// </summary>
        private void SendLiveBag() {
            int intTime = Convert.ToInt32(ResourceHelp.GetResourceString("timerTime"));
            Timer timer = new Timer(intTime);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
        }


        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：
        /// 功能：发送心跳包的委托事件
        /// </summary>  
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            SocketEntity objSocketLive = new SocketEntity()
            {
                ToUser = gdicSessiomMap.Keys.ToList(),
                Tag = SocketEnum.live.ToString()
            };
            foreach (var item in objSocketLive.ToUser)
            {
                if(item!=null &&!gdicLive.ContainsKey(item))
                gdicLive.Add(item,false);
            }

            handlerSendMessage(objSocketLive);
            ReceiveLiveBag();
        }

        private void ReceiveLiveBag()
        {
            int intTime = Convert.ToInt32(ResourceHelp.GetResourceString("receiveTime"));
            Timer timer = new Timer(intTime);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += ReceiveElapsed;
        }


        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：2019-
        /// 功能：发送心跳包的委托事件
        /// </summary>  
        private void ReceiveElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var item in gdicLive)
            {
                if (!item.Value)
                {
                    if (gdicSessiomMap.ContainsKey(item.Key))
                        gdicSessiomMap[item.Key].Close();
                }
            }

        }

        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：2019-
        /// 功能：当有人准备了就开始计时
        /// </summary>
        private void ReceptionLiveBag() {
            int intTime = 5000;
            Timer timer = new Timer(intTime);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += Receprtion;
        }


        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：2019-
        /// 功能：到时间把房间内没准备的人踢出房间
        /// </summary>
        private void Receprtion(object sender, ElapsedEventArgs e)
        {

            foreach (var item in gdicRoomReadyPerson)
            {
                if (item.Value.Count()<gintRoomSize&& gdicSessionRoom[item.Key].Count==gintRoomSize) {
                    List<string> objRoomUser= gdicSessionRoom[item.Key];
                    objRoomUser.RemoveAll(t=>item.Value.Contains(t));
                    gdicSessionRoom[item.Key] = item.Value;
                    SocketEntity objRemvoUser = new SocketEntity() {
                        Tag = SocketEnum.s.ToString(),
                        ToUser = objRoomUser,
                        Message = ResourceHelp.GetResourceString("outRoom")
                    };
                    handlerSendMessage(objRemvoUser);
                }
            }
           
            
        }
#endregion

    }
}