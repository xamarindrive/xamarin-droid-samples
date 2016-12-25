using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using System.Collections.Generic;
using XamarinDrive.ActivityTransition.Domain;

namespace XamarinDrive.ActivityTransition
{
    public class SuperHeroAdapter : BaseAdapter
    {
        Activity _Context;
        List<Domain.Character> SuperHeroes;

        public SuperHeroAdapter(Activity context, List<Domain.Character> superHeroes) : base()
        {
            this._Context = context;
            this.SuperHeroes = superHeroes;
        }

        public override int Count
        {
            get
            {
                return SuperHeroes.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            Domain.Character superHero = SuperHeroes[position];

            if (superHero == null)
            {
                view = _Context.LayoutInflater.Inflate(Resource.Layout.layout_loading_item, parent, false);
                view.Tag = "";
                ProgressBar loading = view.FindViewById<ProgressBar>(Resource.Id.progressBar1);
                loading.Indeterminate = true;
                return view;
            }

            if (view == null || (view != null && !string.IsNullOrEmpty(view.Tag.ToString())))
                view = _Context.LayoutInflater.Inflate(Resource.Layout.SuperHeroRow, parent, false);

            view.Tag = position.ToString();

            TextView txtSuperheroName = view.FindViewById<TextView>(Resource.Id.txtSuperHeroName);
            ImageViewAsync image = view.FindViewById<ImageViewAsync>(Resource.Id.imgSuperHero);


            if (txtSuperheroName == null || image == null) { return view; }

            txtSuperheroName.Text = superHero.Name;
            ImageService.Instance.LoadUrl(superHero.ImageUrl)
                .Transform(new CircleTransformation())
                .LoadingPlaceholder("placeholder_avatar.png")
                .ErrorPlaceholder("placeholder_avatar.png")
                .Into(image);
            view.Click += (s, e) =>
            {
                int id = 0;
                if (int.TryParse(view.Tag.ToString(), out id))
                {
                    Character character = SuperHeroes[id];

                    var sceneTransitionAnimation = ActivityOptions.MakeSceneTransitionAnimation(_Context,
                                new Pair(image, SuperheroDetailActivity.View_SuperHero_Image),
                                new Pair(txtSuperheroName, SuperheroDetailActivity.View_SuperHero_Name));

                    var transitionBundle = sceneTransitionAnimation.ToBundle();
                    _Context.StartActivity(SuperheroDetailActivity.GetStartIntent(_Context, character), transitionBundle);
                }
            };

            return view;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
    }
}