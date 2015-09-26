using System;

using SharpNFC;
using System.Collections.Generic;

namespace NFCSharp_Test3
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			NFCContext context = new NFCContext ();

			NFCDevice device = context.OpenDevice ("pn532_uart:/dev/ttyAMA0");

			List<SharpNFC.PInvoke.nfc_modulation> modulations = new List<SharpNFC.PInvoke.nfc_modulation> ();
			modulations.Add(new SharpNFC.PInvoke.nfc_modulation() {nbr = SharpNFC.PInvoke.nfc_baud_rate.NBR_106,nmt = SharpNFC.PInvoke.nfc_modulation_type.NMT_ISO14443A});

			byte poolCount = 10;
			byte poolingInterval = 5;
			SharpNFC.PInvoke.nfc_target nfctarget = new SharpNFC.PInvoke.nfc_target ();

			while (true) {
				if (Console.ReadLine() != "STOP") {
					device.Pool (modulations, poolCount, poolingInterval,out nfctarget);
					printUID(nfctarget);
				} else {
					break;
				}
			}
		}
		public static void printUID(SharpNFC.PInvoke.nfc_target nfctarget)
		{

			for(int i = 0;i < nfctarget.nti.abtUid.Length;i++)
			{

				Console.Write (nfctarget.nti.abtUid[i].ToString("X"));
				Console.Write (" ");

			}
		}
	}
}
