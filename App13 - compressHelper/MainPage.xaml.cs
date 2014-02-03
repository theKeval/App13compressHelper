using SharpCompress.Archive;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App13___compressHelper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //var filePicker = new FileOpenPicker();
            //filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            //filePicker.FileTypeFilter.Add(".png");
            //filePicker.FileTypeFilter.Add(".jpg");
            //filePicker.FileTypeFilter.Add(".txt");
            //filePicker.FileTypeFilter.Add(".zip");
            //filePicker.FileTypeFilter.Add(".rar");
            //StorageFile abc = await filePicker.PickSingleFileAsync();

            //var folderPicker = new FolderPicker();
            //folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            //folderPicker.FileTypeFilter.Add(".png");
            //folderPicker.FileTypeFilter.Add(".jpg");
            //folderPicker.FileTypeFilter.Add(".txt");

            //StorageFolder xyz = await folderPicker.PickSingleFolderAsync();

            //await extractCompressedFile(abc, xyz);


            //using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress.rar"))
            //{
            //    var reader = ReaderFactory.Open(stream);
            //    while (reader.MoveToNextEntry())
            //    {
            //        if (!reader.Entry.IsDirectory)
            //        {
            //            Console.WriteLine(reader.Entry.FilePath);
            //            reader.WriteEntryToDirectory(@"C:\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
            //        }
            //    }
            //}

        }

        private async Task extractCompressedFile_ArchiveFactory(StorageFile sourceCompressedFile)      //, StorageFolder destinationFolder
        {
            using (Stream fileStream = await sourceCompressedFile.OpenStreamForReadAsync())
            {
                var archive = ArchiveFactory.Open(fileStream);
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(sourceCompressedFile.DisplayName, CreationCollisionOption.FailIfExists);

                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        //We need to ignore directories - only interested in files
                        
                        //StorageFile newFile = await CreateFile(entry.FilePath, destinationFolder);
                        //StorageFile newFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(entry.FilePath, CreationCollisionOption.FailIfExists);

                        StorageFile newFile = await folder.CreateFileAsync(entry.FilePath, CreationCollisionOption.FailIfExists);
                        Stream newFileStream = await newFile.OpenStreamForWriteAsync();

                        MemoryStream streamEntry = new MemoryStream();
                        entry.WriteTo(streamEntry);
                        // buffer for extraction data
                        byte[] data = streamEntry.ToArray();

                        newFileStream.Write(data, 0, data.Length);
                        newFileStream.Flush();
                        newFileStream.Dispose();
                    }
                    else if (entry.IsDirectory)
                    {
                        StorageFolder newFolder = await folder.CreateFolderAsync(entry.FilePath, CreationCollisionOption.ReplaceExisting);
                    }
                }
            }
        }


        public StorageFolder selectedFolder;
        private async Task extractCompressedFile_ReaderFactory(StorageFile sourceCompressedFile)      //, StorageFolder destinationFolder
        {
            using (Stream fileStream = await sourceCompressedFile.OpenStreamForReadAsync())
            {
                var Reader = ReaderFactory.Open(fileStream);
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(sourceCompressedFile.DisplayName, CreationCollisionOption.OpenIfExists);

                //foreach (var entry in archive.Entries)
                while (Reader.MoveToNextEntry())
                {
                    selectedFolder = folder;
                    if (!Reader.Entry.IsDirectory)
                    {
                        if (Reader.Entry.FilePath.Contains("/"))
                        {
                            string[] splitedPath = Reader.Entry.FilePath.Split('/');
                            string FileName = splitedPath[(splitedPath.Count()-1)];

                            foreach (var item in splitedPath)
                            {
                                if (item == splitedPath.First() && !(item == splitedPath.Last()))
                                {
                                    StorageFolder sf = await selectedFolder.CreateFolderAsync(item, CreationCollisionOption.OpenIfExists);
                                    selectedFolder = sf;
                                }
                                else if (!(item == splitedPath.Last()))
                                {
                                    StorageFolder sf1 = await selectedFolder.CreateFolderAsync(item, CreationCollisionOption.OpenIfExists);
                                    selectedFolder = sf1;
                                    //StorageFolder sf1 = await sf.createfol
                                }
                                else if (item == splitedPath.Last())
                                {
                                    StorageFile file = await selectedFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
                                    Stream newFileStream = await file.OpenStreamForWriteAsync();

                                    MemoryStream streamEntry = new MemoryStream();
                                    Reader.WriteEntryTo(streamEntry);
                                    //entry.WriteTo(streamEntry);
                                    // buffer for extraction data
                                    byte[] data = streamEntry.ToArray();

                                    newFileStream.Write(data, 0, data.Length);
                                    newFileStream.Flush();
                                    newFileStream.Dispose();
                                }
                            }
                        }
                        else
                        {
                            StorageFile file = await selectedFolder.CreateFileAsync(Reader.Entry.FilePath, CreationCollisionOption.OpenIfExists);
                            Stream newFileStream = await file.OpenStreamForWriteAsync();

                            MemoryStream streamEntry = new MemoryStream();
                            Reader.WriteEntryTo(streamEntry);
                            //entry.WriteTo(streamEntry);
                            // buffer for extraction data
                            byte[] data = streamEntry.ToArray();

                            newFileStream.Write(data, 0, data.Length);
                            newFileStream.Flush();
                            newFileStream.Dispose();
                        }
                        #region commented old code
                        //StorageFile newFile = await folder.CreateFileAsync(Reader.Entry.FilePath, CreationCollisionOption.FailIfExists);
                        //Stream newFileStream = await newFile.OpenStreamForWriteAsync();

                        //MemoryStream streamEntry = new MemoryStream();
                        //Reader.WriteEntryTo(streamEntry);
                        ////entry.WriteTo(streamEntry);
                        //// buffer for extraction data
                        //byte[] data = streamEntry.ToArray();

                        //newFileStream.Write(data, 0, data.Length);
                        //newFileStream.Flush();
                        //newFileStream.Dispose();
                        #endregion
                    }
                    else if (Reader.Entry.IsDirectory)
                    {
                        if (Reader.Entry.FilePath.Contains("/"))
                        {
                            string[] splitedPathWithBlank = Reader.Entry.FilePath.Split('/');
                            string FolderName = splitedPathWithBlank[splitedPathWithBlank.Length - 2];    // can also be written as splitedPath[splitedPath.count() - 2]
                            List<string> splitedPath = new List<string>();
                            List<string> splitedPathFinal = new List<string>();

                            foreach (var item in splitedPathWithBlank)
                            {
                                if (!(item == splitedPathWithBlank.Last()))
                                {
                                    splitedPath.Add(item);
                                }
                            }

                            var i = 1;
                            foreach (var item in splitedPath)
                            {
                                if (!(i == splitedPath.Count()))
                                {
                                    splitedPathFinal.Add(item);
                                    //item = item + ".last";
                                }
                                else
                                {
                                    splitedPathFinal.Add(item + ".last");
                                }
                                i++;
                            }

                            foreach (var item in splitedPathFinal)
                            {
                                if (item == splitedPathFinal.First() && item != splitedPathFinal.Last())
                                {
                                    StorageFolder sf = await selectedFolder.CreateFolderAsync(item, CreationCollisionOption.OpenIfExists);
                                    selectedFolder = sf;
                                }
                                else if (item != splitedPathFinal.Last())
                                {
                                    StorageFolder sf1 = await selectedFolder.CreateFolderAsync(item, CreationCollisionOption.OpenIfExists);
                                    selectedFolder = sf1;
                                }
                                else if (item == splitedPathFinal.Last())
                                {
                                    StorageFolder sfLast = await selectedFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
                                }
                            }
                        }

                        //StorageFolder newFolder = await folder.CreateFolderAsync(Reader.Entry.FilePath, CreationCollisionOption.ReplaceExisting);
                    }
                }
            }
        }

        private async void UnZip_onTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                tb.Text = "please wait...";

                var filePicker = new FileOpenPicker();
                filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                filePicker.FileTypeFilter.Add(".zip");
                filePicker.FileTypeFilter.Add(".rar");
                StorageFile abc = await filePicker.PickSingleFileAsync();

                //var folderPicker = new FolderPicker();
                //folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                //folderPicker.FileTypeFilter.Add(".png");
                //folderPicker.FileTypeFilter.Add(".jpg");
                //folderPicker.FileTypeFilter.Add(".txt");
                //StorageFolder xyz = await folderPicker.PickSingleFolderAsync();


                //await extractCompressedFile_ArchiveFactory(abc);
                await extractCompressedFile_ReaderFactory(abc);

                tb.Text = "Done.";

            }
            catch (Exception ex)
            {
                MessageDialog ms = new MessageDialog(ex.Message);
                ms.ShowAsync();
            }
        }

        private void Zip_onTapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

    }
}
