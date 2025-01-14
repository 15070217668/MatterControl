﻿using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using MatterHackers.GuiAutomation;
using MatterHackers.Agg.PlatformAbstract;
using System.IO;
using MatterHackers.MatterControl.CreatorPlugins;
using MatterHackers.Agg.UI.Tests;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.DataStorage;
using System.Diagnostics;
using System.Collections.Generic;
using MatterHackers.MatterControl.UI;
using MatterHackers.MatterControl.PrintLibrary.Provider;


namespace MatterHackers.MatterControl.UI
{
	[TestFixture, Category("MatterControl.UI"), RunInApplicationDomain]
	public class HardwareLevelingUITests
	{
		[Test, RequiresSTA, RunInApplicationDomain]
		public void HasHardwareLevelingHidesLevelingSettings()
		{
			Action<AutomationTesterHarness> testToRun = (AutomationTesterHarness resultsHarness) =>
			{
				AutomationRunner testRunner = new AutomationRunner(MatterControlUtilities.DefaultTestImages);
				{
					//Add printer that has hardware leveling
					MatterControlUtilities.SelectAndAddPrinter(testRunner, "Airwolf 3D", "HD", true);

					testRunner.Wait(1);
					testRunner.ClickByName("SettingsAndControls");
					testRunner.Wait(1);
					testRunner.ClickByName("Slice Settings Tab");
					testRunner.Wait(1);
					testRunner.ClickByName("User Level Dropdown");
					testRunner.Wait(1);
					testRunner.ClickByName("Advanced Menu Item");
					testRunner.Wait(1);
					testRunner.ClickByName("Printer Tab");
					testRunner.Wait(1);

					//Make sure Print Leveling tab is not visible 
					bool testPrintLeveling = testRunner.WaitForName("Print Leveling Tab", 3);
					resultsHarness.AddTestResult(testPrintLeveling == false);

					//Add printer that does not have hardware leveling
					MatterControlUtilities.SelectAndAddPrinter(testRunner, "Deezmaker", "Bukito", false);
					testRunner.ClickByName("Slice Settings Tab");
					testRunner.Wait(1);
					testRunner.ClickByName("Printer Tab");

					//Make sure Print Leveling tab is visible
					bool printLevelingVisible = testRunner.WaitForName("Print Leveling Tab", 2);
					resultsHarness.AddTestResult(printLevelingVisible == true);

					MatterControlUtilities.CloseMatterControl(testRunner);
				}
			};

			AutomationTesterHarness testHarness = MatterControlUtilities.RunTest(testToRun);

			Assert.IsTrue(testHarness.AllTestsPassed);
			Assert.IsTrue(testHarness.TestCount == 2); // make sure we ran all our tests
		}
	}
}

