package reign.core;

import android.app.Activity;
import android.view.View;
import com.google.ads.*;

public class AdMobWrapper
{
	private static AdView view;

	private AdMobWrapper() {}

	public static void loadAd(View view, Boolean testMode)
	{
		AdRequest request = new AdRequest();
		
		if (testMode)
		{
			request.addTestDevice(AdRequest.TEST_EMULATOR);
			request.addTestDevice("TEST_DEVICE_ID");
		}
	
		((AdView)view).loadAd(request);
	}
	
	public static void destroy(View view)
	{
		((AdView)view).destroy();
	}
	
	public static View createAdView(Activity activity, String publisherID)
	{
		view = new AdView(activity, AdSize.BANNER, publisherID);
		
		view.setAdListener(new AdListener()
		{
			@Override
			public void onReceiveAd(Ad arg0)
			{
				view.setVisibility(AdView.VISIBLE);
			}
	        
	        @Override
	        public void onPresentScreen(Ad arg0) {}
	        
	        @Override     
	        public void onLeaveApplication(Ad arg0) {}
	
			@Override
	        public void onFailedToReceiveAd(Ad arg0, AdRequest.ErrorCode arg1)
	        {
	        	view.setVisibility(AdView.INVISIBLE);
	        }
	
			@Override
	        public void onDismissScreen(Ad arg0) {}
		});
		
		return view;
	}
}