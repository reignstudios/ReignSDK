using System;
using Android.Runtime;
using Android.Views;
using Android.App;

namespace Reign.Core
{
	public static class AdMobWrapper
    {
        private static IntPtr _helperClass = JNIEnv.FindClass("reign/core/AdMobWrapper");

        public static void LoadAd(View view, bool testMode)
        {
            IntPtr methodId = JNIEnv.GetStaticMethodID(_helperClass, "loadAd", "(Landroid/view/View;Ljava/lang/Boolean;)V");
            var javaTestMode = new Java.Lang.Boolean(testMode);
            JNIEnv.CallStaticVoidMethod(_helperClass, methodId, new JValue(view), new JValue(javaTestMode));
        }

        public static void Destroy(View view)
        {
            IntPtr methodId = JNIEnv.GetStaticMethodID(_helperClass, "destroy", "(Landroid/view/View;)V");
            JNIEnv.CallStaticVoidMethod(_helperClass, methodId, new JValue(view));
        }
        
        public static View CreateAdView(Activity activity, string publisherID)
        {
            IntPtr methodId = JNIEnv.GetStaticMethodID(_helperClass, "createAdView", "(Landroid/app/Activity;Ljava/lang/String;)Landroid/view/View;");
            var javaPublisherID = new Java.Lang.String(publisherID);
            IntPtr viewPtr = JNIEnv.CallStaticObjectMethod(_helperClass, methodId, new JValue(activity), new JValue(javaPublisherID));
            var view = Java.Lang.Object.GetObject<View>(viewPtr, JniHandleOwnership.TransferLocalRef);
        	//JNIEnv.DeleteLocalRef(viewPtr);// Throws Error
        	
        	return view;
        }
    }
}

