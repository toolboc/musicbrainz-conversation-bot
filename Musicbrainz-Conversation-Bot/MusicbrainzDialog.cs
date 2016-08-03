using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Lastfm.Services;
using System.Configuration;
using Microsoft.Bot.Connector;

namespace Musicbrainz_Conversation_Bot
{
    [LuisModel("f49e3852-e143-46a7-98d4-91015ebbe130", "c2a3406f86e649459245d7b3960980ac")]
    [Serializable]
    public class MusicbrainzDialog : LuisDialog<object>
    {
        public MusicbrainzDialog(params ILuisService[] services) : base(services)
        {
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = "Here is a list of things I can do: \n\n Show me top songs from <Artist> \n\n Who sings <Song> \n\n Give me albums by <Artist> \n\n Tell me about <Artist> \n\n Who sounds like <Artist> ";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("ArtistSearch")]
        public async Task ArtistSearch(IDialogContext context, LuisResult result)
        {
            EntityRecommendation artistEntity;
            string artist = null;

            if (result.TryFindEntity("Artist", out artistEntity))
            {
                artist = artistEntity.Entity;
            }
            else
            {
                await context.PostAsync("I think you are searching for a particular artist, but I do not know how to act on your request at this time");
                context.Wait(MessageReceived);
                return;
            }


            string LastFmKey = ConfigurationManager.AppSettings["LastFMKey"];
            Session session = new Session(LastFmKey, "apiSecret");

            var artisan = new ArtistSearch(artist, session).GetFirstMatch();

            //string response = "![](" + artisan.GetImageURL() + ")\n\n";
            //response = response + artisan.Name + "\n\n" + artisan.Bio.GetSummary();

            Activity replyToConversation = context.MakeMessage() as Activity;
            replyToConversation.Attachments = new List<Attachment>();
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: artisan.GetImageURL()));

            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction()
            {
                Value = "http://musicbrainz.westus.cloudapp.azure.com:5000/artist/" + artisan.GetMBID(),
                Type = "openUrl",
                Title = "More info on our Musicbrainz server"
            };
            cardButtons.Add(plButton);
            HeroCard plCard = new HeroCard()
            {
                Title = artisan.Name,
                Subtitle = artisan.Bio.GetSummary(),
                Images = cardImages,
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);


            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("ReleaseSearch")]
        public async Task ReleaseSearch(IDialogContext context, LuisResult result)
        {
            EntityRecommendation artistEntity;
            string artist = null;

            if (result.TryFindEntity("Artist", out artistEntity))
            {
                artist = artistEntity.Entity;
            }
            else
            {
                await context.PostAsync("I think you are searching for albums by a particular artist, but I do not know how to act on your request at this time");
                context.Wait(MessageReceived);
                return;
            }

            string LastFmKey = ConfigurationManager.AppSettings["LastFMKey"];
            Session session = new Session(LastFmKey, "apiSecret");

            var releases = new ArtistSearch(artist, session).GetFirstMatch().GetTopAlbums().DistinctBy(x=>x.Item.Title).Take(5);

            Activity replyToConversation = context.MakeMessage() as Activity;
            replyToConversation.Attachments = new List<Attachment>();

            //string response = null;
            foreach (var release in releases)
            {
                List<CardImage> cardImages = new List<CardImage>();
                List<CardAction> cardButtons = new List<CardAction>();


                var albumArt = release.Item.GetImageURL();
                if (albumArt.Length > 10)
                    cardImages.Add(new CardImage(url: albumArt));
                else
                    continue;

                CardAction plButton = new CardAction()
                {
                    Value = "http://musicbrainz.westus.cloudapp.azure.com:5000/release/" + release.Item.GetMBID(),
                    Type = "openUrl",
                    Title = "More info on our Musicbrainz server"
                };

                cardButtons.Add(plButton);
                HeroCard plCard = new HeroCard()
                {
                    Title = release.Item.Artist.Name,
                    Subtitle = release.Item.Title,
                    Images = cardImages,
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);

                //response = response + "![](" + albumArt + ")" + "[" + release.Item.Title + "](" + release.Item.URL + ")\n\n"; //" (" + release.Item.GetReleaseDate().Year + ")\n\n";
            }

            await context.PostAsync(replyToConversation);

            context.Wait(MessageReceived);
        }

        [LuisIntent("TracklistSearch")]
        public async Task TracklistSearch(IDialogContext context, LuisResult result)
        {
            EntityRecommendation albumEntity;
            string album = null;

            if (result.TryFindEntity("Release", out albumEntity))
            {
                album = albumEntity.Entity;
            }
            else
            {
                await context.PostAsync("I think you are searching for artists songs on an album, but I do not know how to act on your request at this time");
                context.Wait(MessageReceived);
                return;
            }


            var releaseGroup = MusicBrainz.Search.ReleaseGroup(album, limit: 1);

            string response = null;
            foreach(var release in releaseGroup.Data)
            {

                response = response + release.Id + " (" + release.Score + "%)\n\n";
                                                
            }

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SimilarArtists")]
        public async Task SimilarArtists(IDialogContext context, LuisResult result)
        {
            EntityRecommendation artistEntity;
            string artist = null;

            if (result.TryFindEntity("Artist", out artistEntity))
            {
                artist = artistEntity.Entity;
            }
            else
            {
                await context.PostAsync("I think you are searching for artists similar to an artist, but I do not know how to act on your request at this time");
                context.Wait(MessageReceived);
                return;
            }

            string LastFmKey = ConfigurationManager.AppSettings["LastFMKey"];
            Session session = new Session(LastFmKey, "apiSecret");

            var artistSearch = new ArtistSearch(artist, session);
            var similarArtists = artistSearch.GetFirstMatch().GetSimilar().Take(10);

            string response = null;
            foreach (var artisan in similarArtists)
            {

                response = response + "[" + artisan.Name + "](" + artisan.URL +")\n\n";

            }

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        [LuisIntent("TopTracks")]
        public async Task TopTracks(IDialogContext context, LuisResult result)
        {
            EntityRecommendation artistEntity;
            string artist = null;

            if (result.TryFindEntity("Artist", out artistEntity))
            {
                artist = artistEntity.Entity;
            }
            else
            {
                await context.PostAsync("I think you are searching for top songs by an artist, but I do not know how to act on your request at this time");
                context.Wait(MessageReceived);
                return;
            }

            string LastFmKey = ConfigurationManager.AppSettings["LastFMKey"];
            Session session = new Session(LastFmKey, "apiSecret");

            var artistSearch = new ArtistSearch(artist, session);
            var topTracks = artistSearch.GetFirstMatch().GetTopTracks().Take(10);

            string response = null;
            foreach (var track in topTracks)
            {

                response = response + track.Item.Title + "\n\n";

            }

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SongSearch")]
        public async Task SongSearch(IDialogContext context, LuisResult result)
        {
            EntityRecommendation artistEntity;
            string song = null;

            if (result.TryFindEntity("Song", out artistEntity))
            {
                song = artistEntity.Entity;
            }
            else
            {
                await context.PostAsync("I think you are searching for an artist that plays a particular song, but I do not know how to act on your request at this time");
                context.Wait(MessageReceived);
                return;
            }

            var recordings = MusicBrainz.Search.Recording(song, limit: 10);

            string response = null;
            foreach(var recording in recordings.Data)
            {
                foreach(var artist in recording.Artistcredit)
                { 
                    response = response + artist.Artist.Name + " ";
                }
                response = response + "perform(s) " + "\"" + recording.Title + "\"" + " on " + recording.Releaselist.FirstOrDefault().Title + " (" + recording.Score + "%)\n\n";
            }

            await context.PostAsync(response);
            context.Wait(MessageReceived);
        }

        //[LuisIntent("LyricSearch")]
        //public async Task LyricSearch(IDialogContext context, LuisResult result)
        //{
        //    EntityRecommendation artistEntity;
        //    EntityRecommendation songEntity;
        //    string artist = null;
        //    string song = null;

        //    if (result.TryFindEntity("Artist", out artistEntity))
        //    {
        //        artist = artistEntity.Entity;
        //    }


        //    if (result.TryFindEntity("Song", out songEntity))
        //    {
        //        song = songEntity.Entity;
        //    }

        //    var lyrics = MusicApiCollection.Sites.Lyrics.LyricsOverload.Search.Lyric(artist, song);

        //    await context.PostAsync(lyrics.Data.Text);
        //    context.Wait(MessageReceived);
        //}
    }
    }