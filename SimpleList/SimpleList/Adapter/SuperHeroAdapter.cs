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
using SimpleListView.Domain;
using FFImageLoading.Views;
using FFImageLoading;
using FFImageLoading.Transformations;
using Java.Lang;
using Android;

namespace SimpleListView
{
    public class SuperHeroAdapter : BaseAdapter
    {
       Activity _Context;
       List<Domain.Character> SuperHeroes;

        public SuperHeroAdapter(Activity context, List<Domain.Character> superHeroes): base()
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
            if (view == null)
                view = _Context.LayoutInflater.Inflate(Resource.Layout.SuperHeroRow, parent,false);

            Domain.Character superHero = SuperHeroes[position];

            TextView txtSuperheroName = view.FindViewById<TextView>(Resource.Id.txtSuperHeroName);
            ImageViewAsync image = view.FindViewById<ImageViewAsync>(Resource.Id.imgSuperHero);

            txtSuperheroName.Text = superHero.Name;
            ImageService.Instance.LoadUrl(superHero.ImageUrl)
                .Retry(3, 200)
                .DownSample(300, 300)
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