﻿using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.SlicerConfiguration;
using MatterHackers.VectorMath;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System;

namespace MatterHackers.MatterControl.ConfigurationPage.PrintLeveling
{
	public class PrintLevelingData
	{
		public List<Vector3> SampledPositions = new List<Vector3>();
		private static bool activelyLoading = false;

		private static Printer activePrinter = null;

		private static PrintLevelingData instance = null;

		private Vector3 probeOffset0Private;

		private Vector3 probeOffset1Private;

		private Vector3 sampledPosition0Private;

		private Vector3 sampledPosition1Private;

		private Vector3 sampledPosition2Private;

		[JsonConverter(typeof(StringEnumConverter))]
		public enum LevelingSystem { Probe3Points, Probe2Points, Probe7PointRadial, Probe13PointRadial }

		public LevelingSystem CurrentPrinterLevelingSystem
		{
			get
			{
				switch (ActiveSliceSettings.Instance.GetActiveValue("print_leveling_solution"))
				{
					case "2 Point Plane":
						return LevelingSystem.Probe2Points;

					case "7 Point Disk":
						return LevelingSystem.Probe7PointRadial;

					case "13 Point Disk":
						return LevelingSystem.Probe13PointRadial;

					case "3 Point Plane":
					default:
						return LevelingSystem.Probe3Points;
				}
			}
		}

		public Vector3 ProbeOffset0
		{
			get { return probeOffset0Private; }
			set
			{
				if (probeOffset0Private != value)
				{
					probeOffset0Private = value;
					Commit();
				}
			}
		}

		public Vector3 ProbeOffset1
		{
			get { return probeOffset1Private; }
			set
			{
				if (probeOffset1Private != value)
				{
					probeOffset1Private = value;
					Commit();
				}
			}
		}

		public Vector3 SampledPosition0
		{
			get { return sampledPosition0Private; }
			set
			{
				if (sampledPosition0Private != value)
				{
					sampledPosition0Private = value;
					Commit();
				}
			}
		}

		public Vector3 SampledPosition1
		{
			get { return sampledPosition1Private; }
			set
			{
				if (sampledPosition1Private != value)
				{
					sampledPosition1Private = value;
					Commit();
				}
			}
		}

		public Vector3 SampledPosition2
		{
			get { return sampledPosition2Private; }
			set
			{
				if (sampledPosition2Private != value)
				{
					sampledPosition2Private = value;
					Commit();
				}
			}
		}

		public static PrintLevelingData GetForPrinter(Printer printer)
		{
			if (printer != null)
			{
				if (activePrinter != printer)
				{
					CreateFromJsonOrLegacy(printer.PrintLevelingJsonData, printer.PrintLevelingProbePositions);
					activePrinter = printer;
				}
			}
			return instance;
		}

		public void Commit()
		{
			if (!activelyLoading)
			{
				string newLevelingInfo = Newtonsoft.Json.JsonConvert.SerializeObject(this);

				// clear the legacy value
				activePrinter.PrintLevelingProbePositions = "";
				// set the new value
				activePrinter.PrintLevelingJsonData = newLevelingInfo;
				activePrinter.Commit();
			}
		}

		private static void CreateFromJsonOrLegacy(string jsonData, string depricatedPositionsCsv3ByXYZ)
		{
			if (jsonData != null)
			{
				activelyLoading = true;
				instance = Newtonsoft.Json.JsonConvert.DeserializeObject<PrintLevelingData>(jsonData);
				activelyLoading = false;
			}
			else if (depricatedPositionsCsv3ByXYZ != null)
			{
				instance = new PrintLevelingData();
				instance.ParseDepricatedPrintLevelingMeasuredPositions(depricatedPositionsCsv3ByXYZ);
			}
			else
			{
				instance = new PrintLevelingData();
			}
		}

		/// <summary>
		/// Gets the 9 {3 * (x, y, z)} positions that were probed during the print leveling setup.
		/// </summary>
		/// <returns></returns>
		private void ParseDepricatedPrintLevelingMeasuredPositions(string depricatedPositionsCsv3ByXYZ)
		{
			if (depricatedPositionsCsv3ByXYZ != null)
			{
				string[] lines = depricatedPositionsCsv3ByXYZ.Split(',');
				if (lines.Length == 9)
				{
					for (int i = 0; i < 3; i++)
					{
						sampledPosition0Private[i % 3] = double.Parse(lines[0 * 3 + i]);
						sampledPosition1Private[i % 3] = double.Parse(lines[1 * 3 + i]);
						sampledPosition2Private[i % 3] = double.Parse(lines[2 * 3 + i]);
					}
				}
			}
		}

		public bool HasBeenRun()
		{
			switch (CurrentPrinterLevelingSystem)
			{
				case PrintLevelingData.LevelingSystem.Probe2Points:
				case PrintLevelingData.LevelingSystem.Probe3Points:
					if (SampledPosition0.z == 0
						&& SampledPosition1.z == 0
						&& SampledPosition2.z == 0)
					{
						return false;
					}
					break;

				case PrintLevelingData.LevelingSystem.Probe7PointRadial:
					if (SampledPositions.Count != 7) // different criteria for what is not initialized
					{
						return false;
					}
					break;

				case PrintLevelingData.LevelingSystem.Probe13PointRadial:
					if (SampledPositions.Count != 13) // different criteria for what is not initialized
					{
						return false;
					}
					break;

				default:
					throw new NotImplementedException();
			}

			return true;
		}

		public void RunLevelingWizard()
		{
			LevelWizardBase.RuningState runningState = LevelWizardBase.RuningState.UserRequestedCalibration;
			if (ActiveSliceSettings.Instance.LevelingRequiredToPrint)
			{
				// run in the first run state
				runningState = LevelWizardBase.RuningState.InitialStartupCalibration;
			}

			LevelWizardBase.ShowPrintLevelWizard(runningState);
		}
	}
}