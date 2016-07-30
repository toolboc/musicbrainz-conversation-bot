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
            set { _pageTitle = value; }
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
            set { _textInput = value; }
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
            _pageTitle = "This is my page title";
            botService = new BotService();
            ChatMessages = new ObservableCollection<ChatMessage>();
            //ChatMessages.Add(new ChatMessage { text = "Hey there!" });

            LetsGo();
        }

        private async Task SendChatMessage()
        {
            // Push question to client
            ChatMessages.Add(new ChatMessage { text = TextInput });

            // Send a message
            if (await botService.SendMessage(TextInput))
            {
                // Recieve Messages
                ConversationMessages messages = await botService.GetMessages();
                for (int i = 1; i < messages.messages.Length; i++)
                {
                    // Skip over first message
                    // Push Messages to Client
                    ChatMessages.Add(new ChatMessage { text = messages.messages[i].text });
                }
            };
        }
        private async void LetsGo()
        {
            // Start & Create a new conversation
            string ConversationId = await botService.StartConversation();

            //// Send a message
            //if (await botService.SendMessage("Who plays enter sandman?"))
            //{
            //    // Show message
            //    ConversationMessages messages = await botService.GetMessages();
            //    for (int i = 0; i < messages.messages.Length; i++)
            //    {
            //        ChatMessages.Add(new ChatMessage { text = messages.messages[i].text });
            //    }
            //};

            //// Send a message
            //if (await botService.SendMessage("Who plays Hotel California?"))
            //{
            //    // Show message
            //    ConversationMessages messages = await botService.GetMessages();
            //    for (int i = 0; i < messages.messages.Length; i++)
            //    {
            //        ChatMessages.Add(new ChatMessage { text = messages.messages[i].text });
            //    }
            //};

        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
