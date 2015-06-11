﻿/*
Copyright (c) 2015, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using MatterHackers.Agg;
using MatterHackers.MatterControl.PrintQueue;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MatterHackers.MatterControl.PrintLibrary.Provider
{
	public abstract class LibraryProvider
	{
		public static RootedObjectEventHandler DataReloaded = new RootedObjectEventHandler();
		public static RootedObjectEventHandler ItemAdded = new RootedObjectEventHandler();
		public static RootedObjectEventHandler ItemRemoved = new RootedObjectEventHandler();

		private static LibraryProvider currentProvider;

		public static LibraryProvider CurrentProvider
		{
			get
			{
				if (currentProvider == null)
				{
					// hack for the moment
					currentProvider = new LibraryProviderSQLite();
				}
				return currentProvider;
			}
		}

		public abstract int Count { get; }

		public abstract string KeywordFilter { get; set; }

		public abstract PrintItemWrapper GetPrintItemWrapper(int itemIndex);

		public abstract void LoadFilesIntoLibrary(IList<string> files, ReportProgressRatio reportProgress = null, RunWorkerCompletedEventHandler callback = null);

		public abstract void RemoveItem(PrintItemWrapper printItemWrapper);

		public static void OnDataReloaded(EventArgs eventArgs)
		{
			DataReloaded.CallEvents(CurrentProvider, eventArgs);
		}

		public static void OnItemAdded(EventArgs eventArgs)
		{
			ItemAdded.CallEvents(CurrentProvider, eventArgs);
		}

		public static void OnItemRemoved(EventArgs eventArgs)
		{
			ItemRemoved.CallEvents(CurrentProvider, eventArgs);
		}

		public void SetCurrent(LibraryProvider current)
		{
			LibraryProvider.currentProvider = current;
		}
	}
}