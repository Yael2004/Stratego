﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StrategoApp.GameService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PositionDTO", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PositionDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int FinalXField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int FinalYField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int InitialXField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int InitialYField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MoveTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PieceIdField;
        
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
        public int FinalX {
            get {
                return this.FinalXField;
            }
            set {
                if ((this.FinalXField.Equals(value) != true)) {
                    this.FinalXField = value;
                    this.RaisePropertyChanged("FinalX");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FinalY {
            get {
                return this.FinalYField;
            }
            set {
                if ((this.FinalYField.Equals(value) != true)) {
                    this.FinalYField = value;
                    this.RaisePropertyChanged("FinalY");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int InitialX {
            get {
                return this.InitialXField;
            }
            set {
                if ((this.InitialXField.Equals(value) != true)) {
                    this.InitialXField = value;
                    this.RaisePropertyChanged("InitialX");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int InitialY {
            get {
                return this.InitialYField;
            }
            set {
                if ((this.InitialYField.Equals(value) != true)) {
                    this.InitialYField = value;
                    this.RaisePropertyChanged("InitialY");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MoveType {
            get {
                return this.MoveTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.MoveTypeField, value) != true)) {
                    this.MoveTypeField = value;
                    this.RaisePropertyChanged("MoveType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PieceId {
            get {
                return this.PieceIdField;
            }
            set {
                if ((this.PieceIdField.Equals(value) != true)) {
                    this.PieceIdField = value;
                    this.RaisePropertyChanged("PieceId");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GameSessionCreatedResponse", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class GameSessionCreatedResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int GameIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.GameService.OperationResult OperationResultField;
        
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
        public int GameId {
            get {
                return this.GameIdField;
            }
            set {
                if ((this.GameIdField.Equals(value) != true)) {
                    this.GameIdField = value;
                    this.RaisePropertyChanged("GameId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public StrategoApp.GameService.OperationResult OperationResult {
            get {
                return this.OperationResultField;
            }
            set {
                if ((object.ReferenceEquals(this.OperationResultField, value) != true)) {
                    this.OperationResultField = value;
                    this.RaisePropertyChanged("OperationResult");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GameService.IGameService", CallbackContract=typeof(StrategoApp.GameService.IGameServiceCallback))]
    public interface IGameService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/JoinGameSession", ReplyAction="http://tempuri.org/IGameService/JoinGameSessionResponse")]
        void JoinGameSession(int gameId, int player2Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/JoinGameSession", ReplyAction="http://tempuri.org/IGameService/JoinGameSessionResponse")]
        System.Threading.Tasks.Task JoinGameSessionAsync(int gameId, int player2Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/SendPosition", ReplyAction="http://tempuri.org/IGameService/SendPositionResponse")]
        void SendPosition(int gameId, int playerId, StrategoApp.GameService.PositionDTO position);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/SendPosition", ReplyAction="http://tempuri.org/IGameService/SendPositionResponse")]
        System.Threading.Tasks.Task SendPositionAsync(int gameId, int playerId, StrategoApp.GameService.PositionDTO position);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/EndGame", ReplyAction="http://tempuri.org/IGameService/EndGameResponse")]
        void EndGame(int gameId, int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/EndGame", ReplyAction="http://tempuri.org/IGameService/EndGameResponse")]
        System.Threading.Tasks.Task EndGameAsync(int gameId, int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/AbandonGame", ReplyAction="http://tempuri.org/IGameService/AbandonGameResponse")]
        void AbandonGame(int gameId, int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGameService/AbandonGame", ReplyAction="http://tempuri.org/IGameService/AbandonGameResponse")]
        System.Threading.Tasks.Task AbandonGameAsync(int gameId, int playerId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGameServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGameService/OnGameStarted")]
        void OnGameStarted(int gameId, StrategoApp.GameService.OperationResult operationResult);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGameService/OnReceiveOpponentPosition")]
        void OnReceiveOpponentPosition(StrategoApp.GameService.PositionDTO position, StrategoApp.GameService.OperationResult operationResult);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGameService/OnOpponentAbandonedGame")]
        void OnOpponentAbandonedGame(StrategoApp.GameService.OperationResult operationResult);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGameService/OnGameEnded")]
        void OnGameEnded(string resultString, StrategoApp.GameService.OperationResult operationResult);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGameServiceChannel : StrategoApp.GameService.IGameService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GameServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.GameService.IGameService>, StrategoApp.GameService.IGameService {
        
        public GameServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public GameServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public GameServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public GameServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public GameServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void JoinGameSession(int gameId, int player2Id) {
            base.Channel.JoinGameSession(gameId, player2Id);
        }
        
        public System.Threading.Tasks.Task JoinGameSessionAsync(int gameId, int player2Id) {
            return base.Channel.JoinGameSessionAsync(gameId, player2Id);
        }
        
        public void SendPosition(int gameId, int playerId, StrategoApp.GameService.PositionDTO position) {
            base.Channel.SendPosition(gameId, playerId, position);
        }
        
        public System.Threading.Tasks.Task SendPositionAsync(int gameId, int playerId, StrategoApp.GameService.PositionDTO position) {
            return base.Channel.SendPositionAsync(gameId, playerId, position);
        }
        
        public void EndGame(int gameId, int playerId) {
            base.Channel.EndGame(gameId, playerId);
        }
        
        public System.Threading.Tasks.Task EndGameAsync(int gameId, int playerId) {
            return base.Channel.EndGameAsync(gameId, playerId);
        }
        
        public void AbandonGame(int gameId, int playerId) {
            base.Channel.AbandonGame(gameId, playerId);
        }
        
        public System.Threading.Tasks.Task AbandonGameAsync(int gameId, int playerId) {
            return base.Channel.AbandonGameAsync(gameId, playerId);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GameService.ICreateGameService")]
    public interface ICreateGameService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICreateGameService/CreateGameSession", ReplyAction="http://tempuri.org/ICreateGameService/CreateGameSessionResponse")]
        StrategoApp.GameService.GameSessionCreatedResponse CreateGameSession();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICreateGameService/CreateGameSession", ReplyAction="http://tempuri.org/ICreateGameService/CreateGameSessionResponse")]
        System.Threading.Tasks.Task<StrategoApp.GameService.GameSessionCreatedResponse> CreateGameSessionAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICreateGameServiceChannel : StrategoApp.GameService.ICreateGameService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CreateGameServiceClient : System.ServiceModel.ClientBase<StrategoApp.GameService.ICreateGameService>, StrategoApp.GameService.ICreateGameService {
        
        public CreateGameServiceClient() {
        }
        
        public CreateGameServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CreateGameServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CreateGameServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CreateGameServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public StrategoApp.GameService.GameSessionCreatedResponse CreateGameSession() {
            return base.Channel.CreateGameSession();
        }
        
        public System.Threading.Tasks.Task<StrategoApp.GameService.GameSessionCreatedResponse> CreateGameSessionAsync() {
            return base.Channel.CreateGameSessionAsync();
        }
    }
}
