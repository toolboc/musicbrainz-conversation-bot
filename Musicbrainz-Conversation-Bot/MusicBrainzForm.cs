using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Musicbrainz_Conversation_Bot
{

    public enum SearchOptions
    {
        Artist, Album, Song
    };

    [Serializable]
    public class MusicBrainzForm
    {
        public SearchOptions? Option;
        public static IForm<MusicBrainzForm> BuildForm()
        {

            return new FormBuilder<MusicBrainzForm>()
                    .Message("Welcome to the Music Brainz Query Bot!")
                    .Build();
        }
    };
}