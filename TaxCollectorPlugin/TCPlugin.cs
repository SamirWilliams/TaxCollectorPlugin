using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Game.ClientState.Objects.Enums;
using TaxCollectorPlugin.Windows;
using TaxCollectorPlugin.Helpers;
using System.Collections.Generic;

namespace TaxCollectorPlugin;

public class TCPlugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;

    private const string CommandName = "/ptax";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("TaxCollectorPlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public TCPlugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var iconPaths = new Dictionary<string, string>
        {
            { "Limsa Lominsa", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Limsa_Icon.png") },
            { "Gridania", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Gridania_Icon.png") },
            { "Ul'dah", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Uldah_Icon.png") },
            { "Ishgard", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Ishgard_Icon.png") },
            { "Kugane", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Kugane_Icon.png") },
            { "Crystarium", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Crystarium_Icon.png") },
            { "Old Sharlayan", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Sharlayan_Icon.png") },
            { "Tuliyollal", Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "Icons", "Tuliyollal_Icon.png") }
        };

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this,ClientState, iconPaths);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles main tax collector window"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
