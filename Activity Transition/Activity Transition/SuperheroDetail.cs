using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinDrive.ActivityTransition.Domain;
using FFImageLoading.Views;
using FFImageLoading;

namespace XamarinDrive.ActivityTransition
{
    [Activity(Label = "SuperheroDetail")]
    public class SuperheroDetailActivity : Activity
    {
        public const string View_SuperHero_Image = "DETAIL_SUPERHERO_IMAGE";
        public const string View_SuperHero_Name = "DETAIL_SUPERHERO_NAME";


        private ImageViewAsync profileImage;
        private TextView superHeroName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SuperHeroDetail);

            profileImage = FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            superHeroName = FindViewById<TextView>(Resource.Id.txtSuperHeroName);

            profileImage.TransitionName = View_SuperHero_Image;
            superHeroName.TransitionName = View_SuperHero_Name;


            string name = Intent.GetStringExtra("CharacterName");
            string profileUrl = Intent.GetStringExtra("CharacterImage");


            superHeroName.Text = name;
            ImageService.Instance.LoadUrl(profileUrl)                
                .LoadingPlaceholder("placeholder_avatar.png")
                .ErrorPlaceholder("placeholder_avatar.png")
                .Into(profileImage);


        }

        internal static Intent GetStartIntent(Context context, Character character)
        {
            var starter = new Intent(context, typeof(SuperheroDetailActivity));
            starter.PutExtra("Character", character.Id);
            starter.PutExtra("CharacterName", character.Name);
            starter.PutExtra("CharacterImage", character.ImageUrl);
            return starter;
        }
    }
}