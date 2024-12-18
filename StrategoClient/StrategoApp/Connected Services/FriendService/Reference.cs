﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StrategoApp.FriendService {
    using System.Runtime.Serialization;
    using System;
    
    
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerFriendRequestResponse", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PlayerFriendRequestResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int[] FriendRequestIdsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.FriendService.OperationResult ResultField;
        
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
        public int[] FriendRequestIds {
            get {
                return this.FriendRequestIdsField;
            }
            set {
                if ((object.ReferenceEquals(this.FriendRequestIdsField, value) != true)) {
                    this.FriendRequestIdsField = value;
                    this.RaisePropertyChanged("FriendRequestIds");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public StrategoApp.FriendService.OperationResult Result {
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
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FriendService.IFriendOperationsService", CallbackContract=typeof(StrategoApp.FriendService.IFriendOperationsServiceCallback))]
    public interface IFriendOperationsService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/SendFriendRequest", ReplyAction="http://tempuri.org/IFriendOperationsService/SendFriendRequestResponse")]
        void SendFriendRequest(int destinationId, int requesterId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/SendFriendRequest", ReplyAction="http://tempuri.org/IFriendOperationsService/SendFriendRequestResponse")]
        System.Threading.Tasks.Task SendFriendRequestAsync(int destinationId, int requesterId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/AcceptFriendRequest", ReplyAction="http://tempuri.org/IFriendOperationsService/AcceptFriendRequestResponse")]
        void AcceptFriendRequest(int destinationId, int requesterId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/AcceptFriendRequest", ReplyAction="http://tempuri.org/IFriendOperationsService/AcceptFriendRequestResponse")]
        System.Threading.Tasks.Task AcceptFriendRequestAsync(int destinationId, int requesterId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/DeclineFriendRequest", ReplyAction="http://tempuri.org/IFriendOperationsService/DeclineFriendRequestResponse")]
        void DeclineFriendRequest(int destinationId, int requesterId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/DeclineFriendRequest", ReplyAction="http://tempuri.org/IFriendOperationsService/DeclineFriendRequestResponse")]
        System.Threading.Tasks.Task DeclineFriendRequestAsync(int destinationId, int requesterId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFriendOperationsServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/GetFriendOperationSend", ReplyAction="http://tempuri.org/IFriendOperationsService/GetFriendOperationSendResponse")]
        void GetFriendOperationSend(StrategoApp.FriendService.OperationResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/GetFriendOperationAccept", ReplyAction="http://tempuri.org/IFriendOperationsService/GetFriendOperationAcceptResponse")]
        void GetFriendOperationAccept(StrategoApp.FriendService.OperationResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendOperationsService/GetFriendOperationDecline", ReplyAction="http://tempuri.org/IFriendOperationsService/GetFriendOperationDeclineResponse")]
        void GetFriendOperationDecline(StrategoApp.FriendService.OperationResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFriendOperationsServiceChannel : StrategoApp.FriendService.IFriendOperationsService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FriendOperationsServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.FriendService.IFriendOperationsService>, StrategoApp.FriendService.IFriendOperationsService {
        
        public FriendOperationsServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public FriendOperationsServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public FriendOperationsServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public FriendOperationsServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public FriendOperationsServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void SendFriendRequest(int destinationId, int requesterId) {
            base.Channel.SendFriendRequest(destinationId, requesterId);
        }
        
        public System.Threading.Tasks.Task SendFriendRequestAsync(int destinationId, int requesterId) {
            return base.Channel.SendFriendRequestAsync(destinationId, requesterId);
        }
        
        public void AcceptFriendRequest(int destinationId, int requesterId) {
            base.Channel.AcceptFriendRequest(destinationId, requesterId);
        }
        
        public System.Threading.Tasks.Task AcceptFriendRequestAsync(int destinationId, int requesterId) {
            return base.Channel.AcceptFriendRequestAsync(destinationId, requesterId);
        }
        
        public void DeclineFriendRequest(int destinationId, int requesterId) {
            base.Channel.DeclineFriendRequest(destinationId, requesterId);
        }
        
        public System.Threading.Tasks.Task DeclineFriendRequestAsync(int destinationId, int requesterId) {
            return base.Channel.DeclineFriendRequestAsync(destinationId, requesterId);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FriendService.IFriendRemoveService", CallbackContract=typeof(StrategoApp.FriendService.IFriendRemoveServiceCallback))]
    public interface IFriendRemoveService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendRemoveService/RemoveFriend", ReplyAction="http://tempuri.org/IFriendRemoveService/RemoveFriendResponse")]
        void RemoveFriend(int destinationId, int requesterId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendRemoveService/RemoveFriend", ReplyAction="http://tempuri.org/IFriendRemoveService/RemoveFriendResponse")]
        System.Threading.Tasks.Task RemoveFriendAsync(int destinationId, int requesterId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFriendRemoveServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFriendRemoveService/GetFriendOperationRemove", ReplyAction="http://tempuri.org/IFriendRemoveService/GetFriendOperationRemoveResponse")]
        void GetFriendOperationRemove(StrategoApp.FriendService.OperationResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFriendRemoveServiceChannel : StrategoApp.FriendService.IFriendRemoveService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FriendRemoveServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.FriendService.IFriendRemoveService>, StrategoApp.FriendService.IFriendRemoveService {
        
        public FriendRemoveServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public FriendRemoveServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public FriendRemoveServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public FriendRemoveServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public FriendRemoveServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void RemoveFriend(int destinationId, int requesterId) {
            base.Channel.RemoveFriend(destinationId, requesterId);
        }
        
        public System.Threading.Tasks.Task RemoveFriendAsync(int destinationId, int requesterId) {
            return base.Channel.RemoveFriendAsync(destinationId, requesterId);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FriendService.ISendRoomInvitationService", CallbackContract=typeof(StrategoApp.FriendService.ISendRoomInvitationServiceCallback))]
    public interface ISendRoomInvitationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendRoomInvitationService/SendRoomInvitation", ReplyAction="http://tempuri.org/ISendRoomInvitationService/SendRoomInvitationResponse")]
        bool SendRoomInvitation(int playerId, string roomCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendRoomInvitationService/SendRoomInvitation", ReplyAction="http://tempuri.org/ISendRoomInvitationService/SendRoomInvitationResponse")]
        System.Threading.Tasks.Task<bool> SendRoomInvitationAsync(int playerId, string roomCode);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISendRoomInvitationServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISendRoomInvitationService/SendRoomInvitationResponseCall", ReplyAction="http://tempuri.org/ISendRoomInvitationService/SendRoomInvitationResponseCallRespo" +
            "nse")]
        void SendRoomInvitationResponseCall(StrategoApp.FriendService.OperationResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISendRoomInvitationServiceChannel : StrategoApp.FriendService.ISendRoomInvitationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SendRoomInvitationServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.FriendService.ISendRoomInvitationService>, StrategoApp.FriendService.ISendRoomInvitationService {
        
        public SendRoomInvitationServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public SendRoomInvitationServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public SendRoomInvitationServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public SendRoomInvitationServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public SendRoomInvitationServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool SendRoomInvitation(int playerId, string roomCode) {
            return base.Channel.SendRoomInvitation(playerId, roomCode);
        }
        
        public System.Threading.Tasks.Task<bool> SendRoomInvitationAsync(int playerId, string roomCode) {
            return base.Channel.SendRoomInvitationAsync(playerId, roomCode);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FriendService.IPlayerFriendRequestService", CallbackContract=typeof(StrategoApp.FriendService.IPlayerFriendRequestServiceCallback))]
    public interface IPlayerFriendRequestService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPlayerFriendRequestService/GetPlayerFriendRequest", ReplyAction="http://tempuri.org/IPlayerFriendRequestService/GetPlayerFriendRequestResponse")]
        void GetPlayerFriendRequest(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPlayerFriendRequestService/GetPlayerFriendRequest", ReplyAction="http://tempuri.org/IPlayerFriendRequestService/GetPlayerFriendRequestResponse")]
        System.Threading.Tasks.Task GetPlayerFriendRequestAsync(int playerId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPlayerFriendRequestServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerFriendRequestService/ReceiveFriendRequestIds")]
        void ReceiveFriendRequestIds(StrategoApp.FriendService.PlayerFriendRequestResponse response);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPlayerFriendRequestServiceChannel : StrategoApp.FriendService.IPlayerFriendRequestService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PlayerFriendRequestServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.FriendService.IPlayerFriendRequestService>, StrategoApp.FriendService.IPlayerFriendRequestService {
        
        public PlayerFriendRequestServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public PlayerFriendRequestServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public PlayerFriendRequestServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PlayerFriendRequestServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PlayerFriendRequestServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void GetPlayerFriendRequest(int playerId) {
            base.Channel.GetPlayerFriendRequest(playerId);
        }
        
        public System.Threading.Tasks.Task GetPlayerFriendRequestAsync(int playerId) {
            return base.Channel.GetPlayerFriendRequestAsync(playerId);
        }
    }
}
