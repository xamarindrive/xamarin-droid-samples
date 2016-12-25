using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Collections.Generic;
using Marvel.Api;
using Marvel.Api.Results;
using System.Threading.Tasks;
using XamarinDrive.ActivityTransition.Domain;

namespace XamarinDrive.ActivityTransition
{
    [Activity(Label = "SimpleList", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string publicKey = "YOUR PUBLIC KEY";
        private const string privateKey = "YOUR PRIVATE KEY";

        private int offset = 0;
        private bool endReached = false;
        private MarvelRestClient client;

        private SuperHeroAdapter _adapter = null;

        public MainActivity()
        {
            client = new MarvelRestClient(publicKey, privateKey);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            try
            {
                var response = client.FindCharacters(new Marvel.Api.Filters.CharacterRequestFilter()
                {
                    Offset = offset,
                    Limit = 20
                });
                List<Character> Characters = ConvertIntoCharacters(response);

                ListView lstSuperHero = FindViewById<ListView>(Resource.Id.lstSuperHeroes);
                _adapter = new SuperHeroAdapter(this, Characters);
                lstSuperHero.Adapter = _adapter;

                lstSuperHero.Scroll += async (o, e) =>
                {
                    if (e.FirstVisibleItem + e.VisibleItemCount == e.TotalItemCount && !endReached && e.TotalItemCount > 0)
                    {
                        int loaderPosition = _adapter.Count;
                        Characters.Add(null);
                        _adapter.NotifyDataSetChanged();
                        endReached = true;
                        offset += 20;
                        await Task.Delay(100);
                        response = client.FindCharacters(new Marvel.Api.Filters.CharacterRequestFilter()
                        {
                            Offset = offset,
                            Limit = 20
                        });
                        List<Character> charactersToAdd = ConvertIntoCharacters(response);

                        if (Characters.Count >= loaderPosition && Characters[loaderPosition] == null)
                        {
                            Characters.RemoveAt(loaderPosition);
                            _adapter.NotifyDataSetChanged();
                        }
                        if (charactersToAdd.Count > 0)
                        {
                            Characters.AddRange(charactersToAdd);
                            _adapter.NotifyDataSetChanged();
                            endReached = false;
                        }
                    }
                };

            }
            catch (Exception ex)
            {

            }
        }

        private List<Character> ConvertIntoCharacters(CharacterResult response)
        {
            List<Character> Characters = new List<Character>();
            if (response != null && response.Data != null && response.Data.Results != null)
                foreach (var character in response.Data.Results)
                {
                    Characters.Add(new Character()
                    {
                        Name = character.Name,
                        ImageUrl = string.Format("{0}.{1}", character.Thumbnail.Path, character.Thumbnail.Extension)
                    });
                }
            return Characters;

        }
    }
}

