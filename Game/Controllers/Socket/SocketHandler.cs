using Newtonsoft.Json;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using XMGAME.Comm;
using XMGAME.Model;

namespace Game.Controllers.Socket
{
    public class SocketHandler
    {

        public static int RoomSize = 2;
        private Dictionary<string, WebSocketSession> sessionMap = new Dictionary<string, WebSocketSession>();
        private Dictionary<string, List<string>> sessionRoom = new Dictionary<string,List<string>>();
        private Dictionary<string, int> roomReady = new Dictionary<string, int>();
        //private static string roomID 
    

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

        private void server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
             string userKey=  sessionMap.Where(u => u.Value == session).FirstOrDefault().Key;
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

        private void server_NewMessageReceived(WebSocketSession session, string value)
        {
                  
           handlerMessage(value,session);                    
        }

        private void server_NewSessionConnected(WebSocketSession session)
        {
            sessionMap.Add(session.SessionID,session);       
        }

        private void handlerMessage(string message,WebSocketSession session) {
            SocketEntity socketEntity=  JsonHelper.JsonToObject<SocketEntity>(message);
            Debug.WriteLine(socketEntity);
            if (socketEntity.Tag == "m")
            {
                handlerSendMessage(socketEntity);
            }
            else if (socketEntity.Tag == "i")
            {
                handlerInRoom(socketEntity, session);
            }
            else if (socketEntity.Tag == "c")
            {
                updateSessionKey(socketEntity, session);
            }
            else if (socketEntity.Tag == "r")
            {
                handlerReady(socketEntity);
            }
            else if (socketEntity.Tag == "q") {             
                quit(socketEntity);
            }

        }

        public void handlerSendMessage(SocketEntity socket) {
            foreach (var item in socket.ToUser)
            {
                WebSocketSession toUser = sessionMap[item];
             
                toUser.Send(JsonConvert.SerializeObject(socket));
            }
               
           
        }

        private void handlerInRoom(SocketEntity socket,WebSocketSession session) {              
                List<string> room = null;
            if (socket.RoomID != ""&&sessionRoom.ContainsKey(socket.RoomID))
            {
                  room= sessionRoom[socket.RoomID].Count()<RoomSize?sessionRoom[socket.RoomID]:null;
               
            }
            else {
                foreach (List<string> item in sessionRoom.Values)
                {
                    if (item.Count() < RoomSize)
                    {
                        room = item;
                        break;
                    }
                }
            }
               
            if (room != null&&!room.Contains(socket.FromUser)&&room.Count()<RoomSize)
            {
          
                   room.Add(socket.FromUser);                              
                socket.RoomID = sessionRoom.Where(u => u.Value == room).FirstOrDefault().Key;
            }
            else {
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


        private void handlerReady(SocketEntity socket) {       
            int count=  roomReady[socket.RoomID];
            count++;     
            if (count == RoomSize) {
                if (sessionRoom[socket.RoomID].Count() == RoomSize)
                {
                    socket.Tag = "b";
                    socket.ToUser = sessionRoom[socket.RoomID];
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
            }
        }

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

        private void quit(SocketEntity socket)
        {
            List<string> toUser = new List<string>();
            toUser.Add(socket.FromUser);
            roomReady[socket.RoomID] = 0;
            socket.ToUser = toUser;
            handlerSendMessage(socket);
        }



    }
}