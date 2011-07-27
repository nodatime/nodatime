#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using System.Windows;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ZoneInfoCompilerW
{
    /// <summary>
    ///   Interaction logic for FileSelector.xaml
    /// </summary>
    public partial class FileSelector
    {
        /// <summary>
        ///   Property definition for the value of the file selector text field.
        /// </summary>
        public static DependencyProperty FileSelectorNameProperty = DependencyProperty.Register("FileSelectorName", typeof(string), typeof(FileSelector),
                                                                                                new PropertyMetadata(null, FileSelectorNameChangeCallback));

        /// <summary>
        ///   Property definition for the select directory flag. If true then this widget
        ///   selects a directory, otherwise it selects a file.
        /// </summary>
        public static DependencyProperty SelectDirectoryProperty = DependencyProperty.Register("SelectDirectory", typeof(bool), typeof(FileSelector));

        /// <summary>
        ///   Initializes a new instance of the <see cref = "FileSelector" /> class.
        /// </summary>
        public FileSelector()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Gets or sets the file selector file name.
        /// </summary>
        public string FileSelectorName
        {
            get { return (string)GetValue(FileSelectorNameProperty); }
            set { SetValue(FileSelectorNameProperty, value); }
        }

        /// <summary>
        ///   Gets or sets the select a direcxtory flag. If true then this widget
        ///   selects a directory, otherwise it selects a file.
        /// </summary>
        public bool SelectDirectory
        {
            get { return (bool)GetValue(SelectDirectoryProperty); }
            set { SetValue(SelectDirectoryProperty, value); }
        }

        /// <summary>
        ///   Handles a change event from the file name property.
        /// </summary>
        /// <remarks>
        ///   We copy the new value to the text field and its tooltip.
        /// </remarks>
        /// <param name = "d">The object that changed.</param>
        /// <param name = "e">The description of the change.</param>
        private static void FileSelectorNameChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (FileSelector)d;
            selector.fileName.Text = e.NewValue as string;
            selector.fileName.ToolTip = e.NewValue as string;
        }

        /// <summary>
        ///   Handles the select button being clicked.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The event definition.</param>
        private void SelectFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectDirectory)
            {
                var dialog = new FolderBrowserDialog();
                if (string.IsNullOrEmpty(FileSelectorName))
                {
                }
                else
                {
                    dialog.SelectedPath = FileSelectorName;
                }
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FileSelectorName = dialog.SelectedPath;
                }
            }
            else
            {
                // Configure open file dialog box
                var openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = ".xml"; // Default file extension
                openFileDialog.Filter = "XML Documents (.xml)|*.xml"; // Filter files by extension

                // Process open file dialog box results
                if (openFileDialog.ShowDialog() == true)
                {
                    FileSelectorName = openFileDialog.FileName;
                }
            }
        }
    }
}
