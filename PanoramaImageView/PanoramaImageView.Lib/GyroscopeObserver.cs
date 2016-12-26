using System;
using Android.Runtime;
using Android.Hardware;
using System.Collections.Generic;
using Android.Content;


namespace PanoramaImageView.Lib
{
    public class GyroscopeObserver : Java.Lang.Object, ISensorEventListener
    {
        private SensorManager mSensorManger;

        private static float NS2S = 1.0f / 1000000000.0f;

        private long mLastTimestamp;
        private double mRotateRadianY;
        private double mRotateRadianX;
        private double mMaxRotateRadian = Math.PI / 9;

        private LinkedList<PanoramaImageView> mViews = new LinkedList<PanoramaImageView>();

        public void Register(Context context)
        {
            if (mSensorManger == null)
                mSensorManger = (SensorManager)context.GetSystemService(Context.SensorService);

            Sensor mSensor = mSensorManger.GetDefaultSensor(SensorType.Gyroscope);
            mSensorManger.RegisterListener(this, mSensor, SensorDelay.Fastest);

            mLastTimestamp = 0;
            mRotateRadianX = mRotateRadianY = 0;
        }

        public void UnRegister()
        {
            if (mSensorManger != null)
            {
                mSensorManger.UnregisterListener(this);
                mSensorManger = null;
            }
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (mLastTimestamp == 0)
            {
                mLastTimestamp = e.Timestamp;
                return;
            }

            float rotateX = Math.Abs(e.Values[0]);
            float rotateY = Math.Abs(e.Values[1]);
            float rotateZ = Math.Abs(e.Values[2]);

            if (rotateY > rotateX + rotateZ)
            {
                float dT = (e.Timestamp - mLastTimestamp) * NS2S;
                mRotateRadianY += e.Values[1] * dT;
                if (mRotateRadianY > mMaxRotateRadian)
                    mRotateRadianY = mMaxRotateRadian;
                else if (mRotateRadianY < -mMaxRotateRadian)
                    mRotateRadianY = -mMaxRotateRadian;
                else
                {
                    foreach (PanoramaImageView view in mViews)
                        if (view != null && view.getOrientation() == PanoramaImageView.ORIENTATION_HORIZONTAL)
                            view.updateProgress((float)(mRotateRadianY / mMaxRotateRadian));
                }
            }
            else if (rotateX > rotateY + rotateZ)
            {
                float dT = (e.Timestamp - mLastTimestamp) * NS2S;
                mRotateRadianX += e.Values[0] * dT;
                if (mRotateRadianX > mMaxRotateRadian)
                    mRotateRadianX = mMaxRotateRadian;
                else if (mRotateRadianX < -mMaxRotateRadian)
                    mRotateRadianX = -mMaxRotateRadian;
                else
                {
                    foreach (PanoramaImageView view in mViews)
                        if (view != null && view.getOrientation() == PanoramaImageView.ORIENTATION_VERTICAL)
                            view.updateProgress((float)(mRotateRadianX / mMaxRotateRadian));

                }
            }
            mLastTimestamp = e.Timestamp;
        }

        internal void AddPanoramaImageView(PanoramaImageView panoramaImageView)
        {
            if (panoramaImageView != null && !mViews.Contains(panoramaImageView))
            {
                mViews.AddFirst(panoramaImageView);
            }
        }

        public void setMaxRotateRadian(double maxRotateRadian)
        {
            if (maxRotateRadian <= 0 || maxRotateRadian > Math.PI / 2)
            {
                throw new ArgumentException("The maxRotateRadian must be between (0, π/2].");
            }
            this.mMaxRotateRadian = maxRotateRadian;
        }
    }
}