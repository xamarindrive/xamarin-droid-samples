using Android.App;
using Android.Widget;
using Android.OS;
using SimpleListView;
using System;
using System.Collections.Generic;
using SimpleListView.Domain;
using Marvel.Api;

namespace SimpleListView
{
    [Activity(Label = "SimpleList", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string publicKey = "YOUR PUBLIC KEY";
        private const string privateKey = "YOUR PRIVATE KEY";



        private SuperHeroAdapter _adapter = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            try
            {
                MarvelRestClient client = new MarvelRestClient(publicKey, privateKey);
                var response = client.FindCharacters();
                List<Character> Characters = new List<Character>();
                foreach (var character in response.Data.Results)
                {
                    Characters.Add(new Character()
                    {
                        Name = character.Name,
                        ImageUrl = string.Format("{0}.{1}", character.Thumbnail.Path, character.Thumbnail.Extension)
                    });
                }

                ListView lstSuperHero = FindViewById<ListView>(Resource.Id.lstSuperHeroes);
                _adapter = new SuperHeroAdapter(this, Characters);
                lstSuperHero.Adapter = _adapter;

            }
            catch (Exception ex)
            {

            }
        }

    }
}

