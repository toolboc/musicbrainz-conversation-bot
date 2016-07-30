using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;

namespace Musicbrainz_Conversation_Bot
{
    [LuisModel("f49e3852-e143-46a7-98d4-91015ebbe130", "c2a3406f86e649459245d7b3960980ac")]
    [Serializable]
    public class MusicbrainzDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
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

            var artists = MusicBrainz.Search.Artist(artist, limit: 10);

            string response = null;
            foreach (var artisan in artists.Data)
            {
                response = response + artisan.Name + " | " + artisan.Type + " | " + artisan.Country + " | " + artisan.Disambiguation + " | " + "(" + artisan.Score + "%)\n\n";
            }

            await context.PostAsync(response);
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

            var releases = MusicBrainz.Search.ReleaseGroup(artist: artist, type: "Album", limit: 5);

            string response = null;
            foreach (var release in releases.Data)
            {
                response = response + release.Title + " (" + release.Score + "%)\n\n";
            }

            await context.PostAsync(response);
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

            var releaseGroup = MusicBrainz.Search.ReleaseGroup(album, limit: 1);

            string response = null;
            foreach(var release in releaseGroup.Data)
            {

                response = response + release.Id + " (" + release.Score + "%)\n\n";
                                                
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

            var recordings = MusicBrainz.Search.Recording(song, limit: 3);

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