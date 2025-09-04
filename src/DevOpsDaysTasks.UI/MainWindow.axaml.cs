using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DevOpsDaysTasks.Core.Attributes;
using DevOpsDaysTasks.Core.Models;
using DevOpsDaysTasks.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevOpsDaysTasks.UI;

public partial class MainWindow : Window
{
    private readonly ITaskRepository _repo;
    private readonly ObservableCollection<TaskItem> _items = [];

    public MainWindow()
    {
        InitializeComponent();
        Title = $"DevOpsDaysTasks - {GetVersionName()} - {GetVersion()}";
        Grid.ItemsSource = _items;

        _repo = RepositoryFactory.Create(DbKind.SqlServer);
        // If no local DB is installed we can fallback to Sqlite
        // _repo = RepositoryFactory.Create(DbKind.Sqlite);


        Opened += async (_, __) =>
        {
            await _repo.EnsureCreatedAsync();
            var items = await _repo.GetAllAsync();
            _items.Clear();
            foreach (var it in items)
                _items.Add(it);
        };
    }

    private string GetVersion()
    {
        return Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "<version>";
    }

    private string GetVersionName()
    {
        return Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<VersionNameAttribute>()?
            .VersionName ?? "<versionName>";
    }

    private async void OnAdd(object? sender, RoutedEventArgs e)
    {
        var input = new Window
        {
            Title = "Add Task",
            Width = 360,
            Height = 150,
            CanResize = false
        };

        var panel = new StackPanel { Margin = new Thickness(12), Spacing = 8 };
        var box = new TextBox { Watermark = "Task title…" };
        var ok = new Button { Content = "OK", Width = 80 };
        ok.Click += (_, __) => input.Close(box.Text);
        panel.Children.Add(new TextBlock { Text = "Task title:" });
        panel.Children.Add(box);
        panel.Children.Add(ok);
        input.Content = panel;

        var result = await input.ShowDialog<string?>(this);
        if (!string.IsNullOrWhiteSpace(result))
        {
            var item = new TaskItem { Title = result.Trim(), IsDone = false };
            item = await _repo.AddAsync(item);
            _items.Add(item);
        }
    }

    private async void OnAddFromTemplates(object? s, RoutedEventArgs e)
    {
        try
        {
            var tasks = TemplateLoader.LoadDefaultTasks();
            await _repo.AddRangeAsync(tasks);
            var latest = await _repo.GetAllAsync();
            _items.Clear();
            foreach (var t in latest)
                _items.Add(t);
        }
        catch (Exception ex)
        {
            await MessageBox(ex.Message);
        }
    }

    private async void OnMarkDone(object? s, RoutedEventArgs e)
    {
        foreach (var it in Grid.SelectedItems.Cast<TaskItem>())
        {
            it.IsDone = true;
            await _repo.UpdateAsync(it);
        }
    }

    private async void OnMarkUndone(object? s, RoutedEventArgs e)
    {
        foreach (var it in Grid.SelectedItems.Cast<TaskItem>())
        {
            it.IsDone = false;
            await _repo.UpdateAsync(it);
        }
    }

    private async void OnRemove(object? s, RoutedEventArgs e)
    {
        var selected = Grid.SelectedItems.Cast<TaskItem>().ToList();
        foreach (var it in selected)
        {
            _items.Remove(it);
            await _repo.RemoveAsync(it.Id);
        }
    }

    private void OnHelp(object? s, RoutedEventArgs e)
    {
        var path = AppPaths.HelpFilePath;
        if (File.Exists(path))
        {
            try
            {
                using var _ = Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch { }
        }
        else _ = MessageBox("Help not found. Expected ./Help/Help.pdf next to the app.");
    }

    private async Task MessageBox(string text)
    {
        var dlg = new Window
        {
            Width = 380,
            Height = 160,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(12),
                Spacing = 8,
                Children =
                {
                    new TextBlock{ Text = text, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new Button{ Content="OK", Width=80, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left }
                }
            }
        };
        ((Button)((StackPanel)dlg.Content!).Children[1]).Click += (_, __) => dlg.Close();
        await dlg.ShowDialog(this);
    }
}
