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
    }
    
    private async void Open(object? sender, RoutedEventArgs e)
    {
        var storage = StorageProvider;
        var fileToRead = await storage.OpenFilePickerAsync(fileOpenOptions);

        if (fileToRead is { Count: > 0 })
        {
            fileToWrite = fileToRead[0];
            await using var stream = await fileToRead[0].OpenReadAsync();
            using var reader = new StreamReader(stream);
        
            textArea.Text = await reader.ReadToEndAsync();
        }
    }

    private async void Save(object? sender, RoutedEventArgs e)
    {
        if (fileToWrite != null && textArea.Text != null)
        {
            Console.WriteLine($"Save path: {fileToWrite.Path.LocalPath}");
            
            await using var stream = await fileToWrite.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(textArea.Text);
        }
        else if (fileToWrite == null && textArea.Text != null)
        {
            SaveAs(sender, e);
        }
        else
        {
            Console.WriteLine("Text block have no data");
        }
    }
    
    private async void SaveAs(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Output is fine :)");
        
        var storage = StorageProvider;
        fileToWrite = await storage.SaveFilePickerAsync(fileSaveOptions);

        if (fileToWrite != null && textArea.Text != null)
        {
            Console.WriteLine($"Save path: {fileToWrite.Path.LocalPath}");
            
            await using var stream = await fileToWrite.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(textArea.Text);
        }
        else
        {
            Console.WriteLine("Save path is empty");
        }
    }
}
