using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace AltiusDashboard.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			C1.Xamarin.Forms.Chart.Platform.iOS.FlexChartRenderer.Init();
			C1.Xamarin.Forms.Chart.Platform.iOS.FlexPieRenderer.Init();
			C1.Xamarin.Forms.Grid.Platform.iOS.FlexGridRenderer.Init();
			C1.Xamarin.Forms.Input.Platform.iOS.C1InputRenderer.Init();
			C1.Xamarin.Forms.Calendar.Platform.iOS.C1CalendarRenderer.Init();
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
