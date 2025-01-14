﻿using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.ConfigurationPage.PrintLeveling;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrinterCommunication;
using MatterHackers.MatterControl.PrintQueue;
using MatterHackers.MatterControl.SlicerConfiguration;

#if __ANDROID__
using MatterHackers.SerialPortCommunication.FrostedSerial;
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MatterHackers.MatterControl.ActionBar
{
	internal class PrintActionRow : ActionRowBase
	{
		private List<Button> activePrintButtons = new List<Button>();
		private Button addButton;
		private List<Button> allPrintButtons = new List<Button>();
		private Button cancelButton;
		private Button cancelConnectButton;
		private string cancelCurrentPrintMessage = "Cancel the current print?".Localize();
		private string cancelCurrentPrintTitle = "Cancel Print?".Localize();
		private Button connectButton;
		private Button resetConnectionButton;
		private Button doneWithCurrentPartButton;
		private Button pauseButton;
		private QueueDataView queueDataView;
		private Button removeButton;
		private Button reprintButton;
		private Button resumeButton;
		private Button skipButton;
		private Button startButton;
		private Button configureButton;
		private MatterHackers.MatterControl.TextImageButtonFactory textImageButtonFactory = new MatterHackers.MatterControl.TextImageButtonFactory();
		private Stopwatch timeSincePrintStarted = new Stopwatch();

		public PrintActionRow(QueueDataView queueDataView)
		{
			this.queueDataView = queueDataView;
		}

		private event EventHandler unregisterEvents;

		public override void OnClosed(EventArgs e)
		{
			if (unregisterEvents != null)
			{
				unregisterEvents(this, null);
			}
			base.OnClosed(e);
		}

		public void ThemeChanged(object sender, EventArgs e)
		{
			this.Invalidate();
		}

		protected override void AddChildElements()
		{
			addButton = textImageButtonFactory.GenerateTooltipButton("Add".Localize(), "icon_circle_plus.png");
			addButton.ToolTipText = "Add a file to be printed".Localize();
			addButton.Margin = new BorderDouble(6, 6, 6, 3);

			startButton = textImageButtonFactory.GenerateTooltipButton("Print".Localize(), "icon_play_32x32.png");
			startButton.ToolTipText = "Begin printing the selected item.".Localize();
			startButton.Margin = new BorderDouble(6, 6, 6, 3);

			configureButton = textImageButtonFactory.GenerateTooltipButton("Finish Setup...".Localize());
			configureButton.ToolTipText = "Run setup configuration for printer.".Localize();
			configureButton.Margin = new BorderDouble(6, 6, 6, 3);

			string connectButtonText = "Connect".Localize();
			string connectButtonMessage = "Connect to the printer".Localize();
			connectButton = textImageButtonFactory.GenerateTooltipButton(connectButtonText, "icon_power_32x32.png");
			connectButton.ToolTipText = connectButtonMessage;
			connectButton.Margin = new BorderDouble(6, 6, 6, 3);

			string resetConnectionButtontText = "Reset".Localize();
			string resetConnectionButtonMessage = "Reboots the firmware on the controller".Localize();
			resetConnectionButton = textImageButtonFactory.GenerateTooltipButton(resetConnectionButtontText, "e_stop4.png");
			resetConnectionButton.ToolTipText = resetConnectionButtonMessage;
			resetConnectionButton.ToolTipText = resetConnectionButtonMessage;
			resetConnectionButton.Margin = new BorderDouble(6, 6, 6, 3);

			string skipButtonText = "Skip".Localize();
			string skipButtonMessage = "Skip the current item and move to the next in queue".Localize();
			skipButton = makeButton(skipButtonText, skipButtonMessage);

			string removeButtonText = "Remove".Localize();
			string removeButtonMessage = "Remove current item from queue".Localize();
			removeButton = makeButton(removeButtonText, removeButtonMessage);

			string pauseButtonText = "Pause".Localize();
			string pauseButtonMessage = "Pause the current print".Localize();
			pauseButton = makeButton(pauseButtonText, pauseButtonMessage);

			string cancelCancelButtonText = "Cancel Connect".Localize();
			string cancelConnectButtonMessage = "Stop trying to connect to the printer.".Localize();
			cancelConnectButton = makeButton(cancelCancelButtonText, cancelConnectButtonMessage);

			string cancelButtonText = "Cancel".Localize();
			string cancelButtonMessage = "Stop the current print".Localize();
			cancelButton = makeButton(cancelButtonText, cancelButtonMessage);

			string resumeButtonText = "Resume".Localize();
			string resumeButtonMessage = "Resume the current print".Localize();
			resumeButton = makeButton(resumeButtonText, resumeButtonMessage);

			string reprintButtonText = "Print Again".Localize();
			string reprintButtonMessage = "Print current item again".Localize();
			reprintButton = makeButton(reprintButtonText, reprintButtonMessage);

			string doneCurrentPartButtonText = "Done".Localize();
			string doenCurrentPartButtonMessage = "Move to next print in queue".Localize();
			doneWithCurrentPartButton = makeButton(doneCurrentPartButtonText, doenCurrentPartButtonMessage);

			this.Margin = new BorderDouble(0, 0, 10, 0);
			this.HAnchor = HAnchor.FitToChildren;

			this.AddChild(connectButton);
			allPrintButtons.Add(connectButton);

			this.AddChild(addButton);
			allPrintButtons.Add(addButton);

			this.AddChild(startButton);
			allPrintButtons.Add(startButton);

			this.AddChild(configureButton);
			allPrintButtons.Add(configureButton);

			this.AddChild(pauseButton);
			allPrintButtons.Add(pauseButton);

			this.AddChild(resumeButton);
			allPrintButtons.Add(resumeButton);

			this.AddChild(doneWithCurrentPartButton);
			allPrintButtons.Add(doneWithCurrentPartButton);

			this.AddChild(skipButton);
			allPrintButtons.Add(skipButton);

			this.AddChild(cancelButton);
			allPrintButtons.Add(cancelButton);

			this.AddChild(cancelConnectButton);
			allPrintButtons.Add(cancelConnectButton);

			this.AddChild(reprintButton);
			allPrintButtons.Add(reprintButton);

			this.AddChild(removeButton);
			allPrintButtons.Add(removeButton);

			this.AddChild(resetConnectionButton);
			allPrintButtons.Add(resetConnectionButton);

			SetButtonStates();
		}

		protected override void AddHandlers()
		{
			PrinterConnectionAndCommunication.Instance.ActivePrintItemChanged.RegisterEvent(onStateChanged, ref unregisterEvents);
			PrinterConnectionAndCommunication.Instance.CommunicationStateChanged.RegisterEvent(onStateChanged, ref unregisterEvents);
			addButton.Click += onAddButton_Click;
			startButton.Click += onStartButton_Click;
			configureButton.Click += onStartButton_Click;
            skipButton.Click += onSkipButton_Click;
			removeButton.Click += onRemoveButton_Click;
			resumeButton.Click += onResumeButton_Click;
			pauseButton.Click += onPauseButton_Click;
			connectButton.Click += onConnectButton_Click;
			resetConnectionButton.Click += (sender, e) => { UiThread.RunOnIdle(PrinterConnectionAndCommunication.Instance.RebootBoard); };

			cancelButton.Click += (sender, e) => { UiThread.RunOnIdle(CancelButton_Click); };
			cancelConnectButton.Click += (sender, e) => { UiThread.RunOnIdle(CancelPrinting); };
			reprintButton.Click += onReprintButton_Click;
			doneWithCurrentPartButton.Click += onDoneWithCurrentPartButton_Click;
			ActiveTheme.Instance.ThemeChanged.RegisterEvent(ThemeChanged, ref unregisterEvents);
		}

		protected void DisableActiveButtons()
		{
			foreach (Button button in this.activePrintButtons)
			{
				button.Enabled = false;
			}
		}

		protected void EnableActiveButtons()
		{
			foreach (Button button in this.activePrintButtons)
			{
				button.Enabled = true;
			}
		}

		protected override void Initialize()
		{
			textImageButtonFactory.normalTextColor = RGBA_Bytes.White;
			textImageButtonFactory.disabledTextColor = RGBA_Bytes.LightGray;
			textImageButtonFactory.hoverTextColor = RGBA_Bytes.White;
			textImageButtonFactory.pressedTextColor = RGBA_Bytes.White;
			textImageButtonFactory.AllowThemeToAdjustImage = false;

			textImageButtonFactory.borderWidth = 1;
			textImageButtonFactory.FixedHeight = 52 * TextWidget.GlobalPointSizeScaleRatio;
			textImageButtonFactory.fontSize = 14;
			textImageButtonFactory.normalBorderColor = new RGBA_Bytes(255, 255, 255, 100);
			textImageButtonFactory.hoverBorderColor = new RGBA_Bytes(255, 255, 255, 100);
		}

		protected Button makeButton(string buttonText, string buttonToolTip = "")
		{
			Button button = textImageButtonFactory.GenerateTooltipButton(buttonText);
			button.ToolTipText = buttonToolTip;
			button.Margin = new BorderDouble(0, 6, 6, 3);
			return button;
		}

		//Set the states of the buttons based on the status of PrinterCommunication
		protected void SetButtonStates()
		{
			this.activePrintButtons.Clear();
			if (!PrinterConnectionAndCommunication.Instance.PrinterIsConnected
				&& PrinterConnectionAndCommunication.Instance.CommunicationState != PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect)
			{
				if (ActiveTheme.Instance.IsTouchScreen)
				{
					this.activePrintButtons.Add(connectButton);
				}
				ShowActiveButtons();
				EnableActiveButtons();
			}
			else if (PrinterConnectionAndCommunication.Instance.ActivePrintItem == null)
			{
				this.activePrintButtons.Add(addButton);
				ShowActiveButtons();
				EnableActiveButtons();
			}
			else
			{
				switch (PrinterConnectionAndCommunication.Instance.CommunicationState)
				{
					case PrinterConnectionAndCommunication.CommunicationStates.AttemptingToConnect:
						this.activePrintButtons.Add(cancelConnectButton);
						EnableActiveButtons();
						break;

					case PrinterConnectionAndCommunication.CommunicationStates.Connected:
						PrintLevelingData levelingData = PrintLevelingData.GetForPrinter(ActivePrinterProfile.Instance.ActivePrinter);
						if (levelingData != null && ActiveSliceSettings.Instance.LevelingRequiredToPrint
							&& !levelingData.HasBeenRun())
						{
							this.activePrintButtons.Add(configureButton);
						}
						else
						{
							this.activePrintButtons.Add(startButton);
							//Show 'skip' button if there are more items in queue
							if (QueueData.Instance.Count > 1)
							{
								this.activePrintButtons.Add(skipButton);
							}

							this.activePrintButtons.Add(removeButton);
						}

						EnableActiveButtons();
						break;

					case PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint:
						this.activePrintButtons.Add(cancelButton);
						EnableActiveButtons();
						break;

					case PrinterConnectionAndCommunication.CommunicationStates.PrintingFromSd:
					case PrinterConnectionAndCommunication.CommunicationStates.Printing:
						if (!timeSincePrintStarted.IsRunning)
						{
							timeSincePrintStarted.Restart();
						}

						if (!PrinterConnectionAndCommunication.Instance.PrintWasCanceled)
						{
							this.activePrintButtons.Add(pauseButton);
							this.activePrintButtons.Add(cancelButton);
						}
						else if (ActiveTheme.Instance.IsTouchScreen)
						{
							this.activePrintButtons.Add(resetConnectionButton);
						}

						EnableActiveButtons();
						break;

					case PrinterConnectionAndCommunication.CommunicationStates.Paused:
						this.activePrintButtons.Add(resumeButton);
						this.activePrintButtons.Add(cancelButton);
						EnableActiveButtons();
						break;

					case PrinterConnectionAndCommunication.CommunicationStates.FinishedPrint:
						this.activePrintButtons.Add(reprintButton);
						this.activePrintButtons.Add(doneWithCurrentPartButton);
						EnableActiveButtons();
						break;

					default:
						DisableActiveButtons();
						break;
				}
			}

			if (PrinterConnectionAndCommunication.Instance.PrinterIsConnected
				&& ActiveSliceSettings.Instance.ShowResetConnection()
				&& ActiveTheme.Instance.IsTouchScreen)
			{
				this.activePrintButtons.Add(resetConnectionButton);
				ShowActiveButtons();
				EnableActiveButtons();
			}
			ShowActiveButtons();
		}

		protected void ShowActiveButtons()
		{
			foreach (Button button in this.allPrintButtons)
			{
				if (activePrintButtons.IndexOf(button) >= 0)
				{
					button.Visible = true;
				}
				else
				{
					button.Visible = false;
				}
			}
		}

		private void AddButtonOnIdle()
		{
			FileDialog.OpenFileDialog(
				new OpenFileDialogParams(ApplicationSettings.OpenPrintableFileParams, multiSelect: true),
				(openParams) =>
				{
					if (openParams.FileNames != null)
					{
						foreach (string loadedFileName in openParams.FileNames)
						{
							QueueData.Instance.AddItem(new PrintItemWrapper(new PrintItem(Path.GetFileNameWithoutExtension(loadedFileName), Path.GetFullPath(loadedFileName))));
						}
					}
				});
		}

		private void CancelButton_Click()
		{
			if (timeSincePrintStarted.IsRunning && timeSincePrintStarted.ElapsedMilliseconds > (2 * 60 * 1000))
			{
				StyledMessageBox.ShowMessageBox(onConfirmCancelPrint, cancelCurrentPrintMessage, cancelCurrentPrintTitle, StyledMessageBox.MessageType.YES_NO);
			}
			else
			{
				CancelPrinting();
				UiThread.RunOnIdle(SetButtonStates);
			}
		}

		private void CancelPrinting()
		{
			if (PrinterConnectionAndCommunication.Instance.CommunicationState == PrinterConnectionAndCommunication.CommunicationStates.PreparingToPrint)
			{
				SlicingQueue.Instance.CancelCurrentSlicing();
			}
			PrinterConnectionAndCommunication.Instance.Stop();
			timeSincePrintStarted.Reset();
			UiThread.RunOnIdle(SetButtonStates);
		}

		private void onAddButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle(AddButtonOnIdle);
		}

		private void onConnectButton_Click(object sender, EventArgs mouseEvent)
		{
			if (ActivePrinterProfile.Instance.ActivePrinter == null)
			{
#if __ANDROID__
				SetupWizardWindow.Show();
#else
				PrinterActionRow.OpenConnectionWindow(true);
#endif
			}
			else
			{
#if __ANDROID__
				if (!FrostedSerialPort.HasPermissionToDevice())
				{
					// Opens the USB device permissions dialog which will call back into our UsbDevice broadcast receiver to connect
					FrostedSerialPort.RequestPermissionToDevice();
				}
				else
#endif
				{
					ConnectToActivePrinter();
				}
			}
		}

		private void ConnectToActivePrinter()
		{
			PrinterConnectionAndCommunication.Instance.HaltConnectionThread();
			PrinterConnectionAndCommunication.Instance.ConnectToActivePrinter();
		}

		private void onConfirmCancelPrint(bool messageBoxResponse)
		{
			if (messageBoxResponse)
			{
				UiThread.RunOnIdle(CancelPrinting);
			}
		}

		private void onDoneWithCurrentPartButton_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.ResetToReadyState();
			QueueData.Instance.RemoveAt(queueDataView.SelectedIndex);
			// We don't have to change the selected index because we should be on the next one as we deleted the one
			// we were on.
		}

		private void onPauseButton_Click(object sender, EventArgs mouseEvent)
		{
			PrinterConnectionAndCommunication.Instance.RequestPause();
		}

		private void onRemoveButton_Click(object sender, EventArgs mouseEvent)
		{
			QueueData.Instance.RemoveAt(queueDataView.SelectedIndex);
		}

		private void onReprintButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle(PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible);
		}

		private void onResumeButton_Click(object sender, EventArgs mouseEvent)
		{
			if (PrinterConnectionAndCommunication.Instance.PrinterIsPaused)
			{
				PrinterConnectionAndCommunication.Instance.Resume();
			}
		}

		private void onSkipButton_Click(object sender, EventArgs mouseEvent)
		{
			if (QueueData.Instance.Count > 1)
			{
				queueDataView.MoveToNext();
			}
		}

		private void onStartButton_Click(object sender, EventArgs mouseEvent)
		{
			UiThread.RunOnIdle(PrinterConnectionAndCommunication.Instance.PrintActivePartIfPossible);
		}

		private void onStateChanged(object sender, EventArgs e)
		{
			UiThread.RunOnIdle(SetButtonStates);
		}
	}
}