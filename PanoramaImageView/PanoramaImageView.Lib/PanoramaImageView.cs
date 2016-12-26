using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using System;

namespace PanoramaImageView.Lib
{
    public class PanoramaImageView : ImageView
    {
        //Image's Scroll Orientation
        public const SByte ORIENTATION_NONE = -1;
        public const SByte ORIENTATION_HORIZONTAL = 0;
        public const SByte ORIENTATION_VERTICAL = 1;
        private SByte mOrientation = ORIENTATION_NONE;

        //Enable Panaroma effect or not
        public bool mEnablePanoramaMode { get; set; }

        //if true, the image scroll left(top) when the device clockwise rotate along y-axis(x-axis).
        private bool mInvertScrollDirection;

        //Image's Height and width
        private int mDrawableWidth;
        private int mDrawableHeight;

        //View's height and widht
        private int mWidth;
        private int mHeight;

        //Image's offset from initial state( center in the view).
        private float mMaxOffset;

        // The scroll progress
        private float mProgress;

        //show scrollbar or not
        private bool mEnableScrollbar;

        // The paint to draw scrollbar
        private Paint mScrollbarPaint;

        private IOnPanoramaScrollListener mOnPanoramaScrollListener;


        public PanoramaImageView(Context context) : this(context, null) { }
        public PanoramaImageView(Context context, IAttributeSet attr) : this(context, attr, 0) { }
        public PanoramaImageView(Context context, IAttributeSet attr, int defStyleAttr) : base(context, attr, defStyleAttr)
        {
            this.SetScaleType(ScaleType.CenterCrop);
            
            TypedArray typedArray = context.ObtainStyledAttributes(attr, Resource.Styleable.PanoramaImageView);
            mEnablePanoramaMode = typedArray.GetBoolean(Resource.Styleable.PanoramaImageView_piv_enablePanoramaMode, true);
            mInvertScrollDirection = typedArray.GetBoolean(Resource.Styleable.PanoramaImageView_piv_invertScrollDirection, false);
            mEnableScrollbar = typedArray.GetBoolean(Resource.Styleable.PanoramaImageView_piv_show_scrollbar, true);
            typedArray.Recycle();
            
            if (mEnableScrollbar)
                InitScrollbarPaint();
            
        }


        private void InitScrollbarPaint()
        {
            mScrollbarPaint = new Paint(PaintFlags.AntiAlias);
            mScrollbarPaint.Color = Color.White;
            mScrollbarPaint.StrokeWidth = dp2px(1.5f);
        }

        public void SetGyroscopeObserver(GyroscopeObserver observer)
        {
            if (observer != null)
                observer.AddPanoramaImageView(this);
        }

        private float dp2px(float dp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.System.DisplayMetrics);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            mWidth = MeasureSpec.GetSize(widthMeasureSpec) - PaddingLeft - PaddingRight;
            mHeight = MeasureSpec.GetSize(heightMeasureSpec) - PaddingTop - PaddingBottom;

            if (Drawable != null)
            {
                mDrawableHeight = Drawable.IntrinsicHeight;
                mDrawableWidth = Drawable.IntrinsicWidth;

                if (mDrawableWidth * mHeight > mDrawableHeight * mWidth)
                {
                    mOrientation = ORIENTATION_HORIZONTAL;
                    float imgScale = (float)mHeight / (float)mDrawableHeight;
                    mMaxOffset = Math.Abs((mDrawableWidth * imgScale - mWidth) * 0.5f);
                }
                else if (mDrawableHeight * mWidth > mDrawableWidth * mHeight)
                {
                    mOrientation = ORIENTATION_VERTICAL;
                    float imgScale = (float)mWidth / (float)mDrawableWidth;
                    mMaxOffset = Math.Abs((mDrawableHeight * imgScale - mHeight) * 0.5f);
                }
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (!mEnablePanoramaMode || Drawable == null || IsInEditMode)
            {
                base.OnDraw(canvas);
                return;
            }

            //Draw Image
            if (mOrientation == ORIENTATION_HORIZONTAL)
            {
                float currentOffsetX = mMaxOffset * mProgress;
                canvas.Save();
                canvas.Translate(currentOffsetX, 0);
                base.OnDraw(canvas);
                canvas.Restore();
            }
            else if (mOrientation == ORIENTATION_VERTICAL)
            {
                float currentOffsetY = mMaxOffset * mProgress;
                canvas.Save();
                canvas.Translate(0, currentOffsetY);
                base.OnDraw(canvas);
                canvas.Restore();
            }

            // Draw scrollbar
            if (mEnableScrollbar)
            {
                switch (mOrientation)
                {
                    case ORIENTATION_HORIZONTAL:
                        float barBgWidth = mWidth * 0.9f;
                        float barWidth = barBgWidth * mWidth / mDrawableWidth;

                        float barBgStartX = (mWidth - barBgWidth) / 2;
                        float barBgEndX = barBgStartX + barBgWidth;
                        float barStartX = barBgStartX + (barBgWidth - barWidth) / 2 * (1 - mProgress);
                        float barEndX = barStartX + barWidth;
                        float barY = mHeight * 0.95f;

                        mScrollbarPaint.Alpha = 100;
                        canvas.DrawLine(barBgStartX, barY, barBgEndX, barY, mScrollbarPaint);
                        mScrollbarPaint.Alpha = 255;
                        canvas.DrawLine(barStartX, barY, barEndX, barY, mScrollbarPaint);
                        break;
                    case ORIENTATION_VERTICAL:

                        float barBgHeight = mHeight * 0.9f;
                        float barHeight = barBgHeight * mHeight / mDrawableHeight;

                        float barBgStartY = (mHeight - barBgHeight) / 2;
                        float barBgEndY = barBgStartY + barBgHeight;
                        float barStartY = barBgStartY + (barBgHeight - barHeight) / 2 * (1 - mProgress);
                        float barEndY = barStartY + barHeight;
                        float barX = mWidth * 0.95f;

                        mScrollbarPaint.Alpha = 100;
                        canvas.DrawLine(barX, barBgStartY, barX, barBgEndY, mScrollbarPaint);
                        mScrollbarPaint.Alpha = 255;
                        canvas.DrawLine(barX, barStartY, barX, barEndY, mScrollbarPaint);

                        break;
                }
            }
        }

        public override void SetScaleType(ScaleType scaleType)
        {
            base.SetScaleType(scaleType);
        }

        public void SetOnPanoramaScrollListener(IOnPanoramaScrollListener listener)
        {
            mOnPanoramaScrollListener = listener;
        } 

        public void SetInvertScrollDirection(bool invert)
        {
            if (mInvertScrollDirection != invert)
                mInvertScrollDirection = invert;
        }

        public void SetEnableScrollbar(bool enable)
        {
            if(mEnableScrollbar != enable)
            {
                mEnableScrollbar = enable;
                if (mEnableScrollbar)
                {
                    InitScrollbarPaint();
                }else
                {
                    mScrollbarPaint = null;
                }
            }
        }


        public bool isInvertScrollDirection()
        {
            return mInvertScrollDirection;
        }

        public SByte getOrientation()
        {
            return mOrientation;
        }

        public void updateProgress(float progress)
        {
            if (mEnablePanoramaMode)
            {
                mProgress = mInvertScrollDirection ? -progress : progress;
                Invalidate();                
                if (mOnPanoramaScrollListener != null)
                {
                    mOnPanoramaScrollListener.OnScrolled(this, -mProgress);
                    //OnScrolled(this, -mProgress);
                }
            }
        } 
    }

    public interface IOnPanoramaScrollListener
    {
        void OnScrolled(PanoramaImageView view, float offsetProgress);
    }

    public class PanoramaScrollListener : IOnPanoramaScrollListener
    {
        public void OnScrolled(PanoramaImageView view, float offsetProgress)
        {
            
        }
    }
}
