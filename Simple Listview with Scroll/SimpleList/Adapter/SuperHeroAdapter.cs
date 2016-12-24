using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using FFImageLoading;
using FFImageLoading.Transformations;
using Android;

namespace XamarinDrive.SimpleScrollView
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

            if (view == null ||( view != null && view.Tag.ToString() != "SuperHero"))
                view = _Context.LayoutInflater.Inflate(Resource.Layout.SuperHeroRow, parent, false);

            view.Tag = "SuperHero";

            TextView txtSuperheroName = view.FindViewById<TextView>(Resource.Id.txtSuperHeroName);
            ImageViewAsync image = view.FindViewById<ImageViewAsync>(Resource.Id.imgSuperHero);

            txtSuperheroName.Text = superHero.Name;
            ImageService.Instance.LoadUrl(superHero.ImageUrl)
                .Transform(new CircleTransformation())
                .LoadingPlaceholder("placeholder_avatar.png")
                .ErrorPlaceholder("placeholder_avatar.png")
                .Into(image);

            return view;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
    }
}