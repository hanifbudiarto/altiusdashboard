﻿using System;
namespace AltiusDashboard
{
	public class License
	{
		public static string Key = "    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\n";
	}
}
