using System;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public class PageUtils
	{
		public static bool PageTypeIsAlreadyAtTopOfStack(ContentPage parentPage, Type typeOfPageAppearing) 
		{
			var stack = parentPage.Navigation.NavigationStack;
			return (stack[stack.Count - 1].GetType() == typeOfPageAppearing);
		}
	}
}
