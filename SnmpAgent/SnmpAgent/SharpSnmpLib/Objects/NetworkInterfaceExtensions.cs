﻿using System.Net.NetworkInformation;

namespace SharpSnmpLib
{
#if __MonoCS__
	public static class NetworkInterfaceExtensions
	{
		public static IPv4InterfaceStatistics GetIPStatistics(this NetworkInterface networkInterface)
		{
			return networkInterface.GetIPv4Statistics();
		}
	}
#endif
}
