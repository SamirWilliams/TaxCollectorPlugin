using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ImGuiNET;
using TaxCollectorPlugin;
using TaxCollectorPlugin.Helpers;

namespace TaxCollectorPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private TCPlugin plugin;
    private IClientState clientState;
    private Dictionary<string, string> iconPaths;
    private Dictionary<string, int> taxRates;
    private string homeWorld;
    private string currentWorld;
    private int buttonPressChecker = 0;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(TCPlugin plugin, IClientState clientState, Dictionary<string, string> iconPaths)
        : base("Tax Rates##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        Size = new Vector2(450, 350);
        SizeCondition = ImGuiCond.Always;

        this.plugin = plugin;
        this.clientState = clientState;
        this.iconPaths = iconPaths;
        if (this.clientState != null)
        {
            homeWorld = this.clientState.LocalPlayer.HomeWorld.GetWithLanguage(clientState.ClientLanguage).Name;
        }
        else {
            homeWorld = "Unknown Home World";
        }
        
        getTaxRates(homeWorld);

    }

    public void Dispose() { }

    public override void Draw()
    {

        if (clientState != null)
        {
            currentWorld = clientState.LocalPlayer.CurrentWorld.GetWithLanguage(clientState.ClientLanguage).Name;
        }
        else
        {
            currentWorld = "Unknown Current World";  
        }

        if (buttonPressChecker == 0)
        {
            ImGui.Text($"Current Tax Rates are from {homeWorld}");
        }
        else {
            ImGui.Text($"Current Tax Rates are from {currentWorld}");
        }

        if (ImGui.BeginTable("table1", 3))
        {
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed, 22.0f);
            ImGui.TableSetupColumn("two", ImGuiTableColumnFlags.WidthFixed, 100.0f);
            ImGui.TableSetupColumn("three", ImGuiTableColumnFlags.WidthFixed, 200.0f);
            foreach (var entry in taxRates)
            {
                ImGui.TableNextRow();

                // Try to get the icon path for the current city (entry.Key) from the iconPaths dictionary
                if (iconPaths.TryGetValue(entry.Key, out var iconPath))
                {
                    // Now check if the iconPath is not null
                    if (iconPath != null)
                    {
                        // Load the image from the path
                        var iconImage = TCPlugin.TextureProvider.GetFromFile(iconPath).GetWrapOrDefault();

                        // Check if the image was successfully loaded
                        if (iconImage != null)
                        {
                            ImGui.TableNextColumn();                           
                            ImGui.Image(iconImage.ImGuiHandle, new Vector2(iconImage.Width, iconImage.Height));
                        }
                        else
                        {
                            // Handle the case where the image failed to load
                            ImGui.TableNextColumn();
                            ImGui.Text("Image failed to load");
                        }
                    }
                    else
                    {
                        // Handle the case where iconPath is null
                        ImGui.TableNextColumn();
                        ImGui.Text("Icon path is null");
                    }
                }
                else
                {
                    // Handle the case where the key is not found in the iconPaths dictionary
                    ImGui.TableNextColumn();
                    ImGui.Text("No Image");
                }

                // Display the city name and tax rate
                ImGui.TableNextColumn();
                ImGui.Text($"{entry.Key}: ");

                ImGui.TableNextColumn();
                ImGui.Text($"{entry.Value}%%");
            }
            ImGui.EndTable();
        }

        if (ImGui.Button("Update Tax Rates to Home World"))
        {
            getTaxRates(homeWorld);
            buttonPressChecker = 0;
        }

        ImGui.SameLine();

        if (ImGui.Button("Update Tax Rate to Current World")) 
        {
            getTaxRates(currentWorld);
            buttonPressChecker = 1;
}

    }

    private void getTaxRates(string world) {

        taxRates = UniversalisClient.GetTaxRates(world).GetResultSafely();

    }

}
