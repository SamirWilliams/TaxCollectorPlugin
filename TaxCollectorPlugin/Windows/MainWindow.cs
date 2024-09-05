using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using TaxCollectorPlugin;
using TaxCollectorPlugin.Helpers;

namespace TaxCollectorPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private TCPlugin plugin; 
    private string taxRates;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(TCPlugin plugin)
        : base("Tax Rates##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        getTaxRates("3");

    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        ImGui.Spacing();

        ImGui.Text(this.taxRates);

        ImGui.Spacing();

        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUI();
        }
    }

    private void getTaxRates(string world) {
        world = "leviathan";

        this.taxRates = UniversalisClient.GetTaxRates(world).Result;

    }

}
