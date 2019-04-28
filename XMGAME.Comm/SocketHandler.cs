using Newtonsoft.Json;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.Script.Serialization;
using XMGAME.Model;

namespace XMGAME.Comm
{
    public class SocketHandler
    {

        #region 静态变量
        /// <summary>
        ///房间容量
        /// </summary>
        public  static int mintRoomSize = 2;

        /// <summary>
        /// 用户连接SocketSession 集合 键：用户令牌 值：session 
        /// </summary>
        private static Dictionary<string, WebSocketSession> mdicSessiomMap = new Dictionary<string, WebSocketSession>();

        /// <summary>
        /// 游戏房间 key：房间ID value：房间内用户令牌集合
        /// </summary>
        private static Dictionary<string, List<string>> mdicSessionRoom = new Dictionary<string,List<string>>();

        /// <summary>
        /// 房间准备人数 key: 房间ID value :
        /// </summary>
        private static Dictionary<string, int> mdicSessionReady = new Dictionary<string, int>();

        /// <summary>
        /// 执行方法的命名空间
        /// </summary>
        private static string mstrClassPath = "XMGAME.BLL";

        #endregion


        #region 启动WebSocket
        public void SetUp() {

            WebSocketServer webSocket = new WebSocketServer();
            webSocket.NewSessionConnected += HandlerNewSessionConnected;
            webSocket.NewMessageReceived += HandlerNewMessageReceived;
            webSocket.SessionClosed += HanderSessionClosed;
  
            //webSocket.Setup("172.16.31.236", 4008);
              webSocket.Setup("172.16.31.232", 4000);
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
          
             string UserKey=  mdicSessiomMap.Where(u => u.Value == aSession).FirstOrDefault().Key;
            if (UserKey == null) {
                return;
            }
            string RoomId = mdicSessionRoom.Where(u => u.Value.Contains(UserKey)).FirstOrDefault().Key;
            if (RoomId != null&& mdicSessionReady[RoomId] == mintRoomSize) {            
              //  lostConnection(userKey, roomID);
            }                       
             mdicSessiomMap.Remove(UserKey);
             List<string> Room=mdicSessionRoom.Where(u=>u.Value.Contains(UserKey)).FirstOrDefault().Value;
            if (Room != null) {

                Room.Remove(UserKey);
                SocketEntity socketEntity = new SocketEntity()
                {
                    Tag =SocketEnum.s.ToString(),
                    Message = UserKey + "离开游戏间",
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
            if(!mdicSessiomMap.ContainsKey(session.SessionID))
            mdicSessiomMap.Add(session.SessionID,session);       
        }

        #endregion

        #region 处理前台传入的信息
        //处理接收消息
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
            }
          

       
        }

        //处理发送消息
        public void handlerSendMessage(SocketEntity socket, WebSocketSession session = null) {

            if (session != null) {

                session.Send(JsonConvert.SerializeObject(socket));
                return;
            }

            foreach (var item in socket.ToUser)
            {
                WebSocketSession toUser = mdicSessiomMap[item];
             
                toUser.Send(JsonConvert.SerializeObject(socket));
            }
               
           
        }

        //处理进入房间
        private void handlerInRoom(SocketEntity socket) {              
                List<string> room = null;
            //有房间ID并且房没满就进入当前房间
            if (socket.RoomID != ""&&mdicSessionRoom.ContainsKey(socket.RoomID))
            {
                  room= mdicSessionRoom[socket.RoomID].Count()<mintRoomSize?mdicSessionRoom[socket.RoomID]:null;
               
            }
            else {
                //随机进房
                foreach (List<string> item in mdicSessionRoom.Values)
                {
                    if (item.Count() < mintRoomSize)
                    {
                        room = item;
                        break;
                    }
                }
            }
               //把用户放到房间
            if (room != null&&!room.Contains(socket.FromUser)&&room.Count()<mintRoomSize)
            {
          
                   room.Add(socket.FromUser);                              
                socket.RoomID = mdicSessionRoom.Where(u => u.Value == room).FirstOrDefault().Key;
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
                    roomID = DateTime.Now.ToLongTimeString();
                }           
                mdicSessionRoom.Add(roomID,room);
                mdicSessionReady.Add(roomID,0);
                socket.RoomID = roomID;
                
            }
            socket.ToUser =room;
            socket.Message= socket.FromUser + "进入游戏间";         
            handlerSendMessage(socket);

        }

        //游戏准备
        private void handlerReady(SocketEntity socket) {       
            int count=  mdicSessionReady[socket.RoomID];
            count++;     
            //所有人准备了就返回标识码 b 代表开始 
            if (count == mintRoomSize) {
                if (mdicSessionRoom[socket.RoomID].Count() == mintRoomSize)
                {
                    socket.Tag = SocketEnum.b.ToString();
                    socket.ToUser = mdicSessionRoom[socket.RoomID];
                    mdicSessionReady[socket.RoomID] = count;
                    handlerSendMessage(socket);
                }
                else {
                    count--;
                    mdicSessionReady[socket.RoomID] = count;
                }                
            }
            else
            {               
                mdicSessionReady[socket.RoomID] = count;            
                socket.Message = socket.FromUser + "以准备";
                socket.ToUser = mdicSessionRoom[socket.RoomID];
                handlerSendMessage(socket);
            }
        }

        //修改SessionMap 集合Key
        private void updateSessionKey(SocketEntity socket,WebSocketSession session) {
            mdicSessiomMap.Remove(session.SessionID);
            if (mdicSessiomMap.ContainsKey(socket.FromUser))
            {
                mdicSessiomMap[socket.FromUser] = session;
            }
            else {
                mdicSessiomMap.Add(socket.FromUser, session);
            }
           
        }

        //游戏结束退出房间
        private void quit(SocketEntity socket)
        {
            List<string> toUser = new List<string>();
            toUser.Add(socket.FromUser);
            mdicSessionReady[socket.RoomID] = 0;
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
            try
           {
                //得到方法执行数据
                object result = takeRedisData(method, param, backObj);
                responseVo=  getResponseVo(obj: result);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Source);
              responseVo= exMethod(method);
            }
        

            //处理ResponseVo对象并发送数据
            socketEntity.Message =js.Serialize(responseVo);
            if (socketEntity.FromUser != "") {
                List<string> vs = new List<string>();
                vs.Add(socketEntity.FromUser);
                socketEntity.ToUser = vs;
                webSocketSession = null;
            }
          
            handlerSendMessage(socketEntity,webSocketSession);
        }

        #endregion


        private MethodInfo GetActionMethod( out object backObj,string className,string method) {
            Assembly assembly = Assembly.Load(mstrClassPath);
            Type type = assembly.GetType(mstrClassPath + "." + className);
            backObj = assembly.CreateInstance(mstrClassPath + "." + className, false);
            MethodInfo methodEx = type.GetMethod(method);
            return methodEx;
        }

        private object takeRedisData(MethodInfo method, Dictionary<string, object> param, object obj) {


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
                string ArgumentName = at.GetProperty("ArgumentName").GetValue(attribute).ToString();
                string redisKey = key + "." + ArgumentName;
                if (param != null && param.ContainsKey(ArgumentName))
                {
                    redisKey += "::" + param.ContainsKey(ArgumentName);
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

        private ResponseVo exMethod(MethodInfo method) {

         Attribute attribute = method.GetCustomAttribute(typeof(ErroAttribute));
         if (attribute == null)
         return null;

         Type type= attribute.GetType();
         string code=type.GetProperty("Code").GetValue(attribute).ToString();
          
         ResourceManager rm = new ResourceManager("XMGAME.Comm.ErroMessage",typeof(ErroAttribute).Assembly);
         string message= rm.GetString(code);
         ResponseVo responseVo = new ResponseVo()
         {
          Code = Convert.ToInt32(code),
          Message = message
         };

        return responseVo;

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

        private ResponseVo getResponseVo(int code=200 ,string message="成功",object obj=null) {
            ResponseVo responseVo = new ResponseVo() {
                    Code=code,
                    Message=message,
                    Data=obj
            };
 
            return responseVo;
        }

        private void MapperEntity(Dictionary<string, PropertyInfo> fields,ref Dictionary<string, object> param, ref object obj) {

            foreach (var item in fields)
            {
                if (param.ContainsKey(item.Key.ToUpper())) {
                    
                    item.Value.SetValue(obj, param[item.Key.ToUpper()]);
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

        private void lostConnection(string key,string roomID) {

            object obj = null;
            MethodInfo method = GetActionMethod(out obj,"UserBLL", "GetUserByToken");
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            pairs.Add("token",key);
            User ret = (User)takeRedisData(method,pairs,obj);
            //RecordBLL recordBLL = new RecordBLL();
            //recordBLL.GetRecordByUserAndRoom(ret.AccountName,roomID);
            //Record record = new Record()
            //{
            //    AccountName = ret.AccountName,
            //    Integral = -20,
            //    EndTime = DateTime.Now,
            //    RoomID = roomID
            //};

          
            //recordBLL.UpdateRecord(record);      
        }

       

    }
}