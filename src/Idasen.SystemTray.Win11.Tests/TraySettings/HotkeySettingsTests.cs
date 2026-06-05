using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class HotkeySettingsTests
{
    [ Fact ]
    public void GlobalHotkeysEnabled_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.GlobalHotkeysEnabled
                .Should ( )
                .Be ( AppConfiguration.Defaults.GlobalHotkeysEnabled ) ;
    }

    [ Fact ]
    public void StandingKey_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.StandingKey
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.StandingKey ) ;
    }

    [ Fact ]
    public void StandingModifiers_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.StandingModifiers
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.DefaultModifiers ) ;
    }

    [ Fact ]
    public void SeatingKey_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.SeatingKey
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.SeatingKey ) ;
    }

    [ Fact ]
    public void SeatingModifiers_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.SeatingModifiers
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.DefaultModifiers ) ;
    }

    [ Fact ]
    public void Custom1Key_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.Custom1Key
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.Custom1Key ) ;
    }

    [ Fact ]
    public void Custom1Modifiers_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.Custom1Modifiers
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.DefaultModifiers ) ;
    }

    [ Fact ]
    public void Custom2Key_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.Custom2Key
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.Custom2Key ) ;
    }

    [ Fact ]
    public void Custom2Modifiers_ShouldHaveDefaultValue ( )
    {
        var settings = new HotkeySettings ( ) ;

        settings.Custom2Modifiers
                .Should ( )
                .Be ( AppConfiguration.Hotkeys.DefaultModifiers ) ;
    }

    [ Fact ]
    public void HotkeySettings_ShouldBeSerializableToJson ( )
    {
        var settings = new HotkeySettings
        {
            GlobalHotkeysEnabled = false ,
            StandingKey          = "F1" ,
            StandingModifiers    = "Control, Shift"
        } ;

        var json = System.Text.Json.JsonSerializer.Serialize ( settings ) ;

        json.Should ( ).Contain ( "GlobalHotkeysEnabled" ) ;
        json.Should ( ).Contain ( "false" ) ;
        json.Should ( ).Contain ( "F1" ) ;
    }

    [ Fact ]
    public void HotkeySettings_ShouldBeDeserializableFromJson ( )
    {
        var json = """
                   {
                       "GlobalHotkeysEnabled": false,
                       "StandingKey": "F1",
                       "StandingModifiers": "Control, Shift"
                   }
                   """ ;

        var settings = System.Text.Json.JsonSerializer.Deserialize < HotkeySettings > ( json ) ;

        settings.Should ( ).NotBeNull ( ) ;
        settings !.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
        settings.StandingKey.Should ( ).Be ( "F1" ) ;
        settings.StandingModifiers.Should ( ).Be ( "Control, Shift" ) ;
    }

    [ Fact ]
    public void HotkeySettings_PartialDeserialization_ShouldUseDefaults ( )
    {
        var json = """
                   {
                       "GlobalHotkeysEnabled": false
                   }
                   """ ;

        var settings = System.Text.Json.JsonSerializer.Deserialize < HotkeySettings > ( json ) ;

        settings.Should ( ).NotBeNull ( ) ;
        settings !.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
        settings.StandingKey.Should ( ).Be ( AppConfiguration.Hotkeys.StandingKey ) ;
        settings.StandingModifiers.Should ( ).Be ( AppConfiguration.Hotkeys.DefaultModifiers ) ;
    }
}
