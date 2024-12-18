﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StrategoApp.RoomService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RoomCreatedResponse", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class RoomCreatedResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.RoomService.OperationResult ResultField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RoomCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public StrategoApp.RoomService.OperationResult Result {
            get {
                return this.ResultField;
            }
            set {
                if ((object.ReferenceEquals(this.ResultField, value) != true)) {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RoomCode {
            get {
                return this.RoomCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.RoomCodeField, value) != true)) {
                    this.RoomCodeField = value;
                    this.RaisePropertyChanged("RoomCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationResult", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data")]
    [System.SerializableAttribute()]
    public partial class OperationResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsDataBaseErrorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsDataBaseError {
            get {
                return this.IsDataBaseErrorField;
            }
            set {
                if ((this.IsDataBaseErrorField.Equals(value) != true)) {
                    this.IsDataBaseErrorField = value;
                    this.RaisePropertyChanged("IsDataBaseError");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSuccess {
            get {
                return this.IsSuccessField;
            }
            set {
                if ((this.IsSuccessField.Equals(value) != true)) {
                    this.IsSuccessField = value;
                    this.RaisePropertyChanged("IsSuccess");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RoomService.IRoomService", CallbackContract=typeof(StrategoApp.RoomService.IRoomServiceCallback))]
    public interface IRoomService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/CreateRoom", ReplyAction="http://tempuri.org/IRoomService/CreateRoomResponse")]
        bool CreateRoom(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/CreateRoom", ReplyAction="http://tempuri.org/IRoomService/CreateRoomResponse")]
        System.Threading.Tasks.Task<bool> CreateRoomAsync(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/JoinRoom", ReplyAction="http://tempuri.org/IRoomService/JoinRoomResponse")]
        bool JoinRoom(string roomCode, int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/JoinRoom", ReplyAction="http://tempuri.org/IRoomService/JoinRoomResponse")]
        System.Threading.Tasks.Task<bool> JoinRoomAsync(string roomCode, int playerId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/LeaveRoomAsync")]
        void LeaveRoomAsync(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/LeaveRoomAsync")]
        System.Threading.Tasks.Task LeaveRoomAsyncAsync(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/SendMessageToRoom", ReplyAction="http://tempuri.org/IRoomService/SendMessageToRoomResponse")]
        void SendMessageToRoom(string roomCode, int playerId, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/SendMessageToRoom", ReplyAction="http://tempuri.org/IRoomService/SendMessageToRoomResponse")]
        System.Threading.Tasks.Task SendMessageToRoomAsync(string roomCode, int playerId, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/NotifyPlayersOfNewConnectionAsync", ReplyAction="http://tempuri.org/IRoomService/NotifyPlayersOfNewConnectionAsyncResponse")]
        void NotifyPlayersOfNewConnectionAsync(string roomCode, int connectedPlayerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/NotifyPlayersOfNewConnectionAsync", ReplyAction="http://tempuri.org/IRoomService/NotifyPlayersOfNewConnectionAsyncResponse")]
        System.Threading.Tasks.Task NotifyPlayersOfNewConnectionAsyncAsync(string roomCode, int connectedPlayerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/NotifyOpponentToJoinGameAsync", ReplyAction="http://tempuri.org/IRoomService/NotifyOpponentToJoinGameAsyncResponse")]
        void NotifyOpponentToJoinGameAsync(string roomCode, int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/NotifyOpponentToJoinGameAsync", ReplyAction="http://tempuri.org/IRoomService/NotifyOpponentToJoinGameAsyncResponse")]
        System.Threading.Tasks.Task NotifyOpponentToJoinGameAsyncAsync(string roomCode, int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/ReportPlayerAccountAsync", ReplyAction="http://tempuri.org/IRoomService/ReportPlayerAccountAsyncResponse")]
        void ReportPlayerAccountAsync(int reporterId, int reportedId, string reason);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRoomService/ReportPlayerAccountAsync", ReplyAction="http://tempuri.org/IRoomService/ReportPlayerAccountAsyncResponse")]
        System.Threading.Tasks.Task ReportPlayerAccountAsyncAsync(int reporterId, int reportedId, string reason);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRoomServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/RoomCreatedAsync")]
        void RoomCreatedAsync(StrategoApp.RoomService.RoomCreatedResponse response);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/RoomResponseAsync")]
        void RoomResponseAsync(StrategoApp.RoomService.OperationResult response);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/ReceiveMessageAsync")]
        void ReceiveMessageAsync(int playerId, string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/GetConnectedPlayerId")]
        void GetConnectedPlayerId(int connectedPlayerId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/NotifyToJoinGame")]
        void NotifyToJoinGame(int gameId, StrategoApp.RoomService.OperationResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IRoomService/NotifyPlayerReported")]
        void NotifyPlayerReported(StrategoApp.RoomService.OperationResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRoomServiceChannel : StrategoApp.RoomService.IRoomService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RoomServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.RoomService.IRoomService>, StrategoApp.RoomService.IRoomService {
        
        public RoomServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public RoomServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public RoomServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public RoomServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public RoomServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool CreateRoom(int playerId) {
            return base.Channel.CreateRoom(playerId);
        }
        
        public System.Threading.Tasks.Task<bool> CreateRoomAsync(int playerId) {
            return base.Channel.CreateRoomAsync(playerId);
        }
        
        public bool JoinRoom(string roomCode, int playerId) {
            return base.Channel.JoinRoom(roomCode, playerId);
        }
        
        public System.Threading.Tasks.Task<bool> JoinRoomAsync(string roomCode, int playerId) {
            return base.Channel.JoinRoomAsync(roomCode, playerId);
        }
        
        public void LeaveRoomAsync(int playerId) {
            base.Channel.LeaveRoomAsync(playerId);
        }
        
        public System.Threading.Tasks.Task LeaveRoomAsyncAsync(int playerId) {
            return base.Channel.LeaveRoomAsyncAsync(playerId);
        }
        
        public void SendMessageToRoom(string roomCode, int playerId, string message) {
            base.Channel.SendMessageToRoom(roomCode, playerId, message);
        }
        
        public System.Threading.Tasks.Task SendMessageToRoomAsync(string roomCode, int playerId, string message) {
            return base.Channel.SendMessageToRoomAsync(roomCode, playerId, message);
        }
        
        public void NotifyPlayersOfNewConnectionAsync(string roomCode, int connectedPlayerId) {
            base.Channel.NotifyPlayersOfNewConnectionAsync(roomCode, connectedPlayerId);
        }
        
        public System.Threading.Tasks.Task NotifyPlayersOfNewConnectionAsyncAsync(string roomCode, int connectedPlayerId) {
            return base.Channel.NotifyPlayersOfNewConnectionAsyncAsync(roomCode, connectedPlayerId);
        }
        
        public void NotifyOpponentToJoinGameAsync(string roomCode, int gameId) {
            base.Channel.NotifyOpponentToJoinGameAsync(roomCode, gameId);
        }
        
        public System.Threading.Tasks.Task NotifyOpponentToJoinGameAsyncAsync(string roomCode, int gameId) {
            return base.Channel.NotifyOpponentToJoinGameAsyncAsync(roomCode, gameId);
        }
        
        public void ReportPlayerAccountAsync(int reporterId, int reportedId, string reason) {
            base.Channel.ReportPlayerAccountAsync(reporterId, reportedId, reason);
        }
        
        public System.Threading.Tasks.Task ReportPlayerAccountAsyncAsync(int reporterId, int reportedId, string reason) {
            return base.Channel.ReportPlayerAccountAsyncAsync(reporterId, reportedId, reason);
        }
    }
}
