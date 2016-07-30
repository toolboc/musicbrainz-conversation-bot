using MusicBrainzConversations.Mobile.Entities;
using MusicBrainzConversations.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MusicBrainzConversations.Mobile.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private BotService botService;

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }

        private string _botResponse;

        public string BotResponse
        {
            get { return _botResponse; }
            set { _botResponse = value; }
        }

        private string _textInput;

        public string TextInput
        {
            get { return _textInput; }
            set {
                _textInput = value;
                OnPropertyChanged();
            }
        }

        private bool _isActivity;

        public bool IsActivity
        {
            get { return _isActivity; }
            set {
                _isActivity = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChatMessage> ChatMessages { get; private set; }

        public Command SendMessage
        {
            get
            {
                return new Command(async () =>
                {
                    await SendChatMessage();
                });
            }
        }

        public MainPageViewModel()
        {
            //_pageTitle = "This is my page title";
            _isActivity = false;

            ChatMessages = new ObservableCollection<ChatMessage>();

            botService = new BotService();
            LetsGo();
        }

        private async Task SendChatMessage()
        {
            var chatText = TextInput;
            _textInput = "";

            // Clear current messages
            ChatMessages.Clear();

            // Push question to client
            ChatMessages.Add(new ChatMessage { text = chatText });

            _isActivity = true;

            // Send a message
            if (await botService.SendMessage(chatText))
            {
                // Recieve Messages
                ConversationMessages messages = await botService.GetMessages();
                for (int i = 1; i < messages.messages.Length; i++)
                {
                    // Skip over first message
                    // Push Messages to Client
                    ChatMessages.Add(new ChatMessage { text = messages.messages[i].text });
                }
            }
            else
            {
                ChatMessages.Add(new ChatMessage { text = "Oops! There was an error." });
            };
            _isActivity = false;
        }
        private async void LetsGo()
        {
            // Start & Create a new conversation
            string ConversationId = await botService.StartConversation();
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
