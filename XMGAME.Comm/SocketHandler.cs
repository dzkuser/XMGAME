using Newtonsoft.Json;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Script.Serialization;
using XMGAME.Comm;
using XMGAME.Model;

namespace XMGAME.Comm
{
    public class SocketHandler
    {

        public static int RoomSize = 2;
        private static Dictionary<string, WebSocketSession> sessionMap = new Dictionary<string, WebSocketSession>();
        private static Dictionary<string, List<string>> sessionRoom = new Dictionary<string,List<string>>();
        private static Dictionary<string, int> roomReady = new Dictionary<string, int>();
        private static string ClassPath = "XMGAME.BLL";


       
        //启动Socket
        public void SetUp() {

            WebSocketServer webSocket = new WebSocketServer();
            webSocket.NewSessionConnected += server_NewSessionConnected;
            webSocket.NewMessageReceived += server_NewMessageReceived;
            webSocket.SessionClosed += server_SessionClosed;
            try
            {
                webSocket.Setup("127.0.0.1", 4000);
                webSocket.Start();
            }
            catch (Exception)
            {

                throw;
            }
          

        }

        //有session关闭时触发的方法
        private void server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
          
             string userKey=  sessionMap.Where(u => u.Value == session).FirstOrDefault().Key;
            if (userKey == null) {
                return;
            }
            string roomID = sessionRoom.Where(u => u.Value.Contains(userKey)).FirstOrDefault().Key;
            if (roomID != null&& roomReady[roomID] == RoomSize) {
                Debug.Write("掉线---------------------------------------");
              //  lostConnection(userKey, roomID);
            }                       
             sessionMap.Remove(userKey);
             List<string> room=sessionRoom.Where(u=>u.Value.Contains(userKey)).FirstOrDefault().Value;
            if (room != null) {

                room.Remove(userKey);
                SocketEntity socketEntity = new SocketEntity()
                {
                    Tag = "s",
                    Message = userKey + "离开游戏间",
                    ToUser = room
                };
                handlerSendMessage(socketEntity);
            }
          
        }

        //接收消息
        private void server_NewMessageReceived(WebSocketSession session, string value)
        {
                  
           handlerMessage(value,session);                    
        }

        //处理新连接

        private void server_NewSessionConnected(WebSocketSession session)
        {
            if(!sessionMap.ContainsKey(session.SessionID))
            sessionMap.Add(session.SessionID,session);       
        }

        //处理接收消息
        public  void handlerMessage(string message,WebSocketSession session) {
            SocketEntity socketEntity=  JsonHelper.JsonToObject<SocketEntity>(message);
            Debug.WriteLine(socketEntity);


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
                    handlerControllerAction(socketEntity);
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
        public  void handlerSendMessage(SocketEntity socket) {
            foreach (var item in socket.ToUser)
            {
                WebSocketSession toUser = sessionMap[item];
             
                toUser.Send(JsonConvert.SerializeObject(socket));
            }
               
           
        }

        //处理进入房间
        private void handlerInRoom(SocketEntity socket) {              
                List<string> room = null;
            //有房间ID并且房没满就进入当前房间
            if (socket.RoomID != ""&&sessionRoom.ContainsKey(socket.RoomID))
            {
                  room= sessionRoom[socket.RoomID].Count()<RoomSize?sessionRoom[socket.RoomID]:null;
               
            }
            else {
                //随机进房
                foreach (List<string> item in sessionRoom.Values)
                {
                    if (item.Count() < RoomSize)
                    {
                        room = item;
                        break;
                    }
                }
            }
               //把用户放到房间
            if (room != null&&!room.Contains(socket.FromUser)&&room.Count()<RoomSize)
            {
          
                   room.Add(socket.FromUser);                              
                socket.RoomID = sessionRoom.Where(u => u.Value == room).FirstOrDefault().Key;
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
                sessionRoom.Add(roomID,room);
                roomReady.Add(roomID,0);
                socket.RoomID = roomID;
                
            }
            socket.ToUser =room;
            socket.Message= socket.FromUser + "进入游戏间";         
            handlerSendMessage(socket);

        }

        //游戏准备
        private void handlerReady(SocketEntity socket) {       
            int count=  roomReady[socket.RoomID];
            count++;     
            //所有人准备了就返回标识码 b 代表开始 
            if (count == RoomSize) {
                if (sessionRoom[socket.RoomID].Count() == RoomSize)
                {
                    socket.Tag = SocketEnum.b.ToString();
                    socket.ToUser = sessionRoom[socket.RoomID];
                    roomReady[socket.RoomID] = count;
                    handlerSendMessage(socket);
                }
                else {
                    count--;
                    roomReady[socket.RoomID] = count;
                }                
            }
            else
            {               
                roomReady[socket.RoomID] = count;
                socket.Tag = SocketEnum.b.ToString();
                socket.Message = socket.FromUser + "以准备";
                socket.ToUser = sessionRoom[socket.RoomID];
                handlerSendMessage(socket);
            }
        }

        //修改SessionMap 集合Key
        private void updateSessionKey(SocketEntity socket,WebSocketSession session) {
            sessionMap.Remove(session.SessionID);
            if (sessionMap.ContainsKey(socket.FromUser))
            {
                sessionMap[socket.FromUser] = session;
            }
            else {
                sessionMap.Add(socket.FromUser, session);
            }
           
        }

        //游戏结束退出房间
        private void quit(SocketEntity socket)
        {
            List<string> toUser = new List<string>();
            toUser.Add(socket.FromUser);
            roomReady[socket.RoomID] = 0;
            socket.ToUser = toUser;
            handlerSendMessage(socket);
        }

        //处理执行方法
        private void handlerControllerAction(SocketEntity socketEntity) {
            JavaScriptSerializer js = new JavaScriptSerializer();   
            //得到要执行的方法名称和类名
            string message=socketEntity.ActionMethod;
            string [] am=message.Split('.');
            //得到需要传入的参数
            Dictionary<string, object> param = js.Deserialize<Dictionary<string, object>>(socketEntity.Message);
            
            object backObj = null;
            //得到要执行的方法对象和类实例对象
            MethodInfo method = GetActionMethod(out backObj,className:am[0],method:am[1]);    
            
            //得到方法执行数据
            object result = takeRedisData(method,param,backObj);

            //处理ResponseVo对象并发送数据
            socketEntity.Message =js.Serialize(getResponseVo(result));
            List<string> vs = new List<string>();
            vs.Add(socketEntity.FromUser);
            socketEntity.ToUser = vs;
            handlerSendMessage(socketEntity);
        }

        
        private MethodInfo GetActionMethod( out object backObj,string className,string method) {
            Assembly assembly = Assembly.Load(ClassPath);
            Type type = assembly.GetType(ClassPath + "." + className);
            backObj = assembly.CreateInstance(ClassPath + "." + className, false);
            MethodInfo methodEx = type.GetMethod(method);
            return methodEx;
        }

        private object takeRedisData(MethodInfo method, Dictionary<string, object> param, object obj) {


            Dictionary<string, object> paramMethod = new Dictionary<string, object>();
            keyToUpper(ref param);
            //如果参数为实体的话就把参数封装为实体类
            entityMapping(ref paramMethod,method,ref param);

            if (param == null) {
                param = new Dictionary<string, object>();
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
                string redisKey = key + "." +ArgumentName+"::"+ param[ArgumentName];
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

        private object exMethod(object obj,object[] param,MethodInfo method) {

            object result = null;
            try
            {
                result=method.Invoke(obj, param);
            }
            catch (Exception ex)
            {
                Attribute attribute = method.GetCustomAttribute(typeof(ErroAttribute));
                if (attribute != null) {
                    Type type= attribute.GetType();
                    string code=type.GetProperty("Code").GetValue(attribute).ToString();
                    Assembly myAssem = Assembly.GetEntryAssembly();
                    ResourceManager rm = new ResourceManager("ErroMessage", myAssem);
                   string message= rm.GetString(code);

                }

            }
            return result;
        } 
        private void entityMapping(ref Dictionary<string, object> paramMethod,MethodInfo method,ref Dictionary<string, object> param) {
            ParameterInfo[] parameterInfo = method.GetParameters();
            foreach (var item in parameterInfo)
            {
                Type type = item.ParameterType;
                if (type.IsClass)
                {
                    Dictionary<string, PropertyInfo> pairs = type.GetProperties().ToDictionary(t => t.Name);
                    Debug.Write(type.FullName);
                    object newObj = type.Assembly.CreateInstance(type.FullName);
                    MapperEntity(pairs, ref param, ref newObj);
                    paramMethod.Add(item.Name, newObj);
                }
                else
                {
                    paramMethod.Add(item.Name, param[item.Name]);
                }

            }
        }

        private ResponseVo getResponseVo(object obj) {
            ResponseVo responseVo = new ResponseVo();
            if (obj == null)
            {
                responseVo.Code = 500;
                responseVo.Message = "失败";
            }
            else {
                responseVo.Message = "成功";
                responseVo.Code = 200;
                responseVo.Data = obj;
            }
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