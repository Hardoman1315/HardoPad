using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;


namespace app.Views;

public partial class MainWindow : Window
{
    private IStorageFile? fileToWrite;
    
    private readonly FilePickerSaveOptions fileSaveOptions = new()
    {
        DefaultExtension = "txt",
        SuggestedFileName = "New file",
        ShowOverwritePrompt = true,
        FileTypeChoices =
        [
            new FilePickerFileType("Text files")
            {
                Patterns = ["*.txt"]
            },
            new FilePickerFileType("All files")
            {
                Patterns = ["*.*"]
            }
        ]
    };

    private readonly FilePickerOpenOptions fileOpenOptions = new()
    {
        Title = "Open Text File",
        AllowMultiple = false,
        FileTypeFilter =
        [
            new FilePickerFileType("Text files")
            {
                Patterns = ["*.txt"]
            },
            new FilePickerFileType("All files")
            {
                Patterns = ["*.*"]
            }
        ]
    };
    
    
    public MainWindow()
    {
        InitializeComponent();
        
        string traceID = Guid.NewGuid().ToString();
        
        SupportMethods.LogData("Info", traceID, 
            "[Initialization] Starting the application", true);
    }
    
    private async void Open(object? sender, RoutedEventArgs e)
    {
        string traceID = Guid.NewGuid().ToString();
        
        var storage = StorageProvider;
        var fileToRead = await storage.OpenFilePickerAsync(fileOpenOptions);
        
        if (fileToRead is { Count: > 0 })
        {
            SupportMethods.LogData("Info", traceID, 
                $"[Open] Path to file: {fileToRead[0].Path.LocalPath}");
            
            fileToWrite = fileToRead[0];
            await using var stream = await fileToRead[0].OpenReadAsync();
            using var reader = new StreamReader(stream);
        
            textArea.Text = await reader.ReadToEndAsync();
        }
        else
        {
            SupportMethods.LogData("Warning", traceID, 
                $"[Open] File to read was not selected");
        }
    }
    
    private async void Save(object? sender, RoutedEventArgs e)
    {
        string traceID = Guid.NewGuid().ToString();
        
        if (fileToWrite != null && textArea.Text != null)
        {
            SupportMethods.LogData("Info", traceID, 
                $"[Save] Path to file: {fileToWrite.Path.LocalPath}");
            
            await using var stream = await fileToWrite.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(textArea.Text);
        }
        else if (fileToWrite == null && textArea.Text != null)
        {
            SupportMethods.LogData("Info", traceID, 
                "[Save] Current file have no path to save. Invoking 'SaveAs'");
            SaveAs(sender, e);
        }
        else if (fileToWrite != null && textArea.Text == null)
        {
            SupportMethods.LogData("Warning", traceID, 
                "[Save] Current file have no data");
        }
    }
    
    private async void SaveAs(object? sender, RoutedEventArgs e)
    {
        string traceID = Guid.NewGuid().ToString();
        
        var storage = StorageProvider;
        fileToWrite = await storage.SaveFilePickerAsync(fileSaveOptions);
    
        if (fileToWrite != null && textArea.Text != null)
        {
            SupportMethods.LogData("Info", traceID, 
                $"[SaveAs] Path to file: {fileToWrite.Path.LocalPath}");
            
            await using var stream = await fileToWrite.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(textArea.Text);
        }
        else
        {
            SupportMethods.LogData("Warning", traceID, 
                "[SaveAs] Current file have no path to save");
        }
    }
}
