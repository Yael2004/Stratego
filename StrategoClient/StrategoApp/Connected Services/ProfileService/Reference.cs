﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StrategoApp.ProfileService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerInfoShownDTO", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PlayerInfoShownDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LabelPathField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PicturePathField;
        
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
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LabelPath {
            get {
                return this.LabelPathField;
            }
            set {
                if ((object.ReferenceEquals(this.LabelPathField, value) != true)) {
                    this.LabelPathField = value;
                    this.RaisePropertyChanged("LabelPath");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PicturePath {
            get {
                return this.PicturePathField;
            }
            set {
                if ((object.ReferenceEquals(this.PicturePathField, value) != true)) {
                    this.PicturePathField = value;
                    this.RaisePropertyChanged("PicturePath");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerInfoResponse", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PlayerInfoResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.ProfileService.PlayerInfoShownDTO ProfileField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.ProfileService.OperationResult ResultField;
        
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
        public StrategoApp.ProfileService.PlayerInfoShownDTO Profile {
            get {
                return this.ProfileField;
            }
            set {
                if ((object.ReferenceEquals(this.ProfileField, value) != true)) {
                    this.ProfileField = value;
                    this.RaisePropertyChanged("Profile");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public StrategoApp.ProfileService.OperationResult Result {
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
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerStatisticsResponse", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PlayerStatisticsResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.ProfileService.OperationResult ResultField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.ProfileService.PlayerStatisticsDTO StatisticsField;
        
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
        public StrategoApp.ProfileService.OperationResult Result {
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
        public StrategoApp.ProfileService.PlayerStatisticsDTO Statistics {
            get {
                return this.StatisticsField;
            }
            set {
                if ((object.ReferenceEquals(this.StatisticsField, value) != true)) {
                    this.StatisticsField = value;
                    this.RaisePropertyChanged("Statistics");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerStatisticsDTO", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PlayerStatisticsDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int LostGamesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TotalGamesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int WonGamesField;
        
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
        public int LostGames {
            get {
                return this.LostGamesField;
            }
            set {
                if ((this.LostGamesField.Equals(value) != true)) {
                    this.LostGamesField = value;
                    this.RaisePropertyChanged("LostGames");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TotalGames {
            get {
                return this.TotalGamesField;
            }
            set {
                if ((this.TotalGamesField.Equals(value) != true)) {
                    this.TotalGamesField = value;
                    this.RaisePropertyChanged("TotalGames");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int WonGames {
            get {
                return this.WonGamesField;
            }
            set {
                if ((this.WonGamesField.Equals(value) != true)) {
                    this.WonGamesField = value;
                    this.RaisePropertyChanged("WonGames");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerFriendsResponse", Namespace="http://schemas.datacontract.org/2004/07/StrategoServices.Data.DTO")]
    [System.SerializableAttribute()]
    public partial class PlayerFriendsResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.ProfileService.PlayerInfoShownDTO[] FriendsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private StrategoApp.ProfileService.OperationResult ResultField;
        
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
        public StrategoApp.ProfileService.PlayerInfoShownDTO[] Friends {
            get {
                return this.FriendsField;
            }
            set {
                if ((object.ReferenceEquals(this.FriendsField, value) != true)) {
                    this.FriendsField = value;
                    this.RaisePropertyChanged("Friends");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public StrategoApp.ProfileService.OperationResult Result {
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProfileService.IProfileService", CallbackContract=typeof(StrategoApp.ProfileService.IProfileServiceCallback))]
    public interface IProfileService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetPlayerInfo", ReplyAction="http://tempuri.org/IProfileService/GetPlayerInfoResponse")]
        void GetPlayerInfo(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetPlayerInfo", ReplyAction="http://tempuri.org/IProfileService/GetPlayerInfoResponse")]
        System.Threading.Tasks.Task GetPlayerInfoAsync(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/UpdatePlayerProfile", ReplyAction="http://tempuri.org/IProfileService/UpdatePlayerProfileResponse")]
        void UpdatePlayerProfile(StrategoApp.ProfileService.PlayerInfoShownDTO newProfile);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/UpdatePlayerProfile", ReplyAction="http://tempuri.org/IProfileService/UpdatePlayerProfileResponse")]
        System.Threading.Tasks.Task UpdatePlayerProfileAsync(StrategoApp.ProfileService.PlayerInfoShownDTO newProfile);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetPlayerStatistics", ReplyAction="http://tempuri.org/IProfileService/GetPlayerStatisticsResponse")]
        void GetPlayerStatistics(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetPlayerStatistics", ReplyAction="http://tempuri.org/IProfileService/GetPlayerStatisticsResponse")]
        System.Threading.Tasks.Task GetPlayerStatisticsAsync(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetPlayerFriendsList", ReplyAction="http://tempuri.org/IProfileService/GetPlayerFriendsListResponse")]
        void GetPlayerFriendsList(int playerId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProfileService/GetPlayerFriendsList", ReplyAction="http://tempuri.org/IProfileService/GetPlayerFriendsListResponse")]
        System.Threading.Tasks.Task GetPlayerFriendsListAsync(int playerId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProfileServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IProfileService/PlayerInfo")]
        void PlayerInfo([System.ServiceModel.MessageParameterAttribute(Name="playerInfo")] StrategoApp.ProfileService.PlayerInfoResponse playerInfo1);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IProfileService/PlayerStatistics")]
        void PlayerStatistics([System.ServiceModel.MessageParameterAttribute(Name="playerStatistics")] StrategoApp.ProfileService.PlayerStatisticsResponse playerStatistics1);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IProfileService/ReceiveUpdatePlayerProfile")]
        void ReceiveUpdatePlayerProfile(StrategoApp.ProfileService.PlayerInfoResponse result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IProfileService/PlayerFriendsList")]
        void PlayerFriendsList(StrategoApp.ProfileService.PlayerFriendsResponse playerFriends);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProfileServiceChannel : StrategoApp.ProfileService.IProfileService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProfileServiceClient : System.ServiceModel.DuplexClientBase<StrategoApp.ProfileService.IProfileService>, StrategoApp.ProfileService.IProfileService {
        
        public ProfileServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ProfileServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ProfileServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ProfileServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ProfileServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void GetPlayerInfo(int playerId) {
            base.Channel.GetPlayerInfo(playerId);
        }
        
        public System.Threading.Tasks.Task GetPlayerInfoAsync(int playerId) {
            return base.Channel.GetPlayerInfoAsync(playerId);
        }
        
        public void UpdatePlayerProfile(StrategoApp.ProfileService.PlayerInfoShownDTO newProfile) {
            base.Channel.UpdatePlayerProfile(newProfile);
        }
        
        public System.Threading.Tasks.Task UpdatePlayerProfileAsync(StrategoApp.ProfileService.PlayerInfoShownDTO newProfile) {
            return base.Channel.UpdatePlayerProfileAsync(newProfile);
        }
        
        public void GetPlayerStatistics(int playerId) {
            base.Channel.GetPlayerStatistics(playerId);
        }
        
        public System.Threading.Tasks.Task GetPlayerStatisticsAsync(int playerId) {
            return base.Channel.GetPlayerStatisticsAsync(playerId);
        }
        
        public void GetPlayerFriendsList(int playerId) {
            base.Channel.GetPlayerFriendsList(playerId);
        }
        
        public System.Threading.Tasks.Task GetPlayerFriendsListAsync(int playerId) {
            return base.Channel.GetPlayerFriendsListAsync(playerId);
        }
    }
}
