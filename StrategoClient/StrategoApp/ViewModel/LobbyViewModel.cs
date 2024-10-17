using log4net;
using StrategoApp.Helpers;
using StrategoApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategoApp.ViewModel
{
    internal class LobbyViewModel : ViewModelBase, Service.IChatServiceCallback
    {
        private static readonly ILog Log = Log<LobbyViewModel>.GetLogger();

        private bool _isViewEnabled;
        private string _messageToSend;
        private string _errorMessage;
        private int _userId;
        private string _username;
        private ChatServiceClient _chatClient;
        private MainWindowViewModel _mainWindowViewModel;
        private ObservableCollection<string> _messages;
        private ICommand _sendMessagesCommand;

        public ICommand SendMessageCommand { get; }

        public LobbyViewModel(MainWindowViewModel mainWindowViewModel)
        {
            Random random = new Random();
            _userId = random.Next(1, 1000);
            _username = "User " + _userId;
            Connection();
            _mainWindowViewModel = mainWindowViewModel;
            SendMessageCommand = new ViewModelCommand(ClientSendMessage, CanSendMessage);
            _messages = new ObservableCollection<string>();
        }

        public LobbyViewModel()
        {
            {
                _messages = new ObservableCollection<string>();
                _sendMessagesCommand = new ViewModelCommand(ClientSendMessage, CanSendMessage);
                SendMessageCommand = _sendMessagesCommand;

            }
        }

        public void Connection()
        {
            InstanceContext context = new InstanceContext(this);
            _chatClient = new ChatServiceClient(context);

            try
            {
                _chatClient.Connect(_userId, _username);
                MessageBox.Show($"{_username} conectado al chat.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar: {ex.Message}");
            }
        }

        ~LobbyViewModel()
        {
            Disconnection();
        }

        public void Disconnection()
        {
            if (_chatClient != null)
            {
                try
                {
                    _chatClient.Disconnect(_userId);
                    _chatClient.Close();
                    MessageBox.Show("Desconectado del chat.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desconectar: {ex.Message}");
                }
            }
        }

        public ObservableCollection<string> Messages 
        { 
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged(nameof(Messages));
                }

            }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                _messageToSend = value;
                OnPropertyChanged();
            }
        }

        public bool IsViewEnabled
        {
            get { return _isViewEnabled; }
            set
            {
                _isViewEnabled = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private bool CanSendMessage(object obj)
        {
            return !string.IsNullOrWhiteSpace(MessageToSend);
        }

        private void ClientSendMessage(object obj)
        {
            try
            {
                _chatClient.SendMessage(_userId, _username, MessageToSend);
                MessageToSend = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje: {ex.Message}");
            }
        }

        public void ReceiveMessage(string username, string message)
        {
            Messages.Add($"{username}: {message}");
            OnPropertyChanged(nameof(Messages));
        }
    }
}
