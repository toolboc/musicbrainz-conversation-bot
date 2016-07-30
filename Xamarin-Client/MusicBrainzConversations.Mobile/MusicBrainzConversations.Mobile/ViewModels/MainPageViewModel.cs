using MusicBrainzConversations.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public MainPageViewModel()
        {
            _pageTitle = "This is my page title";
            botService = new BotService();
            LetsGo();
        }

        private async void LetsGo()
        {
            // Start & Create a new conversation
            string ConversationId = await botService.StartConversation();

            // Send a message
            if (await botService.SendMessage("Hi there!"))
            {
                // Show message
                ConversationMessages messages = await botService.GetMessages();
            };

            // Send a message
            if (await botService.SendMessage("What's up"))
            {
                // Show message
                ConversationMessages messages = await botService.GetMessages();
            };

        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
