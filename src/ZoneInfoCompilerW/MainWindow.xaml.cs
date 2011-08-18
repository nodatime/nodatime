#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.ComponentModel;
using System.Windows;
using NodaTime.ZoneInfoCompiler.Tzdb;
using NodaTime.ZoneInfoCompiler.winmap;
using ZoneInfoCompilerW.Properties;
using ZoneInfoCompilerW.ViewModel;

namespace ZoneInfoCompilerW
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ListBoxLog logger;
        private readonly TzdbViewModel tzdbModel = new TzdbViewModel();
        private readonly WindowsMapViewModel windowsMapModel = new WindowsMapViewModel();

        /// <summary>
        ///   Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            zoneInfo.DataContext = tzdbModel;
            windowsMap.DataContext = windowsMapModel;
            logger = new ListBoxLog(output);
        }

        /// <summary>
        ///   Invoked by the <see cref="BackgroundWorker" /> to perform the background work.
        ///   This compiles the time zone data base resources.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event arguments.</param>
        private void TzdbWorkerDoWork(object sender, DoWorkEventArgs args)
        {
            var options = new TzdbCompilerOptions();
            options.SourceDirectoryName = tzdbModel.SourceDirectoryName;
            options.OutputFileName = tzdbModel.TargetFileName;
            options.OutputType = tzdbModel.OutputType;

            var compiler = new TzdbZoneInfoCompiler(logger);
            compiler.Execute(options);
        }

        /// <summary>
        ///   Invoked by the <see cref="BackgroundWorker" /> when the background work
        ///   is done.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TzdbWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TzdbConvert.InvokeIfRequired(() => TzdbConvert.IsEnabled = true);
        }

        /// <summary>
        ///   Handles the compile the TZDB button. Starts the compiler in the background.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Tzdb_Convert_Click(object sender, RoutedEventArgs e)
        {
            TzdbConvert.IsEnabled = false;
            var tzdbWorker = new BackgroundWorker();
            tzdbWorker.DoWork += TzdbWorkerDoWork;
            tzdbWorker.RunWorkerCompleted += TzdbWorkerRunWorkerCompleted;
            tzdbWorker.RunWorkerAsync();
        }

        /// <summary>
        ///   Handles the main window closing event. We save certain information for
        ///   use the next time the application is run.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.TzdbSourceDirectoryName = tzdbModel.SourceDirectoryName;
            Settings.Default.TzdbTargetFileName = tzdbModel.TargetFileName;
            Settings.Default.TzdbResourceOutputType = tzdbModel.OutputType;

            Settings.Default.WinmapFilePath = windowsMapModel.FileName;
            Settings.Default.WinmapResourceOutputType = windowsMapModel.OutputType;

            Settings.Default.Save();
        }

        /// <summary>
        ///   Invoked by the <see cref="BackgroundWorker" /> to perform the background work.
        ///   This compiles the Windows mapping file resource.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event arguments.</param>
        private void WinmapWorkerDoWork(object sender, DoWorkEventArgs args)
        {
            // var worker = sender as BackgroundWorker;
            var options = new WindowsMapperCompilerOptions();
            options.SourceFileName = windowsMapModel.FileName;
            options.OutputType = windowsMapModel.OutputType;

            var compiler = new WindowsMapperCompiler(logger);
            compiler.Execute(options);
        }

        /// <summary>
        ///   Invoked by the <see cref="BackgroundWorker" /> when the background work
        ///   is done.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void WinmapWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WinmapConvert.InvokeIfRequired(() => WinmapConvert.IsEnabled = true);
        }

        /// <summary>
        ///   Handles the compile the Windows mapping file button. Starts the compiler in the background.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Winmap_Convert_Click(object sender, RoutedEventArgs e)
        {
            WinmapConvert.IsEnabled = false;
            var winmapWorker = new BackgroundWorker();
            winmapWorker.DoWork += WinmapWorkerDoWork;
            winmapWorker.RunWorkerCompleted += WinmapWorkerRunWorkerCompleted;
            winmapWorker.RunWorkerAsync();
        }
    }
}
