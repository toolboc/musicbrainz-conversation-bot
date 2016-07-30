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

        private bool _isChatList;

        public bool IsChatList
        {
            get { return _isChatList; }
            set
            {
                _isChatList = value;
                OnPropertyChanged();
            }
        }

        private bool _isAlbumList;

        public bool IsAlbumList
        {
            get { return _isAlbumList; }
            set
            {
                _isAlbumList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChatMessage> ChatMessages { get; private set; }
        public ObservableCollection<Album> Albums { get; private set; }

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
            //_textInput = "Type your question here.";

            ChatMessages = new ObservableCollection<ChatMessage>();

            _isChatList = true;
            _isAlbumList = false;

            Albums = new ObservableCollection<Album>();
            Albums.Add(
                new Album("Metallica", "https://lastfm-img2.akamaized.net/i/u/300x300/35ecb4f7affe489991e91d13f0d00485.png", "http://www.last.fm/music/Metallica/Metallica")                 
                );
            Albums.Add(
                new Album("Master of Puppets", "https://lastfm-img2.akamaized.net/i/u/300x300/07f492a00c904cc6ccf868010be4d5a6.png", "http://www.last.fm/music/Metallica/Master%2bof%2bPuppets")
            );
            Albums.Add(
                new Album("Ride the Lightning", "https://lastfm-img2.akamaized.net/i/u/300x300/dfe9c2366530411396c881090c2039d2.png", "http://www.last.fm/music/Metallica/Ride%2bthe%2bLightning")
            );
            Albums.Add(
                new Album("And Justice for All...", "https://lastfm-img2.akamaized.net/i/u/300x300/55bf2f31f75ac374ccdda18d6204824a.png", "http://www.last.fm/music/Metallica/...and%2bJustice%2bfor%2bAll")
            );

            botService = new BotService();
            LetsGo();
        }

        private async Task SendChatMessage()
        {
            var chatText = TextInput;

            // Temporary so we have the correct format. Need to mab the album object

            if (chatText.Contains("album"))
            {
                this.IsAlbumList = true;
                this.IsChatList = false;
                return;
            }

            this.TextInput = "";

            // Clear current messages
            ChatMessages.Clear();

            // Push question to client
            ChatMessages.Add(new ChatMessage { text = chatText });

            this.IsActivity = true;

            // Send a message
            if (await botService.SendMessage(chatText))
            {
                // Recieve Messages
                ConversationMessages messages = await botService.GetMessages();
                for (int i = 1; i < messages.messages.Length; i++)
                {
                    // Skip over first message

                    if (messages.messages[i].text.Contains("https:/"))
                    {
                        this.IsAlbumList = true;
                        this.IsChatList = false;
                    }
                    else
                    {
                        this.IsAlbumList = false;
                        this.IsChatList = true;
                    }

                    if (messages.messages[i].images.Count() > 0)
                    {
                        // its an image
                        var myImage = "http://mbbot.azurewebsites.net" + messages.messages[i].images[i - 1];
                        //ChatMessages.Add(new ChatMessage { text ="", imgsource = "http://mbbot.azurewebsites.net/" + messages.messages[i].images[i-1] });
                        // DO NOTHING FOR NOW
                    }
                    else
                    {
                        // its text
                        // Push Messages to Client
                        ChatMessages.Add(new ChatMessage { text = messages.messages[i].text, imgsource = "" });
                    }

                }
            }
            else
            {
                ChatMessages.Add(new ChatMessage { text = "", imgsource= "https://cdn.meme.am/instances/500x/69841816.jpg" });
                };
            this.IsActivity = false;
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
