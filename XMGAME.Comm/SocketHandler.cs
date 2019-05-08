
using Newtonsoft.Json;
using SuperWebSocket;
using System;
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
        public  static int gintRoomSize =Convert.ToInt32(ResourceHelp.GetResourceString("roomSize"));

        /// <summary>
        /// 用户连接SocketSession 集合 键：用户令牌 值：session 
        /// </summary>
        private static Dictionary<string, WebSocketSession> gdicSessiomMap = new Dictionary<string, WebSocketSession>();

        /// <summary>
        /// 游戏房间 key：房间ID value：房间内用户令牌集合
        /// </summary>
        private static Dictionary<string, List<string>> gdicSessionRoom = new Dictionary<string,List<string>>();

        /// <summary>
        /// 房间准备人数 key: 房间ID value :
        /// </summary>
        private static Dictionary<string, int> gdicSessionReady = new Dictionary<string, int>();

        private static Dictionary<string, bool> gdicLive = new Dictionary<string, bool>();

        private static Dictionary<string, WebSocketSession> gdicSessionClose = new Dictionary<string, WebSocketSession>();

        private static Dictionary<string, List<string>> gdicRoomReadyPerson = new Dictionary<string, List<string>>();

        public static Dictionary<string,string> gdicLoginUser = new Dictionary<string, string>();
        /// <summary>
        /// 执行方法的命名空间
        /// </summary>
        private static string gstrClassPath =ResourceHelp.GetResourceString("mbns");
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

        #region server 的处理方法
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
             string UserKey=  gdicSessiomMap.Where(u => u.Value == aSession).FirstOrDefault().Key;
            if (UserKey == null) {
                UserKey = gdicSessionClose.Where(u => u.Value == aSession).FirstOrDefault().Key;
                if (UserKey == null)
                  return;
            }
            string strRoomId = gdicSessionRoom.Where(u => u.Value.Contains(UserKey)).FirstOrDefault().Key;
            if (strRoomId != null&& gdicSessionReady[strRoomId] == gintRoomSize) {            
              //  lostConnection(UserKey, strRoomId);
            }                       
             gdicSessiomMap.Remove(UserKey);
             List<string> Room=gdicSessionRoom.Where(u=>u.Value.Contains(UserKey)).FirstOrDefault().Value;
            if (Room != null) {

                Room.Remove(UserKey);
                if (Room.Count() == 0) {
                    gdicSessionRoom.Remove(strRoomId);
                    if (gdicSessionReady.ContainsKey(strRoomId))
                        gdicSessionReady.Remove(strRoomId);
                }
                SocketEntity socketEntity = new SocketEntity()
                {
                    Tag =SocketEnum.s.ToString(),
                    Message = ResourceHelp.GetResourceString("lostConn"),
                    ToUser = Room
                };
                handlerSendMessage(socketEntity);
            }
          
        }

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
                  
           handlerMessage(value,session);                    
        }



        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-
        /// 修改时间：
        /// 功能：处理新连接
        /// </summary>
        /// <param name="session">用户session</param>
        private void HandlerNewSessionConnected(WebSocketSession session)
        {                     
            if(!gdicSessiomMap.ContainsKey(session.SessionID))
            gdicSessiomMap.Add(session.SessionID,session);       
        }

        #endregion

        #region 处理前台传入的信息

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="session"></param>
        public void handlerMessage(string message,WebSocketSession session) {

            SocketEntity socketEntity=  JsonHelper.JsonToObject<SocketEntity>(message);
            SocketEnum socketEnum =(SocketEnum) Enum.Parse(typeof(SocketEnum), socketEntity.Tag);
            switch (socketEnum)
            {
                case SocketEnum.i:
                    handlerInRoom(socketEntity);
                    break;
                case SocketEnum.m:
                    handlerSendMessage(socketEntity);
                    break;
                case SocketEnum.ac:
                    handlerControllerAction(socketEntity,session);
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
            }
          

       
        }


        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-5-5
        /// 修改时间：
        /// 功能：接收心跳信息
        /// </summary>
        public void handlerLiveBag(SocketEntity aSocketMessage) {
            if(gdicLive.ContainsKey(aSocketMessage.FromUser))
            gdicLive[aSocketMessage.FromUser] = true;
        }


        //处理发送消息
        public  void handlerSendMessage(SocketEntity aSocketMessage, WebSocketSession aUserSession=null) {

            if (aUserSession!=null) {

                if(!aUserSession.InClosing)
                aUserSession.Send(JsonConvert.SerializeObject(aSocketMessage));
                return;
            }

            foreach (var item in aSocketMessage.ToUser)
            {
                if (gdicSessiomMap.ContainsKey(item)) {
                    WebSocketSession toUser = gdicSessiomMap[item];
                    if (!toUser.InClosing)
                        toUser.Send(JsonConvert.SerializeObject(aSocketMessage));
                }
               
            }
                          
        }


        //处理进入房间
        private void handlerInRoom(SocketEntity socket) {
            if (gdicSessionClose.ContainsKey(socket.FromUser))
            {
                gdicSessionClose[socket.FromUser] = gdicSessiomMap[socket.FromUser];
               
            }
            else
            {
                gdicSessionClose.Add(socket.FromUser, gdicSessiomMap[socket.FromUser]);
            }

            List<string> room = null;
            //有房间ID并且房没满就进入当前房间
            if (socket.RoomID != ""&&gdicSessionRoom.ContainsKey(socket.RoomID))
            {
                  room= gdicSessionRoom[socket.RoomID].Count()<gintRoomSize?gdicSessionRoom[socket.RoomID]:null;
               
            }
            else {
                //随机进房
                foreach (List<string> item in gdicSessionRoom.Values)
                {
                    if (item.Count() < gintRoomSize)
                    {
                        room = item;
                        break;
                    }
                }
            }
               //把用户放到房间
            if (room != null&&!room.Contains(socket.FromUser)&&room.Count()<gintRoomSize)
            {
          
                   room.Add(socket.FromUser);                              
                socket.RoomID = gdicSessionRoom.Where(u => u.Value == room).FirstOrDefault().Key;
            }
            else {
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
                gdicSessionRoom.Add(roomID,room);
                gdicSessionReady.Add(roomID,0);
                socket.RoomID = roomID;
                
            }
            socket.ToUser =room;
            socket.Message= ResourceHelp.GetResourceString("InRoom");         
            handlerSendMessage(socket);

        }

        //游戏准备
        private void handlerReady(SocketEntity socket) {       
            int count=  gdicSessionReady[socket.RoomID];
            count++;     
            //所有人准备了就返回标识码 b 代表开始 
            if (count == gintRoomSize) {
                if (gdicSessionRoom[socket.RoomID].Count() == gintRoomSize)
                {
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
                List<string> objRoomPerson = null;
                if (gdicRoomReadyPerson.ContainsKey(socket.RoomID))
                {
                    objRoomPerson = gdicRoomReadyPerson[socket.RoomID];
                }
                else {
                    objRoomPerson = new List<string>();
                    gdicRoomReadyPerson.Add(socket.RoomID,objRoomPerson);
                }
                objRoomPerson.Add(socket.FromUser);
                gdicRoomReadyPerson[socket.RoomID] = objRoomPerson;
                gdicSessionReady[socket.RoomID] = count;            
                socket.Message =ResourceHelp.GetResourceString("ready");
                socket.ToUser = gdicSessionRoom[socket.RoomID];
                handlerSendMessage(socket);                
            }
        }

        //修改SessionMap 集合Key
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

        //游戏结束退出房间
        private void quit(SocketEntity socket)
        {
            List<string> toUser = new List<string>();
            toUser.Add(socket.FromUser);
            gdicSessionReady[socket.RoomID] = 0;
            socket.ToUser = toUser;
            handlerSendMessage(socket);
        }

        //处理执行方法
        private void handlerControllerAction(SocketEntity socketEntity,WebSocketSession webSocketSession) {
            JavaScriptSerializer js = new JavaScriptSerializer();   
            //得到要执行的方法名称和类名
            string message=socketEntity.ActionMethod;
            string [] am=message.Split('.');
            //得到需要传入的参数
            Dictionary<string, object> param = js.Deserialize<Dictionary<string, object>>(socketEntity.Message);
            
            object backObj = null;
            //得到要执行的方法对象和类实例对象
            MethodInfo method = GetActionMethod(out backObj,className:am[0],method:am[1]);
            ResponseVo responseVo = null;
            //try
            //{
                //得到方法执行数据
                object result = takeRedisData(method, param, backObj);
                responseVo = GetErroResult(method,result);
        //}
        //    catch (Exception)
        //    {

        //        responseVo = getResponseVo(500, ResourceHelp.GetResourceString("500"));
        //    }


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

        private MethodInfo GetActionMethod( out object backObj,string className,string method) {
            Assembly assembly = Assembly.Load(gstrClassPath);
            Type type = assembly.GetType(gstrClassPath + "." + className);
            backObj = assembly.CreateInstance(gstrClassPath + "." + className, false);
            MethodInfo methodEx = type.GetMethod(method);
            return methodEx;
        }

        private object takeRedisData(MethodInfo method, Dictionary<string, object> param, object obj) {

            if (method == null) {
                return null;
            }

            Dictionary<string, object> paramMethod = new Dictionary<string, object>();
            if (param != null) {
                keyToUpper(ref param);
                //如果参数为实体的话就把参数封装为实体类
                entityMapping(ref paramMethod, method, ref param);

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

        private void entityMapping(ref Dictionary<string, object> paramMethod,MethodInfo method,ref Dictionary<string, object> param) {
            ParameterInfo[] parameterInfo = method.GetParameters();
            foreach (var item in parameterInfo)
            {
                Type type = item.ParameterType;
                if (type.IsClass&&type!=typeof(string))
                {
                    Dictionary<string, PropertyInfo> pairs = type.GetProperties().ToDictionary(t => t.Name);
                    Debug.Write(type.FullName);
                    object newObj = type.Assembly.CreateInstance(type.FullName);
                    MapperEntity(pairs, ref param, ref newObj);
                    paramMethod.Add(item.Name, newObj);
                }
                else
                {
                    if(param.ContainsKey(item.Name.ToUpper()))
                    paramMethod.Add(item.Name, param[item.Name.ToUpper()]);
                }

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
        private void MapperEntity(Dictionary<string, PropertyInfo> fields,ref Dictionary<string, object> param, ref object obj) {

            foreach (var item in fields)
            {
                if (param.ContainsKey(item.Key.ToUpper())) {
               
                    object value = null;
                    if (item.Value.GetCustomAttribute(typeof(DateTimeAttribute)) != null)
                    {
                        value = Convert.ToDateTime(param[item.Key.ToUpper()]);
                    }
                    else {
                        value = param[item.Key.ToUpper()];
                    }   
                    item.Value.SetValue(obj, value);
                    param.Remove(item.Key);
                }                                    
            }
        }

      


        private void keyToUpper(ref Dictionary<string, object> param) {
            Dictionary<string, object> paramUpper = new Dictionary<string, object>();
            foreach (var item in param)
            {
                paramUpper.Add(item.Key.ToUpper(),item.Value);
            }
            param = paramUpper;
        }


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
            User ret = (User)takeRedisData(method,pairs,obj);
         
            MethodInfo objRecordMethod = GetActionMethod(out obj,"RecordBLL", "UpdateRecord");
            pairs = new Dictionary<string, object>();
            pairs.Add("AccountName",ret.AccountName);
            pairs.Add("Integral",-20);
            pairs.Add("EndTime",DateTime.Now);
            pairs.Add("RoomID",roomID);

            takeRedisData(objRecordMethod,pairs,obj);
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
     
    }
}